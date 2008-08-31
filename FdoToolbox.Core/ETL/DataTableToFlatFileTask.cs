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
using FdoToolbox.Core.Common;

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
                FdoDataTable table = _options.Table;
                _conn.Open();
                using (FeatureService service = new FeatureService(_conn))
                {
                    ClassDefinition cls = (table as FdoDataTable).GetClassDefinition();

                    //Create schema for it
                    FeatureSchema schema = new FeatureSchema(_options.SchemaName, "");
                    schema.Classes.Add(cls);

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
                                if (col is FdoGeometryColumn)
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
                                else if (col is FdoDataColumn)
                                {
                                    FdoDataColumn dc = col as FdoDataColumn;
                                    object obj = row[col];
                                    DataType dt = dc.GetDataType();
                                    switch(dt)
                                    {
                                        case DataType.DataType_Boolean:
                                            insert.PropertyValues.Add(new PropertyValue(name, new BooleanValue(Convert.ToBoolean(obj))));
                                            break;
                                        case DataType.DataType_Byte:
                                            insert.PropertyValues.Add(new PropertyValue(name, new ByteValue(Convert.ToByte(obj))));
                                            break;
                                        case DataType.DataType_BLOB:
                                            insert.PropertyValues.Add(new PropertyValue(name, new BLOBValue((byte[])obj)));
                                            break;
                                        case DataType.DataType_DateTime:
                                            insert.PropertyValues.Add(new PropertyValue(name, new DateTimeValue(Convert.ToDateTime(obj))));
                                            break;
                                        case DataType.DataType_Decimal:
                                            insert.PropertyValues.Add(new PropertyValue(name, new DecimalValue(Convert.ToDouble(obj))));
                                            break;
                                        case DataType.DataType_Double:
                                            insert.PropertyValues.Add(new PropertyValue(name, new DoubleValue(Convert.ToDouble(obj))));
                                            break;
                                        case DataType.DataType_Int16:
                                            insert.PropertyValues.Add(new PropertyValue(name, new Int16Value(Convert.ToInt16(obj))));
                                            break;
                                        case DataType.DataType_Int32:
                                            insert.PropertyValues.Add(new PropertyValue(name, new Int32Value(Convert.ToInt32(obj))));
                                            break;
                                        case DataType.DataType_Int64:
                                            insert.PropertyValues.Add(new PropertyValue(name, new Int64Value(Convert.ToInt64(obj))));
                                            break;
                                        case DataType.DataType_Single:
                                            insert.PropertyValues.Add(new PropertyValue(name, new SingleValue(Convert.ToSingle(obj))));
                                            break;
                                        case DataType.DataType_String:
                                            insert.PropertyValues.Add(new PropertyValue(name, new StringValue(Convert.ToString(obj))));
                                            break;
                                    }
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
            catch (OSGeo.FDO.Common.Exception)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            finally
            {
                factory.Dispose();
                _conn.Close();
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
            //Get id properties
            foreach (DataColumn col in row.Table.PrimaryKey)
            {
                idNames.Add(col.ColumnName);
            }
            if (idNames.Count > 0)
            {
                StringBuilder msg = new StringBuilder("Unable to write row: [" + message + "]\n\t");
                foreach (string idprop in idNames)
                {
                    msg.Append("Identity Property (" + idprop + "): " + row[idprop] + "\n\t");
                }
                //msg.Append("Geometry Property (" + geomProperty + "): " + row[geomProperty] + "\n\n");
                _OffendingRows.Add(msg.ToString());
            }
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
