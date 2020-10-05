using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace SMEditor.Editor.Tools
{
    public class Terraformer : Tool
    {
        public override void PerformFunction()
        {
            //World.CastRay(new Vector2(0, 0), Vector3.Normalize((Renderer.mainCamera.t.position - Renderer.mainCamera.cameraTarget)));

            Vector3d pos = new Vector3d(Renderer.mainCamera.t.position.X, Renderer.mainCamera.t.position.Y, Renderer.mainCamera.t.position.Z);
            Vector3 dirSDX = Vector3.Normalize((Renderer.mainCamera.t.position - Renderer.mainCamera.cameraTarget) - Renderer.mainCamera.cameraTarget);
            Vector3d dir = new Vector3d(-dirSDX.X, -dirSDX.Y, -dirSDX.Z);
            Ray3d ray = new Ray3d(pos, dir);
            foreach (TerrainChunk tc in World.chunks)
            {
                int hitId = tc.dMeshAABB.FindNearestHitTriangle(ray);
                
            }
        }
    }
}
