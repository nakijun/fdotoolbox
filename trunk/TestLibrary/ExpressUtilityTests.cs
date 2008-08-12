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
using OSGeo.FDO.Connections;
using System.Collections.Specialized;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ClientServices;
using System.IO;

namespace TestLibrary
{
    [Category("FdoToolboxCore")]
    [TestFixture]
    public class ExpressUtilityTests : BaseTest
    {
        [Test]
        public void TestCreateSdf()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestCreateSdfOverwrite()
        {
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                //create again
                result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestCreateSdfLocked()
        {
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);
                    //create again (file is locked, shouldn't happen)
                    result = ExpressUtility.CreateSDF(file);
                    Assert.IsFalse(result);

                    conn.Close();
                }

                //create again (file is unlocked, should happen)
                result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestStringToNvc()
        {
            string str = "Username=Foo;Password=Bar;Service=localhost";
            NameValueCollection nvc = ExpressUtility.ConvertFromString(str);
            
            Assert.AreEqual(nvc["Username"], "Foo");
            Assert.AreEqual(nvc["Password"], "Bar");
            Assert.AreEqual(nvc["Service"], "localhost");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestApplyNullSchema()
        {
            try
            {
                ExpressUtility.ApplySchemaToNewSDF(null, "Test.sdf");
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestApplySchema()
        {
            try
            {
                FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);
                schemas.ReadXml("Test.xml");

                FeatureSchema fs1 = schemas[0];
                IConnection conn = ExpressUtility.ApplySchemaToNewSDF(fs1, "Test.sdf");
                conn.Dispose();

                fs1.Dispose();
                schemas.Dispose();

                schemas = new FeatureSchemaCollection(null);
                schemas.ReadXml("Test.xml");

                fs1 = schemas[0];

                conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        FeatureSchema fs2 = service.GetSchemaByName(fs1.Name);
                        Assert.IsNotNull(fs2);

                        AssertHelper.EqualSchemas(service, fs1, fs2);
                    }

                    conn.Close();
                }
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestApplySchemaExistingSdf()
        {
            string file = "Test.sdf";
            try
            {
                FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);
                schemas.ReadXml("Test.xml");

                FeatureSchema fs1 = schemas[0];

                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                try
                {
                    ExpressUtility.ApplySchemaToSDF("Test.xml", file);
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    Assert.Fail(ex.ToString());
                }

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        FeatureSchema fs2 = service.GetSchemaByName(fs1.Name);
                        Assert.IsNotNull(fs2);

                        AssertHelper.EqualSchemas(service, fs1, fs2);
                    }
                    conn.Close();
                }
                fs1.Dispose();
                schemas.Dispose();
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
