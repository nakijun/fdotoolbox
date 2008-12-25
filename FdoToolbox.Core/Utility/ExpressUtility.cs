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
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Utility class for common FDO tasks and functionality
    /// </summary>
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
        /// <param name="dt"></param>
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
            if (provider.StartsWith("OSGeo.SDF") || provider.StartsWith("OSGeo.SQLite"))
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
            else if (provider.StartsWith("OSGeo.SQLite"))
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
            else if (ext == ".db" || ext == ".sqlite")
                return "OSGeo.SQLite";
            return null;
        }

        /// <summary>
        /// Creates a FDO data source. The provider must be a flat-file provider
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool CreateFlatFileDataSource(string file)
        {
            string provider = GetProviderFromFileExtension(file);
            if (provider != null)
                return CreateFlatFileDataSource(provider, file);
            return false;
        }

        /// <summary>
        /// Creates a FDO bulk copy task. The target file will be created as part of 
        /// this method call. If the target path is a directory, it is assumed that
        /// SHP files are to be created and copied to.
        /// </summary>
        /// <param name="sourceFile">The path to the source file.</param>
        /// <param name="targetPath">
        /// The path to the target file/directory. If it is a directory, it is assumed
        /// that SHP files are to be created and copied to.
        /// </param>
        /// <param name="copySpatialContexts">If true, will also copy spatial contexts</param>
        /// <param name="fixIncompatibleSchema">If true, will try to fix the source schema to make it compatible with the target connection. If false, an exception will be thrown</param>
        /// <returns></returns>
        public static FdoBulkCopy CreateBulkCopy(string sourceFile, string targetPath, bool copySpatialContexts, bool fixIncompatibleSchema)
        {
            FdoBulkCopyOptions options = null;
            FdoConnection source = null;
            FdoConnection target = null;
            //Is a directory. Implies a SHP connection
            if (Directory.Exists(targetPath))
            {
                //SHP doesn't actually support CreateDataStore. We use the following technique:
                // - Connect to base directory
                // - Clone source schema and apply to SHP connection.
                // - A SHP file and related files are created for each feature class.
                string shpdir = Path.GetDirectoryName(targetPath);
                source = CreateFlatFileConnection(sourceFile);
                target = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + shpdir);
            }
            else
            {
                if (!CreateFlatFileDataSource(targetPath))
                    throw new FdoException("Unable to create data source on: " + targetPath);
                source = CreateFlatFileConnection(sourceFile);
                target = CreateFlatFileConnection(targetPath);
            }

            source.Open();
            target.Open();

            options = new FdoBulkCopyOptions(source, target, true);

            using (FdoFeatureService srcService = source.CreateFeatureService())
            using (FdoFeatureService destService = target.CreateFeatureService())
            {
                FeatureSchemaCollection schemas = srcService.DescribeSchema();
                //Assume single-schema
                FeatureSchema fs = schemas[0];
                //Clone and apply to target
                FeatureSchema targetSchema = FdoFeatureService.CloneSchema(fs);
                IncompatibleSchema incSchema;
                bool canApply = destService.CanApplySchema(targetSchema, out incSchema);
                if (canApply)
                {
                    destService.ApplySchema(targetSchema);
                }
                else
                {
                    if (fixIncompatibleSchema)
                    {
                        FeatureSchema fixedSchema = destService.AlterSchema(targetSchema, incSchema);
                        destService.ApplySchema(fixedSchema);
                    }
                    else
                    {
                        throw new Exception(incSchema.ToString());
                    }
                }

                //Copy all classes
                foreach (ClassDefinition cd in fs.Classes)
                {
                    options.AddClassCopyOption(cd.Name, cd.Name);
                }

                if (copySpatialContexts)
                {
                    //Assume single spatial context. So use the active one
                    SpatialContextInfo srcCtx = srcService.GetActiveSpatialContext();
                    if(srcCtx != null)
                        options.AddSourceSpatialContext(srcCtx);
                }
                //Flick on batch support if we can
                if (destService.SupportsBatchInsertion())
                    options.BatchSize = 300; //Madness? THIS IS SPARTA!
            }
            

            return new FdoBulkCopy(options);
        }
    }
}
