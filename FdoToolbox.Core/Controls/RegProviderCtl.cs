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
        }

        public override string Title
        {
            get
            {
                return "Register Provider";
            }
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
