using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Common;
using System.Data;
using FdoToolbox.Core;

namespace FdoToolbox.Lib.ClientServices
{
    public class DbConnectionManager : IDbConnectionManager
    {
        private int counter;

        private Dictionary<string, DbConnectionInfo> _Connections;

        public DbConnectionManager() 
        {
            _Connections = new Dictionary<string, DbConnectionInfo>();
        }

        public void AddConnection(DbConnectionInfo conn)
        {
            string name = conn.Name;
            if (_Connections.ContainsKey(name))
                throw new DbConnectionException("A connection named " + name + " already exists");
            if (conn.Connection.State != ConnectionState.Open)
                conn.Connection.Open();
            _Connections.Add(name, conn);
            if (this.ConnectionAdded != null)
                this.ConnectionAdded(this, new EventArgs<string>(name));
        }

        public DbConnectionInfo GetConnection(string name)
        {
            if (!_Connections.ContainsKey(name))
                return null;

            return _Connections[name];
        }

        public ICollection<string> GetConnectionNames()
        {
            return _Connections.Keys;
        }

        public string CreateUniqueName()
        {
            return "Connection" + (counter++);
        }

        public void RemoveConnection(string name)
        {
            if (_Connections.ContainsKey(name))
            {
                if (this.BeforeConnectionRemove != null)
                {
                    ConnectionBeforeRenameEventArgs e = new ConnectionBeforeRenameEventArgs(name);
                    this.BeforeConnectionRemove(this, e);
                    if (e.Cancel)
                        return;
                }

                DbConnectionInfo conn = _Connections[name];
                if (conn.Connection.State != ConnectionState.Closed)
                    conn.Connection.Close();

                conn.Connection.Dispose();
                _Connections.Remove(name);
                if (this.ConnectionRemoved != null)
                    this.ConnectionRemoved(this, new EventArgs<string>(name));
            }
        }

        public void RenameConnection(string oldName, string newName)
        {
            if (!_Connections.ContainsKey(oldName))
                throw new DbConnectionException("The connection to be renamed could not be found: " + oldName);
            if (_Connections.ContainsKey(newName))
                throw new DbConnectionException("Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists");

            DbConnectionInfo conn = _Connections[oldName];
            _Connections.Remove(oldName);
            conn.Name = newName;
            _Connections.Add(newName, conn);

            if (this.ConnectionRenamed != null)
                this.ConnectionRenamed(this, new ConnectionRenameEventArgs(oldName, newName));
        }

        public bool CanRenameConnection(string oldName, string newName, ref string reason)
        {
            if (!_Connections.ContainsKey(oldName))
            {
                reason = "The connection to be renamed could not be found: " + oldName;
                return false;
            }
            if (_Connections.ContainsKey(newName))
            {
                reason = "Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists";
                return false;
            }
            return true;
        }

        public event ConnectionEventHandler ConnectionAdded;

        public event ConnectionEventHandler ConnectionRemoved;

        public event ConnectionRenamedEventHandler ConnectionRenamed;

        public event ConnectionBeforeRemoveHandler BeforeConnectionRemove;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (string name in GetConnectionNames())
                {
                    _Connections[name].Connection.Close();
                    _Connections[name].Connection.Dispose();
                }
                _Connections.Clear();
            }
        }
    }
}
