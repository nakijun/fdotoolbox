using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Controls
{
    public partial class DbDataPreviewCtl : DbConnectionBoundCtl
    {
        internal DbDataPreviewCtl()
        {
            InitializeComponent();
        }

        public DbDataPreviewCtl(DbConnectionInfo connInfo, string key)
            : base(connInfo, key)
        {
            InitializeComponent();
            this.Title = "Data Preview - " + connInfo.Name;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            grdPreview.DataSource = null;
            lblResults.Text = "";
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            lblResults.Text = "";
            string sql = txtSQL.Text;
            if (string.IsNullOrEmpty(sql))
            {
                AppConsole.Alert("Error", "Please enter the SQL query text");
                return;
            }
            if (!sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
            {
                AppConsole.Alert("Error", "Only SQL SELECT statements are allowed for data previewing");
                return;
            }
            IDbCommand cmd = this.BoundConnection.Connection.CreateCommand();
            using (cmd)
            {
                try
                {
                    cmd.CommandText = sql;
                    IDataReader reader = cmd.ExecuteReader();
                    BindingSource bs = new BindingSource();
                    bs.DataSource = reader;
                    grdPreview.DataSource = bs;
                    lblResults.Text = bs.Count + " results returned";
                }
                catch (Exception ex)
                {
                    AppConsole.Alert("Error", ex.Message);
                }
            }
        }
    }
}
