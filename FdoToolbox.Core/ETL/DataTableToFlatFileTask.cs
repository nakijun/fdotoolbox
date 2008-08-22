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
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using System.IO;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.Utility;
using System.Threading;
using System.Data;

namespace FdoToolbox.Core.ETL
{
    public class DataTableToFlatFileTask : TaskBase
    {
        private DataTableConversionOptions _options;

        private IConnection _conn;

        public DataTableToFlatFileTask(DataTableConversionOptions options)
        {
            _options = options;
            _conn = FeatureAccessManager.GetConnectionManager().CreateConnection(options.FdoProvider);
        }

        private string GetConnectionString()
        {
            if (_options.FdoProvider.StartsWith("OSGeo.SDF"))
                return string.Format("File={0}", _options.File);
            else if (_options.FdoProvider.StartsWith("OSGeo.SHP"))
                return string.Format("DefaultFileLocation={0}", Path.GetDirectoryName(_options.File));
            else
                throw new TaskValidationException("Provider is either not a flat-file provider or method to construct connection string is unknown");
        }

        public override void ValidateTaskParameters()
        {
            SendMessage("Validating task parameters");
            CreateFile();
            _conn.ConnectionString = GetConnectionString();
            if (string.IsNullOrEmpty(_options.ClassName))
                throw new TaskValidationException("No class name specified");
            if (string.IsNullOrEmpty(_options.SchemaName))
                throw new TaskValidationException("No schema name specified");
            if (!_options.UseFdoMetaData && Array.IndexOf<ClassType>(_conn.SchemaCapabilities.ClassTypes, ClassType.ClassType_Class) < 0)
                throw new TaskValidationException("Provider does not support creating non-feature classes");
        }

        private void CreateFile()
        {
            if (_options.FdoProvider.StartsWith("OSGeo.SDF"))
            {
                ExpressUtility.CreateSDF(_options.File);
            }
        }

        public override void DoExecute()
        {
            FgfGeometryFactory factory = new FgfGeometryFactory();
            try
            {
                System.Data.DataTable table = _options.Table;

                //Create ClassDefinition from table definition
                ClassDefinition cls = CreateClass(table);

                //Create schema for it
                FeatureSchema schema = new FeatureSchema(_options.SchemaName, "");
                schema.Classes.Add(cls);

                _conn.Open();
                using (FeatureService service = new FeatureService(_conn))
                {
                    service.ApplySchema(schema);
                }

                int count = 0;
                int pc = 0;
                int oldpc = 0;
                
                using (IInsert insert = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert)
                {
                    insert.SetFeatureClassName(_options.ClassName);

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        bool doinsert = true;
                        insert.PropertyValues.Clear();
                        foreach (System.Data.DataColumn col in table.Columns)
                        {
                            if (row[col] != null && row[col] != DBNull.Value)
                            {
                                string name = col.ColumnName;
                                if (FdoMetaData.HasMetaData(col, FdoMetaDataNames.FDO_GEOMETRY_PROPERTY))
                                {
                                    string fgfText = row[col].ToString();
                                    try
                                    {
                                        byte[] fgf = null;
                                        using (IGeometry geom = factory.CreateGeometry(fgfText))
                                        {
                                            fgf = factory.GetFgf(geom);
                                        }
                                        insert.PropertyValues.Add(new PropertyValue(name, new GeometryValue(fgf)));
                                    }
                                    catch (OSGeo.FDO.Common.Exception ex) 
                                    {
                                        //For one reason or another the FGF byte stream could not be parsed, 
                                        //so abort this insert
                                        doinsert = false;
                                        LogOffendingRow(row, ex.Message);
                                    }
                                }
                                else
                                {
                                    object obj = row[col];
                                    DataType dt = FdoMetaData.GetDataTypeForColumn(col);
                                    if (dt == DataType.DataType_Boolean)
                                        insert.PropertyValues.Add(new PropertyValue(name, new BooleanValue(Convert.ToBoolean(obj))));
                                    else if (dt == DataType.DataType_Byte)
                                        insert.PropertyValues.Add(new PropertyValue(name, new ByteValue(Convert.ToByte(obj))));
                                    else if (dt == DataType.DataType_BLOB)
                                        insert.PropertyValues.Add(new PropertyValue(name, new BLOBValue((byte[])obj)));
                                    else if (dt == DataType.DataType_DateTime)
                                        insert.PropertyValues.Add(new PropertyValue(name, new DateTimeValue(Convert.ToDateTime(obj))));
                                    else if (dt == DataType.DataType_Decimal)
                                        insert.PropertyValues.Add(new PropertyValue(name, new DecimalValue(Convert.ToDouble(obj))));
                                    else if (dt == DataType.DataType_Double)
                                        insert.PropertyValues.Add(new PropertyValue(name, new DoubleValue(Convert.ToDouble(obj))));
                                    else if (dt == DataType.DataType_Int16)
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int16Value(Convert.ToInt16(obj))));
                                    else if (dt == DataType.DataType_Int32)
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int32Value(Convert.ToInt32(obj))));
                                    else if (dt == DataType.DataType_Int64)
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int64Value(Convert.ToInt64(obj))));
                                    else if (dt == DataType.DataType_Single)
                                        insert.PropertyValues.Add(new PropertyValue(name, new SingleValue(Convert.ToSingle(obj))));
                                    else if (dt == DataType.DataType_String)
                                        insert.PropertyValues.Add(new PropertyValue(name, new StringValue(Convert.ToString(obj))));
                                }
                            }
                        }
                        if (doinsert)
                        {
                            using (IFeatureReader reader = insert.Execute())
                            {
                                reader.Close();
                                count++;
                            }

                            pc = (int)(((double)count / (double)table.Rows.Count) * 100);
                            //Only update progress counter when % changes
                            if (pc != oldpc)
                            {
                                oldpc = pc;
                                SendMessage(string.Format("Copying DataTable to class: {0} ({1}% of {2} features)", _options.ClassName, oldpc, _options.Table.Rows.Count));
                                SendCount(oldpc);
                            }
                        }
                    }
                    _conn.Close();
                }
                //Log any offending rows
                if (_OffendingRows != null && _OffendingRows.Count > 0)
                {
                    string logpath = AppGateway.RunningApplication.Preferences.GetStringPref(PreferenceNames.PREF_STR_LOG_PATH);
                    string logFile = Path.Combine(logpath, Path.GetFileName(_options.File) + ".log");
                    if (File.Exists(logFile))
                        File.Delete(logFile);
                    File.WriteAllLines(logFile, _OffendingRows.ToArray());
                    SendMessage("Copy complete. " + _OffendingRows.Count + " rows could not be copied. See " + logFile + " for more information.");
                }
                else
                {
                    SendMessage("Copy complete");
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                throw ex;
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            finally
            {
                factory.Dispose();
                _conn.Dispose();
            }
        }

        private List<string> _OffendingRows;

        private void LogOffendingRow(System.Data.DataRow row, string message)
        {
            if (_OffendingRows == null)
            {
                _OffendingRows = new List<string>();
            }
            List<string> idNames = new List<string>();
            string geomProperty = null;
            //Get id properties
            foreach (DataColumn col in row.Table.Columns)
            {
                if (FdoMetaData.HasMetaData(col, FdoMetaDataNames.FDO_IDENTITY_PROPERTY))
                    idNames.Add(col.ColumnName);
                else if (FdoMetaData.HasMetaData(col, FdoMetaDataNames.FDO_GEOMETRY_PROPERTY))
                    geomProperty = col.ColumnName;
            }
            if (idNames.Count > 0 && !string.IsNullOrEmpty(geomProperty))
            {
                StringBuilder msg = new StringBuilder("Unable to write row: [" + message + "]\n\t");
                foreach (string idprop in idNames)
                {
                    msg.Append("Identity Property (" + idprop + "): " + row[idprop] + "\n\t");
                }
                msg.Append("Geometry Property (" + geomProperty + "): " + row[geomProperty] + "\n\n");
                _OffendingRows.Add(msg.ToString());
            }
        }

        private FeatureClass CreateClass(System.Data.DataTable table)
        {
            FeatureClass fc = new FeatureClass(_options.ClassName, "");
            if (_options.UseFdoMetaData)
            {
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    if (col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY] != null
                     && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_TYPE] != null
                     && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_ELEVATION] != null
                     && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_MEASURE] != null
                     && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_READONLY] != null)
                    {
                        GeometricPropertyDefinition gp = new GeometricPropertyDefinition(col.ColumnName, col.Caption);
                        gp.GeometryTypes = Convert.ToInt32(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_TYPE]);
                        gp.HasElevation = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_ELEVATION]);
                        gp.HasMeasure = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_HAS_MEASURE]);
                        gp.ReadOnly = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_READONLY]);
                        fc.Properties.Add(gp);
                    }
                    else
                    {
                        DataPropertyDefinition dp = new DataPropertyDefinition(col.ColumnName, col.Caption);
                        dp.IsAutoGenerated = FdoMetaData.IsAutoGenerated(col);
                        dp.Nullable = FdoMetaData.IsNullable(col);
                        dp.ReadOnly = FdoMetaData.IsReadOnly(col);
                        dp.DataType = FdoMetaData.GetDataTypeForColumn(col);
                        if (dp.DataType == DataType.DataType_String
                        || dp.DataType == DataType.DataType_BLOB
                        || dp.DataType == DataType.DataType_CLOB)
                            dp.Length = FdoMetaData.GetLength(col);
                        if (dp.DataType == DataType.DataType_Decimal)
                        {
                            dp.Scale = FdoMetaData.GetScale(col);
                            dp.Precision = FdoMetaData.GetPrecision(col);
                        }
                        if (FdoMetaData.HasMetaData(col, FdoMetaDataNames.FDO_DATA_DEFAULT_VALUE))
                            dp.DefaultValue = FdoMetaData.GetDefaultValue(col);

                        fc.Properties.Add(dp);
                        if (FdoMetaData.IsIdentityProperty(col))
                            fc.IdentityProperties.Add(dp);
                    }
                }
            }
            else
            {
                Class cls = new Class(_options.ClassName, "");
                //Infer from DataColumns
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    DataPropertyDefinition dp = new DataPropertyDefinition(col.ColumnName, col.Caption);
                    dp.IsAutoGenerated = col.AutoIncrement;
                    dp.Nullable = col.AllowDBNull;
                    dp.ReadOnly = col.ReadOnly;
                    dp.DataType = GetDataType(col.DataType);
                    if (dp.DataType == DataType.DataType_BLOB || dp.DataType == DataType.DataType_CLOB || dp.DataType == DataType.DataType_String)
                        dp.Length = col.MaxLength;
                    if (dp.DataType == DataType.DataType_String)
                        dp.DefaultValue = col.DefaultValue.ToString();
                    if (dp.DataType == DataType.DataType_Decimal)
                    {
                        //DataColumn does not have this information so use the maximum supported value
                        dp.Scale = _conn.SchemaCapabilities.MaximumDecimalScale;
                        dp.Precision = _conn.SchemaCapabilities.MaximumDecimalPrecision;
                    }
                    cls.Properties.Add(dp);
                    if (Array.IndexOf<System.Data.DataColumn>(table.PrimaryKey, col) >= 0)
                        cls.IdentityProperties.Add(dp);
                }
            }
            return fc;
        }

        private DataType GetDataType(Type type)
        {
            if (type == typeof(bool))
                return DataType.DataType_Boolean;
            else if (type == typeof(byte))
                return DataType.DataType_Byte;
            else if (type == typeof(byte[]))
                return DataType.DataType_BLOB;
            else if (type == typeof(char[]))
                return DataType.DataType_CLOB;
            else if (type == typeof(DateTime))
                return DataType.DataType_DateTime;
            else if (type == typeof(decimal))
                return DataType.DataType_Decimal;
            else if (type == typeof(double))
                return DataType.DataType_Double;
            else if (type == typeof(short))
                return DataType.DataType_Int16;
            else if (type == typeof(int))
                return DataType.DataType_Int32;
            else if (type == typeof(long))
                return DataType.DataType_Int64;
            else if (type == typeof(float))
                return DataType.DataType_Single;
            else if (type == typeof(string))
                return DataType.DataType_String;
            else
                throw new ArgumentException("Unable to get matching DataType for type: " + type);
        }

        public override TaskType TaskType
        {
            get { return TaskType.DataTableToFeatureClass; }
        }

        public override bool IsCountable
        {
            get { return true; }
        }
    }
}
