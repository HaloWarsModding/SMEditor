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

        public static void Init()
        {
            input = new DirectInput();
            keyboard = new Keyboard(input);
            keyboard.SetCooperativeLevel(Renderer.viewport.ParentForm, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
            keyboard.Acquire();
        }

        static Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();

        public static void Poll()
        {
            KeyboardState kstate = keyboard.GetCurrentState();
            foreach(Key k in kstate.AllKeys)
            {
                if (kstate.IsPressed(k)) keyStates[k] = true;
                if (kstate.IsReleased(k)) keyStates[k] = false;
            }
        }

        public static bool KeyIsDown(Key k)
        {
            return keyStates[k];
        }
    }
}
