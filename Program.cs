using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserNotify
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (Process.GetProcesses().Where(x => x.ProcessName.Equals("UserNotify")).Count() == 2)
            {
                Application.Exit();
            }
            else
            {
                Application.Run(new TaskTrayApplicationContext());
            }

            
        }
    }
}
