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
            this.Text = (task.TaskType == TaskType.BulkCopy) ? "Bulk Copy in progress" : "Database join in progress";
            _Task.OnTaskMessage += new TaskProgressMessageEventHandler(OnTaskMessage);
            _Task.OnItemProcessed += new TaskPercentageEventHandler(OnItemProcessed);
            _Task.OnLogTaskMessage += new TaskProgressMessageEventHandler(OnLogTaskMessage);
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
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
            _Task.Execute();
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
            bgWorker.RunWorkerAsync();
            this.ShowDialog();
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