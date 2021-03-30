using SMEditor.Editor.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor.Tools.TerrainTools
{
    class Plateau : TerrainTool
    {
        SliderProperty radius = new SliderProperty("Radius", 12, 0, 50);
        SliderProperty height = new SliderProperty("Height", 0, -999, 999, DecimalPlaceCount.Single);
        public Plateau() : base("Plateau") { }

        bool released = false;
        bool collisionMeshUpdateNeeded = false;
        public override void PerformFunction()
        {
            return;

            released = true;
            if (Input.LMBPressed)
            {
                released = false;
                if (Editor.cursor.hitInfoExists)
                {
                    //foreach (int i in Editor.scenario.terrain.GetVertsInRadius(Editor.scenario.terrain.dMesh.GetTriangle(Editor.cursor.currHitTri).a, radius.GetValue()))
                    //{
                    //    Editor.scenario.terrain.EditVertexHeight(i, height.GetValue(), Terrain.EditMode.Set);
                    //}

                    Editor.scenario.terrain.UpdateRequiredVisuals();
                    collisionMeshUpdateNeeded = true;
                }
            }
            if (Input.RMBPressed)
            {
                released = false;
                if (Editor.cursor.hitInfoExists)
                {
                    //foreach (int i in Editor.scenario.terrain.GetVertsInRadius(Editor.scenario.terrain.dMesh.GetTriangle(Editor.cursor.currHitTri).a, radius.GetValue()))
                    //{
                    //    Editor.scenario.terrain.EditVertexHeight(i, 0, Terrain.EditMode.Set);
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
        public override void Enable()
        {
            base.Enable();
            Editor.propertiesPanel.SetProperties(SelectedType.Tool, "Plateau", new PropertyField[2] { radius, height });
        }
        public override void Disable()
        {
            base.Disable();
            Editor.propertiesPanel.ClearProperties();
        }
    }
}
