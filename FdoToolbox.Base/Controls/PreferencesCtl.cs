using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public partial class PreferencesCtl : UserControl, IPreferencesView
    {
        private PreferencesCtlPresenter _presenter;

        public PreferencesCtl()
        {
            InitializeComponent();
            _presenter = new PreferencesCtlPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.LoadPreferences();
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_PREFERENCES"); }
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            _presenter.SaveChanges();
            MessageService.ShowMessage(ResourceService.GetString("MSG_PREFS_SAVED"));
            ViewContentClosing(this, EventArgs.Empty);
        }

        private IList<IPreferenceSheet> _sheets;

        public IList<IPreferenceSheet> Sheets
        {
            get
            {
                return _sheets;
            }
            set
            {
                _sheets = value;
                tabOptions.TabPages.Clear();
                foreach (IPreferenceSheet sh in _sheets)
                {
                    TabPage page = new TabPage(sh.Title);
                    sh.ContentControl.Dock = DockStyle.Fill;
                    page.Controls.Add(sh.ContentControl);
                    tabOptions.TabPages.Add(page);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }
    }
}
