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
        public Terraformer() : base("Terraformer") { }

        ToolOptionSlider radius = new ToolOptionSlider("Radius");
        ToolOptionSlider intensity = new ToolOptionSlider("Intensity");
        public override void Enable()
        {
            base.Enable();

            intensity.Register(5);

            radius.tb.Maximum = 50;
            radius.tb.Minimum = 1;
            radius.tb.Value = 12;
            radius.Register(43);
        }
        public override void Disable()
        {
            base.Disable();
            intensity.Unregister();
            radius.Unregister();
        }

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
                    foreach(int i in World.terrain.GetVertsInRadius(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, radius.tb.Value))
                    {
                        World.terrain.EditVertexHeight(i, .10F, Terrain.EditMode.Add);
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
                    foreach (int i in World.terrain.GetVertsInRadius(World.terrain.dMesh.GetTriangle(World.cursor.currHitTri).a, radius.tb.Value))
                    {
                        World.terrain.EditVertexHeight(i, -.10F, Terrain.EditMode.Add);
                    }
                    
                    World.terrain.UpdateVisual();
                    collisionMeshUpdateNeeded = true;
                }
            }

            
            if (released && collisionMeshUpdateNeeded)
            {
                collisionMeshUpdateNeeded = false;
                World.terrain.UpdateCollisionModel();
                World.terrain.UpdateNormalsFinal();
            }
        }
    }
}
