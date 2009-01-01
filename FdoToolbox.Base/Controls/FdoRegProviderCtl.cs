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
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoRegProviderCtl : UserControl, IViewContent, IFdoRegProviderView
    {
        private FdoRegProviderPresentation _presenter;

        public FdoRegProviderCtl()
        {
            InitializeComponent();
            _presenter = new FdoRegProviderPresentation(this);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_REGISTER_PROVIDER"); }
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtLibraryPath.Text = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_DLL"));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (_presenter.Register())
            {
                MessageService.ShowMessage(ResourceService.GetString("MSG_PROVIDER_REGISTERED"), ResourceService.GetString("TITLE_REGISTER_PROVIDER"));
                ViewContentClosing(this, EventArgs.Empty);
            }
        }

        public string ProviderName
        {
            get { return txtName.Text; }
        }

        public string DisplayName
        {
            get { return txtDisplayName.Text; }
        }

        public string Description
        {
            get { return txtDescription.Text; }
        }

        public string Version
        {
            get { return txtVersion.Text; }
        }

        public string FdoVersion
        {
            get { return txtFdoVersion.Text; }
        }

        public string LibraryPath
        {
            get { return txtLibraryPath.Text; }
        }

        public bool IsManaged
        {
            get { return chkManaged.Checked; }
        }
    }
}
