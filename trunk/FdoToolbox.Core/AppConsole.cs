using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Application console class. Output is redirected
    /// to the application console window.
    /// </summary>
    public class AppConsole
    {
        private static ConsoleInputStream _In;

        public static ConsoleInputStream In
        {
            get { return _In; }
            set { _In = value; }
        }
        private static ConsoleOutputStream _Out;

        public static ConsoleOutputStream Out
        {
            get { return _Out; }
            set { _Out = value; }
        }
        private static ConsoleOutputStream _Err;

        public static ConsoleOutputStream Err
        {
            get { return _Err; }
            set { _Err = value; }
        }

        public static void WriteLine(string format, params object[] args)
        {
            Out.WriteLine(format, args);
        }

        public static void WriteLine(object obj)
        {
            Out.WriteLine(obj.ToString());
        }

        public static void Alert(string title, string msg)
        {
            Alert(title, msg, true);
        }

        public static void Execute(string cmdName)
        {
            try
            {
                Command cmd = HostApplication.Instance.ModuleManager.GetCommand(cmdName);
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Err.WriteLine("ERROR: " + ex.Message);
            }
        }

        public static void WriteException(Exception ex)
        {
            Err.WriteLine(ex.Message);
        }

        public static void Write(string format, params object [] args)
        {
            Out.Write(format, args);
        }

        public static void Alert(string title, string message, bool writeToConsole)
        {
            MessageBox.Show(message, title);
            if (writeToConsole)
                Out.WriteLine(message);
        }

        public static bool Confirm(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
    }
}
