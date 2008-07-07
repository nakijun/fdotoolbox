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
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Controls
{
    public partial class BulkCopyCtl : BaseDocumentCtl
    {
        public BulkCopyCtl()
        {
            InitializeComponent();

            cmbSrcConn.DataSource = new List<string>(HostApplication.Instance.ConnectionManager.GetConnectionNames());
            cmbDestConn.DataSource = new List<string>(HostApplication.Instance.ConnectionManager.GetConnectionNames());
        }

        public override string Title
        {
            get
            {
                return "Bulk Copy - " + txtName.Text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Cancel();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateCtl())
            {
                BulkCopySource src = new BulkCopySource(
                    new ConnectionInfo(
                        cmbSrcConn.SelectedItem.ToString(),
                        HostApplication.Instance.ConnectionManager.GetConnection(cmbSrcConn.SelectedItem.ToString())
                    ),
                    ((FeatureSchema)cmbSrcSchema.SelectedItem).Name,
                    ((ClassDefinition)cmbSrcClass.SelectedItem).Name
                );
                BulkCopyTarget target = new BulkCopyTarget(
                    new ConnectionInfo(
                        cmbDestConn.SelectedItem.ToString(),
                        HostApplication.Instance.ConnectionManager.GetConnection(cmbDestConn.SelectedItem.ToString())
                    ),
                    ((FeatureSchema)cmbDestSchema.SelectedItem).Name,
                    ((ClassDefinition)cmbDestClass.SelectedItem).Name
                );
                BulkCopyTask bcp = new BulkCopyTask(txtName.Text, src, target);

                bcp.TransformCoordinates = chkTransform.Checked;
                bcp.Target.DeleteBeforeCopy = chkDelete.Checked;
                bcp.CoerceDataTypes = chkCoerce.Checked;

                int limit = -1;
                if (int.TryParse(txtLimit.Text, out limit))
                    bcp.Source.FeatureLimit = limit;
                else
                    bcp.Source.FeatureLimit = -1;

                foreach (DataGridViewRow row in grdMappings.Rows)
                {
                    string source = row.Cells[0].Value.ToString();
                    string dest = row.Cells[1].Value.ToString();
                    if(!string.IsNullOrEmpty(dest))
                        bcp.Source.AddMapping(source, dest);
                }

                HostApplication.Instance.TaskManager.AddTask(bcp);
                this.Close();
            }
        }

        private bool ValidateCtl()
        {
            bool valid = true;
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider.SetError(txtName, "Required");
                valid = false;
            }
            if (cmbDestClass.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbDestClass, "Required");
                valid = false;
            }
            if (cmbDestConn.SelectedIndex < 0){
                errorProvider.SetError(cmbDestConn, "Required");
                valid = false;
            }
            if (cmbDestSchema.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbDestSchema, "Required");
                valid = false;
            }
            if (cmbSrcClass.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbSrcClass, "Required");
                valid = false;
            }
            if (cmbSrcConn.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbSrcConn, "Required");
                valid = false;
            }
            if (cmbSrcSchema.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbSrcSchema, "Required");
                valid = false;
            }
            return valid;
        }

        private void cmbSrcConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbSrcConn.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connName);
            using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = cmd.Execute();
                cmbSrcSchema.DataSource = schemas;
            }
        }

        private void cmbSrcSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbSrcSchema.SelectedItem as FeatureSchema;
            if (schema != null)
                cmbSrcClass.DataSource = schema.Classes;
        }

        private void cmbSrcClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitGrid();
        }
        
        private void cmbDestConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbDestConn.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connName);
            using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = cmd.Execute();
                cmbDestSchema.DataSource = schemas;
            }
        }

        private void cmbDestSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbDestSchema.SelectedItem as FeatureSchema;
            if (schema != null)
                cmbDestClass.DataSource = schema.Classes;
        }

        private void cmbDestClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitGrid();
        }

        private void InitGrid()
        {
            ClassDefinition srcClass = cmbSrcClass.SelectedItem as ClassDefinition;
            ClassDefinition destClass = cmbDestClass.SelectedItem as ClassDefinition;

            if (srcClass != null && destClass != null)
            {
                grdMappings.Rows.Clear();
                
                List<string> targets = new List<string>();
                targets.Add(string.Empty);
                foreach (PropertyDefinition def in destClass.Properties)
                {
                    targets.Add(def.Name);
                }
                COL_TARGET.DataSource = targets;

                foreach (PropertyDefinition def in srcClass.Properties)
                {
                    grdMappings.Rows.Add(def.Name, string.Empty);   
                }
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.Title = "Bulk Copy - " + txtName.Text;
        }
    }
}
