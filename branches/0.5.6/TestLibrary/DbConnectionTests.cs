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
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using System.IO;
using System.Data.OleDb;

namespace FdoToolbox.Tests
{
    [TestFixture]
    [Category("FdoToolboxCore")]
    public class DbConnectionTests : BaseTest
    {
        private DbConnectionInfo CreateConnection()
        {
            string connStr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}",
                Path.Combine(AppGateway.RunningApplication.AppPath, "Cities.mdb"));

            return new DbConnectionInfo("Cities", new OleDbConnection(connStr), "Access");
        }

        [Test]
        public void TestNonExistentDatabase()
        {
            DbConnectionInfo connInfo = CreateConnection();
            connInfo.Connection.Open();
            using (connInfo.Connection)
            {
                Assert.IsNull(connInfo.GetDatabase("Foobar"));
            }
        }

        [Test]
        public void TestNonExistentTable()
        {
            DbConnectionInfo connInfo = CreateConnection();
            connInfo.Connection.Open();
            using (connInfo.Connection)
            {
                Assert.IsNotNull(connInfo.GetDatabase("Cities.mdb"));
                Assert.IsNull(connInfo.GetTable("Cities.mdb", "Foobar"));
            }
        }
    }
}
