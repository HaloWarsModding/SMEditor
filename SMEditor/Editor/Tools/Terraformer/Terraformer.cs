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
            if (World.cursor.hitInfoExists)
            {
                Vector3d v1 = World.terrain.dMesh.GetVertex(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a);
                Vector3d v2 = World.terrain.dMesh.GetVertex(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).b);
                Vector3d v3 = World.terrain.dMesh.GetVertex(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).c);

                World.terrain.dMesh.SetVertex(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).c, v3 + new Vector3d(0, 0.1F, 0));

                World.terrain.dMeshAABB.Build();
                World.terrain.UpdateVisual();
            }
        }
    }
}
