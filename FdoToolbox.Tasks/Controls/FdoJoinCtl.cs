using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoJoinCtl : UserControl, IViewContent
    {
        public FdoJoinCtl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_JOIN_SETTINGS"); }
        }

        public event EventHandler TitleChanged;

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

        public event EventHandler ViewContentClosing;

        public Control ContentControl
        {
            get { return this; }
        }
    }
}
