using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using FdoToolbox.Core;
using ICSharpCode.Core;

namespace FdoToolbox.Tasks
{
    /// <summary>
    /// Task service activity logger
    /// </summary>
    public sealed class EventWatcher
    {
        public static void Initialize()
        {
            TaskManager tm = ServiceManager.Instance.GetService<TaskManager>();
            tm.TaskAdded += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Task added: {0}", e.Data);
            };
            tm.TaskRemoved += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Task removed: {0}", e.Data);
            };
            tm.TaskRenamed += delegate(object sender, TaskRenameEventArgs e)
            {
                LoggingService.InfoFormatted("Task {0} renamed to {1}", e.OldName, e.NewName);
            };
        }
    }
}
