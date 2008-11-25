using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoBulkCopyCtl : UserControl, IViewContent
    {
        public FdoBulkCopyCtl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return "Bulk Copy Settings"; }
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
    }
}
