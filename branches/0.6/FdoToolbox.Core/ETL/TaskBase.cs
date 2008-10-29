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

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

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

        public abstract TaskType TaskType
        {
            get;
        }

        public abstract bool IsCountable
        {
            get;
        }

        public event TaskPercentageEventHandler OnItemProcessed;

        public event TaskProgressMessageEventHandler OnTaskMessage;

        public event TaskProgressMessageEventHandler OnLogTaskMessage;

        protected void SendMessage(string msg)
        {
            if (this.OnTaskMessage != null)
                this.OnTaskMessage(this, new EventArgs<string>(msg));
            if (this.OnLogTaskMessage != null)
                this.OnLogTaskMessage(this, new EventArgs<string>(msg));
        }

        protected void SendCount(int count)
        {
            if (this.OnItemProcessed != null)
                this.OnItemProcessed(this, new EventArgs<int>(count));
        }

        private Thread _RunningThread;

        public Thread ExecutingThread
        {
            get { return _RunningThread; }
            protected set { _RunningThread = value; }
        }
    }
}
