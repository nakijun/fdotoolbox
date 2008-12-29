using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using System.Collections.Specialized;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoDataStoreMgrCtl : UserControl, IFdoDataStoreMgrView, IViewContent
    {
        private FdoDataStoreMgrPresenter _presenter;

        public FdoDataStoreMgrCtl()
        {
            InitializeComponent();
        }

        public FdoDataStoreMgrCtl(FdoConnection conn)
            : this()
        {
            _presenter = new FdoDataStoreMgrPresenter(this, conn);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public bool AddEnabled
        {
            set { btnCreate.Enabled = value; }
        }

        public bool DestroyEnabled
        {
            set { btnDestroy.Enabled = value; }
        }

        public IList<DataStoreInfo> DataStores
        {
            set { grdDataStores.DataSource = value; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_DATA_STORE_MGMT"); }
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

        public Control ContentControl
        {
            get { return this; }
        }

        public string Message
        {
            set { lblMessage.Text = value; }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            NameValueCollection props = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_CREATE_DATASTORE"), FdoFeatureService.GetCreateDataStoreProperties(_presenter.Connection.Provider));
            if (props != null)
            {
                _presenter.CreateDataStore(props);
                MessageService.ShowMessage(ResourceService.GetString("MSG_DATA_STORE_CREATED"));
            }
        }

        private void btnDestroy_Click(object sender, EventArgs e)
        {
            NameValueCollection props = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_DESTROY_DATASTORE"), FdoFeatureService.GetCreateDataStoreProperties(_presenter.Connection.Provider));
            if (props != null)
            {
                _presenter.DestroyDataStore(props);
                MessageService.ShowMessage(ResourceService.GetString("MSG_DATA_STORE_DESTROYED"));
            }
        }
    }
}
