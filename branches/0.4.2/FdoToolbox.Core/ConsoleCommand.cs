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

namespace FdoToolbox.Core
{
    /// <summary>
    /// Base console command class. All console commands derive from
    /// this class.
    /// </summary>
    public abstract class ConsoleCommand : IConsoleCommand
    {
        public abstract int Execute();

        private bool _IsTestOnly;

        /// <summary>
        /// If true the command should run under simulation (no changes, if any,
        /// are performed)
        /// </summary>
        public bool IsTestOnly
        {
            get { return _IsTestOnly; }
            set { _IsTestOnly = value; }
        }

        private bool _IsSilent;

        /// <summary>
        /// If true, suppresses all console output. Check the status code returned
        /// by Execute() to determine successful execution.
        /// </summary>
        public bool IsSilent
        {
            get { return _IsSilent; }
            set { _IsSilent = value; }
        }

        protected void WriteLine(string str)
        {
            if (!IsSilent)
                AppConsole.WriteLine(str);
        }

        protected void Write(string str)
        {
            if (!IsSilent)
                AppConsole.Write(str);
        }

        protected void WriteLine(string format, params object[] args)
        {
            if (!IsSilent)
                AppConsole.WriteLine(format, args);
        }

        protected void Write(string format, params object[] args)
        {
            if (!IsSilent)
                AppConsole.Write(format, args);
        }

        protected void WriteException(Exception ex)
        {
            if (!IsSilent)
                AppConsole.WriteException(ex);
        }

        protected void WriteError(string format, params object[] args)
        {
            if (!IsSilent)
                AppConsole.Err.WriteLine(format, args);
        }

        protected void WriteError(string str)
        {
            if (!IsSilent)
                AppConsole.Err.WriteLine(str);
        }
    }
}
