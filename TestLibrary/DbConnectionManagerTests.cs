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
using System.Data.OleDb;
using System.IO;

namespace FdoToolbox.Tests
{
    public class MockDbConnectionInfo : DbConnectionInfo
    {
        public MockDbConnectionInfo(string name, OleDbConnection conn)
            : base()
        {
            this.Name = name;
            this.Connection = conn;
        }
    }

    [TestFixture]
    [Category("FdoToolboxCore")]
    public class DbConnectionManagerTests : BaseTest
    {
        private OleDbConnection CreateOleDbConnection()
        {
            string connStr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}",
                Path.Combine(AppGateway.RunningApplication.AppPath, "Cities.mdb"));

            return new OleDbConnection(connStr);
        }

        [Test]
        [ExpectedException(typeof(DbConnectionException))]
        public void TestAddIdenticalConnectionNames()
        {
            IDbConnectionManager mgr = new DbConnectionManager();
            using (mgr)
            {
                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", CreateOleDbConnection());
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn1", CreateOleDbConnection());

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
                mgr.ConnectionRenamed += delegate(object sender, ConnectionRenameEventArgs e)
                {
                    Assert.AreNotEqual(e.OldName, e.NewName);
                };

                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", CreateOleDbConnection());
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn2", CreateOleDbConnection());

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
                mgr.ConnectionRenamed += delegate
                {
                    Assert.Fail("Rename should have failed");
                };

                DbConnectionInfo conn1 = new MockDbConnectionInfo("Conn1", CreateOleDbConnection());
                DbConnectionInfo conn2 = new MockDbConnectionInfo("Conn2", CreateOleDbConnection());

                mgr.AddConnection(conn1);
                mgr.AddConnection(conn2);

                string reason = string.Empty;
                Assert.IsFalse(mgr.CanRenameConnection("Conn2", "Conn1", ref reason));

                mgr.RenameConnection("Conn2", "Conn1");
            }
        }
    }
}
