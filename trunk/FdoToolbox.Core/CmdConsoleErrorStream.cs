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
    public class CmdConsoleErrorStream : IConsoleOutputStream
    {
        public System.Drawing.Color TextColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private bool _timestamp;

        public bool TimestampEntries
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }

        public void Write(string s)
        {
            string str = s;
            if (this.TimestampEntries)
            {
                DateTime dt = DateTime.Now;
                str = string.Format("[{0}] {1}", dt.ToString(), s);
            }
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(s);
            Console.ForegroundColor = prevColor;
        }

        public void Write(string fmt, params object[] args)
        {
            string str = fmt;
            if (this.TimestampEntries)
            {
                DateTime dt = DateTime.Now;
                str = string.Format("[{0}] {1}", dt.ToString(), fmt);
            }
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(str, args);
            Console.ForegroundColor = prevColor;
        }

        public void WriteLine(string s)
        {
            string str = s;
            if (this.TimestampEntries)
            {
                DateTime dt = DateTime.Now;
                str = string.Format("[{0}] {1}", dt.ToString(), s);
            }
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ForegroundColor = prevColor;
        }

        public void WriteLine(string fmt, params object[] args)
        {
            string str = fmt;
            if (this.TimestampEntries)
            {
                DateTime dt = DateTime.Now;
                str = string.Format("[{0}] {1}", dt.ToString(), fmt);
            }
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str, args);
            Console.ForegroundColor = prevColor;
        }
    }
}
