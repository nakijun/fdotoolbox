using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    public class TaskManager : ITaskManager
    {
        private Dictionary<string, ITask> _Tasks;

        public TaskManager()
        {
            _Tasks = new Dictionary<string, ITask>();
        }

        public void AddTask(ITask task)
        {
            _Tasks.Add(task.Name, task);
            if (this.TaskAdded != null)
                this.TaskAdded(task.Name);
        }

        public void RemoveTask(string name)
        {
            _Tasks.Remove(name);
            if (this.TaskRemoved != null)
                this.TaskRemoved(name);
        }

        public ITask GetTask(string name)
        {
            if (_Tasks.ContainsKey(name))
                return _Tasks[name];
            return null;
        }

        public ICollection<string> TaskNames
        {
            get { return _Tasks.Keys; }
        }

        public event TaskEventHandler TaskAdded;

        public event TaskEventHandler TaskRemoved;
    }
}
