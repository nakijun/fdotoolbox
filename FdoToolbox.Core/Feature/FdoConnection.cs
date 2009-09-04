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
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.ClientServices;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using Res = FdoToolbox.Core.ResourceUtil;
using FdoToolbox.Core.Connections;
using OSGeo.FDO.Common.Io;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO Connection wrapper class
    /// </summary>
    public class FdoConnection : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        internal FdoConnection(IConnection conn)
        {
            this.InternalConnection = conn;
        }

        private ICapability _caps;
        
        /// <summary>
        /// Gets the capability object for this connection
        /// </summary>
        public ICapability Capability
        {
            get
            {
                if (_caps == null)
                    _caps = new Capability(this);
                return _caps;
            }
        }

        /// <summary>
        /// Gets the type of the data store.
        /// </summary>
        /// <value>The type of the data store.</value>
        public ProviderDatastoreType DataStoreType
        {
            get
            {
                return _Connection.ConnectionInfo.ProviderDatastoreType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnection"/> class.
        /// </summary>
        /// <param name="provider">The provider name.</param>
        public FdoConnection(string provider)
        {
            this.InternalConnection = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoConnection"/> class.
        /// </summary>
        /// <param name="provider">The provider name.</param>
        /// <param name="connectionString">The connection string.</param>
        public FdoConnection(string provider, string connectionString)
            : this(provider)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Saves this connection to a file
        /// </summary>
        /// <param name="file"></param>
        public void Save(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FdoToolbox.Core.Configuration.Connection));
            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;

                FdoToolbox.Core.Configuration.Connection conn = new FdoToolbox.Core.Configuration.Connection();
                conn.Provider = this.Provider;
                conn.ConnectionString = this.ConnectionString;

                serializer.Serialize(writer, conn);
            }
        }

        /// <summary>
        /// Creates an FDO connection from file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FdoConnection LoadFromFile(string file)
        {
            FdoToolbox.Core.Configuration.Connection c = null;
            XmlSerializer serializer = new XmlSerializer(typeof(FdoToolbox.Core.Configuration.Connection));
            using (StreamReader reader = new StreamReader(file))
            {
                c = (FdoToolbox.Core.Configuration.Connection)serializer.Deserialize(reader);
            }
            return new FdoConnection(c.Provider, c.ConnectionString);
        }

        /// <summary>
        /// Gets or sets the connection string of the underlying connection
        /// </summary>
        public string ConnectionString
        {
            get { return this.InternalConnection.ConnectionString; }
            set 
            { 
                this.InternalConnection.ConnectionString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    //HACK: ODBC doesn't want to play nice
                    if (this.Provider.StartsWith("OSGeo.ODBC"))
                    {
                        _safeConnStr = value;
                        return;
                    }

                    List<string> safeParams = new List<string>();
                    string[] parameters = this.ConnectionString.Split(';');
                    IConnectionPropertyDictionary dict = this.InternalConnection.ConnectionInfo.ConnectionProperties;
                    foreach (string p in parameters)
                    {
                        string[] tokens = p.Split('=');
                        if (!dict.IsPropertyProtected(tokens[0]))
                        {
                            safeParams.Add(p);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < tokens[1].Length; i++)
                            {
                                sb.Append("*");
                            }
                            safeParams.Add(tokens[0] + "=" + sb.ToString());
                        }
                    }
                    _safeConnStr = string.Join(";", safeParams.ToArray());
                }
            }
        }

        private string _safeConnStr = null;

        /// <summary>
        /// Gets the connection string with the protected elements obfuscated
        /// </summary>
        public string SafeConnectionString
        {
            get
            {
                return _safeConnStr;
            }
        }

        private string _name;

        /// <summary>
        /// The name of the connection's underlying provider. This does not include the version number. Use the <see cref="ProviderQualified"/>
        /// property for the full provider name
        /// </summary>
        public string Provider
        {
            get 
            {
                if (_name == null)
                {
                    ProviderNameTokens providerName = new ProviderNameTokens(this.InternalConnection.ConnectionInfo.ProviderName);
                    string [] tokens = providerName.GetNameTokens();
                    _name = tokens[0] + "." + tokens[1];
                }
                return _name;
            }
        }

        /// <summary>
        /// The fully-qualified name of the connection's underlying provider
        /// </summary>
        public string ProviderQualified
        {
            get 
            {
                return this.InternalConnection.ConnectionInfo.ProviderName;
            }
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
        internal IConnection InternalConnection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        /// <summary>
        /// Creates a new feature service bound to this connection
        /// </summary>
        /// <returns></returns>
        public FdoFeatureService CreateFeatureService()
        {
            return new FdoFeatureService(this.InternalConnection);
        }

        /// <summary>
        /// Creates a new feature service bound to this connection
        /// </summary>
        /// <param name="forceFullSchemaDiscovery">if set to <c>true</c> disables the use of enhanced IDescribeSchema if the provider supports it</param>
        /// <returns></returns>
        public FdoFeatureService CreateFeatureService(bool forceFullSchemaDiscovery)
        {
            return new FdoFeatureService(this.InternalConnection, forceFullSchemaDiscovery);
        }

        /// <summary>
        /// Opens the underlying connection
        /// </summary>
        public FdoConnectionState Open()
        {
            try
            {
                if (this.InternalConnection.ConnectionState != ConnectionState.ConnectionState_Open)
                    this.InternalConnection.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex) { throw new FdoException(ex); }
            return this.State;
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

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        public FdoConnectionState State
        {
            get
            {
                switch (this.InternalConnection.ConnectionState)
                {
                    case ConnectionState.ConnectionState_Busy:
                        return FdoConnectionState.Busy;
                    case ConnectionState.ConnectionState_Closed:
                        return FdoConnectionState.Closed;
                    case ConnectionState.ConnectionState_Open:
                        return FdoConnectionState.Open;
                    case ConnectionState.ConnectionState_Pending:
                        return FdoConnectionState.Pending;
                }
                throw new InvalidOperationException(Res.GetString("ERR_CONNECTION_UNKNOWN_STATE"));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_caps != null)
                    _caps.Dispose();
                this.Close();
                this.InternalConnection.Dispose();
            }
        }

        /// <summary>
        /// Gets the connect time property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public DictionaryProperty GetConnectTimeProperty(string name)
        {
            if (this.State != FdoConnectionState.Open && this.State != FdoConnectionState.Pending)
                throw new InvalidOperationException(Res.GetString("ERR_CONNECTION_NOT_OPEN"));

            IConnectionPropertyDictionary dict = this.InternalConnection.ConnectionInfo.ConnectionProperties;
            bool enumerable = dict.IsPropertyEnumerable(name);
            DictionaryProperty dp = null;
            if (enumerable)
            {
                EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty();
                ep.Values = dict.EnumeratePropertyValues(name);
                dp = ep;
            }
            else
            {
                dp = new DictionaryProperty();
            }

            dp.Name = name;
            dp.LocalizedName = dict.GetLocalizedName(name);
            dp.DefaultValue = dict.GetPropertyDefault(name);
            dp.Protected = dict.IsPropertyProtected(name);
            dp.Required = dict.IsPropertyRequired(name);

            return dp;
        }

        /// <summary>
        /// Sets the configuration for this connection
        /// </summary>
        /// <param name="file">The configuration file</param>
        public void SetConfiguration(string file)
        {
            CapabilityType cap = CapabilityType.FdoCapabilityType_SupportsConfiguration;
            if (!this.Capability.GetBooleanCapability(cap))
                throw new InvalidOperationException(ResourceUtil.GetStringFormatted("ERR_UNSUPPORTED_CAPABILITY", cap));
            IoFileStream confStream = new IoFileStream(file, "r");
            _Connection.Configuration = confStream;
        }
    }

    /// <summary>
    /// Indicates the current connection state
    /// </summary>
    public enum FdoConnectionState
    {
        /// <summary>
        /// Connection is busy
        /// </summary>
        Busy,
        /// <summary>
        /// Connection is open
        /// </summary>
        Open,
        /// <summary>
        /// Connection is closed
        /// </summary>
        Closed,
        /// <summary>
        /// Connection is pending. Additional parameters are required in order for it to be open
        /// </summary>
        Pending
    }
}
