using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;

namespace FdoToolbox.Base.Services
{
    public abstract class ManagerBase<T> where T : class
    {
        protected Dictionary<string, T> _dict;

        protected ManagerBase()
        {
            _dict = new Dictionary<string, T>();
        }

        public ICollection<string> GetNames()
        {
            return _dict.Keys;
        }

        public virtual void Add(string name, T value)
        {
            if (_dict.ContainsKey(name))
                throw new FdoConnectionException("Unable to add object named " + name + " to the manager");

            _dict.Add(name, value);
            ObjectAdded(this, new EventArgs<string>(name));
        }

        public virtual T Get(string name)
        {
            if (_dict.ContainsKey(name))
                return _dict[name];
            return null;
        }

        public virtual void Rename(string oldName, string newName)
        {
            if(!_dict.ContainsKey(oldName))
                throw new InvalidOperationException("The object to be renamed could not be found: " + oldName);
            if (_dict.ContainsKey(newName))
                throw new InvalidOperationException("Cannot rename object " + oldName + " to " + newName + " as an object of that name already exists");

            T value = _dict[oldName];
            _dict.Remove(oldName);
            _dict.Add(newName, value);

            ManagerObjectRenameArgs e = new ManagerObjectRenameArgs(oldName, newName);
            ObjectRenamed(this, e);
        }

        public virtual void Remove(string name)
        {
            if (_dict.ContainsKey(name))
            {
                ManagerObjectBeforeRemoveEventArgs e = new ManagerObjectBeforeRemoveEventArgs(name);
                BeforeRemove(this, e);
                if (e.Cancel)
                    return;

                _dict.Remove(name);
                ObjectRemoved(this, new EventArgs<string>(name));
            }
        }

        public virtual bool NameExists(string name)
        {
            return _dict.ContainsKey(name);
        }

        public virtual void Clear()
        {
            List<string> names = new List<string>(GetNames());
            foreach (string name in names)
            {
                this.Remove(name);
            }
        }

        public event ManagerObjectBeforeRemoveEventHandler BeforeRemove = delegate { };
        public event ManagerObjectEventHandler ObjectAdded = delegate { };
        public event ManagerObjectEventHandler ObjectRemoved = delegate { };
        public event ManagerObjectRenamedEventHandler ObjectRenamed = delegate { };
    }

    public delegate void ManagerObjectBeforeRemoveEventHandler(object sender, ManagerObjectBeforeRemoveEventArgs e);
    public delegate void ManagerObjectEventHandler(object sender, EventArgs<string> e);
    public delegate void ManagerObjectRenamedEventHandler(object sender, ManagerObjectRenameArgs e);

    public class ManagerObjectBeforeRemoveEventArgs
    {
        public readonly string Name;

        private bool _Cancel;

        public bool Cancel
        {
            get { return _Cancel; }
            set { _Cancel = value; }
        }

        public ManagerObjectBeforeRemoveEventArgs(string name)
        {
            this.Name = name;
        }
    }

    public class ManagerObjectRenameArgs
    {
        public readonly string OldName;
        public readonly string NewName;

        public ManagerObjectRenameArgs(string oldName, string newName)
        {
            this.OldName = oldName;
            this.NewName = newName;
        }
    }
}
