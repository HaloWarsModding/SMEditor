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

namespace SMEditor.Editor.Tools
{
    public static class ToolDock
    {
        public static Dictionary<string, Tool> tools;
        public static void Init()
        {
            tools = new Dictionary<string, Tool>()
            {
                { "Terraformer", new Terraformer() },
                { "Plateau", new Plateau() }
            };

            tools["Terraformer"].button.PerformClick();
        }

        public static void UpdateAll()
        {
            foreach (Tool t in tools.Values) t.Update();
        }
    }

    public class Tool : IUpdateable
    {
        public Tool(string n)
        {
            button.Name = n;
            button.Text = n;
            button.Size = new System.Drawing.Size(24, 24);
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button.Image = System.Drawing.Image.FromFile("Thumbs/tools/" + n + ".png");
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
