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

namespace FdoToolbox.Core.ETL.Specialized
{
    using Operations;
    using FdoToolbox.Core.Feature;
    using OSGeo.FDO.Commands.Feature;
    using FdoToolbox.Core.ETL.Pipelines;
    using FdoToolbox.Core.Configuration;
    using System.Xml.Serialization;
    using System.IO;
    using System.Collections.Specialized;
    using FdoToolbox.Core.ETL.Overrides;
    using OSGeo.FDO.Schema;

    /*
        FdoBulkCopy is a special class. 

        It was originally designed as a specialized EtlProcess that chains a series of FdoInputOperation 
        and FdoOutputOperation objects together in the same pipeline. It turns out that this doesn't quite
        work because FdoOutputOperation is designed to be used as the LAST operation in a pipeline, and throwing
        down variable-sized enumerables can cause premature termination. Chaining operations after the FdoOutputOperation 
        (such as new Fdo[Input/Output]Operation pairs, as the old implenentation does) does not quite work.

        As a result, multi-class bulk copies do not work. So significant behind-the-scenes work had to be done.

        Each Fdo[Input/Output]Operation pair is now encapsulated as a ClassToClassCopy sub-process, and
        the FdoBulkCopy is a collection of these sub-processes. However, in order to maintain an identical interface,
        most of the inherited methods had to be re-implemented. 
     */

    /// <summary>
    /// A specialized form of <see cref="EtlProcess"/> that copies
    /// a series of feature classes from one source to another
    /// </summary>
    public class FdoBulkCopy : FdoSpecializedEtlProcess
    {
        private int _ReportFrequency = 50;

        public event System.ComponentModel.CancelEventHandler BeforeExecute = delegate { };

        /// <summary>
        /// Gets or sets the frequency at which progress feedback is made
        /// </summary>
        /// <value>The report frequency.</value>
        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }

        private FdoBulkCopyOptions _options;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public FdoBulkCopyOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopy"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public FdoBulkCopy(FdoBulkCopyOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoBulkCopy"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="reportFrequency">The report frequency.</param>
        public FdoBulkCopy(FdoBulkCopyOptions options, int reportFrequency)
            : this(options)
        {
            _ReportFrequency = reportFrequency;
        }

        /// <summary>
        /// Registers the specified operation.
        /// </summary>
        /// <param name="op">The operation.</param>
        public new void Register(IFdoOperation op)
        {
            throw new NotSupportedException("Bulk Copy does not support direct registration of operations");
        }

        private List<EtlProcess> subProcesses = new List<EtlProcess>();
        private Dictionary<string, List<Exception>> subProcessErrors = new Dictionary<string, List<Exception>>();

        /// <summary>
        /// Registers the class copy process
        /// </summary>
        /// <param name="copts">The class copy options.</param>
        protected void RegisterClassCopy(FdoClassCopyOptions copts)
        {
            ClassToClassCopy proc = new ClassToClassCopy(copts);
            proc.ReportFrequency = this.ReportFrequency;
            subProcesses.Add(proc);
        }

        private bool execute = true;

        /// <summary>
        /// Initializes the process
        /// </summary>
        protected override void Initialize()
        {
            System.ComponentModel.CancelEventArgs ce = new System.ComponentModel.CancelEventArgs(false);
            this.BeforeExecute(this, ce);
            if (ce.Cancel)
            {
                SendMessage("Bulk Copy Cancelled");
                execute = false;
                return;
            }

            //Copy Spatial Contexts
            IList<SpatialContextInfo> contexts = _options.SourceSpatialContexts;
            if (contexts.Count > 0)
            {   
                using (FdoFeatureService targetService = _options.TargetConnection.CreateFeatureService())
                {
                    if (_options.BatchSize > 0 && !targetService.SupportsBatchInsertion())
                    {
                        SendMessage("Batch insert not supported. Using regular inserts");
                        _options.BatchSize = 0;
                    }

                    if (contexts.Count > 0)
                    {
                        SendMessage("Copying Spatial Contexts");
                        ICopySpatialContext copy = CopySpatialContextOverrideFactory.GetCopySpatialContextOverride(_options.TargetConnection);
                        copy.Execute(contexts, _options.TargetConnection, true);
                    }
                }
            }

            if (_options.ApplySchemaToTarget)
            {
                SendMessage("Applying source schema to target (this may take a while)");
                FeatureSchema targetSchema = CreateTargetSchema(_options.SourceSchema);
                using (FdoFeatureService destService = _options.TargetConnection.CreateFeatureService())
                {
                    if (_options.AlterSchema)
                    {
                        SendMessage("Altering schema to be compatible with target connection");

                        IncompatibleSchema incSchema = null;
                        if (!destService.CanApplySchema(targetSchema, out incSchema))
                        {
                            targetSchema = destService.AlterSchema(targetSchema, incSchema);
                            SendMessage("Schema altered");
                        }
                        else
                        {
                            SendMessage("No alterations required");
                        }
                    }

                    //Check that each feature class in the source schema has an identity property
                    //If none exist, create one
                    foreach (ClassDefinition classDef in targetSchema.Classes)
                    {
                        if (classDef.IdentityProperties.Count == 0)
                        {
                            if (_options.TargetConnection.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes).Length == 0)
                                throw new FdoETLException("Class " + classDef.Name + " has no identity properties. An auto-generated ID could not be created as it was not supported");

                            SendMessage("Class " + classDef.Name + " has no identity properties. Making one for it");
                            DataPropertyDefinition dataDef = new DataPropertyDefinition("AutoID", "Auto-generated ID");
                            dataDef.IsAutoGenerated = true;
                            //Get the highest available auto-generated type. 
                            DataType[] dtypes = (DataType[])_options.TargetConnection.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes);
                            DataType[] autoTypes = new DataType[dtypes.Length];
                            Array.Copy(dtypes, autoTypes, dtypes.Length);
                            Array.Sort<DataType>(autoTypes, new DataTypeComparer());
                            dataDef.DataType = autoTypes[autoTypes.Length - 1];

                            classDef.IdentityProperties.Add(dataDef);
                        }
                        SendMessage("Creating class copy option for " + classDef.Name);
                        FdoClassCopyOptions copt = new FdoClassCopyOptions(classDef.Name, classDef.Name);
                        foreach (PropertyDefinition pd in classDef.Properties)
                        {
                            switch (pd.PropertyType)
                            {
                                case PropertyType.PropertyType_DataProperty:
                                    if (!(pd as DataPropertyDefinition).ReadOnly)
                                    {
                                        copt.AddPropertyMapping(pd.Name, pd.Name);
                                    }
                                    break;
                                case PropertyType.PropertyType_GeometricProperty:
                                    copt.AddPropertyMapping(pd.Name, pd.Name);
                                    break;
                            }
                        }
                        _options.AddClassCopyOption(copt);
                    }
                    destService.ApplySchema(targetSchema);
                    _options.TargetSchema = targetSchema.Name;
                    SendMessage("Target schema applied");
                }
            }
            //Set class copy tasks
            foreach (FdoClassCopyOptions copt in _options.ClassCopyOptions)
            {
                RegisterClassCopy(copt);
            }
        }

        private FeatureSchema CreateTargetSchema(string sourceSchemaName)
        {
            SendMessage("Cloning source schema for target");
            using (FdoFeatureService srcService = _options.SourceConnection.CreateFeatureService())
            {
                FeatureSchema fs = srcService.GetSchemaByName(sourceSchemaName);
                if (fs != null)
                {
                    return FdoFeatureService.CloneSchema(fs);
                }
            }
            throw new FdoETLException("Could not find source schema to clone: " + sourceSchemaName);
        }

        /// <summary>
        /// Executes this process
        /// </summary>
        public override void Execute()
        {
            Initialize();
            if (execute)
            {
                foreach (EtlProcess proc in subProcesses)
                {
                    SendMessage("[Bulk Copy] Running sub-process [" + proc.Name + "]:");
                    proc.ProcessMessage += new MessageEventHandler(OnSubProcessMessage);
                    proc.Execute();
                    List<Exception> errors = new List<Exception>(proc.GetAllErrors());
                    SendMessageFormatted("[Bulk Copy] sub-process completed with {0} errors", errors.Count);
                    LogSubProcessErrors(proc.Name, errors);
                }
            }
        }

        /// <summary>
        /// Logs all the sub-process errors.
        /// </summary>
        /// <param name="procName">Name of the process.</param>
        /// <param name="errors">The errors.</param>
        private void LogSubProcessErrors(string procName, IEnumerable<Exception> errors)
        {
            if (!subProcessErrors.ContainsKey(procName))
                subProcessErrors[procName] = new List<Exception>();

            subProcessErrors[procName].AddRange(errors);
        }

        void OnSubProcessMessage(object sender, MessageEventArgs e)
        {
            SendMessage(e.Message);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _options.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Saves this process to a file
        /// </summary>
        /// <param name="file">The file to save this process to</param>
        /// <param name="name">The name of the process</param>
        public override void Save(string file, string name)
        {
            FdoBulkCopyTaskDefinition def = new FdoBulkCopyTaskDefinition();
            def.name = name;

            def.Source = new FdoCopySource();
            def.Target = new FdoCopyTarget();
            
            def.Source.ConnectionString = _options.SourceConnection.ConnectionString;
            def.Source.Provider = _options.SourceConnection.Provider;
            def.Source.Schema = _options.SourceSchema;
            List<string> contexts = new List<string>();
            foreach (SpatialContextInfo c in _options.SourceSpatialContexts)
            {
                contexts.Add(c.Name);
            }
            def.Source.SpatialContextList = contexts.ToArray();
            
            def.Target.ConnectionString = _options.TargetConnection.ConnectionString;
            def.Target.Provider = _options.TargetConnection.Provider;
            def.Target.Schema = _options.TargetSchema;

            if(_options.BatchSize > 0)
                def.Target.BatchSize = _options.BatchSize;

            List<FdoClassMapping> mappings = new List<FdoClassMapping>();
            foreach (FdoClassCopyOptions copt in _options.ClassCopyOptions)
            {
                FdoClassMapping map = new FdoClassMapping();
                map.DeleteTarget = copt.DeleteTarget;
                List<FdoExpressionMapping> exprs = new List<FdoExpressionMapping>();
                foreach (string key in copt.SourceAliases)
                {
                    FdoExpressionMapping e = new FdoExpressionMapping();
                    e.SourceExpression = copt.GetExpression(key);
                    e.SourceAlias = key;
                    e.TargetProperty = copt.GetTargetPropertyForAlias(key);
                    exprs.Add(e);
                }
                map.Expressions = exprs.ToArray();
                map.Filter = copt.SourceFilter;
                List<FdoPropertyMapping> props = new List<FdoPropertyMapping>();
                foreach (string key in copt.SourcePropertyNames)
                {
                    FdoPropertyMapping p = new FdoPropertyMapping();
                    p.SourceProperty = key;
                    p.TargetProperty = copt.GetTargetProperty(key);
                    props.Add(p);
                }
                map.Properties = props.ToArray();
                map.SourceClass = copt.SourceClassName;
                map.TargetClass = copt.TargetClassName;

                mappings.Add(map);
            }

            def.ClassMappings = mappings.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                serializer.Serialize(writer, def);
            }
        }

        /// <summary>
        /// Determines if this process is capable of persistence
        /// </summary>
        /// <value></value>
        public override bool CanSave
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the file extension associated with this process. For tasks where <see cref="CanSave"/> is
        /// false, an empty string is returned
        /// </summary>
        /// <returns></returns>
        public override string GetFileExtension()
        {
            return TaskDefinitionHelper.BULKCOPYDEFINITION;
        }

        /// <summary>
        /// Gets a description of this process
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return ResourceUtil.GetString("DESC_BULK_COPY_DEFINITION");
        }

        /// <summary>
        /// Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (string key in subProcessErrors.Keys)
            {
                foreach (Exception ex in subProcessErrors[key])
                {
                    yield return ex;
                }
            }
        }
    }
}