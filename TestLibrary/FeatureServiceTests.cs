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
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using System.IO;

namespace FdoToolbox.Tests
{
    [Category("FdoToolboxCore")]
    [TestFixture]
    public class FeatureServiceTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(FeatureServiceException))]
        public void TestUnopenedConnection()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn)) { }
                    Assert.Fail("Service should not accept un-opened connection");
                }
            }
            finally
            {
                if(File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestSchemaClone()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.CloneSchemas();

                            using (schemas)
                            {
                                foreach (FeatureSchema schema in schemas)
                                {
                                    FeatureSchema fs = service.GetSchemaByName(schema.Name);

                                    Assert.IsNotNull(fs);
                                    AssertHelper.EqualSchemas(service, schema, fs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
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
        public void TestCloneClass()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.DescribeSchema();
                            ClassDefinition classDef = schemas[0].Classes[0];

                            Assert.IsNotNull(classDef);

                            ClassDefinition cd = FeatureService.CloneClass(classDef);
                            AssertHelper.EqualClass(classDef, cd);
                        }
                        catch (Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
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
        public void TestCloneProperty()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.DescribeSchema();
                            ClassDefinition classDef = schemas[0].Classes[0];
                            Assert.IsNotNull(classDef);
                            foreach (PropertyDefinition propDef in classDef.Properties)
                            {
                                PropertyDefinition pd = FeatureService.CloneProperty(propDef);
                                AssertHelper.EqualProperty(propDef, pd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
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
        public void TestCreateSpatialContext()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestUpdateSpatialContext()
        {
            Assert.Fail("Not implemented");
        }
    }
}
