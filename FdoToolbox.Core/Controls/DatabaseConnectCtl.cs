using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using System.Data.Common;

namespace FdoToolbox.Core.Controls
{
    public partial class DatabaseConnectCtl : BaseDocumentCtl
    {
        public DatabaseConnectCtl()
        {
            InitializeComponent();
            this.Title = "New Database Connection";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider.SetError(txtName, "Required");
                return;
            }

            string name = txtName.Text;
            string connStr = txtConnStr.Text;
            OleDbConnection conn = new OleDbConnection(connStr);
            if (AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(name) == null)
            {
                btnConnect.Enabled = false;
                try
                {
                    DbConnectionInfo connInfo = new DbConnectionInfo(name, conn);
                    AppGateway.RunningApplication.DatabaseConnectionManager.AddConnection(connInfo);
                }
                catch (Exception ex)
                {
                    AppConsole.Alert("Error", ex.Message);
                }
                finally
                {
                    btnConnect.Enabled = true;
                }
                this.Close();
            }
            else
            {
                errorProvider.SetError(txtName, "A database connection named " + name + " already exists. Please choose another name");
                return;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                IDbConnection conn = new OleDbConnection(txtConnStr.Text);
                using (conn)
                {
                    conn.Open();
                    AppConsole.Alert("Test Connection", "Test successful");
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
            }
        }

        private void cmbDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO: fill textbox with connection string template based on selected driver
        }
    }
}
