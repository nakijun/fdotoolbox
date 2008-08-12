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

namespace TestLibrary
{
    [Category("FdoToolboxCore")]
    [TestFixture]
    public class SpatialConnectionManagerTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(FdoConnectionException))]
        public void TestAddIdenticalConnectionNames()
        {
            string file1 = "Test1.sdf";
            string file2 = "Test2.sdf";

            try
            {
                ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

                bool result = ExpressUtility.CreateSDF(file1);
                Assert.IsTrue(result);

                result = ExpressUtility.CreateSDF(file2);
                Assert.IsTrue(result);

                IConnection conn1 = ExpressUtility.CreateSDFConnection(file1, false);
                IConnection conn2 = ExpressUtility.CreateSDFConnection(file2, false);

                using (mgr)
                {
                    mgr.AddConnection("Conn1", conn1);
                    mgr.AddConnection("Conn1", conn2);
                }
            }
            finally
            {
                if (File.Exists(file1))
                    File.Delete(file1);

                if (File.Exists(file2))
                    File.Delete(file2);
            }
        }

        [Test]
        public void TestRenameToVacantConnectionName()
        {
            string file1 = "Test1.sdf";
            string file2 = "Test2.sdf";

            try
            {
                ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

                bool result = ExpressUtility.CreateSDF(file1);
                Assert.IsTrue(result);

                result = ExpressUtility.CreateSDF(file2);
                Assert.IsTrue(result);

                IConnection conn1 = ExpressUtility.CreateSDFConnection(file1, false);
                IConnection conn2 = ExpressUtility.CreateSDFConnection(file2, false);

                using (mgr)
                {
                    mgr.ConnectionRenamed += delegate(string oldName, string newName)
                    {
                        Assert.AreNotEqual(oldName, newName);
                    };

                    mgr.AddConnection("Conn1", conn1);
                    mgr.AddConnection("Conn2", conn2);

                    mgr.RenameConnection("Conn2", "Conn3");

                    Assert.IsNull(mgr.GetConnection("Conn2"));
                    Assert.IsNotNull(mgr.GetConnection("Conn3"));
                }
            }
            finally
            {
                if (File.Exists(file1))
                    File.Delete(file1);

                if (File.Exists(file2))
                    File.Delete(file2);
            }
        }

        [Test]
        [ExpectedException(typeof(FdoConnectionException))]
        public void TestRenameToExistingConnectionName()
        {
            string file1 = "Test1.sdf";
            string file2 = "Test2.sdf";

            try
            {
                ISpatialConnectionMgr mgr = new SpatialConnectionMgr();

                bool result = ExpressUtility.CreateSDF(file1);
                Assert.IsTrue(result);

                result = ExpressUtility.CreateSDF(file2);
                Assert.IsTrue(result);

                IConnection conn1 = ExpressUtility.CreateSDFConnection(file1, false);
                IConnection conn2 = ExpressUtility.CreateSDFConnection(file2, false);

                using (mgr)
                {
                    mgr.ConnectionRenamed += delegate(string oldName, string newName)
                    {
                        Assert.Fail("Rename should have failed");
                    };

                    mgr.AddConnection("Conn1", conn1);
                    mgr.AddConnection("Conn2", conn2);

                    mgr.RenameConnection("Conn2", "Conn1");
                }
            }
            finally
            {
                if (File.Exists(file1))
                    File.Delete(file1);

                if (File.Exists(file2))
                    File.Delete(file2);
            }
        }
    }
}
