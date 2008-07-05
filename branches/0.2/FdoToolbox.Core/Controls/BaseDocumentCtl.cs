using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Core.Controls
{
    public delegate void TabTitleEventHandler(string title);

    public partial class BaseDocumentCtl : UserControl
    {
        public BaseDocumentCtl()
        {
            InitializeComponent();
        }

        public virtual string Title
        {
            get { return ""; }
            set
            {
                if (this.ParentForm != null)
                {
                    this.ParentForm.Text = value;
                    if (this.OnSetTabText != null)
                        this.OnSetTabText(value);
                }
            }
        }

        public event TabTitleEventHandler OnSetTabText;

        public event EventHandler OnClose;

        public event EventHandler OnAccept;

        public event EventHandler OnCancel;

        public void Accept()
        {
            if (this.OnAccept != null)
                this.OnAccept(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            if (this.OnCancel != null)
                this.OnCancel(this, EventArgs.Empty);
        }

        public void Close()
        {
            if (this.OnClose != null)
                this.OnClose(this, EventArgs.Empty);
        }
    }
}
