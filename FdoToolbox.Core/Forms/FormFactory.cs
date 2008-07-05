using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Controls;

namespace FdoToolbox.Core.Forms
{
    public sealed class FormFactory
    {
        public static Form CreateFormForControl(BaseDocumentCtl ctl)
        {
            Form frm = new Form();
            int width = ctl.Width;
            int height = ctl.Height;
            frm.Text = ctl.Title;
            ctl.Dock = DockStyle.Fill;
            ctl.OnAccept += delegate { frm.DialogResult = DialogResult.OK; };
            ctl.OnCancel += delegate { frm.DialogResult = DialogResult.Cancel; };
            ctl.OnClose += delegate { frm.Close(); };
            frm.Controls.Add(ctl);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Width = width;
            frm.Height = height;
            return frm;
        }

        public static Form CreateFormForControl(BaseDocumentCtl ctl, int width, int height)
        {
            Form frm = CreateFormForControl(ctl);
            frm.Width = width;
            frm.Height = height;
            return frm;
        }
    }
}
