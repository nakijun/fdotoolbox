using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoUnregProviderCtl : UserControl, IViewContent, IFdoUnregProviderView
    {
        private FdoUnregProviderPresenter _presenter;

        public FdoUnregProviderCtl()
        {
            InitializeComponent();
            _presenter = new FdoUnregProviderPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.GetProviders();
            base.OnLoad(e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_UNREGISTER_PROVIDER"); }
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

        private void lstProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.SelectionChanged();
        }

        public IList<FdoToolbox.Core.Feature.FdoProviderInfo> ProviderList
        {
            set 
            {
                lstProviders.DisplayMember = "DisplayName";
                lstProviders.DataSource = value; 
            }
        }

        public IList<string> SelectedProviders
        {
            get 
            {
                IList<string> names = new List<string>();
                foreach (object obj in lstProviders.SelectedItems)
                {
                    names.Add((obj as FdoProviderInfo).Name);
                }
                return names;
            }
        }

        public bool UnregEnabled
        {
            set { btnUnregister.Enabled = value; }
        }

        private void btnUnregister_Click(object sender, EventArgs e)
        {
            if (_presenter.Unregister())
            {
                MessageService.ShowMessage(ResourceService.GetString("MSG_PROVIDER_UNREGISTERED"), ResourceService.GetString("TITLE_UNREGISTER_PROVIDER"));
                _presenter.GetProviders();
            }
        }
    }
}
