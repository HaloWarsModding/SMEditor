using SMEditor.Editor.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
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

                foreach (Terrain.TerrainIndexMapping tim in Editor.scenario.terrain.GetVertsInBrush(Editor.cursor.t.position))
                {
                    Editor.scenario.terrain.Paint(tim, (Terrain.TextureIndex)texture.GetActiveIndex(), (byte)opacity.GetValue());
                }

                Editor.scenario.terrain.UpdateRequiredPaints();
            }
        }

        public override void Enable()
        {
            base.Enable();

            Editor.propertiesPanel.AddTitle(Image.FromFile("thumbs\\tool.png"), "Tool");

            Editor.propertiesPanel.AddLabel("Brush");
            Editor.propertiesPanel.AddProperty(Editor.scenario.terrain.brushShape);
            Editor.propertiesPanel.AddProperty(Editor.scenario.terrain.brushHeightType);
            Editor.propertiesPanel.AddProperty(Editor.scenario.terrain.brushRadius);
            Editor.propertiesPanel.AddProperty(Editor.scenario.terrain.brushFalloff);


            Editor.propertiesPanel.NewGroup();
            Editor.propertiesPanel.AddLabel("Painter");
            Editor.propertiesPanel.AddProperty(opacity);
            Editor.propertiesPanel.AddProperty(texture);

            Editor.propertiesPanel.Present();
        }
        public override void Disable()
        {
            base.Disable();
            Editor.propertiesPanel.Clear();
        }
    }
}

