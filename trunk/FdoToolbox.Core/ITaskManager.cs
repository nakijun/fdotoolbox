using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    public delegate void TaskEventHandler(string name);

    public interface ITaskManager
    {
        void AddTask(ITask task);
        void RemoveTask(string name);
        ITask GetTask(string name);
        ICollection<string> TaskNames { get;}

        event TaskEventHandler TaskAdded;
        event TaskEventHandler TaskRemoved;
    }
}
