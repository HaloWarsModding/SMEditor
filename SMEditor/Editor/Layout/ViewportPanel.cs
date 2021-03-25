using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMEditor.Editor.Layout
{
    public class ViewportPanel : DockableWindow
    {
        public ToolStrip tb;
        public ViewportPanel(bool showOnDefault) : base("Viewport", showOnDefault)
        {
            tb = new ToolStrip();
        }
        public override void Init()
        {
            p.Controls.Add(tb);
            tb.Items.Add("Tools:");
            p.Controls.Add(Program.mainWindow.d3D11Control);
            Program.mainWindow.d3D11Control.Parent = p;
            Program.mainWindow.d3D11Control.Size = new System.Drawing.Size(p.Width - 5, p.Height - 6);
        }
    }
}
