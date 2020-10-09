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

namespace SMEditor.Editor
{
    public static class World
    {
        public static Terrain terrain;
        public static _3dCursor cursor;

        public static bool mouseInBounds = false;
        public static bool loopCursorInBounds = false;
        public static bool freeze3dCursor = false;

        public static void Update()
        {
            if (Input.KeyIsDown(SlimDX.DirectInput.Key.Escape)) loopCursorInBounds = !loopCursorInBounds;

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
    }
}
