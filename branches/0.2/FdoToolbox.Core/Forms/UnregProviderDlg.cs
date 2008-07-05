using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core;
using OSGeo.FDO.ClientServices;

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