using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    public interface ITask
    {
        string Name { get; set; }
        void Execute();
        TaskType TaskType { get; }

        event TaskPercentageEventHandler OnItemProcessed;
        event TaskProgressMessageEventHandler OnTaskMessage;
    }

    public delegate void TaskPercentageEventHandler(int pc);

    public delegate void TaskProgressMessageEventHandler(string msg);

    public enum TaskType
    {
        BulkCopy,
        DbJoin
    }
}
