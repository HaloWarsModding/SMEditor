using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using g3;

namespace SMEditor
{
    static class Convert
    {
        public static Vector3 ToV3(Vector3d v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }
        public static Vector3d ToV3d(Vector3 v)
        {
            return new Vector3d(v.X, v.Y, v.Z);
        }
    }
}
