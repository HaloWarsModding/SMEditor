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
                    foreach(int i in World.terrain.GetVertsInRadius(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, 12))
                    {
                        World.terrain.SetVertexPosition(i, new Vector3d(0, 0.1F, 0));
                    }

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
