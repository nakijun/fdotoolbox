using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core;

namespace FdoToolbox.Tasks.Services
{
    public class TaskManager : IService
    {
        private bool _init = false;

        public bool IsInitialized
        {
            get { return _init; }
        }

        public void InitializeService()
        {
            _init = true;
        }

        public void UnloadService()
        {
            
        }

        public void Load()
        {
            
        }

        public void Save()
        {
            
        }

        public event TaskRenameEventHandler TaskRenamed = delegate { };

        public event TaskEventHandler TaskAdded = delegate { };

        public event TaskEventHandler TaskRemoved = delegate { };

        public event EventHandler Initialize = delegate { };

        public event EventHandler Unload = delegate { };

        private Dictionary<string, EtlProcess> _taskDict = new Dictionary<string, EtlProcess>();

        public void AddTask(string name, EtlProcess task)
        {
            if (_taskDict.ContainsKey(name))
                throw new ArgumentException("A task named " + name + " already exists");

            _taskDict.Add(name, task);
            TaskAdded(this, new EventArgs<string>(name));
        }

        public void RemoveTask(string name)
        {
            if (_taskDict.ContainsKey(name))
            {
                EtlProcess proc = _taskDict[name];
                _taskDict.Remove(name);
                proc.Dispose();
                TaskRemoved(this, new EventArgs<string>(name));
            }
        }

        public void RenameTask(string oldName, string newName)
        {
            if (!_taskDict.ContainsKey(oldName))
                throw new InvalidOperationException("The task to be renamed could not be found: " + oldName);
            if (_taskDict.ContainsKey(newName))
                throw new InvalidOperationException("Cannot rename task " + oldName + " to " + newName + " as a task of that name already exists");

            EtlProcess proc = _taskDict[oldName];
            _taskDict.Remove(oldName);
            _taskDict.Add(newName, proc);
            TaskRenamed(this, new TaskRenameEventArgs(oldName, newName));
        }

        public EtlProcess GetTask(string name)
        {
            if (_taskDict.ContainsKey(name))
                return _taskDict[name];

            return null;
        }

        public bool NameExists(string name)
        {
            return _taskDict.ContainsKey(name);
        }
    }

    public delegate void TaskEventHandler(object sender, EventArgs<string> e);

    public delegate void TaskRenameEventHandler(object sender, TaskRenameEventArgs e);

    public class TaskRenameEventArgs : EventArgs
    {
        private readonly string _oldName;
        private readonly string _newName;

        public string OldName
        {
            get { return _oldName; }
        }

        public string NewName
        {
            get { return _newName; }
        }

        public TaskRenameEventArgs(string oldName, string newName)
        {
            _oldName = oldName;
            _newName = newName;
        }
    }
}
