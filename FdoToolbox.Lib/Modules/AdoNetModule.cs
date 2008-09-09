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
using FdoToolbox.Lib.Commands;
using FdoToolbox.Lib.Controls;
using FdoToolbox.Core.Common;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Lib.Forms;
using System.IO;
using FdoToolbox.Core.IO;
using FdoToolbox.Core.ETL;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Lib.Modules
{
    public class AdoNetModule : ModuleBase
    {
        #region command names

        public const string DB_CONNECT = "dbconnect";
        public const string DB_REFRESH_CONNECTION = "dbrefreshconn";
        public const string DB_RENAME_CONNECTION = "dbrenameconn";
        public const string DB_REMOVE_CONNECTION = "dbremoveconn";
        public const string DB_DATA_PREVIEW = "dbdatapreview";
        public const string DB_LOAD_CONNECTION = "dbloadconn";
        public const string DB_SAVE_CONNECTION = "dbsaveconn";
        public const string DB_POINT_CONVERT = "dbconverttopoints";
        public const string DB_POINT_CONVERT_SDF = "dbconverttosdf";
        public const string DB_REMOVE_ALL_CONNECTIONS = "dbremoveallconnections";

        #endregion

        public override string Name
        {
            get { return "adonet"; }
        }

        public override string Description
        {
            get { return "ADO.net database module"; }
        }

        private IHostApplication _App;

        public override void Initialize() 
        {
            _App = AppGateway.RunningApplication;
        }

        public override void Cleanup() { }

        [Command(AdoNetModule.DB_CONNECT, "Connect to database", Description = "Connect to an ADO.net supported data source", ImageResourceName = "database_connect")]
        public void Connect()
        {
            DatabaseConnectCtl ctl = new DatabaseConnectCtl();
            _App.Shell.ShowDocumentWindow(ctl);
        }

        [Command(AdoNetModule.DB_REFRESH_CONNECTION, "Refresh connection", ImageResourceName = "page_refresh")]
        public void RefreshConnection()
        {
            DbConnectionInfo connInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
                _App.Shell.ObjectExplorer.RefreshDatabaseConnection(connInfo.Name);
        }

        [Command(AdoNetModule.DB_RENAME_CONNECTION, "Rename connection")]
        public void RenameConnection()
        {
            DbConnectionInfo connInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string oldName = connInfo.Name;
                string newName = StringInputDlg.GetInput("Rename Connection", "Enter the new name for the connection", oldName);
                if (!string.IsNullOrEmpty(newName) && oldName != newName)
                {
                    string reason = string.Empty;
                    if (_App.DatabaseConnectionManager.CanRenameConnection(oldName, newName, ref reason))
                        _App.DatabaseConnectionManager.RenameConnection(oldName, newName);
                    else
                        AppConsole.Alert("Error", reason);
                }
            }
        }

        [Command(AdoNetModule.DB_REMOVE_CONNECTION, "Remove connection", ImageResourceName = "cross")]
        public void RemoveConnection()
        { 
            DbConnectionInfo connInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
                _App.DatabaseConnectionManager.RemoveConnection(connInfo.Name);
        }
        
        [Command(AdoNetModule.DB_REMOVE_ALL_CONNECTIONS, "Remove All Connections", ImageResourceName = "cross")]
        public void RemoveAllDbConnections()
        {
            ICollection<string> names = _App.DatabaseConnectionManager.GetConnectionNames();
            List<string> connNames = new List<string>(names);
            foreach (string n in connNames)
            {
                _App.DatabaseConnectionManager.RemoveConnection(n);
            }
        }

        [Command(AdoNetModule.DB_DATA_PREVIEW, "Data Preview", ImageResourceName = "zoom")]
        public void DataPreview()
        {
            //TODO: Don't create directly
            DbConnectionInfo connInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string key = "PREVIEW";
                BaseDocumentCtl ctl = new DbDataPreviewCtl(connInfo, key);
                _App.Shell.ShowDocumentWindow(ctl);
            }
        }

        [Command(AdoNetModule.DB_SAVE_CONNECTION, "Save Connection", ImageResourceName = "disk")]
        public void SaveConnection()
        {
            DbConnectionInfo connInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string connDef = _App.SaveFile("Save connection information", "Connection information (*.dbconn)|*.dbconn");
                if (connDef != null)
                {
                    if (File.Exists(connDef))
                        File.Delete(connDef);
                    DbConnLoader.SaveConnection(connInfo, connDef);
                    AppConsole.WriteLine("Connection saved to {0}", connDef);
                }
            }
        }

        [Command(AdoNetModule.DB_LOAD_CONNECTION, "Load Connection", ImageResourceName = "folder")]
        public void LoadConnection()
        {
            string connDef = _App.OpenFile("Load connection information", "Connection information (*.dbconn)|*.dbconn");
            if (File.Exists(connDef))
            {
                DbConnectionInfo connInfo = DbConnLoader.LoadConnection(connDef);
                DbConnectionInfo conn = _App.DatabaseConnectionManager.GetConnection(connInfo.Name);
                if (conn != null)
                {
                    AppConsole.Write("A connection named {0} already exists. ", connInfo.Name);
                    connInfo.Name = _App.DatabaseConnectionManager.CreateUniqueName();
                    AppConsole.WriteLine("Attempting to load as {0} instead", connInfo.Name);
                }
                _App.DatabaseConnectionManager.AddConnection(connInfo);
                AppConsole.WriteLine("Connection loaded from {0}", connDef);
            }
        }

        [Command(AdoNetModule.DB_POINT_CONVERT, "Convert to Point Feature Class", Description = "Copies the table to a new Point feature class in an FDO data source", ImageResourceName = "shape_handles")]
        public void ConvertToPoints()
        {
            if (_App.SpatialConnectionManager.GetConnectionNames().Count == 0)
            {
                AppConsole.Alert("Error", "Cannot database table to points. There are no FDO data sources open");
                return;
            }
            DbConnectionInfo dbConnInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (dbConnInfo != null)
            {
                string database = _App.Shell.ObjectExplorer.GetSelectedDatabase();
                string table = _App.Shell.ObjectExplorer.GetSelectedTable();
                DbToPointCopyOptions options = CopyDbToPointsDlg.GetCopyOptions(dbConnInfo, database, table);
                if (options != null)
                {
                    DbToPointCopyTask task = new DbToPointCopyTask(options);
                    new TaskProgressDlg(task).Run();
                }
            }
        }

        [Command(AdoNetModule.DB_POINT_CONVERT_SDF, "Convert to Point Feature SDF", Description = "Copies the table to a new Point feature class in a new SDF file", ImageResourceName = "shape_handles")]
        public void ConvertToPointsSdf()
        {
            DbConnectionInfo dbConnInfo = _App.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (dbConnInfo != null)
            {
                string database = _App.Shell.ObjectExplorer.GetSelectedDatabase();
                string table = _App.Shell.ObjectExplorer.GetSelectedTable();
                CopyDbToPointsSdfDlg diag = new CopyDbToPointsSdfDlg(dbConnInfo, database, table);
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FeatureSchema schema = new FeatureSchema(diag.SchemaName, "");

                    IConnection conn = ExpressUtility.ApplySchemaToNewSDF(schema, diag.FilePath);
                    conn.Open();

                    string name = _App.SpatialConnectionManager.CreateUniqueName();
                    FdoConnectionInfo connInfo = new FdoConnectionInfo(name, conn);

                    DbToPointCopyOptions options = new DbToPointCopyOptions(dbConnInfo, connInfo);
                    options.ClassName = diag.ClassName;
                    options.AddColumns(diag.GetColumns());
                    options.Database = diag.Database;
                    options.SchemaName = diag.SchemaName;
                    options.Table = diag.Table;
                    options.XColumn = diag.XColumn;
                    options.YColumn = diag.YColumn;
                    if (diag.ThreeDimensions)
                        options.ZColumn = diag.ZColumn;

                    DbToPointCopyTask task = new DbToPointCopyTask(options);

                    new TaskProgressDlg(task).Run();
                }
            }
        }
    }
}
