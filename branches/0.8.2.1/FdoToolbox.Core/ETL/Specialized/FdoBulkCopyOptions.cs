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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL.Specialized
{
    using Feature;
    using OSGeo.FDO.Schema;

    /// <summary>
    /// Options for <see cref="FdoBulkCopy"/>
    /// </summary>
    public class FdoBulkCopyOptions : IDisposable
    {
        private FdoConnection _sourceConn;

        /// <summary>
        /// Gets the source connection to read features from
        /// </summary>
        public FdoConnection SourceConnection
        {
            get { return _sourceConn; }
        }

        private FdoConnection _targetConn;

        /// <summary>
        /// Gets the target connection to write features to
        /// </summary>
        public FdoConnection TargetConnection
        {
            get { return _targetConn; }
        }

        private List<FdoClassCopyOptions> _classOptions;

        /// <summary>
        /// Gets the collection of class copy options
        /// </summary>
        public ReadOnlyCollection<FdoClassCopyOptions> ClassCopyOptions
        {
            get { return _classOptions.AsReadOnly(); }
        }

        private List<SpatialContextInfo> _spatialContextList;

        /// <summary>
        /// Gets the source spatial contexts.
        /// </summary>
        /// <value>The source spatial contexts.</value>
        public ReadOnlyCollection<SpatialContextInfo> SourceSpatialContexts
        {
            get { return _spatialContextList.AsReadOnly(); }
        }

        private int _BatchSize;

        /// <summary>
        /// Gets or sets the batch size. If greater than zero, a batched
        /// insert operation will be used in place of a regular insert operation
        /// (if supported by the target connection)
        /// </summary>
        public int BatchSize
        {
            get { return _BatchSize; }
            set 
            { 
                _BatchSize = value;
                _classOptions.ForEach(delegate(FdoClassCopyOptions copt)
                {
                    copt.BatchSize = value;
                });
            }
        }

        private string _SourceSchema;

        /// <summary>
        /// Gets or sets the source schema.
        /// </summary>
        /// <value>The source schema.</value>
        public string SourceSchema
        {
            get { return _SourceSchema; }
            set { _SourceSchema = value; }
        }

        private string _TargetSchema;

        /// <summary>
        /// Gets or sets the target schema.
        /// </summary>
        /// <value>The target schema.</value>
        public string TargetSchema
        {
            get { return _TargetSchema; }
            set { _TargetSchema = value; }
        }

        private bool _AlterSchema;

        /// <summary>
        /// If true, will alter the source schema before applying to the target connection
        /// if the source schema is not compatible. Otherwise an exception will be thrown
        /// if the source schema is not compatbile.
        /// </summary>
        public bool AlterSchema
        {
            get { return _AlterSchema; }
            set { _AlterSchema = value; }
        }

        /// <summary>
        /// If true, the mappings are ignored and the full source schema
        /// will be applied to the target. Is only true if the target
        /// schema is undefined or the target schema is empty
        /// </summary>
        public bool ApplySchemaToTarget
        {
            get { return string.IsNullOrEmpty(this.TargetSchema) || EmptyTargetSchema(this.TargetSchema); }
        }

        private bool EmptyTargetSchema(string schemaName)
        {
            bool empty = false;
            using (FdoFeatureService service = this.TargetConnection.CreateFeatureService())
            {
                FeatureSchema schema = service.GetSchemaByName(this.TargetSchema);
                if (schema != null)
                {
                    using (schema)
                    {
                        empty = schema.Classes.Count == 0;
                    }
                }
            }
            return empty;
        }

        /// <summary>
        /// Determines if this object owns the connections.
        /// </summary>
        private bool _owner;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        public FdoBulkCopyOptions(FdoConnection source, FdoConnection target)
        {
            _sourceConn = source;
            _targetConn = target;
            _classOptions = new List<FdoClassCopyOptions>();
            _spatialContextList = new List<SpatialContextInfo>();
            this.BatchSize = 0;
            _owner = false;
        }

        /// <summary>
        /// Internal constructor used by the ExpressUtility.
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="owner">If true, this object owns the connections and will dispose of them when it is disposed</param>
        internal FdoBulkCopyOptions(FdoConnection source, FdoConnection target, bool owner) : this(source, target)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds a class copy option
        /// </summary>
        /// <param name="option">The class copy option</param>
        public void AddClassCopyOption(FdoClassCopyOptions option)
        {
            _classOptions.Add(option);
            option.Parent = this;
        }

        /// <summary>
        /// Adds a source spatial contex to be copied
        /// </summary>
        /// <param name="ctx"></param>
        public void AddSourceSpatialContext(SpatialContextInfo ctx)
        {
            _spatialContextList.Add(ctx);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public void Reset(FdoConnection source, FdoConnection target)
        {
            _spatialContextList.Clear();
            _classOptions.Clear();
            _sourceConn = source;
            _targetConn = target;
            this.BatchSize = 0;
            _owner = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_owner)
            {
                _sourceConn.Dispose();
                _targetConn.Dispose();
            }
        }
    }
}
