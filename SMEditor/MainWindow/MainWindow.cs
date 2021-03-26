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
        //Loading
        bool uiLoaded = false;
        public MainWindowOld()
        {
            InitializeComponent();
        }
        private void PRELOAD(object sender, EventArgs e)
        {
            Editor.Editor.PreUILoad();

            //TODO: Replace this with a proper layout manager.
            dockingManager.DockControl(Editor.Editor.viewportPanel.p, this, DockingStyle.Right, 700);
            dockingManager.EnableContextMenu = false;
        }
        private void POSTLOAD()
        {
            //Rendering interval init
            renderTimer.Interval = 1; //ms
            renderTimer.Tick += new EventHandler(timer_Tick);
            renderTimer.Start();

            //Comes after renderTimer init
            Editor.Editor.PostUILoad();

            uiLoaded = true;
        }
        private void CLOSING(object sender, FormClosingEventArgs e)
        {

        }

        //d3D11Control
        //TODO: move the d3D11Control to the Renderer class.
        private void d3D11Control_Resize(object sender, EventArgs e)
        {
            if(uiLoaded) Renderer.mainCamera.UpdateProjMatrix();
        }

        //Frame Interval
        //TODO MAYBE: Move this functionality to the Editor class?
        Timer renderTimer = new Timer();
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
