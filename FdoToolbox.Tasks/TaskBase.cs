using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Tasks
{
    public abstract class TaskBase : ITask
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
	
    }
}
