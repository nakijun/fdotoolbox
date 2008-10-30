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
using System.Threading;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Base class for executable tasks.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        private string _Name;

        /// <summary>
        /// The name of the task
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Validates the parameters of the task before execution
        /// </summary>
        public abstract void ValidateTaskParameters();

        /// <summary>
        /// Assigns the executing thread to the thread that invoked this method
        /// and then begins execution.
        /// </summary>
        public void Execute()
        {
            this.ExecutingThread = Thread.CurrentThread;
            DoExecute();
        }

        /// <summary>
        /// Perform the actual execution. Be warned that the thread invoking 
        /// this method can be cancelled at any time by the user. The implementation
        /// must catch ThreadAbortException, call Thread.ResetAbort() and perform 
        /// any necessary cleanups.
        /// </summary>
        public abstract void DoExecute();

        /// <summary>
        /// Gets the type of this task
        /// </summary>
        public abstract TaskType TaskType
        {
            get;
        }

        /// <summary>
        /// Returns true if this task can be counted
        /// </summary>
        public abstract bool IsCountable
        {
            get;
        }

        /// <summary>
        /// Fired when a item is processed in the task
        /// </summary>
        public event TaskPercentageEventHandler OnItemProcessed;

        /// <summary>
        /// Fired when a message is sent from the task
        /// </summary>
        public event TaskProgressMessageEventHandler OnTaskMessage;

        /// <summary>
        /// Fired when a message to be logged is sent from the task
        /// </summary>
        public event TaskProgressMessageEventHandler OnLogTaskMessage;

        /// <summary>
        /// Sends a task message
        /// </summary>
        /// <param name="msg"></param>
        protected void SendMessage(string msg)
        {
            if (this.OnTaskMessage != null)
                this.OnTaskMessage(this, new EventArgs<string>(msg));
            if (this.OnLogTaskMessage != null)
                this.OnLogTaskMessage(this, new EventArgs<string>(msg));
        }

        /// <summary>
        /// Send the current progress
        /// </summary>
        /// <param name="count"></param>
        protected void SendCount(int count)
        {
            if (this.OnItemProcessed != null)
                this.OnItemProcessed(this, new EventArgs<int>(count));
        }

        private Thread _RunningThread;

        /// <summary>
        /// The thread that is executing this task
        /// </summary>
        public Thread ExecutingThread
        {
            get { return _RunningThread; }
            protected set { _RunningThread = value; }
        }
    }
}
