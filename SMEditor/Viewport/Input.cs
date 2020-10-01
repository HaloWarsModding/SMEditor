using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.DirectInput;
using SlimDX.RawInput;

namespace SMEditor
{
    static class Input
    {
        private static DirectInput input;
        private static Keyboard keyboard;
        private static Mouse mouse;

        public static void Init()
        {
            input = new DirectInput();
            keyboard = new Keyboard(input);
            keyboard.SetCooperativeLevel(Renderer.viewport.ParentForm, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
            keyboard.Acquire();

            mouse = new Mouse(input);
            mouse.SetCooperativeLevel(Renderer.viewport.ParentForm, CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive);
            mouse.Acquire();
        }

        static Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();
        public static bool LMBPressed = false;
        public static bool RMBPressed = false;
        public static bool MMBPressed = false;
        public static Vector3 mousePos = Vector3.Zero;

        public static void Poll()
        {
            KeyboardState kstate = keyboard.GetCurrentState();
            foreach(Key k in kstate.AllKeys)
            {
                if (kstate.IsPressed(k)) keyStates[k] = true;
                if (kstate.IsReleased(k)) keyStates[k] = false;
            }

            MouseState mState = mouse.GetCurrentState();

            LMBPressed = mState.GetButtons()[0];
            RMBPressed = mState.GetButtons()[1];
            MMBPressed = mState.GetButtons()[2];

            mousePos = new Vector3(mState.X, mState.Y, mState.Z);
        }

        public static bool KeyIsDown(Key k)
        {
            return keyStates[k];
        }
    }
}
