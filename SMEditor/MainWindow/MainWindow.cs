using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SMEditor.Editor;
using SMEditor.Editor.Tools;
using SMEditor.Editor.Layout;
using Syncfusion.Windows.Forms.Tools;

namespace SMEditor
{
    public partial class MainWindowOld : Form
    {
        public MainWindowOld()
        {
            InitializeComponent();
        }

        public ViewportPanel viewport;
        public PropertiesPanel properties;

        private void PRELOAD(object sender, EventArgs e)
        {
            viewport = new ViewportPanel(false);
            properties = new PropertiesPanel(false);

            //init panels
            viewport.Init();
            properties.Init();

            dockingManager.DockControl(viewport.p, this, DockingStyle.Right, 700);
            dockingManager.EnableContextMenu = true;
        }
        private void POSTLOAD()
        {            
            //init static systems
            Renderer.Init();
            Input.Init();
            ToolDock.Init();

            renderTimer.Interval = 2; //ms
            renderTimer.Tick += new EventHandler(timer_Tick);
            renderTimer.Start();

            //init world
            World.terrain = new Terrain(256);
            World.cursor = new _3dCursor();

            d3d11loaded = true;
        }
        private void CLOSING(object sender, FormClosingEventArgs e)
        {

        }

        bool d3d11loaded = false;
        Timer renderTimer = new Timer();
        private void d3D11Control_Load(object sender, EventArgs e)
        {
        }
        private void d3D11Control_Resize(object sender, EventArgs e)
        {
            if(d3d11loaded) Renderer.mainCamera.UpdateProjMatrix();
        }

        public static List<IUpdateable> updateables = new List<IUpdateable>();
        private void timer_Tick(object sender, EventArgs e)
        {
            Input.Poll();
            foreach(IUpdateable u in updateables) u.Update();
            World.Update();
            Renderer.Draw();
        }
    }
}
