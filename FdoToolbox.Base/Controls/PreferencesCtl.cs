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

        public event EventHandler TitleChanged;

        public bool CanClose
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Save()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool SaveAs()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler ViewContentClosing;

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
