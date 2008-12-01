using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using FdoToolbox.Base;
using FdoToolbox.Core.ETL;
using System.Windows.Forms;

using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Tasks.Controls;

namespace FdoToolbox.Tasks.Commands
{
    public class ExecuteTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();
            string name = Workbench.Instance.ObjectExplorer.GetSelectedNode().Name;
            EtlProcess proc = mgr.GetTask(name);

            if (proc != null)
            {
                EtlProcessCtl ctl = new EtlProcessCtl(proc);
                Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
            }
        }
    }

    public class RenameTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskName = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();

            string name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_TASK"), Res.GetString("PROMPT_ENTER_NEW_TASK_NAME"), taskName.Name);
            if (name == null)
                return;

            while (name == string.Empty || mgr.NameExists(name))
            {
                name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_TASK"), Res.GetString("PROMPT_ENTER_NEW_TASK_NAME"), taskName.Name);
                if (name == null)
                    return;
            }

            mgr.RenameTask(taskName.Name, name);
        }
    }

    public class DeleteTaskCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode taskNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            TaskManager mgr = ServiceManager.Instance.GetService<TaskManager>();

            mgr.RemoveTask(taskNode.Name);
        }
    }
}
