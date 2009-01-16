#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using System.Collections.Specialized;
using FdoToolbox.Core;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Core.ETL.Operations;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoJoinCtl : ViewContent, IViewContent, IFdoJoinView
    {
        private FdoJoinPresenter _presenter;

        public FdoJoinCtl()
        {
            InitializeComponent();
            ServiceManager sm = ServiceManager.Instance;
            _presenter = new FdoJoinPresenter(this, 
                sm.GetService<IFdoConnectionManager>(),
                sm.GetService<TaskManager>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_JOIN_SETTINGS"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        public List<string> LeftConnections
        {
            set { cmbLeftConnection.DataSource = value; }
        }

        public List<string> RightConnections
        {
            set { cmbRightConnection.DataSource = value; }
        }

        public List<string> TargetConnections
        {
            set { cmbTargetConnection.DataSource = value; }
        }

        public List<string> LeftSchemas
        {
            set { cmbLeftSchema.DataSource = value; }
        }

        public List<string> RightSchemas
        {
            set { cmbRightSchema.DataSource = value; }
        }

        public List<string> TargetSchemas
        {
            set { cmbTargetSchema.DataSource = value; }
        }

        public List<string> LeftClasses
        {
            set { cmbLeftClass.DataSource = value; }
        }

        public List<string> RightClasses
        {
            set { cmbRightClass.DataSource = value; }
        }

        public Array JoinTypes
        {
            set { cmbJoinTypes.DataSource = value; }
        }

        public Array SpatialPredicates
        {
            set { cmbSpatialPredicate.DataSource = value; }
        }

        public FdoJoinType SelectedJoinType
        {
            get { return (FdoJoinType)cmbJoinTypes.SelectedItem; }
        }

        public string SelectedLeftConnection
        {
            get
            {
                return cmbLeftConnection.SelectedItem.ToString();
            }
            set
            {
                cmbLeftConnection.SelectedItem = value;
            }
        }

        public string SelectedRightConnection
        {
            get
            {
                return cmbRightConnection.SelectedItem.ToString();
            }
            set
            {
                cmbRightConnection.SelectedItem = value;
            }
        }

        public string SelectedTargetConnection
        {
            get
            {
                return cmbTargetConnection.SelectedItem.ToString();
            }
            set
            {
                cmbTargetConnection.SelectedItem = value;
            }
        }

        public string SelectedLeftSchema
        {
            get
            {
                return cmbLeftSchema.SelectedItem.ToString();
            }
            set
            {
                cmbLeftSchema.SelectedItem = value;
            }
        }

        public string SelectedRightSchema
        {
            get
            {
                return cmbRightSchema.SelectedItem.ToString();
            }
            set
            {
                cmbRightSchema.SelectedItem = value;
            }
        }

        public string SelectedTargetSchema
        {
            get
            {
                return cmbTargetSchema.SelectedItem.ToString();
            }
            set
            {
                cmbTargetSchema.SelectedItem = value;
            }
        }

        public string SelectedLeftClass
        {
            get
            {
                return cmbLeftClass.SelectedItem.ToString();
            }
            set
            {
                cmbLeftClass.SelectedItem = value;
            }
        }

        public string SelectedRightClass
        {
            get
            {
                return cmbRightClass.SelectedItem.ToString();
            }
            set
            {
                cmbRightClass.SelectedItem = value;
            }
        }

        public List<string> LeftProperties
        {
            get
            {
                List<string> props = new List<string>();
                foreach (object obj in chkLeftProperties.CheckedItems)
                {
                    props.Add(obj.ToString());
                }
                return props;
            }
            set
            {
                chkLeftProperties.Items.Clear();
                foreach (string prop in value)
                {
                    chkLeftProperties.Items.Add(prop, false);
                }
                COL_LEFT.DataSource = new List<string>(value);
            }
        }

        public List<string> RightProperties
        {
            get
            {
                List<string> props = new List<string>();
                foreach (object obj in chkRightProperties.CheckedItems)
                {
                    props.Add(obj.ToString());
                }
                return props;
            }
            set
            {
                chkRightProperties.Items.Clear();
                foreach (string prop in value)
                {
                    chkRightProperties.Items.Add(prop, false);
                }
                COL_RIGHT.DataSource = new List<string>(value);
            }
        }

        public int BatchSize
        {
            get
            {
                return Convert.ToInt32(numBatchSize.Value);
            }
            set
            {
                numBatchSize.Value = value;
            }
        }

        public bool BatchEnabled
        {
            get
            {
                return numBatchSize.Enabled;
            }
            set
            {
                numBatchSize.Enabled = value;
            }
        }

        public bool SpatialPredicateEnabled
        {
            get
            {
                //return cmbSpatialPredicate.Enabled;
                return chkJoinPredicate.Checked;
            }
            set
            {
                //cmbSpatialPredicate.Enabled = value;
                chkJoinPredicate.Checked = value;
            }
        }

        public OSGeo.FDO.Filter.SpatialOperations SelectedSpatialPredicate
        {
            get
            {
                return (OSGeo.FDO.Filter.SpatialOperations)cmbSpatialPredicate.SelectedItem;
            }
            set
            {
                cmbSpatialPredicate.SelectedItem = value;
            }
        }

        public void ClearJoins()
        {
            grdJoin.Rows.Clear();
        }

        public void AddPropertyJoin(string left, string right)
        {
            grdJoin.Rows.Add(new object[] { left, right });
        }

        public void RemoveJoin(string left)
        {
            int idx = -1;
            foreach (DataGridViewRow row in grdJoin.Rows)
            {
                if (row.Cells[0].Value.ToString() == left)
                {
                    idx = row.Index;
                }
            }

            if (idx >= 0)
                grdJoin.Rows.RemoveAt(idx);
        }

        public NameValueCollection GetJoinedProperties()
        {
            NameValueCollection joinPairs = new NameValueCollection();
            foreach (DataGridViewRow row in grdJoin.Rows)
            {
                joinPairs.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            }
            return joinPairs;
        }

        private void cmbLeftConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Left);
        }

        private void cmbRightConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Right);
        }

        private void cmbTargetConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ConnectionChanged(JoinSourceType.Target);
        }

        private void cmbLeftSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Left);
        }

        private void cmbRightSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Right);
        }

        private void cmbTargetSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SchemaChanged(JoinSourceType.Target);
        }

        private void cmbLeftClass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ClassChanged(JoinSourceType.Left);
        }

        private void cmbRightClass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.ClassChanged(JoinSourceType.Right);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _presenter.SaveTask();
                base.Close();
            }
            catch (TaskValidationException ex)
            {
                MessageService.ShowError(ex.Message);
            }
        }

        public string TaskName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string SelectedTargetClass
        {
            get { return txtTargetClass.Text; }
            set { txtTargetClass.Text = value; }
        }

        private void chkJoinPredicate_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.JoinPredicateCheckChanged();
        }

        private void chkGeometryProperty_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.GeometryPropertyCheckChanged();
        }

        public string LeftPrefix
        {
            get { return txtLeftPrefix.Text; }
            set { txtRightPrefix.Text = value; }
        }

        public string RightPrefix
        {
            get { return txtRightPrefix.Text; }
            set { txtRightPrefix.Text = value; }
        }

        public bool TargetGeometryPropertyEnabled
        {
            get { return chkGeometryProperty.Checked; }
            set { chkGeometryProperty.Checked = value; }
        }

        public bool SpatialPredicateListEnabled
        {
            set { cmbSpatialPredicate.Enabled = value; }
        }


        public bool LeftGeometryEnabled
        {
            get { return rdLeftGeom.Enabled; }
            set { rdLeftGeom.Enabled = value; }
        }

        public string LeftGeometryName
        {
            get { return rdLeftGeom.Text; }
            set { rdLeftGeom.Text = value; }
        }

        public bool LeftGeometryChecked
        {
            get { return rdLeftGeom.Checked; }
            set { rdLeftGeom.Checked = value; }
        }

        public bool RightGeometryEnabled
        {
            get { return rdRightGeom.Enabled; }
            set { rdRightGeom.Enabled = value; }
        }

        public string RightGeometryName
        {
            get { return rdRightGeom.Text; }
            set { rdRightGeom.Text = value; }
        }

        public bool RightGeometryChecked
        {
            get { return rdRightGeom.Checked; }
            set { rdRightGeom.Checked = value; }
        }

        private void btnAddJoin_Click(object sender, EventArgs e)
        {
            grdJoin.Rows.Add();
        }

        private void grdJoin_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteJoin.Enabled = true;
        }

        private void btnDeleteJoin_Click(object sender, EventArgs e)
        {
            int rowIndex = -1;
            if (grdJoin.SelectedRows.Count == 1)
                rowIndex = grdJoin.SelectedRows[0].Index;
            else if (grdJoin.SelectedCells.Count == 1)
                rowIndex = grdJoin.SelectedCells[0].RowIndex;
            
            if (rowIndex >= 0)
                grdJoin.Rows.RemoveAt(rowIndex);
        }


        public bool ForceOneToOne
        {
            get { return chkOneToOne.Checked; }
            set { chkOneToOne.Checked = value; }
        }
    }
}
