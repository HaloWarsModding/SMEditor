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
using SMEditor.Scenario.HWDE;

namespace SMEditor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //d3d11Control
        bool loaded = false;
        private void d3D11Control_Load(object sender, EventArgs e)
        {
            Renderer.Init();
            Input.Init();

            renderTimer.Interval = 20; //ms
            renderTimer.Tick += new EventHandler(timer_Tick);
            renderTimer.Start();

            HWDEScenario s = new HWDEScenario(new HWDEScenarioDescription(
                HWDEScenarioDescription.Size._256x256, 
                HWDEScenarioDescription.SimResolution._4x));

            loaded = true;
        }
        private void d3D11Control_Resize(object sender, EventArgs e)
        {
            if(loaded) Renderer.mainCamera.UpdateProjMatrix();
        }

        //mainwindow
        private void window_GotFocus(object o, EventArgs e)
        {
            if(loaded) Input.Init();
        }
        private void window_LostFocus(object o, EventArgs e)
        {

        }


        Timer renderTimer = new Timer();
        public static List<IUpdateable> updateables = new List<IUpdateable>();
        private void timer_Tick(object sender, EventArgs e)
        {
            Input.Poll();
            foreach(IUpdateable u in updateables)
            {
                u.Update();
            }
            Renderer.Draw();
        }
    }
}
