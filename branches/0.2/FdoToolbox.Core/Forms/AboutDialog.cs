using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace FdoToolbox.Core.Forms
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            lblAppVersion.Text = "Version " + HostApplication.Instance.Version;
            lblProjectUrl.Text = HostApplication.Instance.ProjectUrl;
            txtAbout.Text = Properties.Resources.APP_ABOUT;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}