using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMEditor.Editor.Layout
{
    class Writer : TextWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
        public override void Write(char value)
        {
            if (value == ' ' || value == '\n') return;

            Program.mainWindow.outputPanel.lb.Items.Add(value);
            if (Program.mainWindow.outputPanel.lb.Items.Count > 100)
            {
                Program.mainWindow.outputPanel.lb.Items.RemoveAt(0);
            }
        }
        public override void Write(string value)
        {
            if (value == " "  || value == "\n" || value == "") return;

            Program.mainWindow.outputPanel.lb.Items.Add(value);
            if (Program.mainWindow.outputPanel.lb.Items.Count > 100)
            {
                Program.mainWindow.outputPanel.lb.Items.RemoveAt(0);
            }
        }
    }

    public class OutputPanel : DockableWindow
    {
        public OutputPanel() : base("Output Log", false) { }
        public ListBox lb;
        public override void Init()
        {
            base.Init();

            lb = new ListBox();
            lb.Location = new System.Drawing.Point(5, 3);
            lb.Size = new System.Drawing.Size(p.Width - 10, p.Height);
            lb.HorizontalScrollbar = true;
            lb.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            p.Controls.Add(lb);

            Console.SetOut(new Writer());
        }
    }
}
