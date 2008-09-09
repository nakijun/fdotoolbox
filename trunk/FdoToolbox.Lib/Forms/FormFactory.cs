#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Lib.Controls;

namespace FdoToolbox.Lib.Forms
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
