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
using System.Windows.Forms;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Commands.Schema;
using FdoToolbox.Core.Forms;
using System.Collections.Specialized;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Utility class to supplement the Express Extension Module
    /// </summary>
    public class ExpressUtility
    {
        public const string PROVIDER_SDF = "OSGeo.SDF";
        public const string PROVIDER_SHP = "OSGeo.SHP";

        public const string CONN_FMT_SDF = "File={0}";
        public const string CONN_FMT_SHP = "DefaultFileLocation={0}";

        public static IConnection CreateSDFConnection(string sdfFile, bool readOnly)
        {
            string connStr = string.Format("File={0};ReadOnly={1}", sdfFile, readOnly.ToString().ToUpper());
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(PROVIDER_SDF);
            conn.ConnectionString = connStr;
            return conn;
        }

        public static IConnection CreateSHPConnection(string path)
        {
            string connStr = string.Format("DefaultFileLocation={0}", path);
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(PROVIDER_SHP);
            conn.ConnectionString = connStr;
            return conn;
        }

        /// <summary>
        /// Applies a feature schema definition to an existing sdf file
        /// </summary>
        /// <param name="schemaFile"></param>
        /// <param name="sdfFile"></param>
        public static void ApplySchemaToSDF(string schemaFile, string sdfFile)
        {
            IConnection conn = CreateSDFConnection(sdfFile, false);
            conn.Open();
            using (conn)
            {
                using (FeatureService service = new FeatureService(conn))
                {
                    service.LoadSchemasFromXml(schemaFile);
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Applies an in-memory feature schema to a new sdf file
        /// </summary>
        /// <param name="selectedSchema"></param>
        /// <param name="sdfFile"></param>
        public static IConnection ApplySchemaToNewSDF(FeatureSchema selectedSchema, string sdfFile)
        {
            if (ExpressUtility.CreateSDF(sdfFile))
            {   
                IConnection conn = ExpressUtility.CreateSDFConnection(sdfFile, false);

                try
                {
                    conn.Open();
                    FeatureService service = new FeatureService(conn);
                    service.ApplySchema(FeatureService.CloneSchema(selectedSchema));

                    return conn;
                }
                catch (Exception ex)
                {
                    conn.Dispose();
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Unable to create SDF file at " + sdfFile);
            }
        }

        public static bool CreateSDF(string fileName)
        {
            try
            {
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }
            catch (System.IO.IOException ex)
            {
                AppConsole.WriteException(ex);
                return false;
            }
            bool result = false;
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SDF");
            using (conn)
            {
                using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    try
                    {
                        cmd.DataStoreProperties.SetProperty("File", fileName);
                        cmd.Execute();
                        result = true;
                    }
                    catch (OSGeo.FDO.Common.Exception)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Serializes a DataTable to a new SDF file. In order for certain columns to be picked
        /// up as Geometry Properties or Identity Properties, they must be tagged with the correct
        /// metadata. For other properties, the standard column properties will be used to construct
        /// the class properties.
        /// 
        /// If the columns are not property tagged, they will be treated as string properties.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sdfFile"></param>
        public static void WriteDataTableToSdf(System.Data.DataTable table, string sdfFile)
        {
            if (string.IsNullOrEmpty(table.TableName))
                throw new ArgumentException("The given DataTable object has no table name");

            //Create ClassDefinition from table definition
            FeatureClass fc = new FeatureClass(table.TableName);
            foreach (System.Data.DataColumn col in table.Columns)
            {
                if (col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY] != null
                 && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_TYPE] != null
                 && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_HAS_ELEVATION] != null
                 && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_HAS_MEASURE] != null
                 && col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_READONLY] != null)
                {
                    GeometricPropertyDefinition gp = new GeometricPropertyDefinition(col.ColumnName, col.Caption);
                    gp.GeometryTypes = Convert.ToInt32(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_TYPE]);
                    gp.HasElevation = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_HAS_ELEVATION]);
                    gp.HasMeasure = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_HAS_MEASURE]);
                    gp.ReadOnly = Convert.ToBoolean(col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY_READONLY]);
                    fc.Properties.Add(gp);
                }
                else
                {
                    DataPropertyDefinition dp = new DataPropertyDefinition(col.ColumnName, col.Caption);
                    dp.IsAutoGenerated = col.AutoIncrement;
                    dp.Nullable = col.AllowDBNull;
                    dp.ReadOnly = col.ReadOnly;

                    if (col.DataType == typeof(bool))
                        dp.DataType = DataType.DataType_Boolean;
                    else if (col.DataType == typeof(byte))
                        dp.DataType = DataType.DataType_Byte;
                    else if (col.DataType == typeof(byte[]))
                    {
                        dp.DataType = DataType.DataType_BLOB;
                        dp.Length = col.MaxLength;
                    }
                    else if (col.DataType == typeof(DateTime))
                        dp.DataType = DataType.DataType_DateTime;
                    else if (col.DataType == typeof(decimal))
                        dp.DataType = DataType.DataType_Decimal;
                    else if (col.DataType == typeof(double))
                        dp.DataType = DataType.DataType_Double;
                    else if (col.DataType == typeof(short))
                        dp.DataType = DataType.DataType_Int16;
                    else if (col.DataType == typeof(int))
                        dp.DataType = DataType.DataType_Int32;
                    else if (col.DataType == typeof(long))
                        dp.DataType = DataType.DataType_Int64;
                    else if (col.DataType == typeof(float))
                        dp.DataType = DataType.DataType_Single;
                    else if (col.DataType == typeof(string))
                    {
                        dp.DataType = DataType.DataType_String;
                        dp.DefaultValue = col.DefaultValue;
                        dp.Length = col.MaxLength;
                    }
                    else
                        throw new Exception("Unsupported data type: " + col.DataType);

                    fc.Properties.Add(dp);
                    if (col.ExtendedProperties[FdoMetaDataNames.FDO_IDENTITY_PROPERTY] != null || (table.PrimaryKey != null && Array.IndexOf<System.Data.DataColumn>(table.PrimaryKey, col)))
                    {
                        fc.IdentityProperties.Add(dp);
                    }
                }
            }

            //Create schema for it
            FeatureSchema schema = new FeatureSchema("Default", "Default Schema");
            schema.Classes.Add(fc);

            IConnection conn = ApplySchemaToNewSDF(schema, sdfFile);
            conn.Open();

            FgfGeometryFactory factory = new FgfGeometryFactory();
            using (factory)
            using (conn)
            {
                using (IInsert insert = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert)
                {
                    insert.SetFeatureClassName(table.TableName);

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        foreach (System.Data.DataColumn col in table.Columns)
                        {
                            if (row[col] != null)
                            {
                                string name = col.ColumnName;
                                if (col.ExtendedProperties[FdoMetaDataNames.FDO_GEOMETRY_PROPERTY] != null)
                                {
                                    string fgfText = row[col];
                                    byte[] fgf = null;
                                    using (IGeometry geom = factory.CreateGeometry(fgfText))
                                    {
                                        fgf = factory.GetFgf(geom);
                                    }
                                    insert.PropertyValues.Add(new PropertyValue(name, new GeometryValue(fgf)));
                                }
                                else
                                {
                                    object obj = row[col];

                                    if (col.DataType == typeof(bool))
                                        insert.PropertyValues.Add(new PropertyValue(name, new BooleanValue(Convert.ToBoolean(obj))));
                                    else if (col.DataType == typeof(byte))
                                        insert.PropertyValues.Add(new PropertyValue(name, new ByteValue(Convert.ToByte(obj))));
                                    else if (col.DataType == typeof(byte[]))
                                        insert.PropertyValues.Add(new PropertyValue(name, new BLOBValue((byte[])obj)));
                                    else if (col.DataType == typeof(DateTime))
                                        insert.PropertyValues.Add(new PropertyValue(name, new DateTimeValue(Convert.ToDateTime(obj))));
                                    else if (col.DataType == typeof(decimal))
                                        insert.PropertyValues.Add(new PropertyValue(name, new DecimalValue(Convert.ToDouble(obj))));
                                    else if (col.DataType == typeof(double))
                                        insert.PropertyValues.Add(new PropertyValue(name, new DoubleValue(Convert.ToDouble(obj))));
                                    else if (col.DataType == typeof(short))
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int16Value(Convert.ToInt16(obj))));
                                    else if (col.DataType == typeof(int))
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int32Value(Convert.ToInt32(obj))));
                                    else if (col.DataType == typeof(long))
                                        insert.PropertyValues.Add(new PropertyValue(name, new Int64Value(Convert.ToInt64(obj))));
                                    else if (col.DataType == typeof(float))
                                        insert.PropertyValues.Add(new PropertyValue(name, new SingleValue(Convert.ToSingle(obj))));
                                    else if (col.DataType == typeof(string))
                                        insert.PropertyValues.Add(new PropertyValue(name, new StringValue(Convert.ToString(obj))));
                                }
                            }
                        }
                    }
                    using (IFeatureReader reader = insert.Execute()) { }
                }
                conn.Close();
            }
        }

        public static NameValueCollection ConvertFromString(string str)
        {
            NameValueCollection param = new NameValueCollection();
            if (!string.IsNullOrEmpty(str))
            {
                string[] parameters = str.Split(';');
                if (parameters.Length > 0)
                {
                    foreach (string p in parameters)
                    {
                        string[] pair = p.Split('=');
                        param.Add(pair[0], pair[1]);
                    }
                }
                else
                {
                    string[] pair = str.Split('=');
                    param.Add(pair[0], pair[1]);
                }
            }
            return param;
        }

        public static T[] CombineArray<T>(T[] array1, T[] array2)
        {
            List<T> list = new List<T>(array1);
            list.AddRange(array2);
            return list.ToArray();
        }
    }
}
