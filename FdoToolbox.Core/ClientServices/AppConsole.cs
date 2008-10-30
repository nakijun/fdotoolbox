#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.IO;

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Application console class. Output is redirected
    /// to the application console window when running the FdoToolbox application.
    /// Otherwise output is redirected to stdout
    /// </summary>
    public sealed class AppConsole
    {
        private AppConsole() { }

        private static IConsoleInputStream _In;

        /// <summary>
        /// The console input stream
        /// </summary>
        public static IConsoleInputStream In
        {
            get { return _In; }
            set { _In = value; }
        }
        private static IConsoleOutputStream _Out;

        /// <summary>
        /// The console output stream
        /// </summary>
        public static IConsoleOutputStream Out
        {
            get { return _Out; }
            set { _Out = value; }
        }
        private static IConsoleOutputStream _Err;

        /// <summary>
        /// The console error stream
        /// </summary>
        public static IConsoleOutputStream Err
        {
            get { return _Err; }
            set { _Err = value; }
        }

        /// <summary>
        /// Writes a formatted string to the console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(string format, params object[] args)
        {
            Out.WriteLine(format, args);
        }

        /// <summary>
        /// Writes an object to the console
        /// </summary>
        /// <param name="obj"></param>
        public static void WriteLine(object obj)
        {
            Out.WriteLine(obj.ToString());
        }

        /// <summary>
        /// Displays an alert message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public static void Alert(string title, string msg)
        {
            Alert(title, msg, true);
        }

        /// <summary>
        /// Writes an exception to the console
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteException(Exception ex)
        {
            Err.WriteLine(ex.ToString());
        }

        /// <summary>
        /// Writes a formatted string to the console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object [] args)
        {
            Out.Write(format, args);
        }

        /// <summary>
        /// Displays an alert message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="writeToConsole"></param>
        public static void Alert(string title, string message, bool writeToConsole)
        {
            if (DoAlert != null)
                DoAlert(new MessageEventArgs(title, message));
            else
                throw new ApplicationException("DoAlert handler not initialized");
            if (writeToConsole)
                Out.WriteLine(message);
        }

        /// <summary>
        /// Displays a confirmation message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Confirm(string title, string text)
        {
            if (DoConfirm != null)
                return DoConfirm(new MessageEventArgs(title, text));

            throw new ApplicationException("DoConfirm handler not initialized");
        }

        /// <summary>
        /// Fires when an alert is displayed
        /// </summary>
        public static event AlertHandler DoAlert;

        /// <summary>
        /// Fires when confirmation is requested
        /// </summary>
        public static event ConfirmHandler DoConfirm;
    }
}
