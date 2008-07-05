using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Controls;

namespace FdoToolbox.Core
{
    public interface IShell : IFormWrapper
    {
        IObjectExplorer ObjectExplorer { get; }
        IConsoleWindow ConsoleWindow { get; }
        string Title { set; }

        void SetStatusBarText(string text);
        MenuStrip MainMenu { get; }
        ToolStripMenuItem GetRootMenuItem(string name);

        void ShowDocumentWindow(BaseDocumentCtl ctl);
    }
}
