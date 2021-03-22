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
using SMEditor.Scenario.HWDE;

namespace SMEditor
{
    public partial class MainWindowOld : Form
    {
        public MainWindowOld()
        {
            InitializeComponent();
        }

        bool loaded = false;
        Timer renderTimer = new Timer();
        private void d3D11Control_Load(object sender, EventArgs e)
        {
            Renderer.Init();
            Input.Init();
            ToolDock.Init();

            renderTimer.Interval = 2; //ms
            renderTimer.Tick += new EventHandler(timer_Tick);
            renderTimer.Start();
            
            World.terrain = new Terrain(256);
            World.cursor = new _3dCursor();

            loaded = true;
        }

        private void d3D11Control_Resize(object sender, EventArgs e)
        {
            if(loaded) Renderer.mainCamera.UpdateProjMatrix();
        }


        public static List<IUpdateable> updateables = new List<IUpdateable>();
        private void timer_Tick(object sender, EventArgs e)
        {
            Input.Poll();
            foreach(IUpdateable u in updateables) u.Update();
            World.Update();
            Renderer.Draw();
        }

        private void rangeSlider1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void MainWindowOld_Load(object sender, EventArgs e)
        {

        }

        private void rangeSlider1_ValueChanged_1(object sender, EventArgs e)
        {

        }
    }
}
