#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// A FDO-friendly DataTable
    /// </summary>
    public class FdoFeatureTable : DataTable, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTable"/> class.
        /// </summary>
        public FdoFeatureTable()
            : base()
        {
            this.InitClass();
        }

        /// <summary>
        /// Raised when the table requests more information about a spatial context association
        /// </summary>
        public event FdoSpatialContextRequestEventHandler RequestSpatialContext = delegate { };

        private List<SpatialContextInfo> _spatialContexts = new List<SpatialContextInfo>();

        /// <summary>
        /// Adds a spatial context.
        /// </summary>
        /// <param name="ctx">The context.</param>
        public void AddSpatialContext(SpatialContextInfo ctx)
        {
            _spatialContexts.Add(ctx);
        }

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public SpatialContextInfo GetSpatialContext(string name)
        {
            return _spatialContexts.Find(delegate(SpatialContextInfo s) { return s.Name == name; });
        }

        /// <summary>
        /// Gets the spatial contexts attached to this table
        /// </summary>
        /// <value>The spatial contexts.</value>
        public ICollection<SpatialContextInfo> SpatialContexts
        {
            get { return _spatialContexts; }
        }

        /// <summary>
        /// Gets the active spatial context. If no active spatial contexts are found, the first one is returned.
        /// If no spatial contexts are found, null is returned.
        /// </summary>
        /// <value>The active spatial context.</value>
        public SpatialContextInfo ActiveSpatialContext
        {
            get
            {
                SpatialContextInfo c = _spatialContexts.Find(delegate(SpatialContextInfo s) { return s.IsActive; });
                if (c != null)
                    return c;
                else if (_spatialContexts.Count > 0)
                    return _spatialContexts[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public FdoFeatureTable(DataTable table)
            : base(table.TableName)
        {
            if (table.DataSet != null)
            {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive))
                    this.CaseSensitive = table.CaseSensitive;
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString()))
                    this.Locale = table.Locale;
                if ((table.Namespace != table.DataSet.Namespace))
                    this.Namespace = table.Namespace;

                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
        }

        /// <summary>
        /// Gets the number of rows (features) in this table
        /// </summary>
        [Browsable(true)]
        public int Count
        {
            get { return base.Rows.Count; }
        }

        /// <summary>
        /// Gets the row (feature) at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FdoFeature this[int index]
        {
            get { return (FdoFeature)base.Rows[index]; }
        }

        /// <summary>
        /// Adds a row (feature) to this table
        /// </summary>
        /// <param name="feature"></param>
        public void AddRow(FdoFeature feature)
        {
            base.Rows.Add(feature);
        }

        /// <summary>
        /// Returns an enumerator for enumerating that rows of this table
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return base.Rows.GetEnumerator();
        }

        /// <summary>
        /// Clones this table
        /// </summary>
        /// <returns></returns>
        public new FdoFeatureTable Clone()
        {
            FdoFeatureTable tbl = ((FdoFeatureTable)(base.Clone()));
            tbl.InitVars();
            return tbl;
        }

        /// <summary>
        /// Occurs after a FdoFeature has been change successfully
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureChanged = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature is changing
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureChanging = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature has been deleted
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureDeleting = delegate { };

        /// <summary>
        /// Occurs when a FdoFeature is about to be deleted
        /// </summary>
        public event FdoFeatureChangeEventHandler FeatureDeleted = delegate { };

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance()
        {
            return new FdoFeatureTable();
        }

        internal void InitVars()
        {

        }

        private void InitClass()
        {

        }

        /// <summary>
        /// Creates a new feature with the same schema as the table
        /// </summary>
        /// <returns></returns>
        public new FdoFeature NewRow()
        {
            return (FdoFeature)base.NewRow();
        }

        /// <summary>
        /// Creates a new Feature with the same schema as the table based on a DataRow builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FdoFeature(builder);
        }
        
        /// <summary>
        /// Gets the row type
        /// </summary>
        /// <returns></returns>
        protected override Type GetRowType()
        {
            return typeof(FdoFeature);
        }

        /// <summary>
        /// Removes the row (feature) from the table
        /// </summary>
        /// <param name="row"></param>
        public void RemoveRow(FdoFeature row)
        {
            base.Rows.Remove(row);
        }

        /// <summary>
        /// Raises feature changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            base.OnRowChanged(e);
            this.FeatureChanged(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature changing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);
            this.FeatureChanging(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature deleted
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);
            this.FeatureDeleted(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        /// <summary>
        /// Raises feature deleting
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            base.OnRowDeleting(e);
            this.FeatureDeleting(this, new FdoFeatureChangeEventArgs(e.Row as FdoFeature, e.Action));
        }

        private string _geometryColumn;

        /// <summary>
        /// The default geometry column for this table. If null or empty then this table has no geometries
        /// </summary>
        public string GeometryColumn
        {
            get { return _geometryColumn; }
        }

        /// <summary>
        /// Initializes the table from a reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void InitTable(IFdoReader reader)
        {
            this.Columns.Clear();
            string[] geometries = reader.GeometryProperties;
            _geometryColumn = reader.DefaultGeometryProperty;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                FdoPropertyType ptype = reader.GetFdoPropertyType(name);
                if (ptype == FdoPropertyType.Raster)
                {
                    //Raster's can't conceivably be previewed in tabular form, so omit it
                }
                else if (ptype == FdoPropertyType.Object)
                {
                }
                else if (ptype == FdoPropertyType.Association)
                {
                }
                else
                {
                    Type type = reader.GetFieldType(i);
                    if (Array.IndexOf<string>(geometries, name) >= 0)
                    {
                        type = typeof(FdoGeometry);
                        string assoc = reader.GetSpatialContextAssociation(name);
                        if (!string.IsNullOrEmpty(assoc))
                            this.RequestSpatialContext(this, assoc);
                    }
                    this.Columns.Add(name, type);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="ClassDefinition"/> from this instance
        /// </summary>
        /// <param name="createAutoGeneratedId">If true, will add an auto-generated id property to this class definition</param>
        /// <returns>The class definition</returns>
        public ClassDefinition CreateClassDefinition(bool createAutoGeneratedId)
        {
            ClassDefinition cd = null;
            if (!string.IsNullOrEmpty(this.GeometryColumn))
            {
                FeatureClass fc = new FeatureClass(this.TableName, string.Empty);
                GeometricPropertyDefinition gp = new GeometricPropertyDefinition(this.GeometryColumn, string.Empty);
                fc.Properties.Add(gp);
                fc.GeometryProperty = gp;
                cd = fc;
            }
            else
            {
                cd = new Class(this.TableName, string.Empty);
            }

            if (createAutoGeneratedId)
            {
                int num = 1;
                string name = "AutoID";
                string genName = name + num;
                string theName = string.Empty;
                if (this.Columns[name] != null)
                {
                    while (this.Columns[genName] != null)
                    {
                        genName = name + (num++);
                    }
                    theName = genName;
                }
                else
                {
                    theName = name;
                }

                DataPropertyDefinition id = new DataPropertyDefinition(theName, string.Empty);
                id.IsAutoGenerated = true;
                id.DataType = DataType.DataType_Int32;
                cd.Properties.Add(id);
                cd.IdentityProperties.Add(id);
            }

            //Now process columns
            foreach (DataColumn dc in this.Columns)
            {
                if (dc.ColumnName != this.GeometryColumn)
                {
                    DataPropertyDefinition dp = ExpressUtility.GetDataPropertyForColumn(dc);
                    cd.Properties.Add(dp);
                }
            }
            return cd;
        }
    }
}
