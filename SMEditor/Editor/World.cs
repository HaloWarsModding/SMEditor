using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace SMEditor.Editor
{
    public static class World
    {
        public static Terrain terrain;
        public static _3dCursor cursor;

        public static bool CastRay(Vector2 start, Vector3 dir)
        {
            Matrix mvp = Camera.cbData.viewMatrix * Camera.cbData.projMatrix;
            Vector3 ray = Vector3.Unproject(dir, 1F, start.Y, 1F, 1F, 0F, 1F, mvp);

            Console.WriteLine("R " + ray);
            Console.WriteLine("T " + Renderer.mainCamera.t.position);

            return false;
        }
    }
}
