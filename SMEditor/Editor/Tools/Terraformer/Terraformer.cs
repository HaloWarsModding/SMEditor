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
        bool released = false;
        bool collisionMeshUpdateNeeded = false;
        public override void PerformFunction()
        {
            released = true;
            if (Input.LMBPressed)
            {
                released = false;
                if (World.cursor.hitInfoExists)
                {
                    Console.WriteLine(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a);
                    //World.terrain.dMesh.SetVertex(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).c, v3 + new Vector3d(0, 0.1F, 0));

                    World.terrain.SetVertexPosition(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, Convert.ToV3d((World.terrain.vertices[World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a].position + new Vector3(0, 0.1F, 0))));

                    //World.terrain.dMeshAABB.Build();
                    World.terrain.UpdateVisual();

                    collisionMeshUpdateNeeded = true;
                }
            }
            if(released && collisionMeshUpdateNeeded)
            {
                collisionMeshUpdateNeeded = false;
                World.terrain.UpdateCollisionModel();
            }
        }
    }
}
