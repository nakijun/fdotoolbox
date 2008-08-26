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

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// Extension of DataTable to allow more FDO specific functionality. FDO specific information
    /// is stored as table/column metadata.
    /// </summary>
    public abstract class FdoDataTable<T> : System.Data.DataTable where T : ClassDefinition
    {
        internal FdoDataTable() : base() { }

        public FdoDataTable(string name, string description) : base() 
        {
            this.TableName = name;
            this.Description = description;
        }

        public string Description
        {
            get { return this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_DESCRIPTION].ToString(); }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_CLASS_DESCRIPTION] = value; }
        }

        public virtual void InitFromClass(T classDef)
        {
            this.TableName = classDef.Name;
            this.Description = classDef.Description;
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
                        throw new NotSupportedException("Property type not supported: " + pd.PropertyType);
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
            InitFromClass((T)classDef);

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
                                row[dc] = reader.GetGeometry(name);
                            }
                            break;
                    }
                }
            }
            this.Rows.Add(row);
        }

        public virtual T GetClassDefinition()
        {
            T classDef = CreateClassDefinition();
            classDef.Name = this.TableName;
            classDef.Description = this.Description;
            foreach (DataColumn col in this.Columns)
            {
                PropertyType pt = GetColumnType(col);
                switch(pt)
                {
                    case PropertyType.PropertyType_DataProperty:
                        classDef.Properties.Add((col as FdoDataColumn).GetPropertyDefinition());
                        if (Array.IndexOf<DataColumn>(this.PrimaryKey, col) >= 0)
                            classDef.IdentityProperties.Add((col as FdoDataColumn).GetPropertyDefinition()); 
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        classDef.Properties.Add((col as FdoGeometryColumn).GetPropertyDefinition());
                        break;
                }
            }
            return classDef;
        }

        protected abstract T CreateClassDefinition();
    }
}
