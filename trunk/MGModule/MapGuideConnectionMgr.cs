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

namespace MGModule
{
    public delegate void MgConnectionHandler(Uri host);

    public class MapGuideConnectionMgr : IDisposable
    {
        public event MgConnectionHandler ConnectionAdded;
        public event MgConnectionHandler ConnectionRemoved;

        private Dictionary<Uri, ServerConnectionI> _MGConnections;

        public MapGuideConnectionMgr()
        {
            _MGConnections = new Dictionary<Uri, ServerConnectionI>();
        }

        public void AddConnection(Uri host, ServerConnectionI conn)
        {
            if (!_MGConnections.ContainsKey(host))
            {
                _MGConnections.Add(host, conn);
                if (this.ConnectionAdded != null)
                    this.ConnectionAdded(host);
            }
            else
            {
                throw new ArgumentException("A connection already exists at the host: " + host);
            }
        }

        public ServerConnectionI GetConnection(Uri host)
        {
            if (_MGConnections.ContainsKey(host))
                return _MGConnections[host];

            return null;
        }

        public void RemoveConnection(Uri host)
        {
            bool removed = _MGConnections.Remove(host);
            if (removed && this.ConnectionRemoved != null)
                this.ConnectionRemoved(host);
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
                foreach (Uri uri in _MGConnections.Keys)
                {
                    _MGConnections[uri].Dispose();
                }
                _MGConnections.Clear();
            }
        }
    }
}
