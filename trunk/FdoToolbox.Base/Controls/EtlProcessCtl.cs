#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Base.Controls
{
    public partial class EtlProcessCtl : UserControl, IViewContent
    {
        private EtlProcessCtl()
        {
            InitializeComponent();
        }

        private EtlBackgroundRunner _runner;

        public EtlProcessCtl(IFdoSpecializedEtlProcess proc)
            : this()
        {
            _runner = new EtlBackgroundRunner(proc);
            _runner.ProcessMessage += new MessageEventHandler(OnMessageSent);
        }

        void OnMessageSent(object sender, FdoToolbox.Core.MessageEventArgs e)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new AppendTextHandler(this.AppendText), e.Message);
            }
            else
            {
                this.AppendText(e.Message);
            }
        }

        private delegate void AppendTextHandler(string msg);

        private void AppendText(string msg)
        {
            txtOutput.AppendText(msg + "\n");
            txtOutput.ScrollToCaret();
        }

        protected override void OnLoad(EventArgs e)
        {
            bgEtlProc.RunWorkerAsync();
            base.OnLoad(e);
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_ETL_PROCESS"); }
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

        public event EventHandler ViewContentClosing = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        private void bgEtlProc_DoWork(object sender, DoWorkEventArgs e)
        {
            _runner.Run();
        }

        private void bgEtlProc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnOK.Enabled = true;
            btnCancel.Enabled = false;
            if (e.Cancelled)
            {
                AppendText("ETL Process Cancelled!");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_runner.ExecutingThread.IsAlive)
                _runner.ExecutingThread.Abort();
            else
                ViewContentClosing(this, EventArgs.Empty);
        }
    }
}
