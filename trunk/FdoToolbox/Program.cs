using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FdoToolbox.Core;

namespace FdoToolbox
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
            FormMain frm = new FormMain();
            HostApplication app = HostApplication.Instance;
            app.Initialize(frm);
            app.Run();
        }
    }
}