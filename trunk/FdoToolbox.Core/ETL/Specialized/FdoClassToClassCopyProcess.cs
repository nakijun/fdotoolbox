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
using FdoToolbox.Core.ETL.Operations;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// A specialized ETL process that copies data from one features class to another
    /// </summary>
    public class FdoClassToClassCopyProcess : FdoSpecializedEtlProcess
    {
        private FdoClassCopyOptions _options;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public FdoClassCopyOptions Options
        {
            get { return _options; }
            private set { _options = value; }
        }

        private int _ReportFrequency;

        /// <summary>
        /// Gets or sets the report frequency.
        /// </summary>
        /// <value>The report frequency.</value>
        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }
	

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoClassToClassCopyProcess"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public FdoClassToClassCopyProcess(FdoClassCopyOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return string.IsNullOrEmpty(this.Options.Name) ?
                    string.Format("Copy features from {0} to {1}", this.Options.SourceClassName, this.Options.TargetClassName) :
                    this.Options.Name;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            FdoConnection srcConn = Options.Parent.GetConnection(Options.SourceConnectionName);
            FdoConnection dstConn = Options.Parent.GetConnection(Options.TargetConnectionName);

            //Register delete operation first if delete target specified
            if (Options.DeleteTarget)
            {
                FdoDeleteOperation op = new FdoDeleteOperation(dstConn, Options.TargetClassName);
                Register(op);
            }

            IFdoOperation input = new FdoInputOperation(srcConn, CreateSourceQuery());
            IFdoOperation output = null;
            IFdoOperation convert = null;

            NameValueCollection propertyMappings = new NameValueCollection();
            string[] srcProps = this.Options.SourcePropertyNames;
            string[] srcAliases = this.Options.SourceAliases;
            if (srcProps.Length > 0)
            {
                foreach (string srcProp in srcProps)
                {
                    propertyMappings.Add(srcProp, Options.GetTargetProperty(srcProp));
                }
            }
            if (srcAliases.Length > 0)
            {
                foreach (string srcAlias in srcAliases)
                {
                    propertyMappings.Add(srcAlias, Options.GetTargetPropertyForAlias(srcAlias));
                }
            }
            if (propertyMappings.Count > 0)
            {
                if (Options.BatchSize > 0)
                {
                    FdoBatchedOutputOperation bat = new FdoBatchedOutputOperation(dstConn, Options.TargetClassName, propertyMappings, Options.BatchSize);
                    bat.BatchInserted += delegate(object sender, BatchInsertEventArgs e)
                    {
                        SendMessageFormatted("[Bulk Copy => {0}] {1} feature batch written", Options.TargetClassName, e.BatchSize);
                    };
                    output = bat;
                }
                else
                {
                    output = new FdoOutputOperation(dstConn, Options.TargetClassName, propertyMappings);
                }
            }
            else
            {
                if (Options.BatchSize > 0)
                {
                    FdoBatchedOutputOperation bat = new FdoBatchedOutputOperation(dstConn, Options.TargetClassName, Options.BatchSize);
                    bat.BatchInserted += delegate(object sender, BatchInsertEventArgs e)
                    {
                        SendMessageFormatted("[Bulk Copy => {0}] {1} feature batch written", Options.TargetClassName, e.BatchSize);
                    };
                    output = bat;
                }
                else
                {
                    output = new FdoOutputOperation(dstConn, Options.TargetClassName);
                }
            }

            if (Options.ConversionRules.Count > 0)
            {
                FdoDataValueConversionOperation op = new FdoDataValueConversionOperation(Options.ConversionRules);
                convert = op;
            }

            Register(input);
            if(convert != null)
                Register(convert);
            if (Options.FlattenGeometries)
                Register(new FdoFlattenGeometryOperation());
            Register(output);
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % this.ReportFrequency == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[{0}]: {1} features processed", this.Name, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoBatchedOutputOperation)
            {
                FdoBatchedOutputOperation bop = op as FdoBatchedOutputOperation;
                string className = bop.ClassName;
                SendMessageFormatted("[{0}]: {1}", this.Name, op.Statistics.ToString());
            }
            else if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[{0}]: {1}", this.Name, op.Statistics.ToString());
            }
        }

        private FeatureQueryOptions CreateSourceQuery()
        {
            FeatureQueryOptions query = new FeatureQueryOptions(Options.SourceClassName);
            query.AddFeatureProperty(Options.SourcePropertyNames);


            foreach (string alias in Options.SourceAliases)
            {
                query.AddComputedProperty(alias, Options.GetExpression(alias));
            }

            if (!string.IsNullOrEmpty(Options.SourceFilter))
                query.Filter = Options.SourceFilter;

            return query;
        }

    }
}
