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

namespace FdoToolbox.Core.ETL
{
    public abstract class TaskBase : ITask
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public abstract void ValidateTaskParameters();

        public abstract void Execute();

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
                this.OnTaskMessage(msg);
            if (this.OnLogTaskMessage != null)
                this.OnLogTaskMessage(msg);
        }

        protected void SendCount(int count)
        {
            if (this.OnItemProcessed != null)
                this.OnItemProcessed(count);
        }
    }
}
