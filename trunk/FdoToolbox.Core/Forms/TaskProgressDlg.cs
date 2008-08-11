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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Forms
{
    public partial class TaskProgressDlg : Form
    {
        internal TaskProgressDlg()
        {
            InitializeComponent();
        }

        private ITask _Task;

        public TaskProgressDlg(ITask task)
            : this()
        {
            _Task = task;
            progressBar.Style = (_Task.IsCountable) ? ProgressBarStyle.Continuous : ProgressBarStyle.Marquee;
            this.Text = (task.TaskType == TaskType.BulkCopy) ? "Bulk Copy in progress" : "Database join in progress";
            _Task.OnTaskMessage += new TaskProgressMessageEventHandler(OnTaskMessage);
            _Task.OnItemProcessed += new TaskPercentageEventHandler(OnItemProcessed);
            _Task.OnLogTaskMessage += new TaskProgressMessageEventHandler(OnLogTaskMessage);
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            this.Disposed += delegate { bgWorker.Dispose(); };
        }

        void OnLogTaskMessage(string msg)
        {
            AppConsole.WriteLine("[{0}]: {1}", _Task.TaskType, msg);
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        void OnItemProcessed(int pc)
        {
            bgWorker.ReportProgress(pc);
        }

        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            if (e.Cancelled)
            {
                lblMessage.Text = "Bulk Copy Cancelled";
            }
            else
            {
                btnOK.Enabled = true;
                btnCancel.Enabled = false;
            }
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _Task.Execute();
            }
            catch (Exception ex)
            {
                OnTaskMessage(ex.Message);
                AppConsole.WriteException(ex);
                bgWorker.CancelAsync();
            }
        }

        void OnTaskMessage(string msg)
        {
            if (lblMessage.InvokeRequired)
                lblMessage.Invoke(new EventHandler(delegate(object sender, EventArgs e) { lblMessage.Text = msg; }));
            else
                lblMessage.Text = msg;
        }

        public void Run()
        {
            bool valid = true;
            try
            {
                _Task.ValidateTaskParameters();
                valid = true;
            }
            catch (TaskValidationException ex)
            {
                AppConsole.Alert("Error in Validating Task", ex.Message + "\n\nTask execution will not proceed");
                AppConsole.WriteException(ex);
                valid = false;
            }
            if (valid)
            {
                bgWorker.RunWorkerAsync();
                this.ShowDialog();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bgWorker.CancelAsync();
            this.DialogResult = DialogResult.Cancel;
        }
    }
}