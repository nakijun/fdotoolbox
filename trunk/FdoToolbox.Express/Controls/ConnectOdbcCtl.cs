using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;

namespace FdoToolbox.Express.Controls
{
    public partial class ConnectOdbcCtl : UserControl, IViewContent, IConnectOdbcView
    {
        private ConnectOdbcPresenter _presenter;

        public ConnectOdbcCtl()
        {
            InitializeComponent();
            _presenter = new ConnectOdbcPresenter(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public FdoToolbox.Express.Controls.Odbc.OdbcType[] OdbcTypes
        {
            set { cmbDataType.DataSource = value; }
        }

        public FdoToolbox.Express.Controls.Odbc.OdbcType SelectedOdbcType
        {
            get { return (FdoToolbox.Express.Controls.Odbc.OdbcType)cmbDataType.SelectedItem; }
        }

        public FdoToolbox.Express.Controls.Odbc.IOdbcConnectionBuilder BuilderObject
        {
            get
            {
                return (FdoToolbox.Express.Controls.Odbc.IOdbcConnectionBuilder)propGrid.SelectedObject;
            }
            set
            {
                propGrid.SelectedObject = value;
            }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_CONNECT_ODBC"); }
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

        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.OdbcTypeChanged();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _presenter.TestConnection();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                ViewContentClosing(this, EventArgs.Empty);
        }


        public string ConnectionName
        {
            get { return txtName.Text; }
        }
    }
}
