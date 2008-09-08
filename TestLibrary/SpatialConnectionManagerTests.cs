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
using NUnit.Framework;
using FdoToolbox.Core;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Connections;
using System.IO;

namespace FdoToolbox.Tests
{
    public class MockSpatialConnection : IConnection
    {
        private MockConnectionInfo _CInfo;

        public MockSpatialConnection()
        {
            _connstate = ConnectionState.ConnectionState_Closed;
        }

        public MockSpatialConnection(string provider)
            : this()
        {
            _CInfo = new MockConnectionInfo(provider);
        }

        public ITransaction BeginTransaction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Close()
        {
            _connstate = ConnectionState.ConnectionState_Closed;   
        }

        public OSGeo.FDO.Connections.Capabilities.ICommandCapabilities CommandCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Common.Io.IoStream Configuration
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Connections.Capabilities.IConnectionCapabilities ConnectionCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IConnectionInfo ConnectionInfo
        {
            get { return _CInfo; }
        }

        public ConnectionState ConnectionState
        {
            get { return _connstate; }
        }

        private string _ConnStr;

        public string ConnectionString
        {
            get
            {
                return _ConnStr;
            }
            set
            {
                _ConnStr = value;
            }
        }

        private int timeout;

        public int ConnectionTimeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        public OSGeo.FDO.Commands.ICommand CreateCommand(OSGeo.FDO.Commands.CommandType commandType)
        {
            return null;
        }

        public OSGeo.FDO.Commands.Schema.PhysicalSchemaMapping CreateSchemaMapping()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public OSGeo.FDO.Connections.Capabilities.IExpressionCapabilities ExpressionCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Connections.Capabilities.IFilterCapabilities FilterCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Flush()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public OSGeo.FDO.Connections.Capabilities.IGeometryCapabilities GeometryCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        private ConnectionState _connstate;

        public ConnectionState Open()
        {
            _connstate = ConnectionState.ConnectionState_Open;
            return _connstate;
        }

        public OSGeo.FDO.Connections.Capabilities.IRasterCapabilities RasterCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Connections.Capabilities.ISchemaCapabilities SchemaCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Connections.Capabilities.ITopologyCapabilities TopologyCapabilities
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Dispose()
        {
            Close();
        }
    }

    public class MockConnectionInfo : IConnectionInfo
    {
        private string _Provider;

        public MockConnectionInfo(string provider)
        {
            _Provider = provider;
        }

        public IConnectionPropertyDictionary ConnectionProperties
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public OSGeo.FDO.Common.StringCollection DependentFileNames
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string FeatureDataObjectsVersion
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ProviderDatastoreType ProviderDatastoreType
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ProviderDescription
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ProviderDisplayName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ProviderName
        {
            get { return _Provider; }
        }

        public string ProviderVersion
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    [Category("FdoToolboxCore")]
    [TestFixture]
    public class SpatialConnectionManagerTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(FdoConnectionException))]
        public void TestAddIdenticalConnectionNames()
        {
            ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

            IConnection conn1 = new MockSpatialConnection();
            IConnection conn2 = new MockSpatialConnection();
            using (mgr)
            {
                mgr.AddConnection("Conn1", conn1);
                mgr.AddConnection("Conn1", conn2);
            }
        }

        [Test]
        public void TestRenameToVacantConnectionName()
        {
            ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

            IConnection conn1 = new MockSpatialConnection();
            IConnection conn2 = new MockSpatialConnection();
            using (mgr)
            {
                mgr.ConnectionRenamed += delegate(object sender, ConnectionRenameEventArgs e)
                {
                    Assert.AreNotEqual(e.OldName, e.NewName);
                };

                mgr.AddConnection("Conn1", conn1);
                mgr.AddConnection("Conn2", conn2);

                mgr.RenameConnection("Conn2", "Conn3");

                Assert.IsNull(mgr.GetConnection("Conn2"));
                Assert.IsNotNull(mgr.GetConnection("Conn3"));
            }
        }

        [Test]
        [ExpectedException(typeof(FdoConnectionException))]
        public void TestRenameToExistingConnectionName()
        {
            ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

            IConnection conn1 = new MockSpatialConnection();
            IConnection conn2 = new MockSpatialConnection();
            using (mgr)
            {
                mgr.ConnectionRenamed += delegate(object sender, ConnectionRenameEventArgs e)
                {
                    Assert.Fail("Rename should have failed");
                };

                mgr.AddConnection("Conn1", conn1);
                mgr.AddConnection("Conn2", conn2);

                mgr.RenameConnection("Conn2", "Conn1");
            }
        }
    }
}
