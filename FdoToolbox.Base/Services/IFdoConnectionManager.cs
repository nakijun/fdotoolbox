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
        /// <summary>
        /// Adds a FDO connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="conn">The connection.</param>
        void AddConnection(string name, FdoConnection conn);
        /// <summary>
        /// Removes the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        void RemoveConnection(string name);
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
        /// <summary>
        /// Determines whether a connection exists (by name)
        /// </summary>
        /// <param name="name">The name of the connection</param>
        /// <returns></returns>
        bool NameExists(string name);
        /// <summary>
        /// Refreshes the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        void RefreshConnection(string name);
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <returns></returns>
        FdoConnection GetConnection(string name);
        /// <summary>
        /// Gets the connection by provider and connection string
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The connection string.</param>
        /// <returns></returns>
        FdoConnection GetConnection(string provider, string connStr);
        /// <summary>
        /// Gets the connection names.
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetConnectionNames();
        /// <summary>
        /// Renames a connection.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        void RenameConnection(string oldName, string newName);
        /// <summary>
        /// Determines whether this this connection can be renamed to the specified name
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <returns></returns>
        ConnectionRenameResult CanRenameConnection(string oldName, string newName);
        /// <summary>
        /// Occurs before a connection is removed. Subscribers have the opportunity to 
        /// cancel the removal operation by setting the Cancel property of the 
        /// <see cref="ConnectionBeforeRemoveEventArgs"/> object to true
        /// </summary>
        event ConnectionBeforeRemoveHandler BeforeConnectionRemove;
        /// <summary>
        /// Occurs when a connection is added
        /// </summary>
        event ConnectionEventHandler ConnectionAdded;
        /// <summary>
        /// Occurs when a connection is removed
        /// </summary>
        event ConnectionEventHandler ConnectionRemoved;
        /// <summary>
        /// Occurs when a connection is renamed
        /// </summary>
        event ConnectionRenamedEventHandler ConnectionRenamed;
        /// <summary>
        /// Occurs when a connection is refreshed
        /// </summary>
        event ConnectionEventHandler ConnectionRefreshed;
        /// <summary>
        /// Gets the name of a connection
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        string GetName(FdoConnection fdoConnection);
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
    public delegate void ConnectionBeforeRemoveHandler(object sender, ConnectionBeforeRemoveEventArgs e);

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

    public class ConnectionBeforeRemoveEventArgs : CancelEventArgs
    {
        private readonly string _ConnectionName;

        public string ConnectionName
        {
            get { return _ConnectionName; }
        }

        public ConnectionBeforeRemoveEventArgs(string name)
        {
            _ConnectionName = name;
            this.Cancel = false;
        }
    }
}
