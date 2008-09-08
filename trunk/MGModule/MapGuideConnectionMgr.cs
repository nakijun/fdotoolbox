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
using OSGeo.MapGuide.MaestroAPI;
using FdoToolbox.Core;

namespace MGModule
{
    public delegate void MgConnectionHandler(object sender, EventArgs<string> e);

    public class MapGuideConnectionMgr : IDisposable
    {
        public event MgConnectionHandler ConnectionAdded;
        public event MgConnectionHandler ConnectionRemoved;

        private Dictionary<string, ServerConnectionI> _MGConnections;

        public MapGuideConnectionMgr()
        {
            _MGConnections = new Dictionary<string, ServerConnectionI>();
        }

        public void AddConnection(string key, ServerConnectionI conn)
        {
            if (!_MGConnections.ContainsKey(key))
            {
                conn.AutoRestartSession = true;
                _MGConnections.Add(key, conn);
                if (this.ConnectionAdded != null)
                    this.ConnectionAdded(this, new EventArgs<string>(key));
            }
            else
            {
                throw new ArgumentException("A connection already exists under the key: " + key);
            }
        }

        public ServerConnectionI GetConnection(string key)
        {
            if (_MGConnections.ContainsKey(key))
                return _MGConnections[key];

            return null;
        }

        public void RemoveConnection(string key)
        {
            bool removed = _MGConnections.Remove(key);
            if (removed && this.ConnectionRemoved != null)
                this.ConnectionRemoved(this, new EventArgs<string>(key));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (string key in _MGConnections.Keys)
                {
                    _MGConnections[key].Dispose();
                }
                _MGConnections.Clear();
            }
        }
    }
}
