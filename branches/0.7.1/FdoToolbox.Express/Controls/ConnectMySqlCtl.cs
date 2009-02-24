using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Express.Controls
{
    public partial class ConnectMySqlCtl : ViewContent, IViewContent, IConnectMySqlView
    {
        private ConnectMySqlPresenter _presenter;

        public ConnectMySqlCtl()
        {
            InitializeComponent();
            _presenter = new ConnectMySqlPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_CONNECT_MYSQL"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _presenter.PendingConnect();
        }

        public string Service
        {
            get { return txtService.Text; }
        }

        public string Username
        {
            get { return txtUsername.Text; }
        }

        public string Password
        {
            get { return txtPassword.Text; }
        }

        public bool DataStoreEnabled
        {
            set { cmbDataStore.Enabled = value; }
        }

        public bool SubmitEnabled
        {
            set { btnOK.Enabled = value; }
        }

        public string[] DataStores
        {
            set 
            { 
                cmbDataStore.DataSource = value;
                if (value.Length > 0)
                    cmbDataStore.SelectedIndex = 0;
            }
        }

        public string SelectedDataStore
        {
            get 
            {
                return cmbDataStore.SelectedItem != null ? cmbDataStore.SelectedItem.ToString() : string.Empty;
            }
        }

        public string ConnectionName
        {
            get { return txtConnectionName.Text; }
        }

        public void AlertError(string msg)
        {
            MessageService.ShowError(msg);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                this.Close();
        }
    }
}
