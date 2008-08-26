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

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Task definition manager
    /// </summary>
    public class TaskManager : ITaskManager
    {
        private Dictionary<string, ITask> _Tasks;

        /// <summary>
        /// Constructor
        /// </summary>
        public TaskManager()
        {
            _Tasks = new Dictionary<string, ITask>();
        }

        /// <summary>
        /// Adds a new task definition
        /// </summary>
        /// <param name="task">The task definition</param>
        public void AddTask(ITask task)
        {
            _Tasks.Add(task.Name, task);
            if (this.TaskAdded != null)
                this.TaskAdded(task.Name);
        }

        /// <summary>
        /// Removes a task definition (by name)
        /// </summary>
        /// <param name="name">The name of the task</param>
        public void RemoveTask(string name)
        {
            _Tasks.Remove(name);
            if (this.TaskRemoved != null)
                this.TaskRemoved(name);
        }

        /// <summary>
        /// Gets a task definition (by name)
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <returns>The task definition if found, null otherwise</returns>
        public ITask GetTask(string name)
        {
            if (_Tasks.ContainsKey(name))
                return _Tasks[name];
            return null;
        }

        /// <summary>
        /// Gets the names of all the loaded task definitions
        /// </summary>
        public ICollection<string> TaskNames
        {
            get { return _Tasks.Keys; }
        }

        /// <summary>
        /// Fires when a task definition is added
        /// </summary>
        public event TaskEventHandler TaskAdded;

        /// <summary>
        /// Fires when a task definition is removed
        /// </summary>
        public event TaskEventHandler TaskRemoved;

        /// <summary>
        /// Updates an existing task definition (by name)
        /// </summary>
        /// <param name="taskName">The name of the existing task</param>
        /// <param name="task">The task definition</param>
        public void UpdateTask(string taskName, ITask task)
        {
            _Tasks.Remove(taskName);
            _Tasks.Add(taskName, task);
        }
    }
}
