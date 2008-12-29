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
