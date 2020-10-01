using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor
{
    public class Transform
    {
        public Transform()
        {
            position = new Vector3(0, 0, 0);
            eulerAngles = new Vector3(0, 0, 0);
            rotation = Quaternion.Identity;
        }
        public Vector3 position;
        public Vector3 eulerAngles;
        private Quaternion rotation;
        public Quaternion getRotation() { return rotation; }
        public Matrix GetModelMatrix()
        {
            Matrix mt, mr;

            Matrix.Translation(ref position, out mt);
            Matrix.RotationQuaternion(ref rotation, out mr);

            return mt * mr;
        }

        public void Translate(float x, float y, float z)
        {
            position += new Vector3(x, y, z);
        }
        public void Rotate(float x, float y, float z)
        {
            eulerAngles += new Vector3(x, y, z);

            Quaternion qx, qy, qz;

            qx = Quaternion.RotationAxis(new Vector3(1, 0, 0), eulerAngles.X);
            qy = Quaternion.RotationAxis(new Vector3(0, 1, 0), eulerAngles.Y);
            qz = Quaternion.RotationAxis(new Vector3(0, 0, 1), eulerAngles.Z);

            rotation = qx * qy * qz;
        }
    }
}
