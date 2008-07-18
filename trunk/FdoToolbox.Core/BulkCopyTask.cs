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
namespace FdoToolbox.Core
{
    public delegate string GetSpatialContextNameMethod(IConnection conn);

    public class BulkCopyTask : ITask
    {
        private BulkCopyOptions _Options;
        
        public BulkCopyTask(string name, BulkCopyOptions options)
        {
            _Name = name;
            _Options = options;
            this.SpatialContextPickerMethod += new GetSpatialContextNameMethod(GetSourceSpatialContext);
        }

        private string GetSourceSpatialContext(IConnection conn)
        {
            return SpatialContextPicker.GetName(conn);
        }

        private GetSpatialContextNameMethod _GetSpatialContextMethod;

        /// <summary>
        /// Method that returns the name of the chosen spatial context.
        /// Defaults to prompting a user with a SpatialContextPicker
        /// </summary>
        public GetSpatialContextNameMethod SpatialContextPickerMethod
        {
            get { return _GetSpatialContextMethod; }
            set { _GetSpatialContextMethod = value; }
        }

        public BulkCopyOptions Options
        {
            get { return _Options; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private ClassCollection _SourceClasses;
        private ClassCopyOptions[] _ClassesToCopy;

        public void ValidateTaskParameters()
        {
            IConnection srcConn = _Options.Source.Connection;
            IConnection destConn = _Options.Target.Connection;

            ValidateBulkCopyOptions(srcConn, destConn);
        }

        public void Execute()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            IConnection srcConn = _Options.Source.Connection;
            IConnection destConn = _Options.Target.Connection;
            
            if (_Options.CopySpatialContexts)
            {
                CopySpatialContexts(srcConn, destConn);
            }
            //If target schema is undefined, create it
            if (_Options.ApplySchemaToTarget)
            {
                using (IApplySchema apply = destConn.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema)
                {
                    SendMessage("Applying schema for target (this may take a while)");
                    apply.FeatureSchema = CreateTargetSchema(_Options.SourceSchemaName, srcConn);
                    apply.Execute();
                    SendMessage("Target Schema Applied");
                }
            }
            SendMessage("Begin bulk copy of classes");
            int total = 0;
            int classesCopied = 1;
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
                        cmd.Filter = Filter.Parse("1 = 1");
                        cmd.Execute();
                    }
                    SendMessage("Done deleting");
                }

                //Get count of features to copy
                int count = 0;
                using (ISelect select = srcConn.CreateCommand(CommandType.CommandType_Select) as ISelect)
                {
                    select.SetFeatureClassName(copyOpts.ClassName);
                    
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
            SendMessage("Bulk Copy Completed in " + watch.ElapsedMilliseconds + "ms");
            AppConsole.Alert("Bulk Copy", total + " features copied in " + watch.ElapsedMilliseconds + "ms");
        }

        private void CopySpatialContexts(IConnection srcConn, IConnection destConn)
        {
            SendMessage("Copying spatial contexts to destination");
            if (!destConn.ConnectionCapabilities.SupportsMultipleSpatialContexts())
            {
                if (this.SpatialContextPickerMethod != null)
                {
                    SendMessage("Prompting user for source spatial context");
                    string contextName = this.SpatialContextPickerMethod(srcConn);
                    //Get source spatial context 
                    using (IGetSpatialContexts cmd = srcConn.CreateCommand(CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
                    {
                        using (ISpatialContextReader reader = cmd.Execute())
                        {
                            while (reader.ReadNext())
                            {
                                if (reader.GetName() == contextName)
                                {
                                    SendMessage("Found source spatial context");
                                    List<string> deleteList = new List<string>();
                                    using (IGetSpatialContexts targetContexts = destConn.CreateCommand(CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
                                    {
                                        using (ISpatialContextReader targetReader = targetContexts.Execute())
                                        {
                                            while (targetReader.ReadNext())
                                            {
                                                deleteList.Add(targetReader.GetName());
                                            }
                                        }
                                    }
                                    SendMessage("Deleting all target spatial contexts");
                                    bool targetCanDestroySpatialContext = (Array.Exists<int>(destConn.CommandCapabilities.Commands, delegate(int c) { return c == (int)CommandType.CommandType_DestroySpatialContext; })) ;
                                    if (targetCanDestroySpatialContext)
                                    {
                                        foreach (string contextToDelete in deleteList)
                                        {
                                            using (IDestroySpatialContext destroy = destConn.CreateCommand(CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
                                            {
                                                destroy.Name = contextToDelete;
                                                destroy.Execute();
                                            }
                                        }
                                    }
                                    SendMessage("Copying selected spatial context to target");
                                    using (ICreateSpatialContext create = destConn.CreateCommand(CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
                                    {
                                        create.CoordinateSystem = reader.GetCoordinateSystem();
                                        create.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
                                        create.Description = reader.GetDescription();
                                        create.Extent = reader.GetExtent();
                                        create.ExtentType = reader.GetExtentType();
                                        //If context was destroyed, use new name otherwise
                                        //update with the existing name
                                        if (targetCanDestroySpatialContext)
                                            create.Name = reader.GetName();
                                        else
                                            create.UpdateExisting = true;
                                        create.XYTolerance = reader.GetXYTolerance();
                                        create.ZTolerance = reader.GetZTolerance();
                                        create.Execute();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (IGetSpatialContexts cmd = srcConn.CreateCommand(CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
                {
                    using (ISpatialContextReader reader = cmd.Execute())
                    {
                        while (reader.ReadNext())
                        {
                            using (ICreateSpatialContext create = destConn.CreateCommand(CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
                            {
                                string name = reader.GetName();
                                SendMessage("Copying spatial context: " + name);
                                //SHP-Specific processing (ugh!) It doesn't like it when
                                //CSName != Spatial Context Name
                                if(destConn.ConnectionInfo.ProviderName.StartsWith(ExpressUtility.PROVIDER_SHP))
                                {
                                    string wkt = reader.GetCoordinateSystemWkt();
                                    WKTParser parser = new WKTParser(wkt);
                                    //No wkt. Don't bother creating the context
                                    if(!string.IsNullOrEmpty(parser.CSName))
                                    {
                                        create.CoordinateSystem = parser.CSName;
                                        create.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
                                        create.Description = reader.GetDescription();
                                        create.Extent = reader.GetExtent();
                                        create.ExtentType = reader.GetExtentType();
                                        create.Name = parser.CSName;
                                        create.XYTolerance = reader.GetXYTolerance();
                                        create.ZTolerance = reader.GetZTolerance();
                                        create.Execute();
                                    }
                                }
                                else
                                {
                                    create.CoordinateSystem = reader.GetCoordinateSystem();
                                    create.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
                                    create.Description = reader.GetDescription();
                                    create.Extent = reader.GetExtent();
                                    create.ExtentType = reader.GetExtentType();
                                    create.Name = name;
                                    create.UpdateExisting = true;
                                    create.XYTolerance = reader.GetXYTolerance();
                                    create.ZTolerance = reader.GetZTolerance();
                                    create.Execute();
                                }
                            }
                        }
                    }
                }
            }
        }

        private FeatureSchema CreateTargetSchema(string sourceSchemaName, IConnection srcConn)
        {
            SendMessage("Cloning source schema for target");
            FeatureSchema fs = null;
            using (IDescribeSchema desc = srcConn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = desc.Execute();
                fs = schemas[schemas.IndexOf(sourceSchemaName)];
            }

            if (fs != null)
            {
                return CloneSchema(fs);
            }
            throw new BulkCopyException("Could not find source schema to clone: " + sourceSchemaName);
        }

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

        public static ClassCopyOptions[] GetAllClassCopyOptions(BulkCopyOptions options, ClassCollection srcClasses)
        {
            //Include *all* classes and *all* properties
            options.ClearClassCopyOptions();
            foreach (ClassDefinition classDef in srcClasses)
            {
                options.AddClassCopyOption(new ClassCopyOptions(classDef, true));
            }
            return options.GetClassCopyOptions();;
        }

        public static ClassCollection GetSourceClasses(BulkCopyOptions options)
        {
            IConnection srcConn = options.Source.Connection;
            ClassCollection srcClasses = null;
            using (IDescribeSchema desc = srcConn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection srcSchemas = desc.Execute();
                foreach (FeatureSchema schema in srcSchemas)
                {
                    if (schema.Name == options.SourceSchemaName)
                        srcClasses = schema.Classes;
                }
            }
            return srcClasses;
        }

        private int ProcessReader(Dictionary<int, string> cachedPropertyNames, PropertyDefinitionCollection propDefs, IInsert insert, ClassCopyOptions copyOpts, IFeatureReader sourceReader)
        {
            int inserted = 0;
            string targetClass = copyOpts.TargetClassName;

            insert.SetFeatureClassName(targetClass);
            insert.PropertyValues.Clear();
            for (int i = 0; i < propDefs.Count; i++)
            {
                string name = cachedPropertyNames[i];
                if (!sourceReader.IsNull(name))
                {
                    string target = copyOpts.GetTargetPropertyName(name);
                    if (string.IsNullOrEmpty(target))
                        target = name;

                    DataPropertyDefinition dataDef = propDefs[i] as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = propDefs[i] as GeometricPropertyDefinition;
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
                        insert.PropertyValues.Add(new PropertyValue(name, new GeometryValue(value)));
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
    }

    public class BulkCopyOptions : IDisposable
    {
        private ConnectionInfo _Source;
        private ConnectionInfo _Target;

        private string _TargetSchemaName;
        private string _SourceSchemaName;
        private List<ClassCopyOptions> _SourceClasses;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public BulkCopyOptions(ConnectionInfo source, ConnectionInfo target)
        {
            _Source = source;
            _Target = target;
            _SourceClasses = new List<ClassCopyOptions>();
        }

        private bool _ExpressMode = false;

        /// <summary>
        /// Constructor for express bulk copy tasks. The target will be created
        /// and all necessary connections will be set up. Parameters will be set
        /// up so that ApplySchemaToTarget is true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        public BulkCopyOptions(ExpressProvider source, ExpressProvider target, string sourceFile, string targetFile)
        {
            _SourceClasses = new List<ClassCopyOptions>();
            _ExpressMode = true;
            
            IConnection src = null;
            IConnection dest = null;

            switch (source)
            {
                case ExpressProvider.SDF:
                    src = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SDF);
                    src.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SDF, sourceFile);
                    break;
                case ExpressProvider.SHP:
                    src = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SHP);
                    src.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SHP, sourceFile);
                    break;
            }

            src.Open();

            //SDF and SHP are single schema, so grab the first schema from IDescribeSchema
            //and set it as the source schema name
            using (IDescribeSchema desc = src.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = desc.Execute();
                if (schemas.Count == 0)
                    throw new BulkCopyException("No schemas found on source connection");
                this.SourceSchemaName = schemas[0].Name;
            }

            switch (target)
            {
                case ExpressProvider.SDF:
                    {
                        ExpressUtility.CreateSDF(targetFile);
                        dest = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SDF);
                        dest.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SDF, targetFile);
                    }
                    break;
                case ExpressProvider.SHP:
                    {
                        dest = FeatureAccessManager.GetConnectionManager().CreateConnection(ExpressUtility.PROVIDER_SHP);
                        string name = Path.GetFileNameWithoutExtension(targetFile);
                        string path = Path.GetDirectoryName(targetFile);
                        DeleteRelatedShpFiles(path, name);
                        dest.ConnectionString = string.Format(ExpressUtility.CONN_FMT_SHP, path);
                    }
                    break;
            }

            dest.Open();

            if (src != null && dest != null)
            {
                this.Source = new ConnectionInfo("SOURCE", src);
                this.Target = new ConnectionInfo("TARGET", dest);
            }
        }

        /// <summary>
        /// Deletes a SHP file and all its related files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void DeleteRelatedShpFiles(string path, string name)
        {
            string[] extensions = { "shp", "dbf", "prj", "shx", "idx", "cpg" };
            foreach (string ext in extensions)
            {
                string file = Path.Combine(path, string.Format("{0}.{1}", name, ext));
                File.Delete(file);
                AppConsole.WriteLine("[Express Bulk Copy]: Deleted file {0}", file);
            }
        }

        /// <summary>
        /// The source connection
        /// </summary>
        public ConnectionInfo Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        /// <summary>
        /// The target connection
        /// </summary>
        public ConnectionInfo Target
        {
            get { return _Target; }
            set { _Target = value; }
        }

        private bool _CopySpatialContexts;

        /// <summary>
        /// If true copies all source spatial contexts from the source to the
        /// target. If the target does not support multiple spatial contexts,
        /// then the target spatial context will be overwritten by the context
        /// of the user's choice.
        /// </summary>
        public bool CopySpatialContexts
        {
            get { return _CopySpatialContexts; }
            set { _CopySpatialContexts = value; }
        }

        private bool _CoerceDataTypes;

        public bool CoerceDataTypes
        {
            get { return _CoerceDataTypes; }
            set { _CoerceDataTypes = value; }
        }

        /// <summary>
        /// If true, the mappings are ignored and the full source schema
        /// will be applied to the target. Is only true if the target
        /// schema is undefined
        /// </summary>
        public bool ApplySchemaToTarget
        {
            get { return string.IsNullOrEmpty(this.TargetSchemaName); }
        }

        /// <summary>
        /// The source schema
        /// </summary>
        public string SourceSchemaName
        {
            get { return _SourceSchemaName; }
            set { _SourceSchemaName = value; }
        }

        /// <summary>
        /// The target schema. If this schema does not exist in the
        /// target connection, then every class specified in the source
        /// will be copied across.
        /// </summary>
        public string TargetSchemaName
        {
            get { return _TargetSchemaName; }
            set { _TargetSchemaName = value; }
        }

        /// <summary>
        /// Adds a source class to be copied over
        /// </summary>
        /// <param name="options"></param>
        public void AddClassCopyOption(ClassCopyOptions options)
        {
            _SourceClasses.Add(options);
        }

        /// <summary>
        /// Removes all added class copy options
        /// </summary>
        public void ClearClassCopyOptions()
        {
            _SourceClasses.Clear();
        }

        /// <summary>
        /// Gets all the classes to copy
        /// </summary>
        /// <returns></returns>
        public ClassCopyOptions[] GetClassCopyOptions()
        {
            return _SourceClasses.ToArray();
        }

        public void Dispose()
        {
            //Since connections in express mode are created outside of the
            //connection manager's knowledge we have to clean them up explicitly
            if (_ExpressMode)
            {
                if (this.Source.Connection.ConnectionState == ConnectionState.ConnectionState_Open)
                    this.Source.Connection.Close();
                if (this.Target.Connection.ConnectionState == ConnectionState.ConnectionState_Open)
                    this.Target.Connection.Close();

                this.Source.Connection.Dispose();
                this.Target.Connection.Dispose();
            }
        }
    }

    /// <summary>
    /// Source Class Copy options
    /// </summary>
    public class ClassCopyOptions
    {
        private NameValueCollection _PropertyMappings;
        private Dictionary<string, PropertyDefinition> _PropertyDefinitions;

        private ClassDefinition _ClassDef;

        /// <summary>
        /// The source class name
        /// </summary>
	    public string ClassName
	    {
		    get { return _ClassDef.Name; }
	    }

        private string _TargetClassName;

        /// <summary>
        /// The target class name
        /// </summary>
	    public string TargetClassName
	    {
		    get { return _TargetClassName; }
		    set { _TargetClassName = value; }
	    }
	
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">The name of the source class</param>
        public ClassCopyOptions(ClassDefinition classDef) 
        {
            _ClassDef = classDef;
            _PropertyMappings = new NameValueCollection();
            _PropertyDefinitions = new Dictionary<string, PropertyDefinition>();
        }

        /// <summary>
        /// Alternative constructor. Used for express BCP tasks
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="includeAllProperties"></param>
        public ClassCopyOptions(ClassDefinition classDef, bool includeAllProperties)
            : this(classDef)
        {
            if (includeAllProperties)
            {
                foreach (PropertyDefinition def in classDef.Properties)
                {
                    //Omit any non-writable properties
                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null && dataDef.ReadOnly)
                        continue;
                    if (geomDef != null && geomDef.ReadOnly)
                        continue;
                    this.AddProperty(def, def.Name);
                }
                this.TargetClassName = _ClassDef.Name;
            }
        }

        /// <summary>
        /// The source class definition
        /// </summary>
        public ClassDefinition SourceClassDefinition
        {
            get { return _ClassDef; }
        }

        private bool _DeleteClassData;

        /// <summary>
        /// If true, delete all data on the target before copying
        /// </summary>
        public bool DeleteClassData
        {
            get { return _DeleteClassData; }
            set { _DeleteClassData = value; }
        }
	

        /// <summary>
        /// The properties of the source class to copy
        /// </summary>
        /// <returns></returns>
        public string[] PropertyNames
        {
            get
            {
                return _PropertyMappings.AllKeys;
            }
        }

        /// <summary>
        /// Returns the mapped property name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetTargetPropertyName(string name)
        {
            return _PropertyMappings[name];
        }

        /// <summary>
        /// Returns the associated property definition
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PropertyDefinition GetPropertyDefinition(string name)
        {
            return _PropertyDefinitions[name];
        }

        /// <summary>
        /// Adds a property to be copied from the source class
        /// </summary>
        /// <param name="name">The name of the property</param>
        public void AddProperty(PropertyDefinition srcProp, string targetProp)
        {
            _PropertyMappings.Add(srcProp.Name, targetProp);
            _PropertyDefinitions.Add(srcProp.Name, srcProp);
        }
    }
}
