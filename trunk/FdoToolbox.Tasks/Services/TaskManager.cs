using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.ETL;

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

        private Dictionary<string, EtlProcess> _taskDict = new Dictionary<string, EtlProcess>();

        public void AddTask(string name, EtlProcess task)
        {
            if (_taskDict.ContainsKey(name))
                throw new ArgumentException("A task named " + name + " already exists");

            _taskDict.Add(name, task);
        }

        public void RemoveTask(string name)
        {
            if (_taskDict.ContainsKey(name))
            {
                _taskDict.Remove(name);
            }
        }

        public EtlProcess GetTask(string name)
        {
            if (_taskDict.ContainsKey(name))
                return _taskDict[name];

            return null;
        }
    }
}
