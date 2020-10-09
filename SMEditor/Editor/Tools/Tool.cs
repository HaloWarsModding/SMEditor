using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor.Tools
{
    public static class ToolDock
    {
        public static Dictionary<string, Tool> tools;
        public static void Init()
        {
            tools = new Dictionary<string, Tool>()
            {
                { "Terraformer", new Terraformer() }
            };

            tools["Terraformer"].enabled = true;
        }

        public static void UpdateAll()
        {
            foreach (Tool t in tools.Values) t.Update();
        }
    }

    public class Tool : IUpdateable
    {
        public bool enabled = false;
        public virtual void Enable() { enabled = true; }
        public virtual void Disable() { enabled = false; }
        public virtual void PerformFunction() { }
        public override void Update()
        {
            if (enabled) PerformFunction();
        }
    }
}
