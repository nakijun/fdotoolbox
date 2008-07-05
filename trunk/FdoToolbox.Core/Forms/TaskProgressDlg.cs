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
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
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
            btnOK.Enabled = true;
            btnCancel.Enabled = false;
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
            this.DialogResult = DialogResult.Cancel;
        }
    }
}