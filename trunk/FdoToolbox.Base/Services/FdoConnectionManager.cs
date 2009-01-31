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
using ICSharpCode.Core;

//TODO: Move exception messages to Errors.resx

namespace FdoToolbox.Base.Services
{
    public sealed class FdoConnectionManager : IFdoConnectionManager
    {
        private Dictionary<string, FdoConnection> _ConnectionDict = new Dictionary<string, FdoConnection>();

        public void Clear()
        {
            List<string> names = new List<string>(GetConnectionNames());
            foreach (string name in names)
            {
                this.RemoveConnection(name);
            }
        }

        public bool NameExists(string name)
        {
            if (name == null)
                return false;
            return _ConnectionDict.ContainsKey(name);
        }

        public void AddConnection(string name, FdoToolbox.Core.Feature.FdoConnection conn)
        {
            if (_ConnectionDict.ContainsKey(name))
                throw new FdoConnectionException("Unable to add connection named " + name + " to the connection manager");
            conn.Open();
            _ConnectionDict.Add(name, conn);
            this.ConnectionAdded(this, new EventArgs<string>(name));
        }

        public void RemoveConnection(string name)
        {
            if (_ConnectionDict.ContainsKey(name))
            {
                ConnectionBeforeRenameEventArgs e = new ConnectionBeforeRenameEventArgs(name);
                this.BeforeConnectionRemove(this, e);
                if (e.Cancel)
                    return;

                FdoConnection conn = _ConnectionDict[name];
                conn.Close();
                _ConnectionDict.Remove(name);
                conn.Dispose();
                if (this.ConnectionRemoved != null)
                    this.ConnectionRemoved(this, new EventArgs<string>(name));
            }
        }

        public FdoToolbox.Core.Feature.FdoConnection GetConnection(string name)
        {
            FdoConnection conn = null;
            if (_ConnectionDict.ContainsKey(name))
                conn = _ConnectionDict[name];
            return conn;
        }

        public ICollection<string> GetConnectionNames()
        {
            return _ConnectionDict.Keys;
        }

        public void RenameConnection(string oldName, string newName)
        {
            if (!_ConnectionDict.ContainsKey(oldName))
                throw new FdoConnectionException("The connection to be renamed could not be found: " + oldName);
            if (_ConnectionDict.ContainsKey(newName))
                throw new FdoConnectionException("Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists");

            FdoConnection conn = _ConnectionDict[oldName];
            _ConnectionDict.Remove(oldName);
            _ConnectionDict.Add(newName, conn);

            ConnectionRenameEventArgs e = new ConnectionRenameEventArgs(oldName, newName);
            this.ConnectionRenamed(this, e);
        }

        public ConnectionRenameResult CanRenameConnection(string oldName, string newName)
        {
            string reason = string.Empty;
            bool result = false;
            if (!_ConnectionDict.ContainsKey(oldName))
            {
                reason = "The connection to be renamed could not be found: " + oldName;
                result = false;
                return new ConnectionRenameResult(result, reason);
            }
            if (_ConnectionDict.ContainsKey(newName))
            {
                reason = "Cannot rename connection " + oldName + " to " + newName + " as a connection of that name already exists";
                result = false;
                return new ConnectionRenameResult(result, reason);
            }
            return new ConnectionRenameResult();
        }

        public event ConnectionBeforeRemoveHandler BeforeConnectionRemove = delegate { };

        public event ConnectionEventHandler ConnectionAdded = delegate { };

        public event ConnectionEventHandler ConnectionRemoved = delegate { };

        public event ConnectionRenamedEventHandler ConnectionRenamed = delegate { };

        public event ConnectionEventHandler ConnectionRefreshed = delegate { };

        private bool _init = false;

        public bool IsInitialized
        {
            get { return _init; }
        }

        public void InitializeService()
        {
            LoggingService.Info("Initialized Connection Manager Service");
            _init = true;
            Initialize(this, EventArgs.Empty);
        }

        public void UnloadService()
        {
            Unload(this, EventArgs.Empty);
        }

        public event EventHandler Initialize = delegate { };

        public event EventHandler Unload = delegate { };

        public void RefreshConnection(string name)
        {
            if (NameExists(name))
            {
                FdoConnection conn = this.GetConnection(name);
                conn.Close();
                conn.Open();

                /*
                //TODO: I got a bad feeling that this may break something
                //which may rely on the old connection. Verify this.
                FdoConnection oldConn = this.GetConnection(name);
                string provider = oldConn.Provider;
                string connStr = oldConn.ConnectionString;
                oldConn.Close();
                oldConn.Dispose();

                FdoConnection newConn = new FdoConnection(provider, connStr);
                newConn.Open();
                _ConnectionDict[name] = newConn;
                */
                ConnectionRefreshed(this, new EventArgs<string>(name));
            }
        }


        public void Load()
        {
            string path = Preferences.SessionDirectory;
            if (System.IO.Directory.Exists(path))
            {
                string [] files = System.IO.Directory.GetFiles(path, "*.conn");
                foreach (string f in files)
                {
                    try
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(f);
                        FdoConnection conn = FdoConnection.LoadFromFile(f);
                        this.AddConnection(name, conn);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void Save()
        {
            string path = Preferences.SessionDirectory;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            else
            {
                string [] files = System.IO.Directory.GetFiles(path, "*.conn");
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
            }

            foreach (string key in _ConnectionDict.Keys)
            {
                string file = System.IO.Path.Combine(path, key + ".conn");
                FdoConnection conn = _ConnectionDict[key];
                conn.Save(file);
            }
        }


        public FdoConnection GetConnection(string provider, string connStr)
        {
            foreach (FdoConnection conn in _ConnectionDict.Values)
            {
                if (conn.Provider.StartsWith(provider) && conn.ConnectionString.ToLower() == connStr.ToLower())
                    return conn;
            }
            return null;
        }

        public string GetName(FdoConnection conn)
        {
            foreach (string key in _ConnectionDict.Keys)
            {
                if (_ConnectionDict[key] == conn)
                    return key;
            }
            return null;
        }
    }
}
