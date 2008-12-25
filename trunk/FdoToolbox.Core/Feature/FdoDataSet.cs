using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using System.Globalization;
using System.Xml;
using System.ComponentModel;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Represents an in-memory cache of spatial data
    /// </summary>
    public class FdoDataSet : DataSet
    {
        /// <summary>
		/// Initializes a new instance of the FeatureDataSet class.
		/// </summary>
        public FdoDataSet()
		{
			this.InitClass();
			System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
			//this.Tables.CollectionChanged += schemaChangedHandler;
			this.Relations.CollectionChanged += schemaChangedHandler;
			this.InitClass();
		}

        private FdoFeatureTableCollection _FeatureTables;

        /// <summary>
        /// Gets the collection of tables contained within this data set
        /// </summary>
        public new FdoFeatureTableCollection Tables
        {
            get { return _FeatureTables; }
        }

        /// <summary>
        /// Clones this data set
        /// </summary>
        /// <returns></returns>
        public new FdoDataSet Clone()
        {
            FdoDataSet ds = ((FdoDataSet)base.Clone());
            return ds;
        }

        private void InitClass()
        {
            _FeatureTables = new FdoFeatureTableCollection();
            //this.DataSetName = "FeatureDataSet";
            this.Prefix = "";
            this.Namespace = "http://tempuri.org/FeatureDataSet.xsd";
            this.Locale = CultureInfo.CurrentCulture;
            this.CaseSensitive = false;
            this.EnforceConstraints = true;
        }

        private void SchemaChanged(object sender, CollectionChangeEventArgs e)
        {
        }
    }

    /// <summary>
    /// A <see cref="FdoFeatureTable"/> collection
    /// </summary>
    public class FdoFeatureTableCollection : List<FdoFeatureTable> { }
}
