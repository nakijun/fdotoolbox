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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FdoToolbox.Lib.ClientServices;

namespace FdoToolbox.Lib.Controls
{
    public partial class HelpCtl : BaseDocumentCtl
    {
        public HelpCtl()
        {
            InitializeComponent();
            this.Title = "Help";
        }

        protected override void OnLoad(EventArgs e)
        {
            GoHome();
            base.OnLoad(e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        private void GoHome()
        {
            IHostApplication app = AppGateway.RunningApplication;
            browser.Navigate(Path.Combine(app.AppPath, "Help\\index.htm"));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void GoBack()
        {
            browser.GoBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            GoForward();
        }

        private void GoForward()
        {
            browser.GoForward();
        }
    }
}
