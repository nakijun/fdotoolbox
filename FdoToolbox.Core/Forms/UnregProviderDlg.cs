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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Forms
{
    public partial class UnregProviderDlg : Form
    {
        public UnregProviderDlg()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            using (ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders())
            {
                foreach (Provider prov in providers)
                {
                    lstProviders.Items.Add(prov.Name);
                }
            }
            CheckButtonStatus();
            base.OnLoad(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string name = lstProviders.SelectedItem.ToString();
                FeatureAccessManager.GetProviderRegistry().UnregisterProvider(name);
                AppConsole.Alert("Unregister Provider", "Provider has been unregistered");
                this.DialogResult = DialogResult.OK;
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
            }
        }

        private void lstProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckButtonStatus();
        }

        private void CheckButtonStatus()
        {
            btnOK.Enabled = (lstProviders.SelectedItems.Count == 1);
        }
    }
}