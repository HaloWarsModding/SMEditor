﻿using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;
using SMEditor.Editor.Layout;

namespace SMEditor.Editor.Tools
{
    public class Terraformer : Tool
    {
        public Terraformer() : base("Terraformer") { }

        SliderProperty radius = new SliderProperty("Radius", 25, 0, 150);
        SliderProperty intensity = new SliderProperty("Intensity", 1, 0, 2);
        public override void Enable()
        {
            base.Enable();
            Editor.propertiesPanel.SetProperties(SelectedType.Tool, "Terraformer", 
                new PropertyField[2] { radius, intensity });
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

                foreach (Terrain.TerrainIndexMapping tim in Editor.scenario.terrain.GetVertsInRadius(Editor.cursor.t.position, radius.GetValue()))
                {
                    Editor.scenario.terrain.EditVertexHeight(tim, .10f, Terrain.EditMode.Add);
                }

                Editor.scenario.terrain.UpdateVisual();
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
                    
                    Editor.scenario.terrain.UpdateVisual();
                    collisionMeshUpdateNeeded = true;
                }
            }

            
            if (released && collisionMeshUpdateNeeded)
            {
                collisionMeshUpdateNeeded = false;
                Editor.scenario.terrain.UpdateCollisionModel();
            }
        }
    }
}
