using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.DirectInput;
using SlimDX.RawInput;

namespace SMEditor.Editor
{
    static class Input
    {
        private static DirectInput input;
        private static Keyboard keyboard;
        private static Mouse mouseRel;

        public static void Init()
        {
            input = new DirectInput();
            keyboard = new Keyboard(input);
            keyboard.SetCooperativeLevel(Renderer.viewport.ParentForm, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
            keyboard.Acquire();

            mouseRel = new Mouse(input);
            mouseRel.SetCooperativeLevel(Renderer.viewport.ParentForm, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
            mouseRel.Acquire();
        }

        static Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();
        public static bool LMBPressed = false;
        public static bool RMBPressed = false;
        public static bool MMBPressed = false;
        public static Vector3 mouseDeltaPos = Vector3.Zero;
        public static Vector2 mouseAbsPos = new Vector2();

        public static void Poll()
        {
            KeyboardState kstate = keyboard.GetCurrentState();
            foreach(Key k in kstate.AllKeys)
            {
                if (kstate.IsPressed(k)) keyStates[k] = true;
                if (kstate.IsReleased(k)) keyStates[k] = false;
            }
            
            MouseState mState = mouseRel.GetCurrentState();
            LMBPressed = mState.GetButtons()[0];
            RMBPressed = mState.GetButtons()[1];
            MMBPressed = mState.GetButtons()[2];
            mouseDeltaPos = new Vector3(mState.X, mState.Y, mState.Z);
            
            mouseAbsPos.X = (Renderer.viewport.PointToClient(SlimDX.Windows.RenderForm.MousePosition).X - (Renderer.viewport.Width / 2F)) / (float)Renderer.viewport.Width;
            mouseAbsPos.Y = -(Renderer.viewport.PointToClient(SlimDX.Windows.RenderForm.MousePosition).Y - (Renderer.viewport.Height / 2F)) / (float)Renderer.viewport.Height;
        }
        public static bool KeyIsDown(Key k)
        {
            return keyStates[k];
        }
    }
}
