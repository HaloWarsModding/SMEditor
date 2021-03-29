using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace SMEditor.Editor
{
    public class Camera : IUpdateable
    {
        public Transform t = new Transform();
        public static CBData cbData = new CBData();
        public static float MoveSpeed = .1f;

        public static Buffer cb;
        static bool init = false;
        public Camera()
        {
            if(!init)
            {
                cbData.projMatrix = Matrix.Identity;
                cbData.viewMatrix = Matrix.Identity;
                cbData.modelMatrix = Matrix.Identity;
                cb = new Buffer(Renderer.viewport.Device, 
                    new BufferDescription {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = sizeof(float) * 16 * 3,
                        BindFlags = BindFlags.ConstantBuffer
                    });

                t.position = new Vector3(0, 0, -1); //must come before any rotate/addradius calls.
                Rotate(0, .1f);
                AddRadius(5f);

                UpdateProjMatrix();
                UpdateCameraBuffer();
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Move fucntions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Vector3 cameraTarget = new Vector3(0,0,0);
        public float cameraRadius = 0f;
        public void Rotate(float yaw, float pitch)
        {
            pitch = -pitch;

            Vector3 worldUp = new Vector3(0, 1, 0);

            Vector3 cameraDirection = Vector3.Normalize(t.position - cameraTarget);
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(worldUp, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);
            
            Vector3 cameraFocusVector = t.position - cameraTarget;
            
            if (cameraDirection.Y < -.9000f && pitch > 0) pitch = 0;
            if (cameraDirection.Y > .9000f && pitch < 0) pitch = 0;

            Matrix pitchMat = Matrix.RotationAxis(cameraRight, pitch);
            Matrix yawMat = Matrix.RotationAxis(worldUp, yaw);

            Vector4 vecOut;
            Vector3.Transform(ref cameraFocusVector, ref yawMat, out vecOut);
            Vector3 vecOutXYZ = new Vector3(vecOut.X, vecOut.Y, vecOut.Z);
            Vector3.Transform(ref vecOutXYZ, ref pitchMat, out vecOut);

            t.position = new Vector3(vecOut.X, vecOut.Y, vecOut.Z) + cameraTarget;
            UpdateViewMatrix();
        }
        public void AddRadius(float f)
        {
            float v = Vector3.Distance(t.position, cameraTarget);
            if (v + f < .25f && f < 0) return;

            cameraRadius += f;
            t.position = (Vector3.Normalize(t.position - cameraTarget) * cameraRadius) + cameraTarget;
            UpdateViewMatrix();
        }
        public void MoveAbsolute(Vector3 v)
        {
            cameraTarget += v;
            t.position += v;
            UpdateViewMatrix();
        }
        private void MoveRelativeToScreen(float lr, float ud)
        {
            //get roation
            Vector3 rot = Vector3.Normalize((t.position - cameraTarget));
            Matrix la = Matrix.LookAtLH(t.position, cameraTarget, Vector3.UnitY);

            Vector3 right = new Vector3(la.M11, 0, la.M31);
            Vector3 up = new Vector3(la.M12, la.M22, la.M32);
            right.Normalize();
            up.Normalize();

            Vector3 moveLR = Vector3.Normalize(new Vector3(right.X, 0, right.Z)) * lr;
            Vector3 moveUD = Vector3.Normalize(new Vector3(up.X, 0, up.Z)) * ud;
            Vector3 move = moveLR + moveUD;

            cameraTarget += move * MoveSpeed;
            t.position += move * MoveSpeed;

            UpdateViewMatrix();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Update fucntions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Update()
        {
            if (!Editor.mouseInBounds) return;

            AddRadius(Input.mouseDeltaPos.Z / -10);
            //left shift not pressed
                //mmb pressed
            if(!Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed) 
            {
                Rotate(Input.mouseDeltaPos.X / 150F, Input.mouseDeltaPos.Y / 150F);
            }

            //left shift pressed
                //mmb pressed, alt not pressed
            if (Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed && !Input.KeyIsDown(Key.LeftAlt))
            {
                Editor.loopCursorInBounds = true;
                Editor.Update();
                MoveRelativeToScreen(-Input.mouseDeltaPos.X * 5F, Input.mouseDeltaPos.Y * 5F);
                Editor.loopCursorInBounds = false;
            }
                //mmb pressed, alt pressed
            if (Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed && Input.KeyIsDown(Key.LeftAlt))
            {
                Editor.loopCursorInBounds = true;
                Editor.Update();
                MoveRelativeToScreen(-Input.mouseDeltaPos.X, Input.mouseDeltaPos.Y);
                Editor.loopCursorInBounds = false;
            }

        }
        public void SetModelMatrix(Matrix m)
        {
            cbData.modelMatrix = m;
            UpdateCameraBuffer();
        }
        public void UpdateProjMatrix()
        {
            cbData.projMatrix = Matrix.PerspectiveFovLH(70F, (float)Renderer.viewport.Width / (float)Renderer.viewport.Height, 0.01F, 1000F);
            UpdateCameraBuffer();
        }
        public void UpdateViewMatrix()
        {
            cbData.viewMatrix = Matrix.LookAtLH(t.position, cameraTarget, new Vector3(0, 1, 0));
            UpdateCameraBuffer();
        }
        public void UpdateCameraBuffer()
        {
            Matrix mvp = cbData.modelMatrix * cbData.viewMatrix * cbData.projMatrix;
            mvp = Matrix.Transpose(mvp);

            var db = new DataStream(sizeof(float) * 16 * 3, true, true);
            db.Write(Matrix.Transpose(cbData.modelMatrix));
            db.Write(Matrix.Transpose(cbData.viewMatrix));
            db.Write(Matrix.Transpose(cbData.projMatrix));
            db.Position = 0;
            
            Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, db), cb, 0);

            Renderer.viewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(cb, 0);
            db.Dispose();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CBData
    {
        public Matrix projMatrix,
                      viewMatrix,
                      modelMatrix;
    }
}
