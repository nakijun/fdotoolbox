#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core;
using System.ComponentModel;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// Connection manager interface
    /// </summary>
    public interface IFdoConnectionManager : IService
    {
        void AddConnection(string name, FdoConnection conn);
        void RemoveConnection(string name);
        void Clear();
        bool NameExists(string name);
        void RefreshConnection(string name);
        FdoConnection GetConnection(string name);
        FdoConnection GetConnection(string provider, string connStr);
        ICollection<string> GetConnectionNames();
        void RenameConnection(string oldName, string newName);
        ConnectionRenameResult CanRenameConnection(string oldName, string newName);

        event ConnectionBeforeRemoveHandler BeforeConnectionRemove;
        event ConnectionEventHandler ConnectionAdded;
        event ConnectionEventHandler ConnectionRemoved;
        event ConnectionRenamedEventHandler ConnectionRenamed;
        event ConnectionEventHandler ConnectionRefreshed;
    }

    public class ConnectionRenameResult
    {
        public readonly bool CanRename;
        public readonly string Reason;

        public ConnectionRenameResult()
        {
            this.CanRename = true;
        }

        public ConnectionRenameResult(bool result, string reason)
        {
            this.CanRename = result;
            this.Reason = reason;
        }
    }

    public delegate void ConnectionEventHandler(object sender, EventArgs<string> e);
    public delegate void ConnectionRenamedEventHandler(object sender, ConnectionRenameEventArgs e);
    public delegate void ConnectionBeforeRemoveHandler(object sender, ConnectionBeforeRenameEventArgs e);

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
            _NewName = newName;
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
