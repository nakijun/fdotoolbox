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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Feature;
using System.Data;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// Extension of DataTable to allow more FDO specific functionality. FDO specific information
    /// is stored as table/column metadata.
    /// </summary>
    public abstract class FdoDataTable : System.Data.DataTable 
    {
        protected FdoDataTable() : base() { }

        protected FdoDataTable(string name, string description)
            : base() 
        {
            this.TableName = name;
            this.Description = description;
        }

        public string Description
        {
            get { return this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_DESCRIPTION].ToString(); }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_DESCRIPTION] = value; }
        }

        public ClassType TableType
        {
            get { return (ClassType)this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_TYPE]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_TYPE] = value; }
        }

        public virtual void InitFromTable(DataTable table)
        {
            if (string.IsNullOrEmpty(table.TableName))
                throw new DataTableConversionException("The DataTable has no table name");

            object desc = FdoMetaData.GetMetaData(table, FdoMetaDataNames.FDO_CLASS_DESCRIPTION);
            if (desc != null)
                this.Description = desc.ToString();
            else
                this.Description = string.Empty;

            this.TableType = GetClassType();
            this.TableName = table.TableName;

            this.PrimaryKey = null;
            this.Columns.Clear();

            this.Merge(table);
        }

        protected abstract ClassType GetClassType();

        public virtual void InitFromClass(ClassDefinition classDef)
        {
            this.TableName = classDef.Name;
            this.Description = classDef.Description;
            this.TableType = classDef.ClassType;
            this.PrimaryKey = null;
            this.Columns.Clear();
            List<string> idNames = new List<string>();
            foreach (PropertyDefinition pd in classDef.Properties)
            {
                switch (pd.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        {
                            DataPropertyDefinition dp = pd as DataPropertyDefinition;
                            AddDataProperty(dp);
                            if (classDef.IdentityProperties.Contains(dp))
                                idNames.Add(dp.Name);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        {
                            AddGeometricProperty(pd as GeometricPropertyDefinition);
                        }
                        break;
                    default:
                        throw new DataTableConversionException("Property type not supported: " + pd.PropertyType);
                }
            }
            //Set primary keys
            List<DataColumn> pks = new List<DataColumn>();
            foreach (string name in idNames)
            {
                pks.Add(this.Columns[name]);
            }
            this.PrimaryKey = pks.ToArray();
        }

        private void AddGeometricProperty(GeometricPropertyDefinition gp)
        {
            FdoGeometryColumn col = new FdoGeometryColumn(gp);
            this.Columns.Add(col);
        }

        private void AddDataProperty(DataPropertyDefinition dp)
        {
            FdoDataColumn col = new FdoDataColumn(dp);
            this.Columns.Add(col);
        }

        public void LoadFromFeatureReader(IFeatureReader reader)
        {
            ClassDefinition classDef = reader.GetClassDefinition();
            InitFromClass(classDef);

            while (reader.ReadNext())
            {
                AddRowFromReader(reader);
            }
        }

        private static PropertyType GetColumnType(DataColumn dc)
        {
            return (PropertyType)dc.ExtendedProperties[FdoMetaDataNames.FDO_PROPERTY_TYPE];
        }

        private void AddRowFromReader(IFeatureReader reader)
        {
            DataRow row = this.NewRow();
            foreach (DataColumn dc in this.Columns)
            {
                string name = dc.ColumnName;
                if (!reader.IsNull(name))
                {
                    PropertyType pt = GetColumnType(dc);
                    switch (pt)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            {
                                FdoDataColumn fdc = dc as FdoDataColumn;
                                DataType dt = fdc.GetDataType();
                                switch (dt)
                                {
                                    case DataType.DataType_BLOB:
                                        row[dc] = reader.GetLOB(name).Data;
                                        break;
                                    case DataType.DataType_Boolean:
                                        row[dc] = reader.GetBoolean(name);
                                        break;
                                    case DataType.DataType_Byte:
                                        row[dc] = reader.GetByte(name);
                                        break;
                                    case DataType.DataType_CLOB:
                                        //Not supported
                                        break;
                                    case DataType.DataType_DateTime:
                                        row[dc] = reader.GetDateTime(name);
                                        break;
                                    case DataType.DataType_Decimal:
                                        row[dc] = Convert.ToDecimal(reader.GetDouble(name));
                                        break;
                                    case DataType.DataType_Double:
                                        row[dc] = reader.GetDouble(name);
                                        break;
                                    case DataType.DataType_Int16:
                                        row[dc] = reader.GetInt16(name);
                                        break;
                                    case DataType.DataType_Int32:
                                        row[dc] = reader.GetInt32(name);
                                        break;
                                    case DataType.DataType_Int64:
                                        row[dc] = reader.GetInt64(name);
                                        break;
                                    case DataType.DataType_Single:
                                        row[dc] = reader.GetSingle(name);
                                        break;
                                    case DataType.DataType_String:
                                        row[dc] = reader.GetString(name);
                                        break;
                                }
                            }
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            {
                                byte[] fgf = reader.GetGeometry(name);
                                if (dc.DataType == typeof(byte[]))
                                    row[dc] = fgf;
                                else
                                    row[dc] = FdoGeometryUtil.GetFgfText(fgf);
                            }
                            break;
                    }
                }
            }
            this.Rows.Add(row);
        }

        public virtual ClassDefinition GetClassDefinition()
        {
            ClassDefinition classDef = CreateClassDefinition();
            classDef.Name = this.TableName;
            classDef.Description = this.Description;
            foreach (DataColumn col in this.Columns)
            {
                if (FdoMetaData.HasMetaData(col, FdoMetaDataNames.FDO_PROPERTY_TYPE))
                {
                    PropertyType pt = GetColumnType(col);
                    switch (pt)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            {
                                FdoDataColumn dc = col as FdoDataColumn;
                                PropertyDefinition pd = dc.GetPropertyDefinition();
                                classDef.Properties.Add(pd);
                                if (Array.IndexOf<DataColumn>(this.PrimaryKey, col) >= 0)
                                    classDef.IdentityProperties.Add(pd as DataPropertyDefinition);
                            }
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            {
                                FdoGeometryColumn gc = col as FdoGeometryColumn;
                                classDef.Properties.Add(gc.GetPropertyDefinition());
                            }
                            break;
                    }
                }
                else //There is no fdo metadata, so infer the attributes from the DataColumn
                {
                    DataPropertyDefinition def = new DataPropertyDefinition(col.ColumnName, col.Caption);

                    def.DataType = FdoDataColumn.GetDataTypeFromType(col.DataType);
                    def.IsAutoGenerated = col.AutoIncrement;
                    def.Nullable = col.AllowDBNull;
                    def.ReadOnly = def.IsAutoGenerated ? true : col.ReadOnly;

                    if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_String &&
                        col.DefaultValue != null)
                        def.DefaultValue = col.DefaultValue.ToString();

                    if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_BLOB ||
                        def.DataType == OSGeo.FDO.Schema.DataType.DataType_CLOB ||
                        def.DataType == OSGeo.FDO.Schema.DataType.DataType_String)
                        def.Length = col.MaxLength;

                    classDef.Properties.Add(def);
                    if (Array.IndexOf<DataColumn>(this.PrimaryKey, col) >= 0)
                        classDef.IdentityProperties.Add(def);
                }
            }
            return classDef;
        }

        protected abstract ClassDefinition CreateClassDefinition();
    }
}
