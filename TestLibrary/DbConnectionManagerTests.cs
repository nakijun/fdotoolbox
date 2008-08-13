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
using System.Data;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Tests
{
    public class MockDbConnection : IDbConnection
    {
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDbTransaction BeginTransaction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private ConnectionState state;

        public void Close()
        {
            state = ConnectionState.Closed;
        }

        private string _connstr;

        public string ConnectionString
        {
            get
            {
                return _connstr;
            }
            set
            {
                _connstr = value;
            }
        }

        public int ConnectionTimeout
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IDbCommand CreateCommand()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string Database
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Open()
        {
            state = ConnectionState.Open;
        }

        public ConnectionState State
        {
            get { return state; }
        }

        public void Dispose()
        {
            Close();
        }
    }

    public class MockDbConnectionInfo : DbConnectionInfo
    {
        public MockDbConnectionInfo(string name, IDbConnection conn, string driver)
        {
            this.Name = name;
            this.Connection = conn;
            this.Driver = driver;
        }
    }

    [TestFixture]
    public class DbConnectionManagerTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(DbConnectionException))]
        public void TestAddIdenticalConnectionNames()
        {
            IDbConnectionManager mgr = new DbConnectionManager();
            using (mgr)
            {
                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", new MockDbConnection(), "foobar");
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn1", new MockDbConnection(), "snafu");

                mgr.AddConnection(conn1);
                mgr.AddConnection(conn2);
            }
        }

        [Test]
        public void TestRenameToVacantConnectionName()
        {
            IDbConnectionManager mgr = new DbConnectionManager();
            using (mgr)
            {
                mgr.ConnectionRenamed += delegate(string oldName, string newName)
                {
                    Assert.AreNotEqual(oldName, newName);
                };

                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", new MockDbConnection(), "foobar");
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn2", new MockDbConnection(), "snafu");

                mgr.AddConnection(conn1);
                mgr.AddConnection(conn2);

                string reason = string.Empty;
                Assert.IsTrue(mgr.CanRenameConnection("Conn2", "Conn3", ref reason));

                mgr.RenameConnection("Conn2", "Conn3");
                Assert.IsNull(mgr.GetConnection("Conn2"));
                Assert.IsNotNull(mgr.GetConnection("Conn3"));
            }
        }

        [Test]
        [ExpectedException(typeof(DbConnectionException))]
        public void TestRenameToExistingConnectionName()
        {
            IDbConnectionManager mgr = new DbConnectionManager();
            using (mgr)
            {
                mgr.ConnectionRenamed += delegate(string oldName, string newName)
                {
                    Assert.Fail("Rename should have failed");
                };

                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", new MockDbConnection(), "foobar");
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn2", new MockDbConnection(), "snafu");

                mgr.AddConnection(conn1);
                mgr.AddConnection(conn2);

                string reason = string.Empty;
                Assert.IsFalse(mgr.CanRenameConnection("Conn2", "Conn1", ref reason));

                mgr.RenameConnection("Conn2", "Conn1");
            }
        }
    }
}
