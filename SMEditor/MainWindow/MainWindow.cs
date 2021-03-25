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
using System.Diagnostics;

namespace SMEditor
{
    public partial class MainWindowOld : Form
    {
        public MainWindowOld()
        {
            InitializeComponent();
        }

        public ViewportPanel viewportPanel;
        public PropertiesPanel propertiesPanel;
        public OutputPanel outputPanel;

        private void PRELOAD(object sender, EventArgs e)
        {
            //init panels
            viewportPanel = new ViewportPanel(false);
            propertiesPanel = new PropertiesPanel(false);
            outputPanel = new OutputPanel();

            viewportPanel.Init();
            propertiesPanel.Init();
            outputPanel.Init();

            dockingManager.DockControl(viewportPanel.p, this, DockingStyle.Right, 700);
            dockingManager.EnableContextMenu = false;
        }
        private void POSTLOAD()
        {            
            //init static systems
            Renderer.Init();
            Input.Init();
            ToolDock.Init();

            renderTimer.Interval = 1; //ms
            renderTimer.Tick += new EventHandler(timer_Tick);
            renderTimer.Start();

            //init world
            Editor.Editor.cursor = new _3dCursor();
            Editor.Editor.LoadNewProject(HWDEScenarioSize.small512);

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

        Stopwatch frameTimer = new Stopwatch();
        long time = 0; int frames = 0;
        public static List<IUpdateable> updateables = new List<IUpdateable>();
        private void timer_Tick(object sender, EventArgs e)
        {

            Input.Poll();
            foreach(IUpdateable u in updateables) u.Update();
            Editor.Editor.Update();
            Renderer.Draw();
            

            time += frameTimer.ElapsedMilliseconds;
            frames++;

            if(time >= 1000)
            {
                Text = "SMEditor [" + frames + " fps]";
                time = 0;
                frames = 0;
            }

            frameTimer.Restart();
        }
    }
}
