using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using System.Resources;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;
using System.IO;

namespace FdoToolbox.Core.Utility
{
    public sealed class ExpressUtility
    {
        /// <summary>
        /// Converts a specially formatted string to a NameValueCollection
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the FDO data type for a CLR type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DataType GetFdoDataTypeFromClrType(Type t)
        {
            //No CLOB
            if (t == typeof(byte[]))
                return DataType.DataType_BLOB;
            else if (t == typeof(bool))
                return DataType.DataType_Boolean;
            else if (t == typeof(byte))
                return DataType.DataType_Byte;
            else if (t == typeof(DateTime))
                return DataType.DataType_DateTime;
            else if (t == typeof(decimal))
                return DataType.DataType_Decimal;
            else if (t == typeof(double))
                return DataType.DataType_Double;
            else if (t == typeof(short))
                return DataType.DataType_Int16;
            else if (t == typeof(int))
                return DataType.DataType_Int32;
            else if (t == typeof(long))
                return DataType.DataType_Int64;
            else if (t == typeof(float))
                return DataType.DataType_Single;
            else if (t == typeof(string))
                return DataType.DataType_String;
            else
                throw new ArgumentException(ResourceUtil.GetStringFormatted("ERR_NO_CORRESPONDING_DATA_TYPE", t));
        }

        /// <summary>
        /// Gets the CLR type from a FDO data type
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static Type GetClrTypeFromFdoDataType(DataType dt)
        {
            Type t = null;
            switch (dt)
            {
                case DataType.DataType_BLOB:
                    t = typeof(byte[]);
                    break;
                case DataType.DataType_Boolean:
                    t = typeof(bool);
                    break;
                case DataType.DataType_Byte:
                    t = typeof(byte);
                    break;
                case DataType.DataType_CLOB:
                    t = typeof(byte[]);
                    break;
                case DataType.DataType_DateTime:
                    t = typeof(DateTime);
                    break;
                case DataType.DataType_Decimal:
                    t = typeof(decimal);
                    break;
                case DataType.DataType_Double:
                    t = typeof(double);
                    break;
                case DataType.DataType_Int16:
                    t = typeof(short);
                    break;
                case DataType.DataType_Int32:
                    t = typeof(int);
                    break;
                case DataType.DataType_Int64:
                    t = typeof(long);
                    break;
                case DataType.DataType_Single:
                    t = typeof(float);
                    break;
                case DataType.DataType_String:
                    t = typeof(string);
                    break;
            }
            return t;
        }

        /// <summary>
        /// Converts a NameValueCollection to a connection string style string
        /// </summary>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public static string ConvertFromNameValueCollection(NameValueCollection nameValueCollection)
        {
            string str = string.Empty;
            foreach (string key in nameValueCollection.Keys)
            {
                if(str == string.Empty)
                    str += key + "=" + nameValueCollection[key];
                else
                    str += ";" + key + "=" + nameValueCollection[key];
            }
            return str;
        }

        /// <summary>
        /// Creates a connection to a FDO provider. The FDO provider must be a flat-file provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FdoConnection CreateFlatFileConnection(string provider, string file)
        {
            FdoConnection conn = null;
            if (provider.StartsWith("OSGeo.SDF"))
            {
                conn = new FdoConnection(provider, string.Format("File={0}", file));
            }
            else if (provider.StartsWith("OSGeo.SHP"))
            {
                conn = new FdoConnection(provider, string.Format("DefaultFileLocation={0}", file));
            }
            else
            {
                throw new InvalidOperationException(); //ERR_UNSUPPORTED_FLAT_FILE_PROVIDER
            }
            return conn;
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string provider, string path)
        {
            bool result = false;
            bool sdf = provider.StartsWith("OSGeo.SDF");
            bool sqlite = provider.StartsWith("OSGeo.SQLite");

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (conn.ConnectionInfo.ProviderDatastoreType != ProviderDatastoreType.ProviderDatastoreType_File)
                return false; //ERR_NOT_FLAT_FILE

            string pName = GetFileParameter(provider);
            if (string.IsNullOrEmpty(pName))
                return false; //ERR_FILE_PARAMETER_UNKNOWN

            using (conn)
            {
                using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    try
                    {
                        cmd.DataStoreProperties.SetProperty(pName, path);
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

        private static string GetFileParameter(string provider)
        {
            if (provider.StartsWith("OSGeo.SDF"))
                return "File";

            return null;
        }

        /// <summary>
        /// Creates a connection to a FDO provider. The FDO provider must be a flat-file provider.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public static FdoConnection CreateFlatFileConnection(string sourceFile)
        {
            string provider = GetProviderFromFileExtension(sourceFile);
            if (provider != null)
                return CreateFlatFileConnection(provider, sourceFile);
            return null;
        }

        /// <summary>
        /// Infers the FDO provider name from the file's extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetProviderFromFileExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".sdf")
                return "OSGeo.SDF";
            else if (ext == ".shp")
                return "OSGeo.SHP";
            return null;
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="_destFile"></param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string file)
        {
            string provider = GetProviderFromFileExtension(file);
            if (provider != null)
                return CreateFlatFileDataSource(provider, file);
            return false;
        }
    }
}
