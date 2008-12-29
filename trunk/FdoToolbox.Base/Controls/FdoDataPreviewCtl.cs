using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoDataPreviewCtl : UserControl, IViewContent, IFdoDataPreviewView
    {
        private FdoDataPreviewPresenter _presenter;

        internal FdoDataPreviewCtl()
        {
            InitializeComponent();
            ImageList list = new ImageList();
            list.Images.Add(ResourceService.GetBitmap("table"));
            list.Images.Add(ResourceService.GetBitmap("map"));
            resultTab.ImageList = list;
            TAB_GRID.ImageIndex = 0;
            TAB_MAP.ImageIndex = 1;
        }

        public FdoDataPreviewCtl(FdoConnection conn) : this()
        {
            _presenter = new FdoDataPreviewPresenter(this, conn);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_DATA_PREVIEW"); }
        }

        public event EventHandler TitleChanged = delegate { };

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

        public event EventHandler ViewContentClosing = delegate { };

        public List<QueryMode> QueryModes
        {
            set 
            { 
                cmbQueryMode.ComboBox.Items.Clear();
                foreach (QueryMode mode in value)
                {
                    cmbQueryMode.ComboBox.Items.Add(mode);
                }
                if (value.Count > 0)
                {
                    cmbQueryMode.SelectedIndex = 0;
                }
            }
        }

        public QueryMode SelectedQueryMode
        {
            get { return (QueryMode)cmbQueryMode.ComboBox.SelectedItem; }
        }

        public IQuerySubView QueryView
        {
            get
            {
                if (queryPanel.Controls.Count > 0)
                    return (IQuerySubView)queryPanel.Controls[0];
                return null;
            }
            set 
            {
                queryPanel.Controls.Clear();
                value.ContentControl.Dock = DockStyle.Fill;
                queryPanel.Controls.Add(value.ContentControl);
            }
        }

        private void cmbQueryMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.QueryModeChanged();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            _presenter.ExecuteQuery();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.CancelCurrentQuery();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _presenter.Clear();
        }

        public bool CancelEnabled
        {
            get
            {
                return btnCancel.Enabled;
            }
            set
            {
                btnCancel.Enabled = value;
            }
        }

        public bool ExecuteEnabled
        {
            get
            {
                return btnQuery.Enabled;
            }
            set
            {
                btnQuery.Enabled = value;
            }
        }

        public string CountMessage
        {
            set { lblCount.Text = value; }
        }


        public bool ClearEnabled
        {
            get
            {
                return btnClear.Enabled;
            }
            set
            {
                btnClear.Enabled = value;
            }
        }

        private FdoFeatureTable _table;

        public FdoFeatureTable ResultTable
        {
            get
            {
                return _table;
            }
            set
            {
                if (value == null)
                {
                    _table = null;
                    grdResults.DataSource = null;
                    grdResults.Columns.Clear();
                    grdResults.Rows.Clear();
                    lblCount.Text = string.Empty;
                }
                else
                {
                    _table = value;
                    mapCtl.DataSource = _table;
                    foreach (DataColumn col in _table.Columns)
                    {
                        grdResults.Columns.Add(col.ColumnName, col.ColumnName);
                    }
                    _table.FeatureChanged += new FdoToolbox.Core.FdoFeatureChangeEventHandler(FeatureAdded);
                }
            }
        }

        void FeatureAdded(object sender, FdoToolbox.Core.FdoFeatureChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                if (grdResults.InvokeRequired)
                {
                    grdResults.Invoke(
                        new MethodInvoker(
                            delegate { 
                                grdResults.Rows.Add(e.Feature.GeometriesAsText());
                                lblCount.Text = grdResults.Rows.Count + " results";
                            }
                    ));
                }
                else
                {
                    grdResults.Rows.Add(e.Feature.GeometriesAsText());
                    lblCount.Text = grdResults.Rows.Count + " results";
                }
            }
        }

        public bool MapEnabled
        {
            set
            {
                if (value)
                {
                    if (!resultTab.TabPages.Contains(TAB_MAP))
                        resultTab.TabPages.Add(TAB_MAP);
                }
                else
                {
                    resultTab.TabPages.Remove(TAB_MAP);
                }
            }
        }

        public void DisplayError(Exception exception)
        {
            MessageService.ShowError(exception);
        }
    }
}
