using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using System.Diagnostics;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Common;
using System.IO;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Commands.DataStore;
using FdoToolbox.Core.ClientServices;

namespace TestApp
{
    public class TestApp : ConsoleApplication
    {
        public override void ParseArguments(string[] args)
        {
            
        }

        public override void ShowUsage()
        {
            
        }

        public override void Run(string[] args)
        {
            TestClassDeleteRaw();
            TestClassDelete();
            TestPropertyDeleteRaw();
        }

        private void TestPropertyDeleteRaw()
        {
            AppConsole.WriteLine("TestPropertyDeleteRaw");

            string file = Path.Combine(this.AppPath, "Test.sdf");

            if (File.Exists(file))
                File.Delete(file);

            string connStr = "File=" + file;

            IConnection conn = CreateSDF(file);

            conn.ConnectionString = connStr;
            conn.Open();
            Debug.Assert(conn.ConnectionState == ConnectionState.ConnectionState_Open, "Could not open SDF connection");

            AppConsole.WriteLine("Describing");

            IDescribeSchema describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            FeatureSchemaCollection schemas = null;
            using (describe)
            {
                schemas = describe.Execute();
            }
            Debug.Assert(schemas != null, "Should be zero-size, not a null object");
            Debug.Assert(schemas.Count == 0, "Expected empty SDF file");

            AppConsole.WriteLine("Creating class: Class1");

            //Create one
            FeatureSchema schema = new FeatureSchema("Test", "Test Schema");
            FeatureClass c1 = new FeatureClass("Class1", "");
            DataPropertyDefinition idP1 = new DataPropertyDefinition("ID", "");
            idP1.DataType = DataType.DataType_Int32;
            idP1.IsAutoGenerated = true;
            c1.Properties.Add(idP1);
            c1.IdentityProperties.Add(idP1);

            DataPropertyDefinition fooP = new DataPropertyDefinition("Foo", "");
            fooP.DataType = DataType.DataType_String;
            fooP.Length = 20;
            fooP.Nullable = true;
            c1.Properties.Add(fooP);

            GeometricPropertyDefinition geomP1 = new GeometricPropertyDefinition("Geometry", "");
            geomP1.GeometryTypes = (int)GeometryType.GeometryType_Polygon;

            c1.Properties.Add(geomP1);
            c1.GeometryProperty = geomP1;

            schema.Classes.Add(c1);

            AppConsole.WriteLine("Applying schema");

            //Apply new class
            try
            {
                using (IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                {
                    apply.FeatureSchema = schema;
                    apply.Execute();
                    conn.Flush();
                }
            }
            catch
            {
                Debug.Fail("Could not apply schema");
            }

            AppConsole.WriteLine("Re-describe");

            //re-describe
            describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            schemas = null;
            using (describe)
            {
                schemas = describe.Execute();
            }
            Debug.Assert(schemas != null, "Expected non-null object");
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 1, "Expected 2 classes in schema");
            Debug.Assert(schemas[0].Classes[0].Properties.Count == 3, "Expected 3 properties");

            //Now remove the property "Foo"
            AppConsole.WriteLine("Removing Property: Foo");
            ClassDefinition cdef = schemas[0].Classes[0];
            int idx = cdef.Properties.IndexOf("Foo");
            Debug.Assert(idx >= 0, "Expected property Foo to be there for us to delete");

            cdef.Properties.RemoveAt(idx);

            //Apply changes
            try
            {
                using (IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                {
                    apply.FeatureSchema = cdef.FeatureSchema;
                    apply.Execute();
                }
            }
            catch
            {
                Debug.Fail("Could not apply schema");
            }

            AppConsole.WriteLine("Can flush: {0}", conn.ConnectionCapabilities.SupportsFlush());
            conn.Flush();

            AppConsole.WriteLine("Re-describing");
            //re-describe
            describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            schemas = null;
            using (describe)
            {
                schemas = describe.Execute();
            }
            Debug.Assert(schemas != null, "Expected non-null object");
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 1, "Expected 2 classes in schema");

            Debug.Assert(schemas[0].Classes[0].Properties.Count == 2, "Expected 2 properties");
            cdef = schemas[0].Classes[0];
            idx = cdef.Properties.IndexOf("Foo");
            Debug.Assert(idx < 0, "Property Foo was not deleted");
        }

        private static IConnection CreateSDF(string file)
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SDF");
            Debug.Assert(conn != null, "Unable to create SDF connection");
            try
            {
                using (ICreateDataStore create = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    create.DataStoreProperties.SetProperty("File", file);
                    create.Execute();
                }
            }
            catch
            {
                Debug.Fail("Could not create " + file);
            }
            return conn;
        }

        private void TestClassDeleteRaw()
        {
            AppConsole.WriteLine("TestClassDeleteRaw");

            string file = Path.Combine(this.AppPath,"Test.sdf");

            if (File.Exists(file))
                File.Delete(file);
            
            string connStr = "File=" + file;

            IConnection conn = CreateSDF(file);
            
            conn.ConnectionString = connStr;
            conn.Open();
            Debug.Assert(conn.ConnectionState == ConnectionState.ConnectionState_Open, "Could not open SDF connection");

            AppConsole.WriteLine("Describing");
            IDescribeSchema describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            FeatureSchemaCollection schemas = null;
            using(describe)
            {
                schemas = describe.Execute();
            }
            Debug.Assert(schemas != null, "Should be zero-size, not a null object");
            Debug.Assert(schemas.Count == 0, "Expected empty SDF file");

            //Create one
            AppConsole.WriteLine("Creating class: Class1");
            FeatureSchema schema = new FeatureSchema("Test", "Test Schema");
            FeatureClass c1 = new FeatureClass("Class1", "");
            DataPropertyDefinition idP1 = new DataPropertyDefinition("ID", "");
            idP1.DataType = DataType.DataType_Int32;
            idP1.IsAutoGenerated = true;
            c1.Properties.Add(idP1);
            c1.IdentityProperties.Add(idP1);

            GeometricPropertyDefinition geomP1 = new GeometricPropertyDefinition("Geometry", "");
            geomP1.GeometryTypes = (int)GeometryType.GeometryType_Polygon;

            c1.Properties.Add(geomP1);
            c1.GeometryProperty = geomP1;

            schema.Classes.Add(c1);
            AppConsole.WriteLine("Creating class: Class2");
            FeatureClass c2 = new FeatureClass("Class2", "");
            DataPropertyDefinition idP2 = new DataPropertyDefinition("ID", "");
            idP2.DataType = DataType.DataType_Int32;
            idP2.IsAutoGenerated = true;
            c2.Properties.Add(idP2);
            c2.IdentityProperties.Add(idP2);

            GeometricPropertyDefinition geomP2 = new GeometricPropertyDefinition("Geometry", "");
            geomP2.GeometryTypes = (int)GeometryType.GeometryType_Polygon;

            c2.Properties.Add(geomP2);
            c2.GeometryProperty = geomP2;

            schema.Classes.Add(c2);

            AppConsole.WriteLine("Applying schema");
            try
            {
                using(IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                {
                    apply.FeatureSchema = schema;
                    apply.Execute();
                }
            }
            catch
            {
                Debug.Fail("Could not apply schema");
            }

            AppConsole.WriteLine("Re-describing");
            describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            schemas = null;
            using(describe)
            {
                schemas = describe.Execute();
            }
            Debug.Assert(schemas != null, "Expected non-null object");
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 2, "Expected 2 classes in schema");

            //Now remove Class2
            AppConsole.WriteLine("Removing class: Class2");
            int idx = schemas[0].Classes.IndexOf("Class2");
            Debug.Assert(idx >= 0, "Could not find Class2 in schema Test");
            ClassDefinition cdef = schemas[0].Classes[idx];

            schema = cdef.FeatureSchema;
            schema.Classes.Remove(cdef);

            Debug.Assert(cdef.ElementState == SchemaElementState.SchemaElementState_Detached, "Class2 is not detached");
            AppConsole.WriteLine("Applying schema");
            try
            {
                using(IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                {
                    apply.FeatureSchema = schema;
                    apply.Execute();
                    conn.Flush();
                }
            }
            catch
            {
                Debug.Fail("Could not apply modified schema");
            }

            Debug.Assert(schemas != null, "Expected non-null object");
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");

            idx = schema.Classes.IndexOf("Class2");
            Debug.Assert(idx < 0, "Class2 was not deleted");

            //re-describe
            AppConsole.WriteLine("Re-describe");
            describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema;
            schemas = null;
            using(describe)
            {
                schemas = describe.Execute();
            }
            
            Debug.Assert(schemas != null, "Expected non-null object");
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 1, "Expected 1 class (Class1) in schema");
            idx = schemas[0].Classes.IndexOf("Class2");
            Debug.Assert(idx < 0, "Class2 was not deleted");

            conn.Close();
        }

        private void TestClassDelete()
        {
            AppConsole.WriteLine("TestClassDelete");
            string file = "Test.sdf";
            bool ret = ExpressUtility.CreateSDF(file);
            Debug.Assert(ret, "Could not create " + file);

            IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
            Debug.Assert(conn != null, "Unable to create SDF connection to " + file);

            conn.Open();
            Debug.Assert(conn.ConnectionState == ConnectionState.ConnectionState_Open, "Could not open SDF connection");

            FeatureService service = new FeatureService(conn);
            FeatureSchemaCollection schemas = service.DescribeSchema();

            Debug.Assert(schemas.Count == 0, "Expected empty SDF file");

            //Create one
            FeatureSchema schema = new FeatureSchema("Test", "Test Schema");
            FeatureClass c1 = new FeatureClass("Class1", "");
            DataPropertyDefinition idP1 = new DataPropertyDefinition("ID",""); 
            idP1.DataType = DataType.DataType_Int32;
            idP1.IsAutoGenerated = true;
            c1.Properties.Add(idP1);
            c1.IdentityProperties.Add(idP1);

            GeometricPropertyDefinition geomP1 = new GeometricPropertyDefinition("Geometry", "");
            geomP1.GeometryTypes = (int)GeometryType.GeometryType_Polygon;

            c1.Properties.Add(geomP1);
            c1.GeometryProperty = geomP1;

            schema.Classes.Add(c1);

            FeatureClass c2 = new FeatureClass("Class2", "");
            DataPropertyDefinition idP2 = new DataPropertyDefinition("ID", "");
            idP2.DataType = DataType.DataType_Int32;
            idP2.IsAutoGenerated = true;
            c2.Properties.Add(idP2);
            c2.IdentityProperties.Add(idP2);

            GeometricPropertyDefinition geomP2 = new GeometricPropertyDefinition("Geometry", "");
            geomP2.GeometryTypes = (int)GeometryType.GeometryType_Polygon;

            c2.Properties.Add(geomP2);
            c2.GeometryProperty = geomP2;

            schema.Classes.Add(c2);

            try
            {
                service.ApplySchema(schema);
            }
            catch
            {
                Debug.Assert(false, "Could not apply schema");
            }
            
            schemas = service.DescribeSchema();
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 2, "Expected 2 classes in schema");

            //Now remove Class2

            ClassDefinition cdef = service.GetClassByName("Test", "Class2");
            Debug.Assert(cdef != null, "Could not find Class2 in schema Test");

            schema = cdef.FeatureSchema;
            schema.Classes.Remove(cdef);

            try
            {
                service.ApplySchema(schema);
                service.Connection.Flush();
            }
            catch
            {
                Debug.Assert(false, "Could not apply modified schema");
            }

            schemas = service.DescribeSchema();
            Debug.Assert(schemas.Count == 1, "Expected 1 schema");
            Debug.Assert(schemas[0].Classes.Count == 1, "Expected 1 class (Class1) in schema");

            cdef = service.GetClassByName("Test", "Class2");
            Debug.Assert(cdef == null, "Class2 was not deleted");

            conn.Close();
        }

        private void TestPropertyDelete()
        {
            AppConsole.WriteLine("TestPropertyDelete");
        }
    }
}
