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

namespace FdoToolbox.Tasks.Controls
{
    public partial class EtlProcessCtl : UserControl, IViewContent
    {
        private EtlProcessCtl()
        {
            InitializeComponent();
        }

        private EtlBackgroundRunner _runner;

        public EtlProcessCtl(EtlProcess proc)
            : this()
        {
            _runner = new EtlBackgroundRunner(proc);
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
