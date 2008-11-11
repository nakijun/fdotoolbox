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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Common;
using System.Data;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Commands.Feature;
using FdoToolbox.Core;

namespace FdoToolbox.Tests
{
    [TestFixture]
    [Category("FdoToolboxCore")]
    public class TableTests : BaseTest
    {
        [Test]
        public void TestClassToTable()
        {
            Class c = new Class("Foo", "Bar");

            AddInt32Property(c, "ID", "ID Property", false, true, true);
            AddStringProperty(c, "Name", "Name Property", true, false, 255, "");
            AddDoubleProperty(c, "Ratio", "Ratio Property", true, false);

            FdoTable table = new FdoTable(c);
            Assert.AreEqual("Foo", table.TableName);
            Assert.AreEqual("Bar", table.Description);
            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(1, table.PrimaryKey.Length);

            Assert.IsNotNull(table.Columns["ID"]);
            Assert.IsNotNull(table.Columns["Name"]);
            Assert.IsNotNull(table.Columns["Ratio"]);

            Assert.AreEqual("ID Property", table.Columns["ID"].Caption);
            Assert.AreEqual("Name Property", table.Columns["Name"].Caption);
            Assert.AreEqual("Ratio Property", table.Columns["Ratio"].Caption);

            FdoDataColumn idCol = table.Columns["ID"] as FdoDataColumn;
            FdoDataColumn nameCol = table.Columns["Name"] as FdoDataColumn;
            FdoDataColumn ratioCol = table.Columns["Ratio"] as FdoDataColumn;

            Assert.IsNotNull(idCol);
            Assert.IsNotNull(nameCol);
            Assert.IsNotNull(ratioCol);

            //Nullable
            Assert.AreEqual(false, idCol.AllowDBNull);
            Assert.AreEqual(true, nameCol.AllowDBNull);
            Assert.AreEqual(true, ratioCol.AllowDBNull);

            //Readonly
            Assert.AreEqual(true, idCol.ReadOnly);
            Assert.AreEqual(false, nameCol.ReadOnly);
            Assert.AreEqual(false, ratioCol.ReadOnly);
        
            //Length
            Assert.AreEqual(255, nameCol.MaxLength);
        }

        [Test]
        public void TestFeatureClassToTable()
        {
            FeatureClass c = new FeatureClass("Foo", "Bar");

            AddInt32Property(c, "ID", "ID Property", false, true, true);
            AddStringProperty(c, "Name", "Name Property", true, false, 255, "");
            AddDoubleProperty(c, "Ratio", "Ratio Property", true, false);

            GeometricPropertyDefinition g = new GeometricPropertyDefinition("Geometry", "Geometry Property");
            g.SpatialContextAssociation = "Default";
            g.GeometryTypes = 12;
            g.HasElevation = false;
            g.HasMeasure = true;

            c.Properties.Add(g);
            c.GeometryProperty = g;

            FdoFeatureTable table = new FdoFeatureTable(c);
            Assert.AreEqual("Foo", table.TableName);
            Assert.AreEqual("Bar", table.Description);
            Assert.AreEqual(4, table.Columns.Count);
            Assert.AreEqual(1, table.PrimaryKey.Length);

            Assert.IsNotNull(table.Columns["ID"]);
            Assert.IsNotNull(table.Columns["Name"]);
            Assert.IsNotNull(table.Columns["Ratio"]);
            Assert.IsNotNull(table.Columns["Geometry"]);
            Assert.IsNotNull(table.GeometryColumn);

            Assert.AreEqual("ID Property", table.Columns["ID"].Caption);
            Assert.AreEqual("Name Property", table.Columns["Name"].Caption);
            Assert.AreEqual("Ratio Property", table.Columns["Ratio"].Caption);
            Assert.AreEqual("Geometry Property", table.Columns["Geometry"].Caption);
            Assert.AreEqual("Geometry Property", table.GeometryColumn.Caption);
            Assert.AreEqual(table.GeometryColumn, table.Columns["Geometry"]);

            FdoDataColumn idCol = table.Columns["ID"] as FdoDataColumn;
            FdoDataColumn nameCol = table.Columns["Name"] as FdoDataColumn;
            FdoDataColumn ratioCol = table.Columns["Ratio"] as FdoDataColumn;
            FdoGeometryColumn geomCol = table.GeometryColumn;

            Assert.IsNotNull(idCol);
            Assert.IsNotNull(nameCol);
            Assert.IsNotNull(ratioCol);

            //Nullable
            Assert.AreEqual(false, idCol.AllowDBNull);
            Assert.AreEqual(true, nameCol.AllowDBNull);
            Assert.AreEqual(true, ratioCol.AllowDBNull);

            //Readonly
            Assert.AreEqual(true, idCol.ReadOnly);
            Assert.AreEqual(false, nameCol.ReadOnly);
            Assert.AreEqual(false, ratioCol.ReadOnly);

            //Length
            Assert.AreEqual(255, nameCol.MaxLength);

            //Geometry
            Assert.AreEqual(12, geomCol.GeometryTypes);
            Assert.AreEqual("Default", geomCol.SpatialContextAssociation);
            Assert.AreEqual(false, geomCol.HasElevation);
            Assert.AreEqual(true, geomCol.HasMeasure);
        }

        [Test]
        public void TestTableToClass()
        {
            FdoTable table = new FdoTable("Foo", "Bar");

            FdoDataColumn idCol = new FdoDataColumn("ID", "ID Property");
            idCol.AutoIncrement = true;
            idCol.AllowDBNull = false;  
            idCol.DataType = typeof(int);

            FdoDataColumn nameCol = new FdoDataColumn("Name", "Name property");
            nameCol.MaxLength = 255;
            nameCol.DataType = typeof(string);
            nameCol.DefaultValue = "Foobar";
            nameCol.AllowDBNull = true;
            nameCol.ReadOnly = true;

            FdoDataColumn ratioCol = new FdoDataColumn("Ratio", "Ratio property");
            ratioCol.AllowDBNull = true;
            ratioCol.ReadOnly = false;
            ratioCol.DataType = typeof(double);

            table.Columns.Add(idCol);
            table.Columns.Add(nameCol);
            table.Columns.Add(ratioCol);

            table.PrimaryKey = new DataColumn[] { idCol };

            Class c = (Class)table.GetClassDefinition();

            Assert.AreEqual("Foo", c.Name);
            Assert.AreEqual("Bar", c.Description);

            Assert.AreEqual(3, c.Properties.Count);
            Assert.AreEqual(1, c.IdentityProperties.Count);

            int i1 = c.Properties.IndexOf("ID");
            int i2 = c.Properties.IndexOf("Name");
            int i3 = c.Properties.IndexOf("Ratio");

            Assert.IsTrue(i1 >= 0);
            Assert.IsTrue(i2 >= 0);
            Assert.IsTrue(i3 >= 0);

            DataPropertyDefinition pID = c.Properties[i1] as DataPropertyDefinition;
            DataPropertyDefinition pName = c.Properties[i2] as DataPropertyDefinition;
            DataPropertyDefinition pRatio = c.Properties[i3] as DataPropertyDefinition;

            Assert.IsNotNull(pID);
            Assert.IsNotNull(pName);
            Assert.IsNotNull(pRatio);

            Assert.AreEqual(DataType.DataType_Int32, pID.DataType);
            Assert.AreEqual(DataType.DataType_String, pName.DataType);
            Assert.AreEqual(DataType.DataType_Double, pRatio.DataType);

            Assert.IsTrue(pID.IsAutoGenerated);
            Assert.IsTrue(pID.ReadOnly);
            Assert.IsFalse(pID.Nullable);

            Assert.IsTrue(pName.ReadOnly);
            Assert.IsTrue(pName.Nullable);
            Assert.AreEqual(255, pName.Length);

            Assert.IsFalse(pRatio.ReadOnly);
            Assert.IsTrue(pRatio.Nullable);
            
        }

        [Test]
        [ExpectedException(typeof(DataTableConversionException))]
        public void TestBadRawTable()
        {
            DataTable table = new DataTable();
            FdoDataTable fdoTable = TableFactory.CreateTable(table);
            ClassDefinition classDef = fdoTable.GetClassDefinition();
            Assert.Fail("Should not have got to this point");
        }

        [Test]
        [ExpectedException(typeof(DataTableConversionException))]
        public void TestBadRawTable2()
        {
            DataTable table = new DataTable();

            DataColumn colA = new DataColumn("A", typeof(int));
            DataColumn colB = new DataColumn("B", typeof(byte[]));
            DataColumn colC = new DataColumn("C", typeof(bool));
            DataColumn colD = new DataColumn("D", typeof(byte));
            DataColumn colE = new DataColumn("E", typeof(DateTime));
            DataColumn colF = new DataColumn("F", typeof(decimal));
            DataColumn colG = new DataColumn("G", typeof(float));
            DataColumn colH = new DataColumn("H", typeof(short));
            DataColumn colI = new DataColumn("I", typeof(long));
            DataColumn colJ = new DataColumn("J", typeof(string));
            DataColumn colK = new DataColumn("K", typeof(double));

            table.Columns.Add(colA);
            table.Columns.Add(colB);
            table.Columns.Add(colC);
            table.Columns.Add(colD);
            table.Columns.Add(colE);
            table.Columns.Add(colF);
            table.Columns.Add(colG);
            table.Columns.Add(colH);
            table.Columns.Add(colI);
            table.Columns.Add(colJ);
            table.Columns.Add(colK);

            table.PrimaryKey = new DataColumn[] { colA };

            FdoDataTable fdoTable = TableFactory.CreateTable(table);
            ClassDefinition classDef = fdoTable.GetClassDefinition();
            Assert.Fail("Should not have got to this point");
        }

        [Test]
        public void TestRawTableToClass()
        {
            DataTable table = new DataTable();
            table.TableName = "Foobar";

            DataColumn colA = new DataColumn("A", typeof(int));
            DataColumn colC = new DataColumn("C", typeof(bool));
            DataColumn colD = new DataColumn("D", typeof(byte));
            DataColumn colE = new DataColumn("E", typeof(DateTime));
            DataColumn colG = new DataColumn("G", typeof(float));
            DataColumn colH = new DataColumn("H", typeof(short));
            DataColumn colI = new DataColumn("I", typeof(long));
            DataColumn colJ = new DataColumn("J", typeof(string));
            DataColumn colK = new DataColumn("K", typeof(double));

            colJ.MaxLength = 255;

            table.Columns.Add(colA);
            table.Columns.Add(colC);
            table.Columns.Add(colD);
            table.Columns.Add(colE);
            table.Columns.Add(colG);
            table.Columns.Add(colH);
            table.Columns.Add(colI);
            table.Columns.Add(colJ);
            table.Columns.Add(colK);

            table.PrimaryKey = new DataColumn[] { colA };

            FdoDataTable fdoTable = TableFactory.CreateTable(table);
            ClassDefinition classDef = fdoTable.GetClassDefinition();

            Assert.AreEqual(ClassType.ClassType_Class, classDef.ClassType);
            Assert.AreEqual(fdoTable.TableName, classDef.Name);
            Assert.AreEqual(fdoTable.Description, classDef.Description);
            Assert.AreEqual(fdoTable.Columns.Count, classDef.Properties.Count);
            Assert.AreEqual(fdoTable.PrimaryKey.Length, classDef.IdentityProperties.Count);

            int a = classDef.Properties.IndexOf("A");
            int c = classDef.Properties.IndexOf("C");
            int d = classDef.Properties.IndexOf("D");
            int e = classDef.Properties.IndexOf("E");
            int g = classDef.Properties.IndexOf("G");
            int h = classDef.Properties.IndexOf("H");
            int i = classDef.Properties.IndexOf("I");
            int j = classDef.Properties.IndexOf("J");
            int k = classDef.Properties.IndexOf("K");

            Assert.IsTrue(a >= 0);
            Assert.IsTrue(c >= 0);
            Assert.IsTrue(d >= 0);
            Assert.IsTrue(e >= 0);
            Assert.IsTrue(g >= 0);
            Assert.IsTrue(h >= 0);
            Assert.IsTrue(i >= 0);
            Assert.IsTrue(j >= 0);
            Assert.IsTrue(k >= 0);

            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[a].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[c].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[d].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[e].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[g].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[h].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[i].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[j].PropertyType);
            Assert.AreEqual(PropertyType.PropertyType_DataProperty, classDef.Properties[k].PropertyType);

            DataPropertyDefinition dpa = classDef.Properties[a] as DataPropertyDefinition;
            DataPropertyDefinition dpc = classDef.Properties[c] as DataPropertyDefinition;
            DataPropertyDefinition dpd = classDef.Properties[d] as DataPropertyDefinition;
            DataPropertyDefinition dpe = classDef.Properties[e] as DataPropertyDefinition;
            DataPropertyDefinition dpg = classDef.Properties[g] as DataPropertyDefinition;
            DataPropertyDefinition dph = classDef.Properties[h] as DataPropertyDefinition;
            DataPropertyDefinition dpi = classDef.Properties[i] as DataPropertyDefinition;
            DataPropertyDefinition dpj = classDef.Properties[j] as DataPropertyDefinition;
            DataPropertyDefinition dpk = classDef.Properties[k] as DataPropertyDefinition;

            Assert.AreEqual(DataType.DataType_Int32, dpa.DataType);
            Assert.AreEqual(DataType.DataType_Boolean, dpc.DataType);
            Assert.AreEqual(DataType.DataType_Byte, dpd.DataType);
            Assert.AreEqual(DataType.DataType_DateTime, dpe.DataType);
            Assert.AreEqual(DataType.DataType_Single, dpg.DataType);
            Assert.AreEqual(DataType.DataType_Int16, dph.DataType);
            Assert.AreEqual(DataType.DataType_Int64, dpi.DataType);
            Assert.AreEqual(DataType.DataType_String, dpj.DataType);
            Assert.AreEqual(DataType.DataType_Double, dpk.DataType);

            Assert.AreEqual(255, dpj.Length);
        }

        [Test]
        public void TestTableToFeatureClass()
        {
            FdoFeatureTable table = new FdoFeatureTable("Foo", "Bar");

            FdoDataColumn idCol = new FdoDataColumn("ID", "ID Property");
            idCol.AutoIncrement = true;
            idCol.AllowDBNull = false;
            idCol.DataType = typeof(int);

            FdoDataColumn nameCol = new FdoDataColumn("Name", "Name property");
            nameCol.MaxLength = 255;
            nameCol.DataType = typeof(string);
            nameCol.DefaultValue = "Foobar";
            nameCol.AllowDBNull = true;
            nameCol.ReadOnly = true;

            FdoDataColumn ratioCol = new FdoDataColumn("Ratio", "Ratio property");
            ratioCol.AllowDBNull = true;
            ratioCol.ReadOnly = false;
            ratioCol.DataType = typeof(double);

            FdoGeometryColumn geomCol = new FdoGeometryColumn("Geometry", "Geometry property");
            geomCol.HasElevation = true;
            geomCol.HasMeasure = false;
            geomCol.ReadOnly = true;
            geomCol.GeometryTypes = 12;

            table.Columns.Add(idCol);
            table.Columns.Add(nameCol);
            table.Columns.Add(ratioCol);
            table.Columns.Add(geomCol);

            table.PrimaryKey = new DataColumn[] { idCol };
            table.GeometryColumn = geomCol;

            FeatureClass c = (FeatureClass)table.GetClassDefinition();

            Assert.AreEqual("Foo", c.Name);
            Assert.AreEqual("Bar", c.Description);

            Assert.AreEqual(4, c.Properties.Count);
            Assert.AreEqual(1, c.IdentityProperties.Count);

            int i1 = c.Properties.IndexOf("ID");
            int i2 = c.Properties.IndexOf("Name");
            int i3 = c.Properties.IndexOf("Ratio");
            int i4 = c.Properties.IndexOf("Geometry");

            Assert.IsTrue(i1 >= 0);
            Assert.IsTrue(i2 >= 0);
            Assert.IsTrue(i3 >= 0);
            Assert.IsTrue(i4 >= 0);

            DataPropertyDefinition pID = c.Properties[i1] as DataPropertyDefinition;
            DataPropertyDefinition pName = c.Properties[i2] as DataPropertyDefinition;
            DataPropertyDefinition pRatio = c.Properties[i3] as DataPropertyDefinition;
            GeometricPropertyDefinition g = c.Properties[i4] as GeometricPropertyDefinition;

            Assert.IsNotNull(pID);
            Assert.IsNotNull(pName);
            Assert.IsNotNull(pRatio);
            Assert.IsNotNull(g);

            Assert.AreEqual(DataType.DataType_Int32, pID.DataType);
            Assert.AreEqual(DataType.DataType_String, pName.DataType);
            Assert.AreEqual(DataType.DataType_Double, pRatio.DataType);

            Assert.IsTrue(pID.IsAutoGenerated);
            Assert.IsTrue(pID.ReadOnly);
            Assert.IsFalse(pID.Nullable);

            Assert.IsTrue(pName.ReadOnly);
            Assert.IsTrue(pName.Nullable);
            Assert.AreEqual(255, pName.Length);

            Assert.IsFalse(pRatio.ReadOnly);
            Assert.IsTrue(pRatio.Nullable);

            Assert.IsTrue(g.HasElevation);
            Assert.IsFalse(g.HasMeasure);
            Assert.IsTrue(g.ReadOnly);
            Assert.AreEqual(12, g.GeometryTypes);
        }

        [Test]
        [Explicit]
        public void TestTableLoad()
        {
            string file = "C:\\temp\\World_Countries.sdf";
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SDF");
            conn.ConnectionString = string.Format("File={0}", file);
            conn.Open();
            long count = 0;
            using (conn)
            {
                using (FeatureService service = new FeatureService(conn))
                {
                    FeatureAggregateOptions fa = new FeatureAggregateOptions("World_Countries");
                    fa.AddComputedProperty("FEATCOUNT", "COUNT(Autogenerated_ID)");

                    using (OSGeo.FDO.Commands.Feature.IDataReader reader = service.SelectAggregates(fa))
                    {
                        if (reader.ReadNext())
                        {
                            count = reader.GetInt64("FEATCOUNT");
                        }
                    }

                    FeatureQueryOptions fq = new FeatureQueryOptions("World_Countries");
                    using (IFeatureReader reader = service.SelectFeatures(fq))
                    {
                        DataTable table = null;
                        ClassDefinition classDef = reader.GetClassDefinition();
                        switch (classDef.ClassType)
                        {
                            case ClassType.ClassType_FeatureClass:
                                FdoFeatureTable fft = new FdoFeatureTable(classDef as FeatureClass);
                                fft.LoadFromFeatureReader(reader);
                                table = fft;
                                break;
                            case ClassType.ClassType_Class:
                                FdoTable ft = new FdoTable(classDef as Class);
                                ft.LoadFromFeatureReader(reader);
                                table = ft;
                                break;
                        }
                        Assert.IsNotNull(table);
                        Assert.AreEqual((int)count, table.Rows.Count);
                    }
                }
                conn.Close();
            }
        }

        private void AddInt16Property(ClassDefinition c, string name, string description, bool nullable, bool autoGenerated, bool identity)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_Int16;
            d.IsAutoGenerated = autoGenerated;
            d.Nullable = autoGenerated ? true : nullable;
            d.ReadOnly = autoGenerated ? true : false;
            c.Properties.Add(d);
            if (identity)
                c.IdentityProperties.Add(d);
        }

        private void AddInt32Property(ClassDefinition c, string name, string description, bool nullable, bool autoGenerated, bool identity)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_Int32;
            d.IsAutoGenerated = autoGenerated;
            d.Nullable = autoGenerated ? true : nullable;
            d.ReadOnly = autoGenerated ? true : false;
            c.Properties.Add(d);
            if (identity)
                c.IdentityProperties.Add(d);
        }

        private void AddInt64Property(ClassDefinition c, string name, string description, bool nullable, bool autoGenerated, bool identity)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_Int64;
            d.IsAutoGenerated = autoGenerated;
            d.Nullable = autoGenerated ? true : nullable;
            d.ReadOnly = autoGenerated ? true : false;
            c.Properties.Add(d);
            if (identity)
                c.IdentityProperties.Add(d);
        }

        private void AddFloatProperty(ClassDefinition c, string name, string description, bool nullable, bool readOnly)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_Single;
            d.Nullable = nullable;
            d.ReadOnly = readOnly;
            c.Properties.Add(d);
        }

        private void AddDoubleProperty(ClassDefinition c, string name, string description, bool nullable, bool readOnly)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_Double;
            d.Nullable = nullable;
            d.ReadOnly = readOnly;
            c.Properties.Add(d);
        }

        private void AddStringProperty(ClassDefinition c, string name, string description, bool nullable, bool readOnly, int length, string defaultValue)
        {
            DataPropertyDefinition d = new DataPropertyDefinition(name, description);
            d.DataType = DataType.DataType_String;
            d.Nullable = nullable;
            d.ReadOnly = readOnly;
            d.Length = length;
            d.DefaultValue = defaultValue;
            c.Properties.Add(d);
        }
    }
}
