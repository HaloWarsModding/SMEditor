using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMEditor;

namespace SMEditor
{
    static class Program
    {
        public static MainWindowOld mainWindow;

        [STAThread]
        public static void Main(string[] args)
        {
            mainWindow = new MainWindowOld();
            Application.Run(mainWindow);
        }
    }
}
