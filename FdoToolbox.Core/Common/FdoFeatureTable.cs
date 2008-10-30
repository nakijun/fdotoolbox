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
using System.Data;
using FdoToolbox.Core.ETL;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// The DataTable equivalent of an FDO feature class.
    /// </summary>
    public class FdoFeatureTable : FdoDataTable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public FdoFeatureTable(string name, string description) 
            : base(name, description) 
        { 
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fc"></param>
        public FdoFeatureTable(FeatureClass fc) : base() 
        {
            InitFromClass(fc);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"></param>
        public FdoFeatureTable(DataTable table)
            : base()
        {
            InitFromTable(table);
            if (FdoMetaData.HasMetaData(table, FdoMetaDataNames.FDO_FEATURE_CLASS_GEOMETRY_PROPERTY))
            {
                string name = FdoMetaData.GetMetaData<string>(table, FdoMetaDataNames.FDO_FEATURE_CLASS_GEOMETRY_PROPERTY);
                DataColumn col = this.Columns[name];
                FdoGeometryColumn gc = col as FdoGeometryColumn;
                if (gc != null)
                {
                    this.GeometryColumn = gc;
                }
                else
                {
                    //Convert to FdoGeometryColumn and replace original reference
                    gc = new FdoGeometryColumn(col.ColumnName, col.Caption);
                    gc.HasElevation = FdoMetaData.GetMetaData<bool>(col, FdoMetaDataNames.FDO_GEOMETRY_HAS_ELEVATION);
                    gc.HasMeasure = FdoMetaData.GetMetaData<bool>(col, FdoMetaDataNames.FDO_GEOMETRY_HAS_MEASURE);
                    gc.ReadOnly = FdoMetaData.GetMetaData<bool>(col, FdoMetaDataNames.FDO_GEOMETRY_READONLY);
                    gc.SpatialContextAssociation = FdoMetaData.GetMetaData<string>(col, FdoMetaDataNames.FDO_GEOMETRY_SPATIAL_CONTEXT);
                    gc.GeometryTypes = FdoMetaData.GetMetaData<int>(col, FdoMetaDataNames.FDO_GEOMETRY_TYPE);
                    this.GeometryColumn = gc;
                    this.Columns.Remove(name);
                    this.Columns.Add(gc);
                }
            }
        }

        /// <summary>
        /// Initializes this table from the given class definition
        /// </summary>
        /// <param name="classDef"></param>
        public override void InitFromClass(ClassDefinition classDef)
        {
            if (classDef.ClassType != ClassType.ClassType_FeatureClass)
                throw new DataTableConversionException("The class is not a feature class");

            base.InitFromClass(classDef);
            FeatureClass fc = classDef as FeatureClass;
            if(fc.GeometryProperty != null)
                this.GeometryColumn = (FdoGeometryColumn)this.Columns[fc.GeometryProperty.Name];
        }

        private FdoGeometryColumn _GeomColumn;

        /// <summary>
        /// The geometry column
        /// </summary>
        public FdoGeometryColumn GeometryColumn
        {
            get { return _GeomColumn; }
            set { _GeomColumn = value; }
        }

        /// <summary>
        /// Gets the underlying class definition
        /// </summary>
        /// <returns></returns>
        public override ClassDefinition GetClassDefinition()
        {
            FeatureClass classDef = (FeatureClass)base.GetClassDefinition();
            int gidx = classDef.Properties.IndexOf(_GeomColumn.ColumnName);
            classDef.GeometryProperty = (GeometricPropertyDefinition)classDef.Properties[gidx];
            return classDef;
        }

        /// <summary>
        /// Creates the underlying class definition
        /// </summary>
        /// <returns></returns>
        protected override ClassDefinition CreateClassDefinition()
        {
            return new FeatureClass(this.TableName, this.Description);
        }

        /// <summary>
        /// Gets the underlying class type
        /// </summary>
        /// <returns></returns>
        protected override ClassType GetClassType()
        {
            return ClassType.ClassType_FeatureClass;
        }
    }
}
