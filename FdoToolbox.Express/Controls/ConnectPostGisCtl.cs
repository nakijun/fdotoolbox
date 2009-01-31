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
    public partial class ConnectPostGisCtl : ViewContent, IViewContent, IConnectPostGisView
    {
        private ConnectPostGisPresenter _presenter;

        public ConnectPostGisCtl()
        {
            InitializeComponent();
            _presenter = new ConnectPostGisPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_CONNECT_POSTGIS"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
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

        public string DataStore
        {
            get { return txtDataStore.Text; }
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
