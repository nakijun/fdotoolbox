using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Forms;
using System.IO;
using FdoToolbox.Core.IO;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Core.Modules
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

        #endregion

        public override string Name
        {
            get { return "adonet"; }
        }

        public override string Description
        {
            get { return "ADO.net database module"; }
        }

        public override void Initialize() { }

        public override void Cleanup() { }

        [Command(AdoNetModule.DB_CONNECT, "Connect to database", Description = "Connect to an ADO.net supported data source", ImageResourceName = "database_connect")]
        public void Connect()
        {
            DatabaseConnectCtl ctl = new DatabaseConnectCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(AdoNetModule.DB_REFRESH_CONNECTION, "Refresh connection", ImageResourceName = "page_refresh")]
        public void RefreshConnection()
        {
            DbConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
                HostApplication.Instance.Shell.ObjectExplorer.RefreshDatabaseConnection(connInfo.Name);
        }

        [Command(AdoNetModule.DB_RENAME_CONNECTION, "Rename connection")]
        public void RenameConnection()
        {
            DbConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string oldName = connInfo.Name;
                string newName = StringInputDlg.GetInput("Rename Connection", "Enter the new name for the connection", oldName);
                if (!string.IsNullOrEmpty(newName) && oldName != newName)
                {
                    string reason = string.Empty;
                    if (HostApplication.Instance.DatabaseConnectionManager.CanRenameConnection(oldName, newName, ref reason))
                        HostApplication.Instance.DatabaseConnectionManager.RenameConnection(oldName, newName);
                    else
                        AppConsole.Alert("Error", reason);
                }
            }
        }

        [Command(AdoNetModule.DB_REMOVE_CONNECTION, "Remove connection", ImageResourceName = "cross")]
        public void RemoveConnection()
        { 
            DbConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
                HostApplication.Instance.DatabaseConnectionManager.RemoveConnection(connInfo.Name);
        }

        [Command(AdoNetModule.DB_DATA_PREVIEW, "Data Preview", ImageResourceName = "zoom")]
        public void DataPreview()
        {
            //TODO: Don't create directly
            DbConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string key = "PREVIEW";
                BaseDocumentCtl ctl = new DbDataPreviewCtl(connInfo, key);
                HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
            }
        }

        [Command(AdoNetModule.DB_SAVE_CONNECTION, "Save Connection", ImageResourceName = "disk")]
        public void SaveConnection()
        {
            DbConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (connInfo != null)
            {
                string connDef = HostApplication.Instance.SaveFile("Save connection information", "Connection information (*.dbconn)|*.dbconn");
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
            string connDef = HostApplication.Instance.OpenFile("Load connection information", "Connection information (*.dbconn)|*.dbconn");
            if (File.Exists(connDef))
            {
                DbConnectionInfo connInfo = DbConnLoader.LoadConnection(connDef);
                DbConnectionInfo conn = HostApplication.Instance.DatabaseConnectionManager.GetConnection(connInfo.Name);
                if (conn != null)
                {
                    AppConsole.Write("A connection named {0} already exists. ", connInfo.Name);
                    connInfo.Name = HostApplication.Instance.DatabaseConnectionManager.CreateUniqueName();
                    AppConsole.WriteLine("Attempting to load as {0} instead", connInfo.Name);
                }
                HostApplication.Instance.DatabaseConnectionManager.AddConnection(connInfo);
                AppConsole.WriteLine("Connection loaded from {0}", connDef);
            }
        }

        [Command(AdoNetModule.DB_POINT_CONVERT, "Convert to Point Feature Class", ImageResourceName = "shape_handles")]
        public void ConvertToPoints()
        {
            DbConnectionInfo dbConnInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabaseConnection();
            if (dbConnInfo != null)
            {
                string database = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedDatabase();
                string table = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTable();
                DbToPointCopyOptions options = CopyDbToPointsDlg.GetCopyOptions(dbConnInfo, database, table);
                if (options != null)
                {
                    DbToPointCopyTask task = new DbToPointCopyTask(options);
                    new TaskProgressDlg(task).Run();
                }
            }
        }
    }
}