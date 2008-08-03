using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Forms;

namespace FdoToolbox.Core.Modules
{
    public class AdoNetModule : ModuleBase
    {
        #region command names

        public const string DB_CONNECT = "dbconnect";
        public const string DB_REFRESH_CONNECTION = "dbrefreshconn";
        public const string DB_RENAME_CONNECTION = "dbrenameconn";
        public const string DB_REMOVE_CONNECTION = "dbremoveconn";

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
    }
}
