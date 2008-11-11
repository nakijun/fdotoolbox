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
using FdoToolbox.Core.ETL;
using OSGeo.FDO.Common;
using OSGeo.FDO.Schema;
using System.Data;

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// Extension of DataColumn with FDO-friendly attributes
    /// </summary>
    public abstract class FdoColumn : System.Data.DataColumn
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected FdoColumn() { }

        /// <summary>
        /// Initialize this column with the underlying property definition
        /// </summary>
        /// <param name="def"></param>
        protected void LoadBaseAttributes(PropertyDefinition def)
        {
            this.ColumnName = def.Name;
            this.Caption = def.Description;
            this.ColumnType = def.PropertyType;
            this.IsSystem = def.IsSystem;
        }

        /// <summary>
        /// Sets the underlying property definition's base attributes from this column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="def"></param>
        protected void GetBaseAttributes<T>(ref T def) where T : PropertyDefinition
        {
            def.IsSystem = this.IsSystem;
        }

        /// <summary>
        /// The column type
        /// </summary>
        public PropertyType ColumnType
        {
            get { return (PropertyType)this.ExtendedProperties[FdoMetaDataNames.FDO_PROPERTY_TYPE]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_PROPERTY_TYPE] = value; } 
        }

        /// <summary>
        /// Returns true if this column is system generated
        /// </summary>
        public bool IsSystem
        {
            get { return (bool)this.ExtendedProperties[FdoMetaDataNames.FDO_SYSTEM_PROPERTY]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_SYSTEM_PROPERTY] = value; }
        }

        /// <summary>
        /// Sets the underlying property definition's attributes from this column
        /// </summary>
        /// <param name="def"></param>
        protected abstract void LoadAttributes(PropertyDefinition def);

        /// <summary>
        /// Gets the underlying property definition
        /// </summary>
        /// <returns></returns>
        public abstract PropertyDefinition GetPropertyDefinition();
    }

    /// <summary>
    /// FDO Geometry data column
    /// </summary>
    public class FdoGeometryColumn : FdoColumn
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FdoGeometryColumn() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public FdoGeometryColumn(string name, string description) 
        {
            GeometricPropertyDefinition def = new GeometricPropertyDefinition(name, description);
            this.LoadAttributes(def);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="def"></param>
        public FdoGeometryColumn(GeometricPropertyDefinition def) 
        {
            this.LoadAttributes(def);
        }

        /// <summary>
        /// Returns true if this geometry has elevation (Z-component)
        /// </summary>
        public bool HasElevation
        {
            get { return (bool)this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_ELEVATION]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_ELEVATION] = value; }
        }

        /// <summary>
        /// Returns true if this geometry has measure
        /// </summary>
        public bool HasMeasure
        {
            get { return (bool)this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_MEASURE]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_MEASURE] = value; }
        }

        /// <summary>
        /// Gets the geometry types of this geometry
        /// </summary>
        public int GeometryTypes
        {
            get { return (int)this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_TYPE]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_TYPE] = value; }
        }

        /// <summary>
        /// Gets the spatial context association
        /// </summary>
        public string SpatialContextAssociation
        {
            get { return this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_SPATIAL_CONTEXT].ToString(); }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_SPATIAL_CONTEXT] = value; }
        }

        /// <summary>
        /// Sets the underlying property definition's attributes from this column
        /// </summary>
        /// <param name="def"></param>
        protected override void LoadAttributes(PropertyDefinition pd)
        {
            this.LoadBaseAttributes(pd);
            GeometricPropertyDefinition def = pd as GeometricPropertyDefinition;

            this.ColumnType = PropertyType.PropertyType_GeometricProperty;
            this.HasElevation = def.HasElevation;
            this.HasMeasure = def.HasMeasure;
            this.ReadOnly = def.ReadOnly;
            this.GeometryTypes = def.GeometryTypes;
            this.SpatialContextAssociation = def.SpatialContextAssociation;
        }

        /// <summary>
        /// Gets the underlying property definition
        /// </summary>
        /// <returns></returns>
        public override PropertyDefinition GetPropertyDefinition()
        {
            GeometricPropertyDefinition def = new GeometricPropertyDefinition(this.ColumnName, this.Caption);
            this.GetBaseAttributes(ref def);
            def.GeometryTypes = this.GeometryTypes;
            def.HasElevation = this.HasElevation;
            def.HasMeasure = this.HasMeasure;
            def.ReadOnly = this.ReadOnly;
            def.SpatialContextAssociation = this.SpatialContextAssociation;
            return def;
        }
    }

    /// <summary>
    /// FDO data column
    /// </summary>
    public class FdoDataColumn : FdoColumn
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FdoDataColumn() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public FdoDataColumn(string name, string description) 
        {
            DataPropertyDefinition def = new DataPropertyDefinition(name, description);
            this.LoadAttributes(def);
            this.DefaultValue = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="def"></param>
        public FdoDataColumn(DataPropertyDefinition def)
        {
            this.LoadAttributes(def);
        }

        internal DataType GetDataType()
        {
            return GetDataTypeFromType(this.DataType);
        }

        /// <summary>
        /// Gets the FDO data type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DataType GetDataTypeFromType(Type t)
        {
            //No CLOB
            if (t == typeof(byte[]))
                return OSGeo.FDO.Schema.DataType.DataType_BLOB;
            else if (t == typeof(bool))
                return OSGeo.FDO.Schema.DataType.DataType_Boolean;
            else if (t == typeof(byte))
                return OSGeo.FDO.Schema.DataType.DataType_Byte;
            else if (t == typeof(DateTime))
                return OSGeo.FDO.Schema.DataType.DataType_DateTime;
            else if (t == typeof(decimal))
                return OSGeo.FDO.Schema.DataType.DataType_Decimal;
            else if (t == typeof(double))
                return OSGeo.FDO.Schema.DataType.DataType_Double;
            else if (t == typeof(short))
                return OSGeo.FDO.Schema.DataType.DataType_Int16;
            else if (t == typeof(int))
                return OSGeo.FDO.Schema.DataType.DataType_Int32;
            else if (t == typeof(long))
                return OSGeo.FDO.Schema.DataType.DataType_Int64;
            else if (t == typeof(float))
                return OSGeo.FDO.Schema.DataType.DataType_Single;
            else if (t == typeof(string))
                return OSGeo.FDO.Schema.DataType.DataType_String;
            else
                throw new ArgumentException("Could not find corresponding DataType for Type: " + t);
        }

        /// <summary>
        /// Gets the CLR type from a FDO data type
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Type GetTypeFromDataType(DataType dt)
        {
            Type t = null;
            switch (dt)
            {
                case OSGeo.FDO.Schema.DataType.DataType_BLOB:
                    t = typeof(byte[]);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Boolean:
                    t = typeof(bool);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Byte:
                    t = typeof(byte);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_CLOB:
                    t = typeof(byte[]);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_DateTime:
                    t = typeof(DateTime);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Decimal:
                    t = typeof(decimal);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Double:
                    t = typeof(double);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Int16:
                    t = typeof(short);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Int32:
                    t = typeof(int);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Int64:
                    t = typeof(long);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_Single:
                    t = typeof(float);
                    break;
                case OSGeo.FDO.Schema.DataType.DataType_String:
                    t = typeof(string);
                    break;
            }
            return t;
        }

        /// <summary>
        /// Gets the scale of this data column
        /// </summary>
        public int Scale
        {
            get { return (int)this.ExtendedProperties[FdoMetaDataNames.FDO_DATA_SCALE]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_DATA_SCALE] = value; }
        }

        /// <summary>
        /// Gets the precision of this data column
        /// </summary>
        public int Precision
        {
            get { return (int)this.ExtendedProperties[FdoMetaDataNames.FDO_DATA_PRECISION]; }
            set { this.ExtendedProperties[FdoMetaDataNames.FDO_DATA_PRECISION] = value; }
        }

        /// <summary>
        /// Sets the underlying property definition's attributes from this column
        /// </summary>
        /// <param name="def"></param>
        protected override void LoadAttributes(PropertyDefinition pd)
        {
            this.LoadBaseAttributes(pd);

            DataPropertyDefinition def = pd as DataPropertyDefinition;
            this.DataType = GetTypeFromDataType(def.DataType);
            this.AllowDBNull = def.Nullable;
            this.AutoIncrement = def.IsAutoGenerated;
            this.ReadOnly = def.ReadOnly;
            this.Caption = def.Description;

            //For strings, BLOBS and CLOBS only set length if it is > 0
            if ((def.DataType == OSGeo.FDO.Schema.DataType.DataType_String ||
                def.DataType == OSGeo.FDO.Schema.DataType.DataType_BLOB ||
                def.DataType == OSGeo.FDO.Schema.DataType.DataType_CLOB) &&
                def.Length > 0)
                this.MaxLength = def.Length;

            if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_Decimal)
            {
                this.Scale = def.Scale;
                this.Precision = def.Precision;
            }

            if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_String)
                this.DefaultValue = def.DefaultValue;
        }

        /// <summary>
        /// Gets the underlying property definition
        /// </summary>
        /// <returns></returns>
        public override PropertyDefinition GetPropertyDefinition()
        {
            DataPropertyDefinition def = new DataPropertyDefinition(this.ColumnName, this.Caption);
            this.GetBaseAttributes(ref def);
            def.DataType = GetDataTypeFromType(this.DataType);
            def.IsAutoGenerated = this.AutoIncrement;
            def.Nullable = this.AllowDBNull;
            def.ReadOnly = def.IsAutoGenerated ? true : this.ReadOnly;
            
            if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_String &&
                this.DefaultValue != null)
                def.DefaultValue = this.DefaultValue.ToString();
            
            if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_BLOB ||
                def.DataType == OSGeo.FDO.Schema.DataType.DataType_CLOB ||
                def.DataType == OSGeo.FDO.Schema.DataType.DataType_String)
                def.Length = this.MaxLength;

            if (def.DataType == OSGeo.FDO.Schema.DataType.DataType_Decimal)
            {
                def.Precision = this.Precision;
                def.Scale = this.Scale;
            }

            return def;
        }
    }
}
