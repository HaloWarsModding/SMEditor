using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;
using SMEditor.Editor.Layout;

namespace SMEditor.Editor.Tools.TerrainTools
{
    public class Terraformer : TerrainTool
    {
        public Terraformer() : base("Terraformer") { }

        SliderProperty radius = new SliderProperty("Radius", 25, 0, 150);
        SliderProperty intensity = new SliderProperty("Intensity", 1, 0, 2);
        public override void Enable()
        {
            base.Enable();
            Editor.propertiesPanel.AddProperty(SelectedType.Tool, "Terraformer", 
                new PropertyField[] { radius, intensity });
        }
        public override void Disable()
        {
            base.Disable();
            Editor.propertiesPanel.ClearProperties();
        }

        bool released = false;
        bool collisionMeshUpdateNeeded = false;
        public override void PerformFunction()
        {
            released = true;
            if (Input.LMBPressed)
            {
                released = false;

                foreach (Terrain.TerrainIndexMapping tim in Editor.scenario.terrain.GetVertsInBrush(Editor.cursor.t.position, radius.GetValue()))
                {
                    Editor.scenario.terrain.EditVertexHeight(tim, .10f, Terrain.EditMode.Add);
                }

                Editor.scenario.terrain.UpdateRequiredVisuals();
                collisionMeshUpdateNeeded = true;
            }
            if (Input.RMBPressed)
            {
                released = false;
                if (Editor.cursor.hitInfoExists)
                {
                    //foreach (int i in Editor.scenario.terrain.GetVertsInRadius(Editor.scenario.terrain.dMesh.GetTriangle(Editor.cursor.currHitTri).a, radius.GetValue()))
                    //{
                    //    Editor.scenario.terrain.EditVertexHeight(i, -.10F, Terrain.EditMode.Add);
                    //}
                    
                    Editor.scenario.terrain.UpdateRequiredVisuals();
                    collisionMeshUpdateNeeded = true;
                }
            }

            
            if (released && collisionMeshUpdateNeeded)
            {
                collisionMeshUpdateNeeded = false;
                Editor.scenario.terrain.UpdateRequiredCollisionModels();
            }
        }
    }
}
