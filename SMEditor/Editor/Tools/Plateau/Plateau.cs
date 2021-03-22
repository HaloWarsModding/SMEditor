using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor.Tools
{
    class Plateau : Tool
    {
        public Plateau() : base("Plateau") { }

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
                    foreach (int i in World.terrain.GetVertsInRadius(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, 12))
                    {
                        World.terrain.EditVertexHeight(i, .10F, Terrain.EditMode.Set);
                    }

                    World.terrain.UpdateVisual();
                    collisionMeshUpdateNeeded = true;
                }
            }
            if (Input.RMBPressed)
            {
                released = false;
                if (World.cursor.hitInfoExists)
                {
                    foreach (int i in World.terrain.GetVertsInRadius(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, 12))
                    {
                        World.terrain.EditVertexHeight(i, -.10F, Terrain.EditMode.Set);
                    }

                    World.terrain.UpdateVisual();
                    collisionMeshUpdateNeeded = true;
                }
            }


            if (released && collisionMeshUpdateNeeded)
            {
                collisionMeshUpdateNeeded = false;
                World.terrain.UpdateCollisionModel();
            }
        }
    }
}
