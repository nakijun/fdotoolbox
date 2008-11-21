using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public partial class BasePreferencesCtl : UserControl, IPreferenceSheet
    {
        public BasePreferencesCtl()
        {
            InitializeComponent();
            numLimit.Minimum = decimal.MinValue;
            numLimit.Maximum = decimal.MaxValue;
        }

        private void BasePreferencesCtl_Load(object sender, EventArgs e)
        {
            txtFdoPath.Text = Preferences.FdoPath;
            txtLogPath.Text = Preferences.LogPath;
            txtSession.Text = Preferences.SessionDirectory;
            txtWorking.Text = Preferences.WorkingDirectory;
            numLimit.Value = Convert.ToDecimal(Preferences.DataPreviewWarningLimit);
        }

        private void btnFdo_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select FDO Path");
            if (path != null)
            {
                txtFdoPath.Text = path;
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Log Path");
            if (path != null)
            {
                txtLogPath.Text = path;
            }
        }

        private void btnWorking_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Working Directory");
            if (path != null)
            {
                txtWorking.Text = path;
            }
        }

        private void btnSession_Click(object sender, EventArgs e)
        {
            string path = FileService.GetDirectory("Select Session Path");
            if (path != null)
            {
                txtSession.Text = path;
            }
        }


        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_PREFS_GENERAL"); }
        }

        public void ApplyChanges()
        {
            Preferences.DataPreviewWarningLimit = Convert.ToInt32(numLimit.Value);
            Preferences.FdoPath = txtFdoPath.Text;
            Preferences.LogPath = txtLogPath.Text;
            Preferences.SessionDirectory = txtSession.Text;
            Preferences.WorkingDirectory = txtWorking.Text;
        }
    }
}
