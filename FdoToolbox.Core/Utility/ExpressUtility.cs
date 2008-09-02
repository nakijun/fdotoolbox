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

namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Utility class to supplement the Express Extension Module
    /// </summary>
    public sealed class ExpressUtility
    {
        public const string PROVIDER_SDF = "OSGeo.SDF";
        public const string PROVIDER_SHP = "OSGeo.SHP";

        public const string CONN_FMT_SDF = "File={0}";
        public const string CONN_FMT_SHP = "DefaultFileLocation={0}";

        private ExpressUtility() { }

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
                catch (Exception)
                {
                    conn.Dispose();
                    throw;
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