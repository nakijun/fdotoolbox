using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoConnectCtl : UserControl, IFdoConnectView
    {
        private FdoConnectCtlPresenter _presenter;

        public FdoConnectCtl()
        {
            InitializeComponent();
            InitializeGrid();
            this.Title = ResourceService.GetString("TITLE_NEW_DATA_CONNECTION");
            _presenter = new FdoConnectCtlPresenter(this, ServiceManager.Instance.GetService<FdoConnectionManager>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.GetProviderList();
            _presenter.ProviderChanged();
            base.OnLoad(e);
        }

        private void InitializeGrid()
        {
            grdProperties.Rows.Clear();
            grdProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);
        }

        public string ConnectionName
        {
            get { return txtConnectionName.Text; }
        }

        public FdoToolbox.Core.Feature.FdoProviderInfo SelectedProvider
        {
            get { return cmbProvider.SelectedItem as FdoProviderInfo; }
        }

        public System.Collections.Specialized.NameValueCollection ConnectProperties
        {
            get 
            {
                NameValueCollection props = new NameValueCollection();
                foreach (DataGridViewRow row in grdProperties.Rows)
                {
                    object n = row.Cells[0].Value;
                    object v = row.Cells[1].Value;
                    if (n != null && v != null)
                    {
                        props.Add(n.ToString(), v.ToString());
                    }
                }
                return props;
            }
        }

        public void FlagNameError(string msg)
        {
            errorProvider1.Clear();
            errorProvider1.SetError(txtConnectionName, msg);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            private set { _Title = value; TitleChanged(this, EventArgs.Empty); }
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

        public IList<FdoProviderInfo> ProviderList
        {
            set 
            {
                cmbProvider.DisplayMember = "DisplayName";
                cmbProvider.DataSource = value;
            }
        }

        public void ResetGrid()
        {
            grdProperties.Rows.Clear();
        }

        public void AddEnumerableProperty(string name, string defaultValue, string[] values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdProperties.Rows.Add(row);
        }

        private void cmbProvider_SelectionChanged(object sender, EventArgs e)
        {
            _presenter.ProviderChanged();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _presenter.TestConnection();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_presenter.Connect())
                MessageService.ShowError("Connection test failed");
            else
                ViewContentClosing(this, EventArgs.Empty);
        }

        public event EventHandler ViewContentClosing = delegate { };

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        public void AddProperty(FdoToolbox.Core.Connections.DictionaryProperty p)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = p.LocalizedName;

            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            if (p.IsFile || p.IsPath)
            {
                valueCell.ContextMenuStrip = ctxHelper;
                valueCell.ToolTipText = "Right click for helpful options";
            }
            valueCell.Value = p.DefaultValue;
            
            //DataGridViewCell valueCell = null;
            //if (p.IsFile)
            //{
            //    DataGridViewFileCell cell = new DataGridViewFileCell();
            //    cell.Mode = DataGridViewFileCell.DataGridViewFileCellMode.File;
            //    cell.Value = p.DefaultValue;
            //    valueCell = cell;
            //}
            //else if (p.IsPath)
            //{
            //    DataGridViewFileCell cell = new DataGridViewFileCell();
            //    cell.Mode = DataGridViewFileCell.DataGridViewFileCellMode.Directory;
            //    cell.Value = p.DefaultValue;
            //    valueCell = cell;
            //}
            //else
            //{
            //    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            //    cell.Value = p.DefaultValue;
            //    valueCell = cell;
            //}

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);
            
            grdProperties.Rows.Add(row);
        }

        private void grdProperties_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                grdProperties.CurrentCell = grdProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void insertCurrentApplicationPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grdProperties.CurrentCell.Value = FileUtility.ApplicationRootPath;
        }

        private void insertFilePathOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_ALL"));
            if (FileService.FileExists(file))
            {
                grdProperties.CurrentCell.Value = file;
            }
        }

        private void insertFilePathSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_FILE"), ResourceService.GetString("FILTER_ALL"));
            if (file != null)
            {
                grdProperties.CurrentCell.Value = file;
            }
        }

        private void insertDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = FileService.GetDirectory(ResourceService.GetString("TITLE_SELECT_DIRECTORY"));
            if (dir != null)
            {
                grdProperties.CurrentCell.Value = dir;
            }
        }
    }
}
