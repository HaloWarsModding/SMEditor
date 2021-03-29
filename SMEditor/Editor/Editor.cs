using SlimDX;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;
using SMEditor.Editor.Tools;
using SMEditor;
using SMEditor.Editor.Layout;
using SlimDX.Direct3D11;

namespace SMEditor.Editor
{
    public static class Editor
    {
        //Editor
        public static _3dCursor cursor;
        public static void ClearCursorState()
        {
            //Basically, deselect everything so that there is no issues with selecting something else.
            foreach (Tool t in ToolDock.tools.Values) t.Disable();
        }

        //Scenario
        public static HWDEScenario scenario;
        public static void LoadNewProject(HWDEScenarioSize s)
        {
            if (projectLoaded) UnloadProject();
            scenario = new HWDEScenario(s);
            projectLoaded = true;
        }
        public static void UnloadProject()
        {
            scenario.Dispose();
            projectLoaded = false;
        }

        //Panels
        public static ViewportPanel viewportPanel;
        public static PropertiesPanel propertiesPanel;
        public static OutputPanel outputPanel;

        public static bool mouseInBounds = false;
        public static bool loopCursorInBounds = false;
        public static bool freeze3dCursor = false;
        public static bool projectLoaded = false;

        public static void Update()
        {
            if (!projectLoaded) return;

            if(!freeze3dCursor) cursor.UpdatePositionOnTerrain();

            mouseInBounds = true;

            if (Input.mouseAbsPosScreen.X < 0)
            {
                if (loopCursorInBounds) Cursor.Position = new System.Drawing.Point(Renderer.viewport.PointToScreen(new System.Drawing.Point(Renderer.viewport.Width, 0)).X, Cursor.Position.Y);
                if (!loopCursorInBounds) mouseInBounds = false;
            }
            if (Input.mouseAbsPosScreen.X > Renderer.viewport.Width)
            {
                if (loopCursorInBounds) Cursor.Position = new System.Drawing.Point(Renderer.viewport.PointToScreen(new System.Drawing.Point(0, 0)).X, Cursor.Position.Y);
                if (!loopCursorInBounds) mouseInBounds = false;
            }

            if (Input.mouseAbsPosScreen.Y < 0)
            {
                if (loopCursorInBounds) Cursor.Position = new System.Drawing.Point(Cursor.Position.X, Renderer.viewport.PointToScreen(new System.Drawing.Point(0, Renderer.viewport.Height)).Y);
                if (!loopCursorInBounds) mouseInBounds = false;
            }
            if (Input.mouseAbsPosScreen.Y > Renderer.viewport.Height)
            {
                if (loopCursorInBounds) Cursor.Position = new System.Drawing.Point(Cursor.Position.X, Renderer.viewport.PointToScreen(new System.Drawing.Point(0, 0)).Y);
                if (!loopCursorInBounds) mouseInBounds = false;
            }

            ToolDock.UpdateAll();
        }
        public static void PreUILoad()
        {            
            //init panels
            viewportPanel = new ViewportPanel(false);
            propertiesPanel = new PropertiesPanel(false);
            outputPanel = new OutputPanel();

            viewportPanel.Init();
            propertiesPanel.Init();
            outputPanel.Init();
        }
        public static void PostUILoad()
        {
            //init static systems
            Renderer.Init();
            Input.Init();
            ToolDock.Init();

            //init world
            cursor = new _3dCursor();
            LoadNewProject(HWDEScenarioSize.small512);
        }
    }
}
