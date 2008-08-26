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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using System.Collections.Specialized;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Commands;
using System.Xml;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Filter;
using System.Diagnostics;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Commands.SpatialContext;
using FdoToolbox.Core.Forms;
using FdoToolbox.Core.Controls;
using System.IO;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Commands.SQL;
using System.Threading;
#region overview
/*
 * Bulk Copy overview
 * 
 * A bulk copy task is initialized with a BulkCopyOptions object that defines
 * the parameters of the bulk copy operation. This object can be viewed as:
 * 
 *  [source class]:[target class]
 *      [properties]
 *          [source property 1]:[target property 1]
 *          [source property 2]:[target property 2]
 *          ...
 * 
 * Each source property maps to a valid target property. We use this when
 * we begin executing the insert commands on the target.
 * 
 * A high level overview of the process is as follows:
 * 
 * for each [source class] to copy
 *      ISelect on that [source class] with the specified [list of properties]
 *      for each result from the returned feature reader
 *          prepare an IInsert on the [target class]
 *          add each [source property] reader value using the [target property name]
 *          execute the IInsert
 * 
 * To make the initial implementation simpler, we don't allow updates on the 
 * target schema as there would be issues regarding cloning, capabilities and
 * various corner cases that would need to be resolved.
 * 
 * Schema/Class creation will be all or nothing. ie. The
 * following scenarios are supported.
 * 
 *  - [Source Schema] to [Empty data store] (full schema/class creation)
 *  - [Source Schema] to [Target Schema] with all mappings *valid* (no creation)
 * 
 * Creation of non-existent properties and classes will come in the future
 */
#endregion
namespace FdoToolbox.Core.ETL
{
    public class SpatialBulkCopyTask : TaskBase
    {
        private SpatialBulkCopyOptions _Options;

        private FeatureService _SrcService;
        private FeatureService _DestService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public SpatialBulkCopyTask(string name, SpatialBulkCopyOptions options)
        {
            this.Name = name;
            _Options = options;
            _SrcService = new FeatureService(options.Source.Connection);
            _DestService = new FeatureService(options.Target.Connection);
            this.CopySpatialContextOverride = OverrideFactory.GetCopySpatialContextOverride(options.Target.Connection);
            this.ClassNameOverride = OverrideFactory.GetClassNameOverride(options.Source.Connection);
            _ErrorMsgs = new List<string>();
        }

        /// <summary>
        /// The options object
        /// </summary>
        public SpatialBulkCopyOptions Options
        {
            get { return _Options; }
        }

        private ICopySpatialContextOverride _CopySpatialContextOverride;

        /// <summary>
        /// A custom copy spatial context method. Use this to handle
        /// corner cases with specific FDO providers
        /// </summary>
        public ICopySpatialContextOverride CopySpatialContextOverride
        {
            get { return _CopySpatialContextOverride; }
            set { _CopySpatialContextOverride = value; }
        }

        private IClassNameOverride _ClassNameOverride;

        /// <summary>
        /// A custom class name override method. Use this to get the underlying
        /// class name for certain FDO providers (eg. Oracle)
        /// </summary>
        public IClassNameOverride ClassNameOverride
        {
            get { return _ClassNameOverride; }
            set { _ClassNameOverride = value; }
        }

        private ClassCollection _SourceClasses;
        private ClassCopyOptions[] _ClassesToCopy;

        /// <summary>
        /// Validate the task parameters. Any exceptions thrown should
        /// prevent execution.
        /// </summary>
        public override void ValidateTaskParameters()
        {
            IConnection srcConn = _Options.Source.Connection;
            IConnection destConn = _Options.Target.Connection;

            ValidateBulkCopyOptions(srcConn, destConn);
        }

        public override void DoExecute()
        {
            IDelete srcDelete = null;
            ISelect srcSelect = null;
            IInsert insertCmd = null;
            IFeatureReader srcReader = null;
            IFeatureReader insReader = null;
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                IConnection srcConn = _Options.Source.Connection;
                IConnection destConn = _Options.Target.Connection;

                if (_Options.CopySpatialContexts)
                {
                    if (this.CopySpatialContextOverride != null)
                        this.CopySpatialContextOverride.CopySpatialContexts(srcConn, destConn, _Options.SourceSpatialContexts);
                    else
                        CopySpatialContexts(_Options.Source, _Options.Target);
                }
                //If target schema is undefined, create it
                if (_Options.ApplySchemaToTarget)
                {
                    SendMessage("Applying schema for target (this may take a while)");
                    FeatureSchema targetSchema = CreateTargetSchema(_Options.SourceSchemaName, srcConn);
                    foreach (ClassDefinition classDef in targetSchema.Classes)
                    {
                        if (classDef.IdentityProperties.Count == 0)
                        {
                            if (destConn.SchemaCapabilities.SupportedAutoGeneratedTypes.Length == 0)
                                throw new BulkCopyException("Class " + classDef.Name + " has no identity properties. An auto-generated ID could not be created as it was not supported");

                            SendMessage("Class " + classDef.Name + " has no identity properties. Making one for it");
                            DataPropertyDefinition dataDef = new DataPropertyDefinition("AutoID", "Auto-generated ID");
                            dataDef.IsAutoGenerated = true;
                            //Get the highest available auto-generated type. 
                            DataType[] dtypes = destConn.SchemaCapabilities.SupportedAutoGeneratedTypes;
                            DataType[] autoTypes = new DataType[dtypes.Length];
                            Array.Copy(dtypes, autoTypes, dtypes.Length);
                            Array.Sort<DataType>(autoTypes, new DataTypeComparer());
                            dataDef.DataType = autoTypes[autoTypes.Length - 1];

                            classDef.IdentityProperties.Add(dataDef);
                        }
                    }
                    _DestService.ApplySchema(targetSchema);
                    _Options.TargetSchemaName = targetSchema.Name;
                    SendMessage("Target Schema Applied");
                }
                
                //Map data types
                SendMessage("Mapping source - target data types");
                foreach (ClassCopyOptions copyOpts in _ClassesToCopy)
                {
                    ClassDefinition srcClass = copyOpts.SourceClassDefinition;
                    ClassDefinition targetClass = _DestService.GetClassByName(_Options.TargetSchemaName, copyOpts.TargetClassName);
                    foreach (string srcPropName in copyOpts.PropertyNames)
                    {
                        string targetPropName = copyOpts.GetTargetPropertyName(srcPropName);
                        PropertyDefinition srcProp = srcClass.Properties[srcClass.Properties.IndexOf(srcPropName)];
                        PropertyDefinition targetProp = targetClass.Properties[targetClass.Properties.IndexOf(targetPropName)];

                        if (srcProp.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            DataPropertyDefinition srcData = srcProp as DataPropertyDefinition;
                            DataPropertyDefinition targetData = targetProp as DataPropertyDefinition;
                            DataTypeMapping mapping = new DataTypeMapping(srcData, targetData);
                            copyOpts.AddDataTypeMapping(mapping);
                        }
                    }
                }
                SendMessage("Mapping completed");
                
                SendMessage("Begin bulk copy of classes");
                long total = 0;
                int classesCopied = 1;
                string globFilter = _Options.GlobalSpatialFilter;
                foreach (ClassCopyOptions copyOpts in _ClassesToCopy)
                {
                    int copied = 0;
                    SendMessage(string.Format("Bulk Copying class {0} of {1}: {2}", classesCopied, _ClassesToCopy.Length, copyOpts.ClassName));
                    string[] propNames = copyOpts.PropertyNames;

                    //See if we need to delete
                    if (copyOpts.DeleteClassData)
                    {
                        SendMessage("Deleting data in feature class: " + copyOpts.TargetClassName);
                        using (srcDelete = destConn.CreateCommand(CommandType.CommandType_Delete) as IDelete)
                        {
                            srcDelete.SetFeatureClassName(copyOpts.TargetClassName);
                            //cmd.Filter = Filter.Parse("1 = 1");
                            srcDelete.Execute();
                        }
                        SendMessage("Done deleting");
                    }

                    //Determine the filter
                    Filter theFilter = null;
                    Filter spatialFilter = null;
                    if (!string.IsNullOrEmpty(globFilter) && copyOpts.SourceClassDefinition.ClassType == ClassType.ClassType_FeatureClass)
                    {
                        spatialFilter = Filter.Parse(((FeatureClass)copyOpts.SourceClassDefinition).GeometryProperty.Name + " " + globFilter);
                    }
                    Filter attrFilter = null;
                    if (!string.IsNullOrEmpty(copyOpts.AttributeFilter))
                        attrFilter = Filter.Parse(copyOpts.AttributeFilter);

                    //Logical AND both attribute and spatial filters
                    if (attrFilter != null && spatialFilter != null)
                    {
                        theFilter = Filter.Parse("(" + attrFilter.ToString() + ") AND (" + spatialFilter.ToString() + ")");
                    }
                    //Set spatial filter
                    else if (attrFilter == null && spatialFilter != null)
                    {
                        theFilter = spatialFilter;
                    }
                    //Set attribute filter
                    else if (attrFilter != null && spatialFilter == null)
                    {
                        theFilter = attrFilter;
                    }

                    //Get count of features to copy
                    long count = GetFeatureCount(srcConn, copyOpts, theFilter);
                    //Select from source class
                    using (srcSelect = srcConn.CreateCommand(CommandType.CommandType_Select) as ISelect)
                    {
                        srcSelect.SetFeatureClassName(copyOpts.ClassName);
                        if (theFilter != null)
                            srcSelect.Filter = theFilter;

                        foreach (string propName in propNames)
                        {
                            srcSelect.PropertyNames.Add((Identifier)Identifier.Parse(propName));
                        }
                        using (srcReader = srcSelect.Execute())
                        {
                            //Cache for subsequent iterations
                            ClassDefinition classDef = srcReader.GetClassDefinition();
                            insertCmd = destConn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
                            PropertyDefinitionCollection propDefs = classDef.Properties;

                            insertCmd.SetFeatureClassName(copyOpts.TargetClassName);

                            //The class definition inside the reader *may* not always contain
                            //only the properties we specified, so only cache the ones that
                            //match the original property list
                            Dictionary<int, string> cachedPropertyNames = new Dictionary<int, string>();
                            Dictionary<int, string> cachedDataPropertyNames = new Dictionary<int, string>();
                            Dictionary<int, string> cachedIdentityPropertyNames = new Dictionary<int, string>();
                            for (int i = 0; i < propDefs.Count; i++)
                            {
                                string pName = propDefs[i].Name;
                                if (Array.IndexOf<string>(propNames, pName) >= 0)
                                {
                                    cachedPropertyNames.Add(i, pName);
                                    if (propDefs[i].PropertyType == PropertyType.PropertyType_DataProperty)
                                    {
                                        cachedDataPropertyNames.Add(i, pName);
                                        if (classDef.IdentityProperties.Contains((DataPropertyDefinition)propDefs[i]))
                                        {
                                            cachedIdentityPropertyNames.Add(i, pName);
                                        }
                                    }
                                }
                            }

                            //If batch insert size defined, prepare insert command for
                            //batch insertion.
                            if (_Options.BatchInsertSize > 0)
                            {
                                foreach (int key in cachedPropertyNames.Keys)
                                {
                                    string name = cachedPropertyNames[key];
                                    string targetName = copyOpts.GetTargetPropertyName(name);
                                    string paramName = PARAM_PREFIX + targetName;
                                    insertCmd.PropertyValues.Add(new PropertyValue(targetName, new Parameter(paramName)));
                                }
                            }

                            //Loop through the feature reader and process each
                            //result
                            int oldpc = 0;
                            bool hasMore = srcReader.ReadNext();
                            while (hasMore)
                            {
                                //No batch size, do vanilla IInsert
                                if (_Options.BatchInsertSize <= 0)
                                {
                                    try
                                    {
                                        int result = ProcessReader(cachedPropertyNames, propDefs, insertCmd, copyOpts, srcReader);
                                        copied += result;
                                    }
                                    catch (BulkCopyException ex)
                                    {
                                        LogOffendingFeature(ex, cachedIdentityPropertyNames, propDefs, copyOpts, srcReader);
                                    }
                                    hasMore = srcReader.ReadNext();
                                }
                                else //batched insert
                                {
                                    insertCmd.BatchParameterValues.Clear();
                                    int batchCount = 0;
                                    //Load up batched values
                                    do
                                    {
                                        try
                                        {
                                            ParameterValueCollection propVals = ProcessReaderBatched(cachedPropertyNames, propDefs, insertCmd, copyOpts, srcReader);
                                            insertCmd.BatchParameterValues.Add(propVals);
                                            hasMore = srcReader.ReadNext();
                                            batchCount++;
                                        }
                                        catch (BulkCopyException ex)
                                        {
                                            LogOffendingFeature(ex, cachedIdentityPropertyNames, propDefs, copyOpts, srcReader);
                                        }
                                    }
                                    while (batchCount < _Options.BatchInsertSize && hasMore);
                                    //Execute batch
                                    using (insReader = insertCmd.Execute())
                                    {
                                        copied += batchCount;
                                        insReader.Close();
                                    }
                                }
                                int pc = (int)(((double)copied / (double)count) * 100);
                                //Only update progress counter when % changes
                                if (pc != oldpc)
                                {
                                    oldpc = pc;
                                #if DEBUG || TEST
                                    SendMessage(string.Format("Bulk Copying class {0} of {1}: {2} ({3} of {4} features)", classesCopied, _ClassesToCopy.Length, copyOpts.ClassName, copied, count));
                                #else
                                    SendMessage(string.Format("Bulk Copying class {0} of {1}: {2} ({3}% of {4} features)", classesCopied, _ClassesToCopy.Length, copyOpts.ClassName, oldpc, count));
                                #endif
                                    SendCount(oldpc);
                                }
                            }

                            //Clean them up
                            insertCmd.Dispose();
                            srcReader.Close();
                        }
                    }
                    classesCopied++;
                    total += copied;
                }
                watch.Stop();
                if (_ErrorMsgs.Count == 0)
                    SendMessage("Bulk Copy: " + total + " features copied in " + watch.ElapsedMilliseconds + "ms");
                else
                {
                    string logPath = AppGateway.RunningApplication.Preferences.GetStringPref(PreferenceNames.PREF_STR_LOG_PATH);
                    string logFile = Path.Combine(logPath, "bcp" + DateTime.Now.ToShortTimeString() + ".log");
                    using (StreamWriter writer = new StreamWriter(logFile, false))
                    {
                        foreach (string msg in _ErrorMsgs)
                        {
                            writer.WriteLine(msg);
                        }
                    }
                    SendMessage("Bulk Copy: " + total + " features copied in " + watch.ElapsedMilliseconds + "ms. " + _ErrorMsgs.Count + " features failed to copy. See " + logFile + " for more information");
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            finally
            {
                if (srcDelete != null)
                    srcDelete.Dispose();
                if (srcSelect != null)
                    srcSelect.Dispose();
                if (insertCmd != null)
                    insertCmd.Dispose();
                if (srcReader != null)
                    srcReader.Dispose();
                if (insReader != null)
                    insReader.Dispose();
            }
        }

        private long GetFeatureCount(IConnection srcConn, ClassCopyOptions copyOpts, Filter theFilter)
        {
            long count = 0;
            // Try to get the count in this order of precedence:
            //
            // 1. SQL - select count(*) from table
            // 2. ISelectAggregates - Count() function
            // 3. Brute force counting
            if (Array.IndexOf<int>(srcConn.CommandCapabilities.Commands, (int)CommandType.CommandType_SQLCommand) >= 0)
            {
                using (ISQLCommand cmd = srcConn.CreateCommand(CommandType.CommandType_SQLCommand) as ISQLCommand)
                {
                    string col = "FEATURECOUNT";
                    string tableName = (this.ClassNameOverride != null) ? this.ClassNameOverride.GetClassName(copyOpts.ClassName) : copyOpts.ClassName;
                    cmd.SQLStatement = string.Format("SELECT COUNT(*) AS {0} FROM {1}", col, tableName);
                    if(theFilter != null)
                        cmd.SQLStatement += string.Format(" WHERE {0}", theFilter.ToString());
                    AppConsole.WriteLine("Issuing SQL: {0}", cmd.SQLStatement);
                    using(ISQLDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.ReadNext())
                        {
                            count = reader.GetInt64(col);
                        }
                        reader.Close();
                    }
                }
            }
            else if ((Array.IndexOf<int>(srcConn.CommandCapabilities.Commands, (int)CommandType.CommandType_SelectAggregates) >= 0) &&
                      srcConn.ExpressionCapabilities.Functions.Contains("Count"))
            {
                using (ISelectAggregates select = srcConn.CreateCommand(CommandType.CommandType_SelectAggregates) as ISelectAggregates)
                {
                    string prop = "FEATURECOUNT";
                    select.SetFeatureClassName(copyOpts.ClassName);
                    if (theFilter != null)
                        select.Filter = theFilter;
                    //Count() requires a property name, so pluck the first property name from the copy options
                    select.PropertyNames.Add(new ComputedIdentifier(prop, Expression.Parse("COUNT(" + copyOpts.SourceClassDefinition.IdentityProperties[0].Name + ")")));
                    using (IDataReader reader = select.Execute())
                    {
                        if (reader.ReadNext())
                        {
                            count = reader.GetInt64(prop);
                        }
                        reader.Close();
                    }
                }
            }
            else
            {
                using (ISelect select = srcConn.CreateCommand(CommandType.CommandType_Select) as ISelect)
                {
                    select.SetFeatureClassName(copyOpts.ClassName);
                    if (theFilter != null)
                        select.Filter = theFilter;

                    using (IFeatureReader reader = select.Execute())
                    {
                        while (reader.ReadNext())
                        {
                            count++;
                        }
                        reader.Close();
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Copy the list of spatial contexts from the source to the target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void CopySpatialContexts(SpatialConnectionInfo source, SpatialConnectionInfo target)
        {
            if (_Options.SourceSpatialContexts.Count == 0)
            {
                SendMessage("Didn't specify any spatial contexts to copy. Skipping");
                return;
            }

            IConnection srcConn = source.Connection;
            IConnection destConn = target.Connection;
            SendMessage("Copying spatial contexts to destination");
            if (!destConn.ConnectionCapabilities.SupportsMultipleSpatialContexts())
            {
                string contextName = _Options.SourceSpatialContexts[0];
                SendMessage("Copying context: " + contextName);
                bool targetCanDestroySpatialContext = (Array.Exists<int>(destConn.CommandCapabilities.Commands, delegate(int c) { return c == (int)CommandType.CommandType_DestroySpatialContext; }));
                //Get source spatial context 
                SpatialContextInfo srcContext = _SrcService.GetSpatialContext(contextName);
                if (srcContext != null)
                {
                    SendMessage("Found source spatial context: " + contextName);
                    List<string> deleteList = new List<string>();
                    SpatialContextInfo destContext = _DestService.GetSpatialContext(contextName);
                    SendMessage("Deleting all target spatial contexts");
                    if (targetCanDestroySpatialContext)
                    {
                        _DestService.DestroySpatialContext(destContext);
                    }
                    SendMessage("Copying selected spatial context " + contextName + " to target");
                    _DestService.CreateSpatialContext(srcContext, !targetCanDestroySpatialContext);
                }
            }
            else
            {
                List<SpatialContextInfo> srcContexts = _SrcService.GetSpatialContexts();
                foreach (string ctxName in _Options.SourceSpatialContexts)
                {
                    SpatialContextInfo srcContext = srcContexts.Find(delegate(SpatialContextInfo ctx) { return ctx.Name == ctxName; });
                    if (srcContext != null)
                    {
                        _DestService.CreateSpatialContext(srcContext, true);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the target schema from the source schema
        /// </summary>
        /// <param name="sourceSchemaName"></param>
        /// <param name="srcConn"></param>
        /// <returns></returns>
        private FeatureSchema CreateTargetSchema(string sourceSchemaName, IConnection srcConn)
        {
            SendMessage("Cloning source schema for target");
            FeatureSchema fs = _SrcService.GetSchemaByName(sourceSchemaName);
            if (fs != null)
            {
                return CloneSchema(fs);
            }
            throw new BulkCopyException("Could not find source schema to clone: " + sourceSchemaName);
        }

        /// <summary>
        /// Perform an in-memory clone of a given Feature Schema
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static FeatureSchema CloneSchema(FeatureSchema fs)
        {
            //FDO doesn't have clone support for schemas so we'll use
            //in-memory XML serialization as a workaround.
            using (IoStream stream = new IoMemoryStream())
            {
                fs.WriteXml(stream);
                stream.Reset();
                FeatureSchemaCollection newSchemas = new FeatureSchemaCollection(null);
                newSchemas.ReadXml(stream);
                stream.Close();
                return newSchemas[0];
            }
        }

        /// <summary>
        /// Validates the options supplied for this bulk copy task
        /// </summary>
        /// <param name="srcConn">The source connection</param>
        /// <param name="destConn">The target connection</param>
        /// <param name="srcClasses">The classes to be copied</param>
        /// <param name="classesToCopy">The copy options for each specified class</param>
        private void ValidateBulkCopyOptions(IConnection srcConn, IConnection destConn)
        {
            //TODO: Validate length of data properties so that:
            //[source property length] <= [target property length]

            //Validate each source class copy option specified.
            SendMessage("Validating Bulk Copy Options");

            FeatureService destService = new FeatureService(destConn);
            if (_Options.ApplySchemaToTarget)
            {
                FeatureSchema targetSchema = CreateTargetSchema(_Options.SourceSchemaName, srcConn);
                IncompatibleSchema schema = null;
                if (!destService.CanApplySchema(targetSchema, out schema))
                    throw new TaskValidationException("The source schema cannot be applied to the target:\n\n" + schema.ToString());
            }

            if (_Options.BatchInsertSize > 0 && !destService.SupportsBatchInsertion())
                throw new TaskValidationException("Target connection does not support batch insertion");

            if (string.IsNullOrEmpty(_Options.SourceSchemaName))
                throw new TaskValidationException("Source schema name not defined");

            if (!Array.Exists<int>(_Options.Target.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)CommandType.CommandType_ApplySchema; }))
                throw new TaskValidationException("Target connection does not support IApplySchema");

            if (_Options.CopySpatialContexts && _Options.SourceSpatialContexts.Count == 0)
                throw new TaskValidationException("Source spatial context name(s) not defined");

            //Get source schema classes
            _SourceClasses = GetSourceClasses(_Options);
            if (_SourceClasses == null)
                throw new TaskValidationException("Unable to get classes of feature schema: " + _Options.SourceSchemaName);

            _ClassesToCopy = _Options.GetClassCopyOptions();
            
            if (_Options.ApplySchemaToTarget)
            {
                _ClassesToCopy = GetAllClassCopyOptions(_Options, _SourceClasses);
            }

            bool checkedForDelete = false;
            foreach (ClassCopyOptions copyOpts in _ClassesToCopy)
            {
                if (!checkedForDelete && copyOpts.DeleteClassData)
                {
                    if (!Array.Exists<int>(_Options.Target.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)CommandType.CommandType_Delete; }))
                        throw new TaskValidationException("Target connection does not support delete (IDelete)");
                    checkedForDelete = true;
                }

                try
                {
                    if(!string.IsNullOrEmpty(copyOpts.AttributeFilter))
                        using (Filter filter = Filter.Parse(copyOpts.AttributeFilter)) { }   
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    throw new TaskValidationException("Error parsing filter", ex);
                }

                try
                {
                    if (copyOpts.SourceClassDefinition.ClassType == ClassType.ClassType_FeatureClass && !string.IsNullOrEmpty(_Options.GlobalSpatialFilter))
                    {
                        string geomProp = (copyOpts.SourceClassDefinition as FeatureClass).GeometryProperty.Name;
                        string filter = geomProp + " " + _Options.GlobalSpatialFilter;
                        using (Filter spatialFilter = Filter.Parse(filter)) { } 
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    throw new TaskValidationException("Error parsing global spatial filter", ex);
                }

                string className = copyOpts.ClassName;
                int idx = _SourceClasses.IndexOf(className);
                if (idx < 0)
                    throw new TaskValidationException("Unable to find class " + className + " in schema " + _Options.SourceSchemaName);

                ClassDefinition classDef = _SourceClasses[idx];
                string[] propNames = copyOpts.PropertyNames;

                if (propNames.Length == 0)
                    throw new TaskValidationException("Nothing to copy from class " + className + " in schema " + _Options.SourceSchemaName);

                if (_Options.ApplySchemaToTarget)
                {
                    foreach (string propName in propNames)
                    {
                        int pidx = classDef.Properties.IndexOf(propName);
                        if (pidx < 0)
                        {
                            throw new TaskValidationException("Unable to find source property " + propName + " in class " + classDef.Name + " of schema " + _Options.SourceSchemaName);
                        }
                        else
                        {
                            PropertyDefinition propDef = classDef.Properties[pidx];
                            if (propDef.PropertyType == PropertyType.PropertyType_DataProperty)
                            {
                                DataType dtype = (propDef as DataPropertyDefinition).DataType;
                                if (Array.IndexOf<DataType>(destConn.SchemaCapabilities.DataTypes, dtype) < 0)
                                    throw new TaskValidationException(string.Format("Source class {0} has a property {1} whose data type {2} is not supported by the target connection", classDef.Name, propDef.Name, dtype));
                            }
                        }
                    }
                }
                else
                {
                    ClassDefinition targetClass = destService.GetClassByName(_Options.TargetSchemaName, copyOpts.TargetClassName);
                    if (targetClass == null)
                        throw new TaskValidationException("Unable to find target class: " + copyOpts.TargetClassName);

                    foreach (string propName in propNames)
                    {
                        int sidx = classDef.Properties.IndexOf(propName);
                        if (sidx < 0)
                            throw new TaskValidationException("Unable to find source property " + propName + " in class " + classDef.Name + " of schema " + _Options.SourceSchemaName);
                        string tprop = copyOpts.GetTargetPropertyName(propName);
                        int tidx = targetClass.Properties.IndexOf(tprop);
                        if (tidx < 0)
                            throw new TaskValidationException("Unable to find target property " + tprop + " in class " + copyOpts.TargetClassName);

                        PropertyDefinition srcProp = classDef.Properties[sidx];
                        PropertyDefinition tgProp = targetClass.Properties[tidx];
                        if (srcProp.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            if (tgProp.PropertyType != PropertyType.PropertyType_DataProperty)
                                throw new TaskValidationException("Target property " + tgProp.QualifiedName + " is not the same property type as property " + srcProp.QualifiedName);
                            DataType srcType = (srcProp as DataPropertyDefinition).DataType;
                            DataType tgType = (tgProp as DataPropertyDefinition).DataType;
                            if (!IsConvertable(srcType, tgType))
                                throw new TaskValidationException("Data Type of " + srcProp.QualifiedName + " (" + srcType + ") cannot be converted to (" + tgType + ")");
                        }
                    }
                }
            }
            SendMessage("Validation Completed");
        }

        public static bool IsConvertable(DataType srcType, DataType tgType)
        {
            if (srcType == tgType)
                return true;

            switch (srcType)
            {
                //BLOB - n/a (Target must be BLOB)
                case DataType.DataType_BLOB:
                    return tgType == DataType.DataType_BLOB;
                //Boolean
                case DataType.DataType_Boolean:
                    return tgType == DataType.DataType_Boolean ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Byte ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //Byte
                case DataType.DataType_Byte:
                    return tgType == DataType.DataType_Byte ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //CLOB
                case DataType.DataType_CLOB:
                    return tgType == DataType.DataType_CLOB;
                //DateTime
                case DataType.DataType_DateTime:
                    return tgType == DataType.DataType_DateTime ||
                           tgType == DataType.DataType_String;
                //Decimal
                case DataType.DataType_Decimal:
                    return tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Single ||
                           tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //Double
                case DataType.DataType_Double:
                    return tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Single ||
                           tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //Int16
                case DataType.DataType_Int16:
                    return tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_Single ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //Int32
                case DataType.DataType_Int32:
                    return tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_Single ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int64;
                //Int64
                case DataType.DataType_Int64:
                    return tgType == DataType.DataType_Int64 ||
                           tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_Single ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32;
                //Single
                case DataType.DataType_Single:
                    return tgType == DataType.DataType_String ||
                           tgType == DataType.DataType_Double ||
                           tgType == DataType.DataType_Decimal ||
                           tgType == DataType.DataType_Int16 ||
                           tgType == DataType.DataType_Int32 ||
                           tgType == DataType.DataType_Int64;
                //String
                case DataType.DataType_String:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all the class copy options for this task
        /// </summary>
        /// <param name="options"></param>
        /// <param name="srcClasses"></param>
        /// <returns></returns>
        public static ClassCopyOptions[] GetAllClassCopyOptions(SpatialBulkCopyOptions options, ClassCollection srcClasses)
        {
            //Include *all* classes and *all* properties
            options.ClearClassCopyOptions();
            foreach (ClassDefinition classDef in srcClasses)
            {
                options.AddClassCopyOption(new ClassCopyOptions(classDef, true));
            }
            return options.GetClassCopyOptions();
        }

        /// <summary>
        /// Gets all the source class definitions
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ClassCollection GetSourceClasses(SpatialBulkCopyOptions options)
        {
            FeatureService service = new FeatureService(options.Source.Connection);
            FeatureSchema schema = service.GetSchemaByName(options.SourceSchemaName);
            if (schema != null)
                return schema.Classes;
            return null;
        }

        const string PARAM_PREFIX = "param_";

        private ParameterValueCollection ProcessReaderBatched(Dictionary<int, string> cachedPropertyNames, PropertyDefinitionCollection propDefs, IInsert insert, ClassCopyOptions copyOpts, IFeatureReader sourceReader)
        {
            ParameterValueCollection propVals = new ParameterValueCollection();
            foreach (int key in cachedPropertyNames.Keys)
            {
                string name = cachedPropertyNames[key];
                PropertyDefinition def = propDefs[key];
                if (!sourceReader.IsNull(name))
                {
                    string target = copyOpts.GetTargetPropertyName(name);
                    string paramName = PARAM_PREFIX + target;
                    if (string.IsNullOrEmpty(target))
                        target = name;

                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null)
                    {
                        DataTypeMapping mapping = copyOpts.GetDataTypeMapping(name);
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_BLOB:
                                {
                                    byte[] value = sourceReader.GetLOB(name).Data;
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new BLOBValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Boolean:
                                {
                                    bool value = sourceReader.GetBoolean(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new BooleanValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Byte:
                                {
                                    byte value = sourceReader.GetByte(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new ByteValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_CLOB:
                                {
                                    byte[] value = sourceReader.GetLOB(name).Data;
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new CLOBValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_DateTime:
                                {
                                    DateTime value = sourceReader.GetDateTime(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new DateTimeValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Decimal:
                                {
                                    double value = sourceReader.GetDouble(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new DecimalValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Double:
                                {
                                    double value = sourceReader.GetDouble(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new DecimalValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int16:
                                {
                                    short value = sourceReader.GetInt16(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new Int16Value(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int32:
                                {
                                    int value = sourceReader.GetInt32(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new Int32Value(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int64:
                                {
                                    long value = sourceReader.GetInt64(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new Int64Value(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Single:
                                {
                                    float value = sourceReader.GetSingle(dataDef.Name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new SingleValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_String:
                                {
                                    string value = sourceReader.GetString(dataDef.Name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        propVals.Add(new ParameterValue(paramName, new StringValue(value)));
                                    else
                                        propVals.Add(new ParameterValue(paramName, GetConvertedValue(mapping, value)));
                                }
                                break;
                        }
                    }
                    else if (geomDef != null)
                    {
                        byte[] value = sourceReader.GetGeometry(name);
                        propVals.Add(new ParameterValue(paramName, new GeometryValue(value)));
                    }
                }
            }
            return propVals;
        }

        private int ProcessReader(Dictionary<int, string> cachedPropertyNames, PropertyDefinitionCollection propDefs, IInsert insert, ClassCopyOptions copyOpts, IFeatureReader sourceReader)
        {
            int inserted = 0;
            string targetClass = copyOpts.TargetClassName;

            insert.PropertyValues.Clear();
            foreach (int key in cachedPropertyNames.Keys)
            {
                string name = cachedPropertyNames[key];
                PropertyDefinition def = propDefs[key];
                if (!sourceReader.IsNull(name))
                {
                    string target = copyOpts.GetTargetPropertyName(name);
                    if (string.IsNullOrEmpty(target))
                        target = name;

                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null)
                    {
                        DataTypeMapping mapping = copyOpts.GetDataTypeMapping(name);
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_BLOB:
                                {
                                    byte[] value = sourceReader.GetLOB(name).Data;
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new BLOBValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Boolean:
                                {
                                    bool value = sourceReader.GetBoolean(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new BooleanValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Byte:
                                {
                                    byte value = sourceReader.GetByte(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new ByteValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_CLOB:
                                {
                                    byte[] value = sourceReader.GetLOB(name).Data;
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new CLOBValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_DateTime:
                                {
                                    DateTime value = sourceReader.GetDateTime(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new DateTimeValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Decimal:
                                {
                                    double value = sourceReader.GetDouble(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new DecimalValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Double:
                                {
                                    double value = sourceReader.GetDouble(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new DoubleValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int16:
                                {
                                    short value = sourceReader.GetInt16(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new Int16Value(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int32:
                                {
                                    int value = sourceReader.GetInt32(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new Int32Value(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Int64:
                                {
                                    long value = sourceReader.GetInt64(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new Int64Value(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_Single:
                                {
                                    float value = sourceReader.GetSingle(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new SingleValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                            case DataType.DataType_String:
                                {
                                    string value = sourceReader.GetString(name);
                                    if (mapping == null || (mapping.SourceDataType == mapping.TargetDataType))
                                        insert.PropertyValues.Add(new PropertyValue(target, new StringValue(value)));
                                    else
                                        insert.PropertyValues.Add(new PropertyValue(target, GetConvertedValue(mapping, value)));
                                }
                                break;
                        }
                    }
                    else if (geomDef != null)
                    {
                        byte[] value = sourceReader.GetGeometry(name);
                        insert.PropertyValues.Add(new PropertyValue(target, new GeometryValue(value)));
                    }
                }
            }
            IFeatureReader insertResult = insert.Execute();
            using (insertResult)
            {
                inserted++;
                insertResult.Close();
            }
            return inserted;
        }

        private List<string> _ErrorMsgs;

        private void LogOffendingFeature(BulkCopyException ex, Dictionary<int, string> cachedIdentityPropertyNames, PropertyDefinitionCollection propDefs, ClassCopyOptions copyOpts, IFeatureReader srcReader)
        {
            StringBuilder msg = new StringBuilder("Error: " + ex.Message + "\n\tIdentity Properties:\n");
            foreach (int pidx in cachedIdentityPropertyNames.Keys)
            {
                DataPropertyDefinition dp = propDefs[pidx] as DataPropertyDefinition;
                string name = dp.Name;
                switch (dp.DataType)
                { 
                    case DataType.DataType_BLOB:
                        break;
                    case DataType.DataType_Boolean:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetBoolean(name));
                        break;
                    case DataType.DataType_Byte:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetByte(name));
                        break;
                    case DataType.DataType_CLOB:
                        break;
                    case DataType.DataType_DateTime:
                        break;
                    case DataType.DataType_Decimal:
                        break;
                    case DataType.DataType_Double:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetDouble(name));
                        break;
                    case DataType.DataType_Int16:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetInt16(name));
                        break;
                    case DataType.DataType_Int32:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetInt32(name));
                        break;
                    case DataType.DataType_Int64:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetInt64(name));
                        break;
                    case DataType.DataType_Single:
                        break;
                    case DataType.DataType_String:
                        msg.AppendFormat("\t\t{0}: {1}\n", name, srcReader.GetString(name));
                        break;
                }
            }
            _ErrorMsgs.Add(msg.ToString());
        }

#if TEST
        public static LiteralValue GetConvertedValue(DataTypeMapping mapping, object obj)
#else
        private static LiteralValue GetConvertedValue(DataTypeMapping mapping, object obj)
#endif
        {
            try
            {
                switch (mapping.SourceDataType)
                {
                    case DataType.DataType_BLOB:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_BLOB:
                                    return new BLOBValue((byte[])obj);
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Boolean:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Boolean:
                                    return new BooleanValue(Convert.ToBoolean(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Byte:
                                    return new ByteValue(Convert.ToByte(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Byte:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Byte:
                                    return new ByteValue(Convert.ToByte(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_CLOB:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_CLOB:
                                    return new CLOBValue((byte[])obj);
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_DateTime:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_DateTime:
                                    return new DateTimeValue(Convert.ToDateTime(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Decimal:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToDouble(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Double:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToDouble(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Int16:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Int32:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Int64:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_Single:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToSingle(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                    case DataType.DataType_String:
                        {
                            switch (mapping.TargetDataType)
                            {
                                case DataType.DataType_String:
                                    return new StringValue(obj.ToString());
                                case DataType.DataType_Boolean:
                                    return new BooleanValue(Convert.ToBoolean(obj));
                                case DataType.DataType_Byte:
                                    return new ByteValue(Convert.ToByte(obj));
                                case DataType.DataType_DateTime:
                                    return new DateTimeValue(Convert.ToDateTime(obj));
                                case DataType.DataType_Decimal:
                                    return new DecimalValue(Convert.ToDouble(obj));
                                case DataType.DataType_Double:
                                    return new DoubleValue(Convert.ToDouble(obj));
                                case DataType.DataType_Int16:
                                    return new Int16Value(Convert.ToInt16(obj));
                                case DataType.DataType_Int32:
                                    return new Int32Value(Convert.ToInt32(obj));
                                case DataType.DataType_Int64:
                                    return new Int64Value(Convert.ToInt64(obj));
                                case DataType.DataType_Single:
                                    return new SingleValue(Convert.ToSingle(obj));
                                default:
                                    throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
                            }
                        }
                }
            }
            catch (InvalidCastException ex)
            {
                throw new BulkCopyException("Invalid conversion", ex);
            }
            catch (OverflowException ex)
            {
                throw new BulkCopyException("Invalid conversion", ex);
            }
            catch (FormatException ex)
            {
                throw new BulkCopyException("Invalid conversion", ex);
            }
            throw new BulkCopyException("Cannot convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
        }

        /// <summary>
        /// The type of task (BulkCopy)
        /// </summary>
        public override TaskType TaskType
        {
            get { return TaskType.SpatialBulkCopy; }
        }

        public override bool IsCountable
        {
            get { return true; }
        }
    }
}
