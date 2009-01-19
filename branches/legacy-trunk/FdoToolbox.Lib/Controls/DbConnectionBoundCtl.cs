using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Lib.Controls
{
    public partial class DbConnectionBoundCtl : BaseDocumentCtl
    {
        protected DatabaseConnection _BoundConnection;

        protected string _Key;

        internal DbConnectionBoundCtl()
        {
            InitializeComponent();
        }

        public DbConnectionBoundCtl(DatabaseConnection connInfo, string key)
            : this()
        {
            _BoundConnection = connInfo;
            _Key = key;
        }

        public DatabaseConnection BoundConnection
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