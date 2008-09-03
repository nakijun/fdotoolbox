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
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ETL;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Forms
{
    public partial class CopyDbToPointsSdfDlg : Form
    {
        private DbConnectionInfo _source;

        internal CopyDbToPointsSdfDlg()
        {
            InitializeComponent();
        }

        public CopyDbToPointsSdfDlg(
            DbConnectionInfo source, 
            string db,
            string table)
            : this()
        {
            _source = source;
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

        public ReadOnlyCollection<string> GetColumns()
        {
            List<string> columns = new List<string>();
            foreach (object obj in chkColumns.CheckedItems)
            {
                columns.Add(obj.ToString());
            }
            return columns.AsReadOnly();
        }

        public string FilePath { get { return txtFilePath.Text; } }
        public string ClassName { get { return txtClass.Text; } }
        public string SchemaName { get { return txtSchema.Text; } }
        public string Database { get { return txtDatabase.Text; } }
        public string Table { get { return txtTable.Text; } }
        public string XColumn { get { return cmbX.SelectedItem.ToString(); } }
        public string YColumn { get { return cmbY.SelectedItem.ToString(); } }
        public string ZColumn { get { return cmbZ.SelectedItem.ToString(); } }
        public bool ThreeDimensions { get { return chk3d.Checked; } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(ValidateFields())
                this.DialogResult = DialogResult.OK;
        }

        private bool ValidateFields()
        {
            bool valid = true;
            return valid;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtFilePath.Text = AppGateway.RunningApplication.SaveFile("Save SDF", "SDF files (*.sdf)|*.sdf");
        }
    }
}