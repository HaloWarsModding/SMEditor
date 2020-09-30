using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor
{
    public class IUpdateable
    {
        public IUpdateable()
        {
            MainWindow.updateables.Add(this);
        }
        public virtual void Update() { }
    }
}
