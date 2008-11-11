using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoSqlQueryCtl : UserControl, ISubView, IFdoSqlQueryView
    {
        private TextEditorControl _editor;

        public FdoSqlQueryCtl()
        {
            InitializeComponent();
            _editor = new TextEditorControl();
            _editor.Dock = DockStyle.Fill;
            this.Controls.Add(_editor);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string SQLString
        {
            get { return _editor.Text; }
        }
    }
}
