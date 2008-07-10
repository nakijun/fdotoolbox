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
 *      ISelect on that [source class] with the specified list of properties
 *      for each result from the returned feature reader
 *          prepare an IInsert on the [target class]
 *          add each [source property] reader value using the [target property name]
 *          execute the IInsert
 * 
 * To make the initial implementation simpler, we don't allow the creation 
 * of new classes/properties as this requires cloning feature schemas and 
 * class definitions, something that FDO currently doesn't support.
 * 
 * Creation of non-existent properties and classes will come in a later 
 * release.
 */
#endregion
namespace FdoToolbox.Core
{
    public class BulkCopyTask : ITask
    {
        private BulkCopyOptions _Options;

        public BulkCopyTask(string name, BulkCopyOptions options)
        {
            _Name = name;
            _Options = options;
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
	
        public void Execute()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            IConnection srcConn = _Options.Source.Connection;
            IConnection destConn = _Options.Target.Connection;

            ClassCollection srcClasses;
            ClassCopyOptions[] classesToCopy;
            ValidateBulkCopyOptions(srcConn, out srcClasses, out classesToCopy);
            int classesCopied = 1;
            using (IApplySchema apply = destConn.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                FeatureSchema schema = null;
                //Try to find the target schema of the given name and update it
                //otherwise create it
                using (IDescribeSchema desc = destConn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    FeatureSchemaCollection schemas = desc.Execute();
                    foreach (FeatureSchema fs in schemas)
                    {
                        if (fs.Name == _Options.TargetSchemaName)
                            schema = fs;
                    }
                    if (_Options.ApplySchemaToTarget || schema == null)
                        schema = CreateTargetSchema(_Options.SourceSchemaName, srcConn);
                    else
                        schema = UpdateTargetSchema(schema, classesToCopy);
                }
                SendMessage("Applying schema for target (this may take a while)");
                apply.FeatureSchema = schema;
                apply.Execute();
                SendMessage("Target Schema Applied");
            }
            SendMessage("Begin bulk copy of classes");
            int total = 0;
            foreach (ClassCopyOptions copyOpts in classesToCopy)
            {
                int copied = 0;
                SendMessage(string.Format("Bulk Copying class {0} of {1}: {2}", classesCopied, classesToCopy.Length, copyOpts.ClassName));
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
                        
                        //Loop through the feature reader and process each
                        //result
                        int oldpc = 0;
                        while (sourceReader.ReadNext())
                        {
                            copied += ProcessReader(classDef, insert, copyOpts, sourceReader);
                            int pc = (int)(((double)copied / (double)count) * 100);
                            //Only update progress counter when % changes
                            if(pc != oldpc)
                            {
                                oldpc = pc;
                                SendMessage(string.Format("Bulk Copying class {0} of {1}: {2} ({3}% of {4} features)", classesCopied, classesToCopy.Length, copyOpts.ClassName, oldpc, count));
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
            throw new BulkCopyException("Could not find source schema to clone: " + sourceSchemaName);
        }

        /// <summary>
        /// Validates the options supplied for this bulk copy task
        /// </summary>
        /// <param name="srcConn">The source connection</param>
        /// <param name="srcClasses">The classes to be copied</param>
        /// <param name="classesToCopy">The copy options for each specified class</param>
        private void ValidateBulkCopyOptions(IConnection srcConn, out ClassCollection srcClasses, out ClassCopyOptions[] classesToCopy)
        {
            //Validate each source class copy option specified.
            SendMessage("Validating Bulk Copy Options");

            if (string.IsNullOrEmpty(_Options.SourceSchemaName))
                throw new BulkCopyException("Source schema name not defined");
            
            if (!Array.Exists<int>(_Options.Target.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)CommandType.CommandType_ApplySchema; }))
                throw new BulkCopyException("Target connection does not support IApplySchema");
            
            //Get source schema classes
            srcClasses = null;
            using (IDescribeSchema desc = srcConn.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection srcSchemas = desc.Execute();
                foreach (FeatureSchema schema in srcSchemas)
                {
                    if (schema.Name == _Options.SourceSchemaName)
                        srcClasses = schema.Classes;
                }
            }
            if (srcClasses == null)
                throw new BulkCopyException("Unable to get classes of feature schema: " + _Options.SourceSchemaName);

            classesToCopy = _Options.GetClassCopyOptions();
            
            if (_Options.ApplySchemaToTarget)
            {
                //Include *all* classes and *all* properties
                _Options.ClearClassCopyOptions();
                foreach (ClassDefinition classDef in srcClasses)
                {
                    _Options.AddClassCopyOption(new ClassCopyOptions(classDef, true));
                }
                classesToCopy = _Options.GetClassCopyOptions();
            }

            bool checkedForDelete = false;
            foreach (ClassCopyOptions copyOpts in classesToCopy)
            {
                if (!checkedForDelete && copyOpts.DeleteClassData)
                {
                    if (!Array.Exists<int>(_Options.Target.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)CommandType.CommandType_Delete; }))
                        throw new BulkCopyException("Target connection does not support delete (IDelete)");
                    checkedForDelete = true;
                }

                string className = copyOpts.ClassName;
                int idx = srcClasses.IndexOf(className);
                if (idx < 0)
                    throw new BulkCopyException("Unable to find class " + className + " in schema " + _Options.SourceSchemaName);

                ClassDefinition classDef = srcClasses[idx];
                string[] propNames = copyOpts.PropertyNames;

                if (propNames.Length == 0)
                    throw new BulkCopyException("Nothing to copy from class " + className + " in schema " + _Options.SourceSchemaName);

                foreach (string propName in propNames)
                {
                    if (classDef.Properties.IndexOf(propName) < 0)
                        throw new BulkCopyException("Unable to find source property " + propName + " in class " + classDef.Name + " of schema " + _Options.SourceSchemaName);
                }
            }
            SendMessage("Validation Completed");
        }

        private FeatureSchema UpdateTargetSchema(FeatureSchema targetSchema, ClassCopyOptions[] classesToCopy)
        {
            SendMessage("Updating target schema");
            //for each copy option, we want to verify that the target properties
            //exist in the target class, otherwise we will create the new property
            //in that class, thereby updating it.
            foreach (ClassCopyOptions copyOpts in classesToCopy)
            {
                //See if target class exists
                int idx = targetSchema.Classes.IndexOf(copyOpts.TargetClassName);
                //Class not found. Create it
                if (idx < 0)
                {
                    ClassDefinition sourceDef = copyOpts.SourceClassDefinition;
                    ClassDefinition newDef = null;
                    switch (sourceDef.ClassType)
                    {
                        case ClassType.ClassType_Class:
                            break;
                        case ClassType.ClassType_FeatureClass:
                            break;
                        case ClassType.ClassType_NetworkClass:
                            break;
                        case ClassType.ClassType_NetworkLayerClass:
                            break;
                        case ClassType.ClassType_NetworkLinkClass:
                            break;
                        case ClassType.ClassType_NetworkNodeClass:
                            break;
                    }
                    if(newDef != null)
                        targetSchema.Classes.Add(newDef);
                }
                else //Class found. Update it
                {
                    ClassDefinition classDef = targetSchema.Classes[idx];
                    foreach (string srcPropName in copyOpts.PropertyNames)
                    {
                        string targetName = copyOpts.GetTargetPropertyName(srcPropName);
                        int pidx = classDef.Properties.IndexOf(targetName);
                        //Property not found. Create it and add to class
                        if (pidx < 0)
                        {
                            classDef.Properties.Add(copyOpts.GetPropertyDefinition(srcPropName));
                            //TODO: See if it is also an identity property
                        }
                    }
                }
                
            }
            return targetSchema;
        }


        private int ProcessReader(ClassDefinition classDef, IInsert insert, ClassCopyOptions copyOpts, IFeatureReader sourceReader)
        {
            int inserted = 0;
            string targetClass = copyOpts.TargetClassName;

            insert.SetFeatureClassName(targetClass);
            insert.PropertyValues.Clear();
            for (int i = 0; i < classDef.Properties.Count; i++)
            {
                if (!sourceReader.IsNull(classDef.Properties[i].Name))
                {
                    string target = copyOpts.GetTargetPropertyName(classDef.Properties[i].Name);
                    if (string.IsNullOrEmpty(target))
                        target = classDef.Properties[i].Name;

                    DataPropertyDefinition dataDef = classDef.Properties[i] as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = classDef.Properties[i] as GeometricPropertyDefinition;
                    if (dataDef != null)
                    {
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_BLOB:
                                insert.PropertyValues.Add(new PropertyValue(target, new BLOBValue(sourceReader.GetLOB(dataDef.Name).Data)));
                                break;
                            case DataType.DataType_Boolean:
                                insert.PropertyValues.Add(new PropertyValue(target, new BooleanValue(sourceReader.GetBoolean(dataDef.Name))));
                                break;
                            case DataType.DataType_Byte:
                                insert.PropertyValues.Add(new PropertyValue(target, new ByteValue(sourceReader.GetByte(dataDef.Name))));
                                break;
                            case DataType.DataType_CLOB:
                                insert.PropertyValues.Add(new PropertyValue(target, new CLOBValue(sourceReader.GetLOB(dataDef.Name).Data)));
                                break;
                            case DataType.DataType_DateTime:
                                insert.PropertyValues.Add(new PropertyValue(target, new DateTimeValue(sourceReader.GetDateTime(dataDef.Name))));
                                break;
                            case DataType.DataType_Decimal:
                                insert.PropertyValues.Add(new PropertyValue(target, new DecimalValue(sourceReader.GetDouble(dataDef.Name))));
                                break;
                            case DataType.DataType_Double:
                                insert.PropertyValues.Add(new PropertyValue(target, new DecimalValue(sourceReader.GetDouble(dataDef.Name))));
                                break;
                            case DataType.DataType_Int16:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int16Value(sourceReader.GetInt16(dataDef.Name))));
                                break;
                            case DataType.DataType_Int32:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int32Value(sourceReader.GetInt32(dataDef.Name))));
                                break;
                            case DataType.DataType_Int64:
                                insert.PropertyValues.Add(new PropertyValue(target, new Int64Value(sourceReader.GetInt64(dataDef.Name))));
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
                        byte[] value = sourceReader.GetGeometry(geomDef.Name);
                        insert.PropertyValues.Add(new PropertyValue(geomDef.Name, new GeometryValue(value)));
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

    public class BulkCopyOptions
    {
        private ConnectionInfo _Source;
        private ConnectionInfo _Target;

        private string _TargetSchemaName;
        private string _SourceSchemaName;
        private List<ClassCopyOptions> _SourceClasses;

        public BulkCopyOptions(ConnectionInfo source, ConnectionInfo target)
        {
            _Source = source;
            _Target = target;
            _SourceClasses = new List<ClassCopyOptions>();
        }

        public ConnectionInfo Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public ConnectionInfo Target
        {
            get { return _Target; }
            set { _Source = value; }
        }

        private bool _CopySpatialContexts;

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

        private bool _ApplySchemaToTarget;

        /// <summary>
        /// If true, the mappings are ignored and the full source schema
        /// will be applied to the target
        /// </summary>
        public bool ApplySchemaToTarget
        {
            get { return _ApplySchemaToTarget; }
            set { _ApplySchemaToTarget = value; }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public ClassCopyOptions[] GetClassCopyOptions()
        {
            return _SourceClasses.ToArray();
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
        /// Alternative constructor.
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
                    this.AddProperty(def, def.Name);
                }
                this.TargetClassName = _ClassDef.Name;
            }
        }

        public ClassDefinition SourceClassDefinition
        {
            get { return _ClassDef; }
        }

        private bool _DeleteClassData;

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
