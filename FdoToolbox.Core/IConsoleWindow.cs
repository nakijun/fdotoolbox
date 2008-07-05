using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Core
{
    public delegate void ConsoleInputHandler(string input);

    public interface IConsoleWindow
    {
        event ConsoleInputHandler ConsoleInput;
        TextBoxBase TextWindow { get; }
        TextBoxBase InputTextBox { get; }
    }
}
