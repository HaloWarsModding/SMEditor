using SMEditor.Editor.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor.Tools.TerrainTools
{
    class TexturePainter : TerrainTool
    {
        SliderProperty opacity = new SliderProperty("Opacity", 255, 0, 255);
        EnumProperty texture = new EnumProperty("Texture", new string[]{"01", "02"}, 0);
        public TexturePainter() : base("Texture Painter", "paintbrush")
        {
        }

        bool released;
        public override void PerformFunction()
        {
            released = true;
            if (Input.LMBPressed)
            {
                released = false;

                foreach (Terrain.TerrainIndexMapping tim in Editor.scenario.terrain.GetVertsInRadius(Editor.cursor.t.position, 12))
                {
                    Editor.scenario.terrain.Paint(tim, (Terrain.TextureIndex)texture.GetActiveIndex(), (byte)opacity.GetValue());
                }

                Editor.scenario.terrain.UpdateRequiredPaints();
            }
        }

        public override void Enable()
        {
            base.Enable();
            Editor.propertiesPanel.SetProperties(SelectedType.Tool, "Texture Painter",
                new PropertyField[] { texture, opacity });
        }
        public override void Disable()
        {
            base.Disable();
        }
    }
}

