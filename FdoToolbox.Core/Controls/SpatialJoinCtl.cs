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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core.Controls
{
    public partial class SpatialJoinCtl : BaseDocumentCtl
    {
        public SpatialJoinCtl()
        {
            InitializeComponent();
            this.Title = "Database Join";

            cmbCardinality.DataSource = Enum.GetValues(typeof(SpatialJoinCardinality));
            cmbJoinType.DataSource = Enum.GetValues(typeof(SpatialJoinType));

            cmbPrimaryConnection.DataSource = new List<string>(HostApplication.Instance.SpatialConnectionManager.GetConnectionNames());
            cmbSecondaryConnection.DataSource = new List<string>(HostApplication.Instance.DatabaseConnectionManager.GetConnectionNames());
            cmbTargetConnection.DataSource = new List<string>(HostApplication.Instance.SpatialConnectionManager.GetConnectionNames());
        }

        public void LoadSettings(SpatialJoinTask task)
        {
            txtTaskName.Text = task.Name;
            SpatialJoinOptions options = task.Options;

            cmbPrimaryConnection.SelectedItem = options.PrimarySource.Name;
            cmbSecondaryConnection.SelectedItem = options.SecondarySource.Name;
            cmbTargetConnection.SelectedItem = options.Target.Name;

            cmbPrimarySchema.SelectedValue = options.SchemaName;
            cmbTargetSchema.SelectedValue = options.TargetSchema;

            cmbPrimaryClass.SelectedValue = options.ClassName;
            cmbSecondaryTable.SelectedValue = options.TableName;
            txtTargetClassName.Text = options.TargetClassName;

            string[] propertyNames = options.GetPropertyNames();
            foreach (string propName in propertyNames)
            {
                int idx = chkPrimaryProperties.Items.IndexOf(propName);
                if (idx >= 0)
                    chkPrimaryProperties.SetItemChecked(idx, true);
            }

            string[] columnNames = options.GetColumnNames();
            foreach (string colName in columnNames)
            {
                int idx = chkSecondaryColumns.Items.IndexOf(colName);
                if (idx >= 0)
                    chkSecondaryColumns.SetItemChecked(idx, true);
            }

            cmbCardinality.SelectedItem = options.Cardinality;
            cmbJoinType.SelectedItem = options.JoinType;

            string[] joinedProperties = options.GetJoinedProperties();
            foreach (string prop in joinedProperties)
            {
                grdJoins.Rows.Add(prop, options.GetMatchingColumn(prop));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            SpatialJoinOptions options = new SpatialJoinOptions();

            IConnection priConn = HostApplication.Instance.SpatialConnectionManager.GetConnection(cmbPrimaryConnection.SelectedItem.ToString());
            SpatialConnectionInfo priConnInfo = new SpatialConnectionInfo(cmbPrimaryConnection.SelectedItem.ToString(), priConn);
            string priSchema = (cmbPrimarySchema.SelectedItem as FeatureSchema).Name;
            string priClass = (cmbPrimaryClass.SelectedItem as ClassDefinition).Name;

            DbConnectionInfo secConn = HostApplication.Instance.DatabaseConnectionManager.GetConnection(cmbSecondaryConnection.SelectedItem.ToString());
            string secTable = (cmbSecondaryTable.SelectedItem as MyMeta.ITable).Name;

            IConnection tConn = HostApplication.Instance.SpatialConnectionManager.GetConnection(cmbTargetConnection.SelectedItem.ToString());
            SpatialConnectionInfo tConnInfo = new SpatialConnectionInfo(cmbTargetConnection.SelectedItem.ToString(), tConn);
            string tSchema = (cmbTargetSchema.SelectedItem as FeatureSchema).Name;
            string tClass = txtTargetClassName.Text;

            options.SetPrimary(priConnInfo, priSchema, priClass);
            options.SetSecondary(secConn, secTable);
            options.SetTarget(tConnInfo, tSchema, tClass);
            options.SecondaryPrefix = txtColumnPrefix.Text;

            foreach (object obj in chkPrimaryProperties.CheckedItems)
            {
                options.AddProperty(obj.ToString());
            }

            foreach (object obj in chkSecondaryColumns.CheckedItems)
            {
                options.AddColumn(obj.ToString());
            }

            options.JoinType = (SpatialJoinType)cmbJoinType.SelectedItem;
            options.Cardinality = (SpatialJoinCardinality)cmbCardinality.SelectedItem;

            foreach (DataGridViewRow row in grdJoins.Rows)
            {
                object prop = row.Cells[0].Value;
                object col = row.Cells[1].Value;
                if (prop != null && col != null)
                    options.AddJoinPair(prop.ToString(), col.ToString());
            }

            SpatialJoinTask task = new SpatialJoinTask(options);
            task.Name = txtTaskName.Text;
            HostApplication.Instance.TaskManager.AddTask(task);
            this.Close();
        }

        private bool ValidateFields()
        {
            bool valid = true;
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtTaskName.Text))
            {
                errorProvider.SetError(txtTaskName, "Required");
                valid = false;
            }
            if (HostApplication.Instance.TaskManager.GetTask(txtTaskName.Text) != null)
            {
                errorProvider.SetError(txtTaskName, "A task named " + txtTaskName.Text + " already exists");
                valid = false;
            }
            if (string.IsNullOrEmpty(txtTargetClassName.Text))
            {
                errorProvider.SetError(txtTargetClassName, "Required");
                valid = false;
            }
            if (chkPrimaryProperties.CheckedItems.Count == 0)
            {
                errorProvider.SetError(chkPrimaryProperties, "Please select at least one property");
                valid = false;
            }
            if (chkSecondaryColumns.CheckedItems.Count == 0)
            {
                errorProvider.SetError(chkSecondaryColumns, "Please select at least one column");
                valid = false;
            }
            if (grdJoins.Rows.Count == 0)
            {
                errorProvider.SetError(grdJoins, "Please define at least one set of property/column to join on");
                valid = false;
            }
            if (string.IsNullOrEmpty(txtColumnPrefix.Text))
            {
                List<string> names = new List<string>();
                foreach (object obj in chkPrimaryProperties.CheckedItems)
                {
                    names.Add(obj.ToString());
                }
                foreach (object obj in chkSecondaryColumns.CheckedItems)
                {
                    if (names.Contains(obj.ToString()))
                    {
                        errorProvider.SetError(txtColumnPrefix, "At least one clashing property/column was found. Please define a column prefix");
                        valid = false;
                    }
                }
            }
            return valid;
        }

        private void cmbPrimaryConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = cmbPrimaryConnection.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.SpatialConnectionManager.GetConnection(name);
            if (conn != null)
            {
                using (FeatureService service = new FeatureService(conn))
                {
                    cmbPrimarySchema.DataSource = service.DescribeSchema();
                }
            }
        }

        private void cmbTargetConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = cmbTargetConnection.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.SpatialConnectionManager.GetConnection(name);
            if (conn != null)
            {
                using (FeatureService service = new FeatureService(conn))
                {
                    cmbTargetSchema.DataSource = service.DescribeSchema();
                }
            }
        }

        private void cmbSecondaryConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = cmbSecondaryConnection.SelectedItem.ToString();
            DbConnectionInfo connInfo = HostApplication.Instance.DatabaseConnectionManager.GetConnection(name);
            if (connInfo != null)
            {
                cmbSecondaryTable.DataSource = connInfo.MetaData.DefaultDatabase.Tables;
            }
        }

        private void cmbPrimarySchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbPrimarySchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbPrimaryClass.DataSource = schema.Classes;
            }
        }

        private void cmbPrimaryClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbPrimaryClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                chkPrimaryProperties.Items.Clear();
                foreach (PropertyDefinition propDef in classDef.Properties)
                {
                    chkPrimaryProperties.Items.Add(propDef.Name, false);
                }
                grdJoins.Rows.Clear();
                COL_PROPERTY.DataSource = classDef.Properties;
                COL_PROPERTY.DisplayMember = "Name";
                CheckGridButtonStatus();
            }
        }

        private void cmbSecondaryTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyMeta.ITable table = cmbSecondaryTable.SelectedItem as MyMeta.ITable;
            if (table != null)
            {
                chkSecondaryColumns.Items.Clear();
                foreach (MyMeta.IColumn col in table.Columns)
                {
                    chkSecondaryColumns.Items.Add(col.Name, false);
                }
                grdJoins.Rows.Clear();
                COL_COLUMN.DataSource = table.Columns;
                COL_COLUMN.DisplayMember = "Name";
                CheckGridButtonStatus();
            }
        }

        private void CheckGridButtonStatus()
        {
            btnAddJoin.Enabled = (chkPrimaryProperties.Items.Count > 0 && chkSecondaryColumns.Items.Count > 0);
            btnDeleteJoin.Enabled = (grdJoins.Rows.Count > 0);
        }

        private void btnAddJoin_Click(object sender, EventArgs e)
        {
            string property = chkPrimaryProperties.Items[0].ToString();
            string column = chkSecondaryColumns.Items[0].ToString();

            grdJoins.Rows.Add(property, column);
        }

        private void btnDeleteJoin_Click(object sender, EventArgs e)
        {
            if (grdJoins.SelectedRows.Count == 1)
                grdJoins.Rows.Remove(grdJoins.SelectedRows[0]);
            else if (grdJoins.SelectedCells.Count == 1)
                grdJoins.Rows.RemoveAt(grdJoins.SelectedCells[0].RowIndex);
        }

        private void grdJoins_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteJoin.Enabled = (grdJoins.Rows.Count > 0);
        }
    }
}
