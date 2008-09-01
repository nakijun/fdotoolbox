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
using System.Collections.Generic;
using System.Text;
using System.Data;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using System.Threading;
using FdoToolbox.Core.Utility;
#region overview
/**
 * Spatial Join overview
 * 
 * A spatial join task is initialized with a SpatialJoinOptions object that
 * defines the parameters of the spatial join operation. The main join
 * parameters are:
 * 
 * - The "Left" FDO connection
 * - The "Right" database connection
 * - The "Target" FDO connection (this can be the "Left" connection)
 * - What type of join (Left Outer | Inner)
 * - What to do when there is a match (1:1 | 1:m)
 * - The properties/columns to read on both sides.
 * - The properties/columns to join on
 * - The class to create on the target connection.
 * - Prefixes to prevent property/column name clashes.
 * 
 * When the join executes, data from the "Right" connection is read into
 * a DataTable. Table definition information is also stored in this DataTable.
 * 
 * Data from the "Left" connection is a standard FDO ISelect query, producing
 * an IFeatureReader.
 * 
 * Using the column definitions from the "Right" DataTable and the property 
 * definitions of the "Left" IFeatureReader's class definition. We create the
 * joined feature class on the "Target" connection. 
 * 
 * Then we start iterating through the "Left" source. Processing each
 * result with the given join rules.
 */
#endregion
namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Task that joins/merges a spatial data source with a non-spatial data source
    /// </summary>
    public class SpatialJoinTask : TaskBase, IDisposable
    {
        private SpatialJoinOptions _Options;

        /// <summary>
        /// The options that drive the join task
        /// </summary>
        public SpatialJoinOptions Options
        {
            get { return _Options; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The spatial join options</param>
        public SpatialJoinTask(SpatialJoinOptions options)
        {
            _Options = options;
        }

        /// <summary>
        /// Validates the join task parameters
        /// </summary>
        public override void ValidateTaskParameters()
        {
            // The following must be true in order to proceed:
            //
            // 1 - Target must support IApplySchema
            // 2 - Target class must not be blank
            // 3 - Target class must not already exist
            // 4 - If no prefixes are defined, there must be no clashes in property/column names

            using (FeatureService targetService = new FeatureService(_Options.Target.InternalConnection))
            {
                if (!targetService.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema))
                    throw new TaskValidationException("Target connection does not support IApplySchema");

                if (string.IsNullOrEmpty(_Options.TargetClassName))
                {
                    throw new TaskValidationException("Target class name is not defined");
                }
                else
                {
                    ClassDefinition classDef = targetService.GetClassByName(_Options.TargetSchema, _Options.TargetClassName);
                    if (classDef != null)
                        throw new TaskValidationException("Cannot create joined class. A class named " + _Options.TargetClassName + " already exists");
                }
            }

            if (string.IsNullOrEmpty(_Options.SecondaryPrefix))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (string str in _Options.GetPropertyNames())
                {
                    dict.Add(str, "");
                }
                foreach (string str in _Options.GetColumnNames())
                {
                    if (dict.ContainsKey(str))
                        throw new TaskValidationException("There is a name clash between at least one column/property");
                }
            }
        }

        private ClassDefinition _PrimaryClass;

        public override void DoExecute()
        {
            OSGeo.FDO.Commands.Feature.IFeatureReader primaryReader = null;
            System.Data.DataTable secondaryTable = null;
            IInsert insertCmd = null;
            try
            {
                //TODO: Use data readers on both sides of the join instead of loading
                //the secondary source into a potentially big DataTable. Of course that
                //would change the whole implementation, but it would/should be more
                //efficient.

                //Read both sources
                SendMessage("Reading primary source");
                primaryReader = LoadPrimarySource();
                SendMessage("Reading secondary source");
                secondaryTable = LoadSecondarySource();
                SendMessage("Creating joined feature class");
                CreateJoinedFeatureClass(primaryReader, secondaryTable);

                string[] joinedColumns = _Options.GetJoinedColumns();
                string[] joinedProperties = _Options.GetJoinedProperties();

                int inserted = 0;

                insertCmd = _Options.Target.InternalConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
                insertCmd.SetFeatureClassName(_Options.TargetClassName);
                SendMessage("Joining...");
                using (primaryReader)
                using (secondaryTable)
                using (insertCmd)
                {
                    while (primaryReader.ReadNext())
                    {
                        //Find matches
                        string expr = BuildSecondarySearchExpression(primaryReader, joinedProperties);
                        DataRow[] matchedRows = secondaryTable.Select(expr);
                        //No matches
                        if (matchedRows.Length == 0)
                        {
                            if (_Options.JoinType == SpatialJoinType.Inner)
                                continue;
                            else if (_Options.JoinType == SpatialJoinType.LeftOuter)
                                inserted += DoJoinedInsert(insertCmd, primaryReader, null);
                        }
                        else //There is at least one match
                        {
                            // 1:1 perform merged insert with first matching row
                            if (_Options.Cardinality == SpatialJoinCardinality.OneToOne)
                            {
                                inserted += DoJoinedInsert(insertCmd, primaryReader, matchedRows[0]);
                            }
                            // 1:m perform merged insert with each matching row
                            else if (_Options.Cardinality == SpatialJoinCardinality.OneToMany)
                            {
                                foreach (DataRow matchedRow in matchedRows)
                                {
                                    inserted += DoJoinedInsert(insertCmd, primaryReader, matchedRow);
                                }
                            }
                        }
                    }
                    primaryReader.Close();
                }

                SendMessage(inserted + " features processed");
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            finally
            {
                if (primaryReader != null)
                    primaryReader.Dispose();
                if (secondaryTable != null)
                    secondaryTable.Dispose();
                if (insertCmd != null)
                    insertCmd.Dispose();
            }
        }

        /// <summary>
        /// Constructs the search expression to be used on the secondary table
        /// </summary>
        /// <param name="primaryReader"></param>
        /// <param name="joinedProperties"></param>
        /// <returns></returns>
        private string BuildSecondarySearchExpression(OSGeo.FDO.Commands.Feature.IFeatureReader primaryReader, string[] joinedProperties)
        {
            //Construct secondary search expression
            List<string> conditions = new List<string>();
            foreach (string propName in joinedProperties)
            {
                string col = _Options.GetMatchingColumn(propName);
                if (!string.IsNullOrEmpty(_Options.SecondaryPrefix))
                    col = _Options.SecondaryPrefix + col;
                object value = ReadSourceValue(propName, primaryReader);
                if(value != null)
                    conditions.Add(col + " = '" + EscapeString(value.ToString()) + "'");
                else
                    conditions.Add(col + " IS NULL");
            }
            string expr = string.Join(" AND ", conditions.ToArray());
            return expr;
        }

        private static string EscapeString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.Replace("'", "''");
            return str;

        }

        /// <summary>
        /// Creates the feature class that the merged inserts will insert into
        /// </summary>
        /// <param name="primaryReader"></param>
        /// <param name="secondaryTable"></param>
        private void CreateJoinedFeatureClass(OSGeo.FDO.Commands.Feature.IFeatureReader primaryReader, DataTable secondaryTable)
        {
            _PrimaryClass = primaryReader.GetClassDefinition();
            ClassDefinition targetClass = FeatureService.CloneClass(_PrimaryClass);
            targetClass.Name = _Options.TargetClassName;

            //Now add secondary columns to it
            foreach (DataColumn col in secondaryTable.Columns)
            {
                DataPropertyDefinition propDef = new DataPropertyDefinition(col.ColumnName, "");
                propDef.ReadOnly = col.ReadOnly;
                propDef.Nullable = true; //To cater for Left Outer joins, this *must* be true
                if (col.DefaultValue != null)
                    propDef.DefaultValue = col.DefaultValue.ToString();
                if (col.DataType == typeof(byte[]))
                {
                    propDef.DataType = DataType.DataType_BLOB;
                    propDef.Length = col.MaxLength;
                }
                else if (col.DataType == typeof(bool))
                    propDef.DataType = DataType.DataType_Boolean;
                else if (col.DataType == typeof(byte))
                    propDef.DataType = DataType.DataType_Byte;
                else if (col.DataType == typeof(char[]))
                {
                    propDef.DataType = DataType.DataType_CLOB;
                    propDef.Length = col.MaxLength;
                }
                else if (col.DataType == typeof(DateTime))
                    propDef.DataType = DataType.DataType_DateTime;
                else if (col.DataType == typeof(Decimal))
                    propDef.DataType = DataType.DataType_Decimal;
                else if (col.DataType == typeof(double))
                    propDef.DataType = DataType.DataType_Double;
                else if (col.DataType == typeof(Int16))
                    propDef.DataType = DataType.DataType_Int16;
                else if (col.DataType == typeof(Int32))
                    propDef.DataType = DataType.DataType_Int32;
                else if (col.DataType == typeof(Int64))
                    propDef.DataType = DataType.DataType_Int64;
                else if (col.DataType == typeof(Single))
                    propDef.DataType = DataType.DataType_Single;
                else if (col.DataType == typeof(string))
                {
                    propDef.DataType = DataType.DataType_String;
                    propDef.Length = col.MaxLength;
                }
                else
                    throw new SpatialJoinException("Unknown/unsupported secondary table column type " + col.DataType);

                targetClass.Properties.Add(propDef);
            }
            if (targetClass.IdentityProperties.Count == 0)
            {
                //Add an auto-generated id just to be safe
                DataPropertyDefinition autoId = new DataPropertyDefinition("AutoID", "Auto-generated ID");
                autoId.IsAutoGenerated = true;
                autoId.ReadOnly = true;
                autoId.Nullable = false;
                autoId.DataType = DataType.DataType_Int32;

                targetClass.Properties.Add(autoId);
                targetClass.IdentityProperties.Add(autoId);
            }

            //Now add class to target schema and apply it
            using (FeatureService service = new FeatureService(_Options.Target.InternalConnection))
            {
                FeatureSchema schema = service.GetSchemaByName(_Options.TargetSchema);
                if (schema != null)
                {
                    schema.Classes.Add(targetClass);
                }
                else
                {
                    schema = new FeatureSchema(_Options.TargetSchema, "");
                    schema.Classes.Add(targetClass);
                }
                service.ApplySchema(schema);
            }
        }
        
        /// <summary>
        /// Performs the joined insert
        /// </summary>
        /// <param name="insertCmd"></param>
        /// <param name="primaryReader"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private int DoJoinedInsert(IInsert insertCmd, OSGeo.FDO.Commands.Feature.IFeatureReader primaryReader, DataRow row)
        {
            insertCmd.PropertyValues.Clear();
            //Process primary properties
            foreach (PropertyDefinition propDef in _PrimaryClass.Properties)
            {
                string propName = propDef.Name;
                DataPropertyDefinition dataDef = propDef as DataPropertyDefinition;
                GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                if (!primaryReader.IsNull(propName))
                {
                    PropertyValue propVal = null;
                    if (dataDef != null)
                    {   
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_BLOB:
                                propVal = new PropertyValue(propName, new BLOBValue(primaryReader.GetLOB(propName).Data));
                                break;
                            case DataType.DataType_Boolean:
                                propVal = new PropertyValue(propName, new BooleanValue(primaryReader.GetBoolean(propName)));
                                break;
                            case DataType.DataType_Byte:
                                propVal = new PropertyValue(propName, new ByteValue(primaryReader.GetByte(propName)));
                                break;
                            case DataType.DataType_CLOB:
                                propVal = new PropertyValue(propName, new CLOBValue(primaryReader.GetLOB(propName).Data));
                                break;
                            case DataType.DataType_DateTime:
                                propVal = new PropertyValue(propName, new DateTimeValue(primaryReader.GetDateTime(propName)));
                                break;
                            case DataType.DataType_Decimal:
                                propVal = new PropertyValue(propName, new DecimalValue(primaryReader.GetDouble(propName)));
                                break;
                            case DataType.DataType_Double:
                                propVal = new PropertyValue(propName, new DecimalValue(primaryReader.GetDouble(propName)));
                                break;
                            case DataType.DataType_Int16:
                                propVal = new PropertyValue(propName, new Int16Value(primaryReader.GetInt16(propName)));
                                break;
                            case DataType.DataType_Int32:
                                propVal = new PropertyValue(propName, new Int32Value(primaryReader.GetInt32(propName)));
                                break;
                            case DataType.DataType_Int64:
                                propVal = new PropertyValue(propName, new Int64Value(primaryReader.GetInt64(propName)));
                                break;
                            case DataType.DataType_Single:
                                propVal = new PropertyValue(propName, new SingleValue(primaryReader.GetSingle(propName)));
                                break;
                            case DataType.DataType_String:
                                propVal = new PropertyValue(propName, new StringValue(primaryReader.GetString(propName)));
                                break;
                        }
                    }
                    else if (geomDef != null)
                    {
                        propVal = new PropertyValue(propName, new GeometryValue(primaryReader.GetGeometry(propName)));
                    }
                    if (propVal != null)
                        insertCmd.PropertyValues.Add(propVal);
                }
            }
            //Now process secondary columns
            if (row != null)
            {
                foreach (DataColumn col in row.Table.Columns)
                {
                    string propName = col.ColumnName;
                    if (row[col] != null && row[col] != DBNull.Value)
                    {
                        PropertyValue propVal = null;
                        if (col.DataType == typeof(byte[]))
                            propVal = new PropertyValue(propName, new BLOBValue(primaryReader.GetLOB(propName).Data));
                        else if (col.DataType == typeof(bool))
                            propVal = new PropertyValue(propName, new BooleanValue(Convert.ToBoolean(row[col])));
                        else if (col.DataType == typeof(byte))
                            propVal = new PropertyValue(propName, new ByteValue(Convert.ToByte(row[col])));
                        else if (col.DataType == typeof(char[]))
                            propVal = new PropertyValue(propName, new CLOBValue((byte[])row[col]));
                        else if (col.DataType == typeof(DateTime))
                            propVal = new PropertyValue(propName, new DateTimeValue(Convert.ToDateTime(row[col])));
                        else if (col.DataType == typeof(Decimal))
                            propVal = new PropertyValue(propName, new DecimalValue(Convert.ToDouble(row[col])));
                        else if (col.DataType == typeof(double))
                            propVal = new PropertyValue(propName, new DoubleValue(Convert.ToDouble(row[col])));
                        else if (col.DataType == typeof(Int16))
                            propVal = new PropertyValue(propName, new Int16Value(Convert.ToInt16(row[col])));
                        else if (col.DataType == typeof(Int32))
                            propVal = new PropertyValue(propName, new Int32Value(Convert.ToInt32(row[col])));
                        else if (col.DataType == typeof(Int64))
                            propVal = new PropertyValue(propName, new Int64Value(Convert.ToInt64(row[col])));
                        else if (col.DataType == typeof(Single))
                            propVal = new PropertyValue(propName, new SingleValue(Convert.ToSingle(row[col])));
                        else if (col.DataType == typeof(string))
                            propVal = new PropertyValue(propName, new StringValue(row[col].ToString()));
                        else
                            throw new SpatialJoinException("Unknown/unsupported secondary table column type " + col.DataType);

                        if (propVal != null)
                            insertCmd.PropertyValues.Add(propVal);
                    }
                }
            }

            //Now insert
            int inserted = 0;
            using (IFeatureReader reader = insertCmd.Execute())
            {
                reader.Close();
                inserted++;
            }
            return inserted;
        }

        /// <summary>
        /// Reads the source property value from the feature reader
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="primaryReader"></param>
        /// <returns></returns>
        private object ReadSourceValue(string propName, OSGeo.FDO.Commands.Feature.IFeatureReader primaryReader)
        {
            object retVal = null;
            int idx = _PrimaryClass.Properties.IndexOf(propName);
            if (idx >= 0)
            {
                DataPropertyDefinition propDef = _PrimaryClass.Properties[idx] as DataPropertyDefinition;
                if (propDef != null)
                {
                    switch (propDef.DataType)
                    {
                        case DataType.DataType_BLOB:
                            retVal = primaryReader.GetLOB(propName).Data;
                            break;
                        case DataType.DataType_Boolean:
                            retVal = primaryReader.GetBoolean(propName);
                            break;
                        case DataType.DataType_Byte:
                            retVal = primaryReader.GetByte(propName);
                            break;
                        case DataType.DataType_CLOB:
                            retVal = primaryReader.GetLOB(propName).Data;
                            break;
                        case DataType.DataType_DateTime:
                            retVal = primaryReader.GetDateTime(propName);
                            break;
                        case DataType.DataType_Decimal:
                            retVal = primaryReader.GetDouble(propName);
                            break;
                        case DataType.DataType_Double:
                            retVal = primaryReader.GetDouble(propName);
                            break;
                        case DataType.DataType_Int16:
                            retVal = primaryReader.GetInt16(propName);
                            break;
                        case DataType.DataType_Int32:
                            retVal = primaryReader.GetInt32(propName);
                            break;
                        case DataType.DataType_Int64:
                            retVal = primaryReader.GetInt64(propName);
                            break;
                        case DataType.DataType_Single:
                            retVal = primaryReader.GetSingle(propName);
                            break;
                        case DataType.DataType_String:
                            retVal = primaryReader.GetString(propName);
                            break;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Loads the primary feature source
        /// </summary>
        /// <returns></returns>
        private OSGeo.FDO.Commands.Feature.IFeatureReader LoadPrimarySource()
        {
            ISelect select = _Options.PrimarySource.InternalConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect;
            select.SetFeatureClassName(_Options.ClassName);
            select.PropertyNames.Clear();

            string[] propertyNames = _Options.GetPropertyNames();
            string[] joinedProperties = _Options.GetJoinedProperties();

            if (!string.IsNullOrEmpty(_Options.PrimaryPrefix))
            {
                foreach (string propName in propertyNames)
                {
                    select.PropertyNames.Add(new ComputedIdentifier(_Options.PrimaryPrefix + propName, Expression.Parse(propName)));
                }

                foreach (string propName in joinedProperties)
                {
                    ComputedIdentifier id = new ComputedIdentifier(_Options.PrimaryPrefix + propName, Expression.Parse(propName));
                    if (!select.PropertyNames.Contains(id))
                        select.PropertyNames.Add(id); 
                }
            }
            else
            {
                foreach (string propName in propertyNames)
                {
                    select.PropertyNames.Add((Identifier)Identifier.Parse(propName));
                }

                foreach (string propName in joinedProperties)
                {
                    Identifier id = (Identifier)Identifier.Parse(propName);
                    if (!select.PropertyNames.Contains(id))
                        select.PropertyNames.Add(id);
                }
            }
            return select.Execute();
        }

        /// <summary>
        /// Loads the secondary data source
        /// </summary>
        /// <returns></returns>
        private DataTable LoadSecondarySource()
        {
            IDbConnection conn = _Options.SecondarySource.Connection;
            string[] joinColumns = _Options.GetJoinedColumns();
            string [] columns = _Options.GetColumnNames();
            columns = ExpressUtility.CombineArray<string>(columns, joinColumns);
            if (!string.IsNullOrEmpty(_Options.SecondaryPrefix))
            {
                List<string> aliases = new List<string>();
                foreach (string col in columns)
                {
                    string alias = col + " AS " + (_Options.SecondaryPrefix) + col;
                    if(!aliases.Contains(alias))
                        aliases.Add(alias);
                }
                columns = aliases.ToArray();
            }
            string sql = string.Format("SELECT {0} FROM {1} ORDER BY {2}", string.Join(", ", columns), _Options.TableName, string.Join(", ", joinColumns));
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DataTable table = new DataTable();
            System.Data.IDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
            return table;
        }

        public override TaskType TaskType
        {
            get { return TaskType.DbJoin; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_PrimaryClass != null)
                    _PrimaryClass.Dispose();
            }
        }

        public override bool IsCountable
        {
            get { return false; }
        }
    }
}
