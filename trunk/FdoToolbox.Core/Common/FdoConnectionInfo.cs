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
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// FDO Connection wrapper class
    /// </summary>
    public class FdoConnectionInfo : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conn"></param>
        public FdoConnectionInfo(string name, IConnection conn)
        {
            this.Name = name;
            this.InternalConnection = conn;
        }

        private string _Name;

        /// <summary>
        /// The name of the connection
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// The connection string of the underlying connection
        /// </summary>
        public string ConnectionString
        {
            get { return this.InternalConnection.ConnectionString; }
        }

        /// <summary>
        /// The name of the connection's underlying provider
        /// </summary>
        public string Provider
        {
            get { return this.InternalConnection.ConnectionInfo.ProviderName; }
        }

        /// <summary>
        /// Refreshes this connection
        /// </summary>
        public void Refresh()
        {
            Close();
            this.InternalConnection.Open();
        }

        private IConnection _Connection;

        /// <summary>
        /// The underlying FDO connection
        /// </summary>
        public IConnection InternalConnection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        /// <summary>
        /// Creates a new feature service
        /// </summary>
        /// <returns></returns>
        public FeatureService CreateFeatureService()
        {
            return new FeatureService(this.InternalConnection);
        }

        /// <summary>
        /// Closes the underlying connection
        /// </summary>
        public void Close()
        {
            if (this.InternalConnection.ConnectionState != ConnectionState.ConnectionState_Closed)
                this.InternalConnection.Close();
        }

        /// <summary>
        /// Disposes this connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
                this.InternalConnection.Dispose();
            }
        }
    }
}
