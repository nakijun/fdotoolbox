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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.Common;
using FdoToolbox.Lib.ClientServices;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Lib.Forms
{
    public partial class CopyDbToPointsDlg : Form
    {
        private DatabaseConnection _source;

        internal CopyDbToPointsDlg()
        {
            InitializeComponent();
        }

        internal CopyDbToPointsDlg(
            DatabaseConnection source, 
            string db,
            string table)
            : this()
        {
            _source = source;
            cmbTargetConnection.DataSource = new List<string>(AppGateway.RunningApplication.FdoConnectionManager.GetConnectionNames());
            txtDatabase.Text = db;
            txtTable.Text = table;
            chk3d_CheckedChanged(this, EventArgs.Empty);
        }

        private void chk3d_CheckedChanged(object sender, EventArgs e)
        {
            cmbZ.Enabled = chk3d.Checked;
        }

        protected override void OnLoad(EventArgs e)
        {
            TableInfo table = _source.GetTable(txtTable.Text);

            if (table != null)
            {
                List<string> colNames = new List<string>();
                foreach (ColumnInfo col in table.Columns)
                {
                    colNames.Add(col.Name);
                    chkColumns.Items.Add(col.Name, false);
                }
                
                cmbX.DataSource = new List<string>(colNames);
                cmbY.DataSource = new List<string>(colNames);
                cmbZ.DataSource = new List<string>(colNames);
            }
            base.OnLoad(e);
        }

        public DbToPointCopyOptions GetCopyOptions()
        {
            FdoConnection spConnInfo = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(cmbTargetConnection.SelectedItem.ToString());
            DbToPointCopyOptions options = new DbToPointCopyOptions(_source, spConnInfo);
            options.ClassName = txtClass.Text;
            options.Database = txtDatabase.Text;
            options.SchemaName = cmbTargetSchema.SelectedValue.ToString();
            options.Table = txtTable.Text;
            options.XColumn = cmbX.SelectedItem.ToString();
            options.YColumn = cmbY.SelectedItem.ToString();
            if (chk3d.Checked)
                options.ZColumn = cmbZ.SelectedItem.ToString();

            foreach (object obj in chkColumns.CheckedItems)
            {
                options.AddColumn(obj.ToString());
            }

            return options;
        }

        public static DbToPointCopyOptions GetCopyOptions(DatabaseConnection source, string db, string table)
        {
            CopyDbToPointsDlg diag = new CopyDbToPointsDlg(source, db, table);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.GetCopyOptions();
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(ValidateFields())
                this.DialogResult = DialogResult.OK;
        }

        private bool ValidateFields()
        {
            errorProvider.Clear();
            bool valid = true;
            if (string.IsNullOrEmpty(txtClass.Text))
            {
                errorProvider.SetError(txtClass, "Required");
                valid = false;
            }
            return valid;
        }

        private void cmbTargetConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            FdoConnection conn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(cmbTargetConnection.SelectedItem.ToString());
            if (conn != null)
            {
                using (FeatureService service = conn.CreateFeatureService())
                {
                    cmbTargetSchema.DataSource = service.DescribeSchema();
                }
            }
        }
    }
}