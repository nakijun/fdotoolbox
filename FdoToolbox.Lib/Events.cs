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
using System.ComponentModel;
using FdoToolbox.Lib.Modules;
using FdoToolbox.Core;

namespace FdoToolbox.Lib
{
    public delegate void ConsoleInputHandler(object sender, EventArgs<string> e);
    public delegate void CommandExecuteHandler();
    public delegate void ConnectionEventHandler(object sender, EventArgs<string> e);
    public delegate void ConnectionRenamedEventHandler(object sender, ConnectionRenameEventArgs e);
    public delegate void ConnectionBeforeRemoveHandler(object sender, ConnectionBeforeRenameEventArgs e);
    public delegate void TabTitleEventHandler(object sender, EventArgs<string> e);
    public delegate void ModuleEventHandler(object sender, EventArgs<IModule> module);

    public class ConnectionRenameEventArgs : EventArgs
    {
        private readonly string _OldName;

        public string OldName
        {
            get { return _OldName; }
        }

        private readonly string _NewName;

        public string NewName
        {
            get { return _NewName; }
        }

        public ConnectionRenameEventArgs(string oldName, string newName)
        {
            _OldName = oldName;
            _NewName = NewName;
        }
    }

    public class ConnectionBeforeRenameEventArgs : CancelEventArgs
    {
        private readonly string _ConnectionName;

        public string ConnectionName
        {
            get { return _ConnectionName; }
        }

        public ConnectionBeforeRenameEventArgs(string name)
        {
            _ConnectionName = name;
            this.Cancel = false;
        }
    }
}