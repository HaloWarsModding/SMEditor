using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace SMEditor
{
    public class Camera : IUpdateable
    {
        public Transform t = new Transform();
        static CBData cbData = new CBData();
        public static float MoveSpeed = .1f;

        static Buffer cb;
        static bool init = false;
        public Camera()
        {
            if(!init)
            {
                cbData.projMatrix = Matrix.Identity;
                cbData.viewMatrix = Matrix.Identity;
                cbData.modelMatrix = Matrix.Identity;
                CBData.stride = Marshal.SizeOf(new CBData());
                cb = new Buffer(Renderer.viewport.Device, 
                    new BufferDescription {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = CBData.stride,
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
        Vector3 cameraTarget = new Vector3(0,0,0);
        float cameraRadius = 0f;
        public void Rotate(float yaw, float pitch)
        {
            pitch = -pitch;

            Vector3 worldUp = new Vector3(0, 1, 0);

            Vector3 cameraDirection = Vector3.Normalize(t.position - cameraTarget);
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(worldUp, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);
            
            Vector3 cameraFocusVector = t.position - cameraTarget;
            
            if (cameraDirection.Y < -.9900f && pitch > 0) pitch = 0;
            if (cameraDirection.Y > .9900f && pitch < 0) pitch = 0;

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
            if (v < .1f && f < 0) return;

            cameraRadius += f;
            t.position = (Vector3.Normalize(t.position - cameraTarget) * cameraRadius) + cameraTarget;
            Console.WriteLine(t.position - cameraTarget);
            UpdateViewMatrix();
        }
        public void MoveAbsolute(Vector3 v)
        {
            cameraTarget += v;
            t.position += v;
            UpdateViewMatrix();
        }
        private void MoveRelativeToScreen(float lr, float ud) //left & right
        {
            //get roation
            Vector3 rot = Vector3.Normalize((t.position - cameraTarget));
            Matrix la = Matrix.LookAtLH(t.position, cameraTarget, Vector3.UnitY);

            Vector3 right = new Vector3(la.M11, 0, la.M31);
            Vector3 up = new Vector3(la.M12, la.M22, la.M32);
            right.Normalize();
            up.Normalize();

            Vector3 moveLR = Vector3.Normalize(new Vector3(right.X, 0, right.Z)) * lr;
            Vector3 moveUD = Vector3.Normalize(new Vector3(up.X, up.Y, up.Z)) * ud;
            Vector3 move = Vector3.Normalize(moveLR + moveUD);

            cameraTarget += move * MoveSpeed;
            t.position += move * MoveSpeed;

            UpdateViewMatrix();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Update fucntions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Update()
        {
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.W)) Rotate(0, 0.01f);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.S)) Rotate(0, -.01f);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.A)) Rotate(0.01f, 0);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.D)) Rotate(-.01f, 0);

            if (Input.KeyIsDown(SlimDX.DirectInput.Key.Q)) AddRadius(-.1f);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.E)) AddRadius(0.1f);

            if (Input.KeyIsDown(SlimDX.DirectInput.Key.J)) MoveRelativeToScreen(-.1f, 0);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.L)) MoveRelativeToScreen(0.1f, 0);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.I)) MoveRelativeToScreen(0, 0.1f);
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.K)) MoveRelativeToScreen(0, -.1f);

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

            var db = new DataStream(Marshal.SizeOf(new Matrix()), true, true);
            db.Write(mvp);
            db.Position = 0;

            Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, db), cb, 0);
            Renderer.viewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(cb, 0);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CBData
    {

        public Matrix projMatrix,
                      viewMatrix,
                      modelMatrix;
        public static int stride;
    }
}
