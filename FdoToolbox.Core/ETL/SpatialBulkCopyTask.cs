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
    public class SpatialBulkCopyTask : ITask
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
            _Name = name;
            _Options = options;
            _SrcService = new FeatureService(options.Source.Connection);
            _DestService = new FeatureService(options.Target.Connection);
            this.CopySpatialContextOverride = OverrideFactory.GetCopySpatialContextOverride(options.Target.Connection);
        }

        /// <summary>
        /// The options object
        /// </summary>
        public SpatialBulkCopyOptions Options
        {
            get { return _Options; }
        }

        private string _Name;

        /// <summary>
        /// The name of the task
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
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
	

        private ClassCollection _SourceClasses;
        private ClassCopyOptions[] _ClassesToCopy;

        /// <summary>
        /// Validate the task parameters. Any exceptions thrown should
        /// prevent execution.
        /// </summary>
        public void ValidateTaskParameters()
        {
            IConnection srcConn = _Options.Source.Connection;
            IConnection destConn = _Options.Target.Connection;

            ValidateBulkCopyOptions(srcConn, destConn);
        }

        /// <summary>
        /// Execute the task. Must validate the task first before executing.
        /// </summary>
        public void Execute()
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
                _DestService.ApplySchema(CreateTargetSchema(_Options.SourceSchemaName, srcConn));
                SendMessage("Target Schema Applied");
            }
            SendMessage("Begin bulk copy of classes");
            int total = 0;
            int classesCopied = 1;
            string globFilter = _Options.GlobalSpatialFilter;
            foreach (ClassCopyOptions copyOpts in _ClassesToCopy)
            {
                int copied = 0;
                SendMessage(string.Format("Bulk Copying class {0} of {1}: {2}", classesCopied, _ClassesToCopy.Length, copyOpts.ClassName));
                string [] propNames = copyOpts.PropertyNames;

                //See if we need to delete
                if (copyOpts.DeleteClassData)
                {
                    SendMessage("Deleting data in feature class: " + copyOpts.TargetClassName);
                    using (IDelete cmd = destConn.CreateCommand(CommandType.CommandType_Delete) as IDelete)
                    {
                        cmd.SetFeatureClassName(copyOpts.TargetClassName);
                        //cmd.Filter = Filter.Parse("1 = 1");
                        cmd.Execute();
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
                int count = 0;
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
                    }
                }
                //Select from source class
                using (ISelect select = srcConn.CreateCommand(CommandType.CommandType_Select) as ISelect)
                {
                    select.SetFeatureClassName(copyOpts.ClassName);
                    if(theFilter != null)
                        select.Filter = theFilter;

                    foreach (string propName in propNames)
                    {
                        select.PropertyNames.Add((Identifier)Identifier.Parse(propName));
                    }
                    using (IFeatureReader sourceReader = select.Execute())
                    {
                        //Cache for subsequent iterations
                        ClassDefinition classDef = sourceReader.GetClassDefinition();
                        IInsert insert = destConn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
                        PropertyDefinitionCollection propDefs = classDef.Properties;

                        Dictionary<int, string> cachedPropertyNames = new Dictionary<int, string>();
                        for (int i = 0; i < propDefs.Count; i++)
                        {
                            cachedPropertyNames.Add(i, propDefs[i].Name);
                        }

                        //Loop through the feature reader and process each
                        //result
                        int oldpc = 0;
                        while (sourceReader.ReadNext())
                        {
                            copied += ProcessReader(cachedPropertyNames, propDefs, insert, copyOpts, sourceReader);
                            int pc = (int)(((double)copied / (double)count) * 100);
                            //Only update progress counter when % changes
                            if(pc != oldpc)
                            {
                                oldpc = pc;
                                SendMessage(string.Format("Bulk Copying class {0} of {1}: {2} ({3}% of {4} features)", classesCopied, _ClassesToCopy.Length, copyOpts.ClassName, oldpc, count));
                                SendCount(oldpc);
                            }
                        }

                        //Clean them up
                        insert.Dispose();
                        sourceReader.Close();
                    }
                }
                classesCopied++;
                total += copied;
            }
            watch.Stop();
            SendMessage("Bulk Copy: " + total + " features copied in " + watch.ElapsedMilliseconds + "ms");
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
            SendMessage("Validation Completed");
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

        private int ProcessReader(Dictionary<int, string> cachedPropertyNames, PropertyDefinitionCollection propDefs, IInsert insert, ClassCopyOptions copyOpts, IFeatureReader sourceReader)
        {
            int inserted = 0;
            string targetClass = copyOpts.TargetClassName;

            insert.SetFeatureClassName(targetClass);
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
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_BLOB:
                                insert.PropertyValues.Add(new PropertyValue(target, new BLOBValue(sourceReader.GetLOB(name).Data)));
                                break;
                            case DataType.DataType_Boolean:
                                insert.PropertyValues.Add(new PropertyValue(target, new BooleanValue(sourceReader.GetBoolean(name))));
                                break;
                            case DataType.DataType_Byte:
                                insert.PropertyValues.Add(new PropertyValue(target, new ByteValue(sourceReader.GetByte(name))));
                                break;
                            case DataType.DataType_CLOB:
                                insert.PropertyValues.Add(new PropertyValue(target, new CLOBValue(sourceReader.GetLOB(name).Data)));
                                break;
                            case DataType.DataType_DateTime:
                                insert.PropertyValues.Add(new PropertyValue(target, new DateTimeValue(sourceReader.GetDateTime(name))));
                                break;
                            case DataType.DataType_Decimal:
                                insert.PropertyValues.Add(new PropertyValue(target, new DecimalValue(sourceReader.GetDouble(name))));
                                break;
                            case DataType.DataType_Double:
                                insert.PropertyValues.Add(new PropertyValue(target, new DecimalValue(sourceReader.GetDouble(name))));
                                break;
                            case DataType.DataType_Int16:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int16Value(sourceReader.GetInt16(name))));
                                break;
                            case DataType.DataType_Int32:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int32Value(sourceReader.GetInt32(name))));
                                break;
                            case DataType.DataType_Int64:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int64Value(sourceReader.GetInt64(name))));
                                break;
                            case DataType.DataType_Single:
                                insert.PropertyValues.Add(new PropertyValue(target, new SingleValue(sourceReader.GetSingle(dataDef.Name))));
                                break;
                            case DataType.DataType_String:
                                insert.PropertyValues.Add(new PropertyValue(target, new StringValue(sourceReader.GetString(dataDef.Name))));
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

        /// <summary>
        /// The type of task (BulkCopy)
        /// </summary>
        public TaskType TaskType
        {
            get { return TaskType.BulkCopy; }
        }

        private void SendMessage(string msg)
        {
            if (this.OnTaskMessage != null)
                this.OnTaskMessage(msg);
            if (this.OnLogTaskMessage != null)
                this.OnLogTaskMessage(msg);
        }

        private void SendCount(int count)
        {
            if (this.OnItemProcessed != null)
                this.OnItemProcessed(count);
        }

        public event TaskPercentageEventHandler OnItemProcessed;

        public event TaskProgressMessageEventHandler OnTaskMessage;
       
        public event TaskProgressMessageEventHandler OnLogTaskMessage;

        public bool IsCountable
        {
            get { return true; }
        }
    }
}
