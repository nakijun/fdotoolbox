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

namespace FdoToolbox.Core
{
    /// <summary>
    /// Utility class to supplement the Express Extension Module
    /// </summary>
    public class ExpressUtility
    {
        public static IConnection CreateSDFConnection()
        {
            IConnection conn = null;
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Create SDF connection";
            diag.Filter = "SDF Files (*.sdf)|*.sdf";
            diag.Multiselect = false;
            diag.ShowReadOnly = true;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string connStr = string.Format("File={0};ReadOnly={1}", diag.FileName, diag.ReadOnlyChecked.ToString().ToUpper());
                conn =  FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SDF");
                conn.ConnectionString = connStr;
            }
            return conn;
        }

        public static IConnection CreateSHPConnection()
        {
            IConnection conn = null;
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Create SHP connection";
            diag.Filter = "SHP Files (*.shp)|*.shp";
            diag.Multiselect = false;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string connStr = string.Format("DefaultFileLocation={0}", diag.FileName);
                conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
                conn.ConnectionString = connStr;
            }
            return conn;
        }

        public static bool CreateSDF(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
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
    }
}
