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

namespace FdoToolbox.Core
{
    public class BulkCopyTask : ITask
    {
        public BulkCopyTask(string name, BulkCopySource src, BulkCopyTarget dest)
        {
            _Name = name;
            _Source = src;
            _Target = dest;
        }

        public event TaskProgressMessageEventHandler OnTaskMessage;

        private BulkCopyTarget _Target;

        public BulkCopyTarget Target
        {
            get { return _Target; }
            set { _Target = value; }
        }

        private BulkCopySource _Source;

        public BulkCopySource Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
	
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private void SendMessage(string msg)
        {
            if (this.OnTaskMessage != null)
                this.OnTaskMessage(msg);
        }

        private void SendCount(int count)
        {
            if (this.OnItemProcessed != null)
                this.OnItemProcessed(count);
        }

        public void Execute()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SendMessage("Preparing for bulk copy");   
            IConnection connSrc = this.Source.ConnInfo.Connection;
            IConnection connTarget = this.Target.ConnInfo.Connection;

            //Open both connections
            if (connSrc.ConnectionState != ConnectionState.ConnectionState_Open)
                connSrc.Open();
            if (connTarget.ConnectionState != ConnectionState.ConnectionState_Open)
                connTarget.Open();

            //Verify capabilities of target
            if (!IsTargetWritable())
                throw new BulkCopyException("The bulk copy target is not writable");

            //Get count
            SendMessage("Determinig number of features to copy");
            int count = this.Source.FeatureLimit < 0 ? 0 : this.Source.FeatureLimit;
            if (count == 0)
            {
                using (ISelect select = connSrc.CreateCommand(CommandType.CommandType_Select) as ISelect)
                {
                    select.SetFeatureClassName(this.Source.ClassName);
                    select.PropertyNames.Add((Identifier)Identifier.Parse(this.Source.SourcePropertyNames[0]));
                    using (IFeatureReader reader = select.Execute())
                    {
                        while (reader.ReadNext())
                        {
                            count++;
                        }
                    }
                }
            }

            //Issue ISelect on source
            SendMessage("Bulk Copying");
            int copied = 0;
            using (ISelect select = connSrc.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
            {
                select.SetFeatureClassName(this.Source.ClassName);
                if (this.Source.SourcePropertyNames.Length == 0)
                {
                    for(int i = 0; i < this.Source.SourcePropertyNames.Length; i++)
                        select.PropertyNames.Add((Identifier)Identifier.Parse(this.Source.SourcePropertyNames[i]));
                }
                using(IFeatureReader sourceReader = select.Execute())
                {
                    ClassDefinition classDef = sourceReader.GetClassDefinition();
                    IInsert insert = this.Target.ConnInfo.Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
                    if(this.Source.FeatureLimit == -1)
                    {
                        //Loop through the feature reader and process each
                        //result
                        while(sourceReader.ReadNext())
                        {
                            copied += ProcessReader(classDef, insert, sourceReader);
                            int pc = (int)(((double)copied / (double)count) * 100);
                            SendCount(pc);
                        }
                    }
                    else
                    {
                        //Loop through the feature reader and process each
                        //result
                        while(sourceReader.ReadNext())
                        {
                            if(copied < this.Source.FeatureLimit)
                            {
                                copied += ProcessReader(classDef, insert, sourceReader);
                                int pc = (int)(((double)copied / (double)count) * 100);
                                SendCount(pc);
                            }
                        }
                    }
                    insert.Dispose();
                    sourceReader.Close();
                }
            }
            watch.Stop();
            SendMessage("Bulk Copy Completed in " + watch.ElapsedMilliseconds + "ms");
            AppConsole.Alert("Bulk Copy", copied + " features copied in " + watch.ElapsedMilliseconds + "ms");
        }

        private int ProcessReader(ClassDefinition classDef, IInsert insert, IFeatureReader sourceReader)
        {
            int inserted = 0;
         
            insert.SetFeatureClassName(this.Target.ClassName);
            //PropertyValueCollection propVals = insert.PropertyValues;
            insert.PropertyValues.Clear();
            for (int i = 0; i < classDef.Properties.Count; i++ )
            { 
                string target = this.Source.GetTargetPropertyName(classDef.Properties[i].Name);
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
            IFeatureReader insertResult = insert.Execute();
            using (insertResult)
            {
                inserted++;
                insertResult.Close();
            }
            return inserted;
        }

        private bool IsTargetWritable()
        {
            IConnection connTarget = this.Target.ConnInfo.Connection;
            bool writable = false;
            writable = Array.IndexOf<int>(connTarget.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_Insert) >= 0;
            //Can delete source via IDelete or Class destruction?
            if (this.Target.DeleteBeforeCopy && Array.IndexOf<int>(connTarget.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_Delete) < 0)
                writable = connTarget.SchemaCapabilities.SupportsSchemaModification;
            return writable;
        }

        

        private bool _CoerceDataTypes;

        /// <summary>
        /// If source -> target properties are of different
        /// data types then coerce the source property data type
        /// to the target's property data type.
        /// 
        /// Data coercion is not implemented, so this property is 
        /// currently ignored.
        /// </summary>
        public bool CoerceDataTypes
        {
            get { return _CoerceDataTypes; }
            set { _CoerceDataTypes = value; }
        }

        private bool _TransformCoordinates;

        /// <summary>
        /// If the source and target geometry properties
        /// have different spatial contexts then transform
        /// the coordinate data to from the source spatial
        /// context to the target spatial context.
        /// 
        /// Coordinate Transformation is currently not
        /// implemented so this property is ignored.
        /// </summary>
        /// 
        public bool TransformCoordinates
        {
            get { return _TransformCoordinates; }
            set { _TransformCoordinates = value; }
        }

        public TaskType TaskType
        {
            get { return TaskType.BulkCopy; }
        }


        public event TaskPercentageEventHandler OnItemProcessed;
    }

    public class BulkCopyCommon
    {
        private ConnectionInfo _ConnInfo;

        public ConnectionInfo ConnInfo
        {
            get { return _ConnInfo; }
            set { _ConnInfo = value; }
        }

        private string _SchemaName;

        public string SchemaName
        {
            get { return _SchemaName; }
            set { _SchemaName = value; }
        }

        private string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        public BulkCopyCommon(ConnectionInfo cinfo, string schema, string className)
        {
            this.ConnInfo = cinfo;
            this.SchemaName = schema;
            this.ClassName = className;
        }
    }

    public class BulkCopySource : BulkCopyCommon
    {
        public BulkCopySource(ConnectionInfo cinfo, string schema, string className)
            : base(cinfo, schema, className)
        {
            _PropertyNameMappings = new NameValueCollection();
        }

        private NameValueCollection _PropertyNameMappings;

        /// <summary>
        /// The list of source properties to include in
        /// the bulk copy process. If empty, then all the
        /// properties in the source will be copied across.
        /// </summary>
        public string[] SourcePropertyNames
        {
            get { return _PropertyNameMappings.AllKeys; }
        }

        public string GetTargetPropertyName(string name)
        {
            return _PropertyNameMappings[name];
        }

        private int _FeatureLimit;

        public int FeatureLimit
        {
            get { return _FeatureLimit; }
            set { _FeatureLimit = value; }
        }

        public void AddMapping(string srcProperty, string targetProperty)
        {
            _PropertyNameMappings.Add(srcProperty, targetProperty);
        }
    }

    public class BulkCopyTarget : BulkCopyCommon
    {
        public BulkCopyTarget(ConnectionInfo cinfo, string schema, string className)
            : base(cinfo, schema, className)
        {
        }

        private bool _DeleteBeforeCopy;

        public bool DeleteBeforeCopy
        {
            get { return _DeleteBeforeCopy; }
            set { _DeleteBeforeCopy = value; }
        }
    }
}
