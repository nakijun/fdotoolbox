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
using FdoToolbox.Core.Common;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.ETL
{
    public class DbToPointCopyTask : TaskBase
    {
        private DbToPointCopyOptions _Options;

        public DbToPointCopyOptions Options
        {
            get { return _Options; }
            set { _Options = value; }
        }

        public DbToPointCopyTask(DbToPointCopyOptions options)
        {
            this.Options = options;
        }
	
        public override void ValidateTaskParameters()
        {
            using (FeatureService service = new FeatureService(this.Options.Target.Connection))
            {
                ClassDefinition classDef = service.GetClassByName(
                    this.Options.SchemaName,
                    this.Options.ClassName);

                if (classDef != null)
                    throw new TaskValidationException("A class named " + this.Options.ClassName + " already exists");
            }

            System.Data.IDbCommand cmd = this.Options.Source.Connection.CreateCommand();
            if (!string.IsNullOrEmpty(this.Options.ZColumn))
            {
                cmd.CommandText = string.Format("SELECT {0}, {1}, {2} FROM {3} WHERE {0} IS NULL OR {1} IS NULL OR {2} IS NULL",
                    this.Options.XColumn,
                    this.Options.YColumn,
                    this.Options.ZColumn,
                    this.Options.Table);
            }
            else
            {
                cmd.CommandText = string.Format("SELECT {0}, {1} FROM {2} WHERE {0} IS NULL OR {1} IS NULL",
                    this.Options.XColumn,
                    this.Options.YColumn,
                    this.Options.Table);
            }

            using (System.Data.IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    throw new TaskValidationException("One or more records has a null X, Y or Z value");

                reader.Close();
            }

            //TODO: MyMeta should be doing this job, but the column types are
            //not being picked up, so for now do a select * and infer the schema 
            //from the data reader.
            cmd.CommandText = string.Format("SELECT * FROM {0}", this.Options.Table);
            using (System.Data.IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    CheckNumericColumn(reader, this.Options.XColumn);
                    CheckNumericColumn(reader, this.Options.YColumn);
                    CheckNumericColumn(reader, this.Options.ZColumn);

                    foreach (string colName in this.Options.ColumnList)
                    {
                        if (reader.GetOrdinal(colName) < 0)
                            throw new TaskValidationException(
                                string.Format("Could not find column {0} in table {1}",
                                    colName,
                                    this.Options.Table));
                    }
                }
                reader.Close();
            }
            

            /*
            MyMeta.ITable table = this.Options.Source.GetTable(this.Options.Database, this.Options.Table);
            foreach (string colName in this.Options.ColumnList)
            {
                bool found = false;
                foreach (MyMeta.IColumn column in table.Columns)
                {
                    if (column.Name == colName)
                        found = true;

                    //If X, Y or Z column, make sure it is numeric
                    if (column.Name == this.Options.XColumn || 
                        column.Name == this.Options.YColumn ||
                        column.Name == this.Options.ZColumn)
                    {
                        switch (column.LanguageType)
                        {
                            case "int":
                                break;
                            case "double":
                                break;
                            case "float":
                                break;
                            case "long":
                                break;
                            case "decimal":
                                break;
                            case "byte":
                                break;
                            case "short":
                                break;
                            default:
                                throw new TaskValidationException(
                                    string.Format(
                                        "Column {0} is not a numeric column", 
                                        colName));
                        }
                    }
                }
                if (!found)
                    throw new TaskValidationException(
                        string.Format(
                            "Could not find column {0} in table {1}", 
                            colName, 
                            table.Name));
            }
             */
        }

        private void CheckNumericColumn(System.Data.IDataReader reader, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                int ordinal = reader.GetOrdinal(name);
                if (ordinal >= 0)
                {
                    Type t = reader.GetFieldType(ordinal);
                    if (t != typeof(int) &&
                        t != typeof(double) &&
                        t != typeof(float) &&
                        t != typeof(long) &&
                        t != typeof(decimal) &&
                        t != typeof(byte) &&
                        t != typeof(short))
                    {
                        throw new TaskValidationException(
                            string.Format(
                                "Column {0} is not a numeric column",
                                name));
                    }
                }
            }
        }

        const string GEOM_PROP = "Geometry";

        public override void DoExecute()
        {
            FeatureService service = new FeatureService(this.Options.Target.Connection);
            List<string> columns = GetColumnList();

            System.Data.IDbCommand dbCmd = this.Options.Source.Connection.CreateCommand();
            System.Data.DataTable table = new System.Data.DataTable();
            SendMessage("Reading database");
            using (dbCmd)
            {
                dbCmd.CommandText = string.Format("SELECT {0} FROM {1}", string.Join(", ", columns.ToArray()), this.Options.Table);
                using (System.Data.IDataReader reader = dbCmd.ExecuteReader())
                {
                    table.Load(reader);
                    reader.Close();
                }
            }
            int total = table.Rows.Count;
            FeatureSchema schema = service.GetSchemaByName(this.Options.SchemaName);

            SendMessage("Creating Feature Class");
            FeatureClass klass = CreateTargetClass(table);
            schema.Classes.Add(klass);

            SendMessage("Applying schema");
            service.ApplySchema(schema);

            SendMessage("Begin copy");
            columns = new List<string>(this.Options.ColumnList);
            columns.Remove(this.Options.XColumn);
            columns.Remove(this.Options.YColumn);
            columns.Remove(this.Options.ZColumn);
            int copied = 0;
            int oldpc = 0;
            FgfGeometryFactory factory = new FgfGeometryFactory();
            IInsert insert = service.CreateCommand<IInsert>(CommandType.CommandType_Insert);
            insert.SetFeatureClassName(this.Options.Table); //class name = table name
            foreach (System.Data.DataRow row in table.Rows)
            {
                insert.PropertyValues.Clear();
                foreach (string colName in columns)
                {
                    System.Data.DataColumn dcol = table.Columns[colName];
                    object obj = row[colName];
                    if (dcol.DataType == typeof(byte[]))
                        insert.PropertyValues.Add(new PropertyValue(colName, new BLOBValue((byte[])obj)));
                    else if (dcol.DataType == typeof(byte))
                        insert.PropertyValues.Add(new PropertyValue(colName, new ByteValue(Convert.ToByte(obj))));
                    else if (dcol.DataType == typeof(bool))
                        insert.PropertyValues.Add(new PropertyValue(colName, new BooleanValue(Convert.ToBoolean(obj))));
                    else if (dcol.DataType == typeof(DateTime))
                        insert.PropertyValues.Add(new PropertyValue(colName, new DateTimeValue(Convert.ToDateTime(obj))));
                    else if (dcol.DataType == typeof(double))
                        insert.PropertyValues.Add(new PropertyValue(colName, new DoubleValue(Convert.ToDouble(obj))));
                    else if (dcol.DataType == typeof(decimal))
                        insert.PropertyValues.Add(new PropertyValue(colName, new DecimalValue(Convert.ToDouble(obj))));
                    else if (dcol.DataType == typeof(short))
                        insert.PropertyValues.Add(new PropertyValue(colName, new Int16Value(Convert.ToInt16(obj))));
                    else if (dcol.DataType == typeof(int))
                        insert.PropertyValues.Add(new PropertyValue(colName, new Int32Value(Convert.ToInt32(obj))));
                    else if (dcol.DataType == typeof(long))
                        insert.PropertyValues.Add(new PropertyValue(colName, new Int64Value(Convert.ToInt64(obj))));
                    else if (dcol.DataType == typeof(float))
                        insert.PropertyValues.Add(new PropertyValue(colName, new SingleValue(Convert.ToSingle(obj))));
                    else if (dcol.DataType == typeof(string))
                        insert.PropertyValues.Add(new PropertyValue(colName, new StringValue(Convert.ToString(obj))));
                }

                IPoint pt = null;

                //3D
                if (!string.IsNullOrEmpty(this.Options.ZColumn))
                {
                    double x = Convert.ToDouble(row[this.Options.XColumn]);
                    double y = Convert.ToDouble(row[this.Options.YColumn]);
                    double z = Convert.ToDouble(row[this.Options.ZColumn]);
                    pt = factory.CreatePoint(factory.CreatePositionXYZ(x, y, z));
                }
                else //2D 
                {
                    double x = Convert.ToDouble(row[this.Options.XColumn]);
                    double y = Convert.ToDouble(row[this.Options.YColumn]);
                    pt = factory.CreatePoint(factory.CreatePositionXY(x, y));
                }

                insert.PropertyValues.Add(
                    new PropertyValue(GEOM_PROP, 
                    new GeometryValue(factory.GetFgf(pt))));

                using (IFeatureReader reader = insert.Execute())
                {
                    copied++;
                    reader.Close();
                }

                pt.Dispose();

                int pc = (int)(((double)copied / (double)total) * 100);
                //Only update progress counter when % changes
                if (pc != oldpc)
                {
                    oldpc = pc;
                    SendMessage(string.Format("Copying class : {0} ({1}% of {2} features)", this.Options.Table, oldpc, total));
                    SendCount(oldpc);
                }
            }
            factory.Dispose();
            SendMessage("Copy complete");
        }

        private FeatureClass CreateTargetClass(System.Data.DataTable table)
        {
            FeatureClass fc = new FeatureClass(this.Options.Table, "");

            GeometricPropertyDefinition geomDef = new GeometricPropertyDefinition(GEOM_PROP, "");
            geomDef.GeometryTypes = (int)OSGeo.FDO.Common.GeometryType.GeometryType_Point;
            geomDef.HasElevation = !string.IsNullOrEmpty(this.Options.ZColumn);
            geomDef.HasMeasure = false;
            geomDef.ReadOnly = false;

            fc.Properties.Add(geomDef);

            foreach (System.Data.DataColumn column in table.Columns)
            {
                if (column.ColumnName != this.Options.XColumn &&
                    column.ColumnName != this.Options.YColumn &&
                    column.ColumnName != this.Options.ZColumn)
                {
                    DataPropertyDefinition def = new DataPropertyDefinition(column.ColumnName, "");
                    def.IsAutoGenerated = column.AutoIncrement;
                    def.Nullable = column.AllowDBNull;
                    if(column.DataType == typeof(byte[])) {
                        def.DataType = DataType.DataType_BLOB;
                        def.Length = column.MaxLength;
                    } else if (column.DataType == typeof(bool)) {
                        def.DataType = DataType.DataType_Boolean;
                    } else if (column.DataType == typeof(byte)) {
                        def.DataType = DataType.DataType_Byte;
                    } else if (column.DataType == typeof(DateTime)) {
                        def.DataType = DataType.DataType_DateTime;
                    } else if (column.DataType == typeof(decimal)) {
                        def.DataType = DataType.DataType_Decimal;
                    } else if (column.DataType == typeof(double)) {
                        def.DataType = DataType.DataType_Double;
                    } else if (column.DataType == typeof(short)) {
                        def.DataType = DataType.DataType_Int16;
                    } else if (column.DataType == typeof(int)) {
                        def.DataType = DataType.DataType_Int32;
                    } else if (column.DataType == typeof(long)) {
                        def.DataType = DataType.DataType_Int64;
                    } else if (column.DataType == typeof(float)) {
                        def.DataType = DataType.DataType_Single;
                    } else if (column.DataType == typeof(string)) {
                        def.DataType = DataType.DataType_String;
                        def.Length = column.MaxLength;
                    }

                    fc.Properties.Add(def);
                    if (Array.IndexOf<System.Data.DataColumn>(table.PrimaryKey, column) >= 0)
                        fc.IdentityProperties.Add(def);
                }
            }
            //Create an auto-id if no identity properties found
            if (fc.IdentityProperties.Count == 0)
            {
                DataPropertyDefinition autoID = new DataPropertyDefinition("AutoID", "Auto-generated ID");
                autoID.DataType = DataType.DataType_Int32;
                autoID.IsAutoGenerated = true;

                fc.Properties.Add(autoID);
                fc.IdentityProperties.Add(autoID);
            }
            return fc;
        }

        private List<string> GetColumnList()
        {
            //Store all the columns
            List<string> columns = new List<string>(this.Options.ColumnList);
            if (!columns.Contains(this.Options.XColumn))
                columns.Add(this.Options.XColumn);
            if (!columns.Contains(this.Options.YColumn))
                columns.Add(this.Options.YColumn);
            if (!string.IsNullOrEmpty(this.Options.ZColumn) && !columns.Contains(this.Options.ZColumn))
                columns.Add(this.Options.ZColumn);
            return columns;
        }
        
        public override TaskType TaskType
        {
            get { return TaskType.NonSpatialToSpatialBulkCopy; }
        }

        public override bool IsCountable
        {
            get { return true; }
        }
    }
}
