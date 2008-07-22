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

        public void UpdateTask(string taskName, ITask task)
        {
            _Tasks.Remove(taskName);
            _Tasks.Add(taskName, task);
        }
    }
}
