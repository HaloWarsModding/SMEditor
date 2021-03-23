using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMEditor.Editor
{
    public class DockableWindow
    {
        public ToolStripMenuItem tsmi;
        public Panel p;

        private string name;

        public DockableWindow(string _name, bool hideOnDefault)
        {
            //init
            tsmi = new ToolStripMenuItem();
            p = new Panel();

            if(!hideOnDefault) Program.mainWindow.dockingManager.SetEnableDocking(p, true);

            name = _name;
            Program.mainWindow.dockingManager.SetDockLabel(p, name);
            Program.mainWindow.dockingManager.SetMenuButtonVisibility(p, false);
            
            tsmi.CheckOnClick = true;
            tsmi.CheckedChanged += new EventHandler(TSMICheckChanged);

            //add to "Window" tab of tool strip
            tsmi.Text = name;
            tsmi.Name = name + "_menuItem";
            tsmi.Checked = !hideOnDefault;
            Program.mainWindow.windowToolStripMenuItem.DropDownItems.Add(tsmi);
        }
        public virtual void Init()
        {

        }

        private void TSMICheckChanged(object sender, EventArgs e)
        {
            Program.mainWindow.dockingManager.SetEnableDocking(p, tsmi.Checked);

            if (tsmi.Checked)
            {
                Program.mainWindow.dockingManager.DockControl(p, Program.mainWindow, Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom, 150);
                Program.mainWindow.dockingManager.SetMenuButtonVisibility(p, false);
                Program.mainWindow.dockingManager.SetCloseButtonVisibility(p, false);
                Program.mainWindow.dockingManager.SetDockLabel(p, name);
            }
        }
    }
}
