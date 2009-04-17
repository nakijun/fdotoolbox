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
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Express.Controls
{
    public partial class ConnectPostGisCtl : ViewContent, IConnectPostGisView
    {
        private ConnectPostGisPresenter _presenter;

        public ConnectPostGisCtl()
        {
            InitializeComponent();
            _presenter = new ConnectPostGisPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_CONNECT_POSTGIS"); }
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
