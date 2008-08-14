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
using OSGeo.FDO.Common;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.Common;

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
                        catch (OSGeo.FDO.Common.Exception ex)
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
                        catch (OSGeo.FDO.Common.Exception ex)
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
                        catch (OSGeo.FDO.Common.Exception ex)
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
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                SpatialContextInfo ctx = new SpatialContextInfo();
                ctx.Name = "Default";
                ctx.CoordinateSystem = "";
                ctx.CoordinateSystemWkt = "";
                ctx.Description = "Default Spatial Context";
                ctx.ExtentType = OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                ctx.XYTolerance = 0.0001;
                ctx.ZTolerance = 0.0001;

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                conn.Open();
                Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn))
                    {
                        service.CreateSpatialContext(ctx, false);

                        List<SpatialContextInfo> contexts = service.GetSpatialContexts();
                        SpatialContextInfo sc = service.GetSpatialContext(ctx.Name);

                        Assert.IsNotNull(sc);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);
                    }
                }
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestUpdateSpatialContext()
        {
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                SpatialContextInfo ctx = new SpatialContextInfo();
                ctx.Name = "Default";
                ctx.CoordinateSystem = "";
                ctx.CoordinateSystemWkt = "";
                ctx.Description = "Default Spatial Context";
                ctx.ExtentType = OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                ctx.XYTolerance = 0.0001;
                ctx.ZTolerance = 0.0001;

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                conn.Open();
                Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn))
                    {
                        service.CreateSpatialContext(ctx, false);

                        List<SpatialContextInfo> contexts = service.GetSpatialContexts();
                        SpatialContextInfo sc = service.GetSpatialContext(ctx.Name);

                        Assert.IsNotNull(sc);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);

                        sc.XYTolerance = 0.2;
                        sc.ZTolerance = 0.3;
                        sc.Description = "Foobar";

                        service.CreateSpatialContext(sc, true);

                        ctx = service.GetSpatialContext(sc.Name);
                        Assert.IsNotNull(ctx);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.IsActive, ctx.IsActive);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);

                    }
                }
            }
            finally
            {
                if (!File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestSchemaCanBeApplied()
        {
            FeatureSchema schema = new FeatureSchema("Default", "");
            FeatureClass fc = new FeatureClass("Class1", "");

            DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
            id.DataType = DataType.DataType_Int32;
            id.IsAutoGenerated = true;

            fc.Properties.Add(id);
            fc.IdentityProperties.Add(id);

            GeometricPropertyDefinition geom = new GeometricPropertyDefinition("Geometry", "");
            geom.GeometryTypes = (int)GeometryType.GeometryType_Point;

            fc.Properties.Add(geom);
            schema.Classes.Add(fc);

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
            conn.ConnectionString = "DefaultFileLocation=" + AppGateway.RunningApplication.AppPath;
            using (conn)
            {
                conn.Open();
                using (FeatureService service = new FeatureService(conn))
                {
                    IncompatibleSchema incSchema = null;
                    bool result = service.CanApplySchema(schema, out incSchema);
                    Assert.IsNull(incSchema);
                    Assert.IsTrue(result);
                }
                conn.Close();
            }
        }

        [Test]
        public void TestSchemaCannotBeApplied()
        {
            FeatureSchema schema = new FeatureSchema("Default", "");
            FeatureClass fc = new FeatureClass("Class1", "");

            DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
            id.DataType = DataType.DataType_Int32;
            id.IsAutoGenerated = true;

            fc.Properties.Add(id);
            fc.IdentityProperties.Add(id);

            //Unsupported property in SHP
            DataPropertyDefinition d1 = new DataPropertyDefinition("Unsupported", "");
            d1.DataType = DataType.DataType_Int64;
            d1.Nullable = true;

            fc.Properties.Add(d1);

            GeometricPropertyDefinition geom = new GeometricPropertyDefinition("Geometry", "");
            geom.GeometryTypes = (int)GeometryType.GeometryType_Point;

            fc.Properties.Add(geom);
            schema.Classes.Add(fc);

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
            conn.ConnectionString = "DefaultFileLocation=" + AppGateway.RunningApplication.AppPath;
            using (conn)
            {
                conn.Open();
                using (FeatureService service = new FeatureService(conn))
                {
                    IncompatibleSchema incSchema = null;
                    bool result = service.CanApplySchema(schema, out incSchema);
                    Assert.IsNotNull(incSchema);
                    Assert.IsFalse(result);
                }
                conn.Close();
            }
        }
    }
}
