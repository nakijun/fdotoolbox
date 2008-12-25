using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using OSGeo.FDO.Geometry;

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
        /// Creates a new Feature with teh same schema as the table based on a DataRow builder
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
                Type type = reader.GetFieldType(i);
                if (Array.IndexOf<string>(geometries, name) >= 0)
                    type = typeof(IGeometry);
                this.Columns.Add(name, type);
            }
        }
    }
}
