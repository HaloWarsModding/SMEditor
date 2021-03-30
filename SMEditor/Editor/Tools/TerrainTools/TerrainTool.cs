using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing.Drawing2D;
using Syncfusion.WinForms.Controls;

namespace SMEditor.Editor.Tools.TerrainTools
{
    public static class TerrainToolDock
    {
        public static Dictionary<string, TerrainTool> tools;
        public static void Init()
        {
            tools = new Dictionary<string, TerrainTool>();

            AddLabel("Sculpting:");
            tools.Add("Terraformer", new Terraformer());
            tools.Add("Plateau", new Plateau());

            AddSeparator();
            AddLabel(" Texturing:");
            tools.Add("Texture Painter", new TexturePainter());

            tools["Texture Painter"].button.PerformClick();
        }

        public static void UpdateAll()
        {
            foreach (TerrainTool t in tools.Values) t.Update();
        }

        private static void AddLabel(string text = "|")
        {
            ToolStripLabel l = new ToolStripLabel();
            l.Name = "spacer";
            l.Text = text;
            l.Size = new System.Drawing.Size(24, 24);
            l.DisplayStyle = ToolStripItemDisplayStyle.Text;
            l.AutoToolTip = false;
            Editor.viewportPanel.tb.Items.Add(l);
        }
        private static void AddSeparator()
        {
            Editor.viewportPanel.tb.Items.Add(new ToolStripSeparator());
        }
    }

    public class TerrainTool : IUpdateable
    {
        public TerrainTool(string n, string imageNameOverride = "")
        {
            if (imageNameOverride == "") imageNameOverride = n;
            button.Name = n;
            button.Text = n;
            button.Size = new System.Drawing.Size(24, 24);
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button.Image = System.Drawing.Image.FromFile("Thumbs/tools/" + imageNameOverride + ".png");
            button.Click += new EventHandler(button_click);
            Editor.viewportPanel.tb.Items.Add(button);
        }
        protected bool enabled = false;
        public virtual void Enable()
        {
            button.Checked = true;
            enabled = true;
        }
        public virtual void Disable()
        {
            Editor.propertiesPanel.ClearProperties();
            button.Checked = false;
            enabled = false;
        }
        public virtual void PerformFunction() { }
        public override void Update()
        {
            if (enabled && Editor.mouseInBounds) PerformFunction();
        }

        public ToolStripButton button = new ToolStripButton();
        private void button_click(object o, EventArgs e)
        {
            Editor.ClearCursorState();
            Enable();
        }
    }
}
