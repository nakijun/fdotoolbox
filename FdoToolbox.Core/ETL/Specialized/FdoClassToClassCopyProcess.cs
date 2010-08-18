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
using OSGeo.FDO.Schema;

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

        class PreClassCopyModifyOperation : FdoOperationBase
        {
            private FdoConnection _source;
            private FdoConnection _target;
            private FdoClassCopyOptions _opts;

            public PreClassCopyModifyOperation(FdoClassCopyOptions opts, FdoConnection source, FdoConnection target)
            {
                if (opts.PreCopyTargetModifier == null)
                    throw new ArgumentException("No pre-copy modifier specified");

                _source = source;
                _target = target;
                _opts = opts;
            }

            private int counter = 0;

            public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
            {
                if (counter < 1) //Shouldn't be reentrant, but just play it safe.
                {
                    if (typeof(CreateTargetClassFromSource).IsAssignableFrom(_opts.PreCopyTargetModifier.GetType()))
                    {
                        using (var ssvc = _source.CreateFeatureService())
                        {
                            var ct = (CreateTargetClassFromSource)_opts.PreCopyTargetModifier;
                            var cls = ssvc.GetClassByName(ct.Schema, ct.Name);
                            Info("Creating a cloned copy of source class " + ct.Schema + ":" + ct.Name);

                            var cloned = FdoFeatureService.CloneClass(cls);
                            foreach (var prop in ct.PropertiesToCreate)
                            {
                                Info("Adding property to cloned class: " + prop.Name);
                                cloned.Properties.Add(FdoFeatureService.CloneProperty(prop));
                            }

                            //Add an auto-generated identity property if none exist
                            if (cloned.IdentityProperties.Count == 0)
                            {
                                var id = new DataPropertyDefinition("FID", "Auto-Generated Feature Id");
                                id.IsAutoGenerated = true;
                                id.Nullable = false;
                                id.DataType = DataType.DataType_Int32;

                                cloned.Properties.Add(id);
                                cloned.IdentityProperties.Add(id);

                                Info("Adding an auto-generated id to this cloned class");
                            }

                            using (var tsvc = _target.CreateFeatureService())
                            {
                                Info("Getting current schema");
                                var schema = tsvc.GetSchemaByName(_opts.TargetSchema);
                                Info("Adding cloned class to this schema");
                                schema.Classes.Add(cloned);
                                Info("Applying schema back to target connection");
                                tsvc.ApplySchema(schema);
                                Info("Updated schema applied");
                            }
                        }
                    }
                    else if (typeof(UpdateTargetClass).IsAssignableFrom(_target.GetType()))
                    {
                        var ut = (UpdateTargetClass)_opts.PreCopyTargetModifier;
                        using (var tsvc = _target.CreateFeatureService())
                        {
                            var schema = tsvc.GetSchemaByName(_opts.TargetSchema);
                            var cidx = schema.Classes.IndexOf(ut.Name);
                            if (cidx < 0)
                            {
                                throw new InvalidOperationException("Target class to be updated " + _opts.TargetSchema + ":" + ut.Name + " not found");
                            }
                            else
                            {
                                var cls = schema.Classes[cidx];
                                foreach (var prop in ut.PropertiesToCreate)
                                {
                                    Info("Adding property to class: " + prop.Name);
                                    cls.Properties.Add(FdoFeatureService.CloneProperty(prop));
                                }
                                Info("Applying modified schema");
                                tsvc.ApplySchema(schema);
                                Info("Modified schema applied");
                            }
                        }
                    }

                    counter++;
                }
                return rows;
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

            if (Options.PreCopyTargetModifier != null)
            {
                Register(new PreClassCopyModifyOperation(Options, srcConn, dstConn));
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
            if (Options.ForceWkb)
                Register(new FdoForceWkbOperation());
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
