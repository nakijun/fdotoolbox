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
using OSGeo.FDO.Connections;
using FdoToolbox.Core.IO;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Tests
{
    [TestFixture]
    public class ConnectionLoaderTests : BaseTest
    {
        [Test]
        [ExpectedException]
        public void TestBadDbConnectionString()
        {
            string file = "DbConnBadConnectionString.dbconn";
            DbConnectionInfo connInfo = DbConnLoader.LoadConnection(file);
            using (connInfo.Connection) { }
        }

        [Test]
        [ExpectedException(typeof(OSGeo.FDO.Common.Exception))]
        public void TestBadSpatialConnectionString()
        {
            string file = "SpatialConnBadConnectionString.conn";
            SpatialConnectionInfo connInfo = SpatialConnLoader.LoadConnection(file);
            using (connInfo.Connection) { }
        }

        [Test]
        [ExpectedException(typeof(OSGeo.FDO.Common.Exception))]
        public void TestBadFdoProvider()
        {
            string file = "SpatialConnBadProvider.conn";
            SpatialConnectionInfo connInfo = SpatialConnLoader.LoadConnection(file);
            using (connInfo.Connection) { }
        }
    }
}
