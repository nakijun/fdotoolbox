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

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoJoinCtl : UserControl, IViewContent, IFdoJoinView
    {
        private FdoJoinPresenter _presenter;

        public FdoJoinCtl()
        {
            InitializeComponent();
            _presenter = new FdoJoinPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
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

        public event EventHandler TitleChanged;

        public bool CanClose
        {
            get { return true; }
        }

        public bool Close()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }

        public bool SaveAs()
        {
            return true;
        }

        public event EventHandler ViewContentClosing;

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

        public NameValueCollection TargetGeometryProperties
        {
            set
            {
                cmbGeometryProperty.Items.Clear();
                cmbGeometryProperty.DisplayMember = "Name";
                cmbGeometryProperty.ValueMember = "Value";
                foreach (string name in value.Keys)
                {
                    NameValuePair pair = new NameValuePair(name, value[name]);
                    cmbGeometryProperty.Items.Add(pair);
                }
            }
        }

        public string SelectedTargetGeometryProperty
        {
            get { return cmbGeometryProperty.SelectedValue.ToString(); }
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
                return cmbSpatialPredicate.Enabled;
            }
            set
            {
                cmbSpatialPredicate.Enabled = value;
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
    }
}
