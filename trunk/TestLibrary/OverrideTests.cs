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
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.Common;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Tests
{
    [TestFixture]
    [Category("FdoToolboxCore")]
    public class OverrideTests : BaseTest
    {
        [Test(Description = "Test that the correct override is loaded")]
        public void TestLoadBcpTaskMySql()
        {
            SpatialConnectionInfo source = new MockSdfConnection();
            SpatialConnectionInfo target = new MockMySqlConnection();

            source.Connection.Open();
            target.Connection.Open();

            SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(source, target);
            SpatialBulkCopyTask task = new SpatialBulkCopyTask("TEST", options);

            Assert.IsNotNull(task.CopySpatialContextOverride);
            Assert.IsTrue(task.CopySpatialContextOverride is MySqlCopySpatialContextOverride);
        }

        [Test(Description = "Test that the correct override is loaded")]
        public void TestLoadBcpTaskOracle()
        {
            SpatialConnectionInfo source = new MockOracleConnection();
            SpatialConnectionInfo target = new MockSdfConnection();

            source.Connection.Open();
            target.Connection.Open();

            SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(source, target);
            SpatialBulkCopyTask task = new SpatialBulkCopyTask("TEST", options);

            Assert.IsNotNull(task.ClassNameOverride);
            Assert.IsTrue(task.ClassNameOverride is OracleClassNameOverride);
        }

        [Test(Description = "Test that the correct override is loaded")]
        public void TestLoadBcpTaskShp()
        {
            SpatialConnectionInfo source = new MockSdfConnection();
            SpatialConnectionInfo target = new MockShpConnection();

            source.Connection.Open();
            target.Connection.Open();

            SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(source, target);
            SpatialBulkCopyTask task = new SpatialBulkCopyTask("TEST", options);

            Assert.IsNotNull(task.CopySpatialContextOverride);
            Assert.IsTrue(task.CopySpatialContextOverride is ShpCopySpatialContextOverride);
        }
    }

    class MockSdfConnection : SpatialConnectionInfo 
    {
        public MockSdfConnection()
            : base("Foo", null)
        {
            this.Connection = new MockSpatialConnection("OSGeo.SDF");
        }
    }

    class MockMySqlConnection : SpatialConnectionInfo
    {
        public MockMySqlConnection()
            : base("Foo", null)
        {
            this.Connection = new MockSpatialConnection("OSGeo.MySQL");
        }
    }

    class MockShpConnection : SpatialConnectionInfo
    {
        public MockShpConnection()
            : base("Foo", null)
        {
            this.Connection = new MockSpatialConnection("OSGeo.SHP");
        }
    }

    class MockOracleConnection : SpatialConnectionInfo
    {
        public MockOracleConnection()
            : base("Foo", null)
        {
            this.Connection = new MockSpatialConnection("OSGeo.KingOracle");
        }
    }
}
