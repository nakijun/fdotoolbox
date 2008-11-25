using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;

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

        public event EventHandler Initialize = delegate { };

        public event EventHandler Unload = delegate { };

        private Dictionary<string, ITask> _taskDict = new Dictionary<string, ITask>();

        public void AddTask(ITask task)
        {
            if (_taskDict.ContainsKey(task.Name))
                throw new ArgumentException("A task named " + task.Name + " already exists");

            _taskDict.Add(task.Name, task);
        }

        public void RemoveTask(string name)
        {
            if (_taskDict.ContainsKey(name))
            {
                _taskDict.Remove(name);
            }
        }

        public ITask GetTask(string name)
        {
            if (_taskDict.ContainsKey(name))
                return _taskDict[name];

            return null;
        }
    }
}
