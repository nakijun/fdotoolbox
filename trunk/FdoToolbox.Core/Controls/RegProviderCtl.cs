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
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core;
using FdoToolbox.Core.Controls;

namespace FdoToolbox.Core.Controls
{
    public partial class RegProviderCtl : BaseDocumentCtl
    {
        public RegProviderCtl()
        {
            InitializeComponent();
            this.Title = "Register Provider";
            openFileDialog.InitialDirectory = HostApplication.Instance.AppPath;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtLibraryPath.Text = openFileDialog.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                FeatureAccessManager.GetProviderRegistry().RegisterProvider(
                    txtName.Text,
                    txtDisplayName.Text,
                    txtDescription.Text,
                    txtVersion.Text,
                    txtFdoVersion.Text,
                    txtLibraryPath.Text,
                    chkManaged.Checked
                );
                AppConsole.Alert("", "New provider registered: " + txtName.Text);
                this.Close();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
            }
        }
    }
}
