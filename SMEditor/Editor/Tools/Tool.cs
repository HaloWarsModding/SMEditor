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
            Program.mainWindow.contextToolBar.Items.Add(button);
        }
        protected bool enabled = false;
        public virtual void Enable() { enabled = true; }
        public virtual void Disable() { enabled = false; }
        public virtual void PerformFunction() { }
        public override void Update()
        {
            if (enabled && World.mouseInBounds) PerformFunction();
        }

        public ToolStripButton button = new ToolStripButton();
        private void button_click(object o, EventArgs e)
        {
            foreach (Tool t in ToolDock.tools.Values) t.Disable();
            Enable();
        }
    }
    

    public class ToolOption
    {
        public virtual void Register(int heightOffset) { }
        public virtual void Unregister() { }
    }

    public class ToolOptionSlider : ToolOption
    {
        public ToolOptionSlider(string _name)
        {
            l.Text = _name;

            tb.Height = 45;
            tb.Style = TrackBarEx.Theme.Office2016Colorful;
            tb.ValueChanged += new EventHandler(tbVal_changed);
            
            n.ReadOnly = true;
        }
        public override void Register(int heightOffset)
        {
            tb.Height = 45;
            tb.Width = 232;
            tb.Location = new System.Drawing.Point(58, 40 + heightOffset);
            tb.BringToFront();
            Program.mainWindow.layout.Panel1.Controls.Add(tb);
            tb.Anchor = AnchorStyles.Right | AnchorStyles.Top;


            n.Width = 50;
            n.Location = new System.Drawing.Point(8, 40 + heightOffset);
            n.Parent = Program.mainWindow.layout.Panel1;
            Program.mainWindow.layout.Panel1.Controls.Add(n);


            //last
            l.BackColor = System.Drawing.Color.Transparent;
            l.Location = new System.Drawing.Point(8, 25 + heightOffset);
            Program.mainWindow.layout.Panel1.Controls.Add(l);
        }
        public override void Unregister()
        {
            Program.mainWindow.layout.Panel1.Controls.Remove(tb);
            Program.mainWindow.layout.Panel1.Controls.Remove(l);
            Program.mainWindow.layout.Panel1.Controls.Remove(n);
        }
        
        public TrackBarEx tb = new TrackBarEx();
        public Label l = new Label();
        public NumericTextBox n = new NumericTextBox();

        private void tbVal_changed(object o, EventArgs e)
        {
            n.Text = tb.Value.ToString();
        }
    }
}
