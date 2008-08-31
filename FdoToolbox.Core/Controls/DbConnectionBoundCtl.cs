using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Core.Controls
{
    public partial class DbConnectionBoundCtl : BaseDocumentCtl
    {
        protected DbConnectionInfo _BoundConnection;

        protected string _Key;

        internal DbConnectionBoundCtl()
        {
            InitializeComponent();
        }

        public DbConnectionBoundCtl(DbConnectionInfo connInfo, string key)
            : this()
        {
            _BoundConnection = connInfo;
            _Key = key;
        }

        public DbConnectionInfo BoundConnection
        {
            get { return _BoundConnection; }
        }

        public string GetKey() { return _Key; }

        public void SetKey(string key) { _Key = key; }

        public virtual string GetTabType()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
