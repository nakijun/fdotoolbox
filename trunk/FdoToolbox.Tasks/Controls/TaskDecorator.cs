using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Tasks.Controls
{
    public class TaskDecorator : IObjectExplorerDecorator
    {
        private string RootNodeName = "NODE_TASKS";

        const string PATH_TASKS = "/ObjectExplorer/ContextMenus/Tasks";
        const string PATH_SELECTED_TASK = "/ObjectExplorer/ContextMenus/SelectedTask";

        const string NODE_TASK = "NODE_TASK";

        const string IMG_TASK = "application_go";
        const string IMG_COPY = "table_go";
        const string IMG_JOIN = "table_relationship";

        private IObjectExplorer _explorer;

        public void Decorate(IObjectExplorer explorer)
        {
            _explorer = explorer;

            _explorer.RegisterImage(IMG_TASK);
            _explorer.RegisterImage(IMG_JOIN);

            _explorer.RegisterRootNode(RootNodeName, "Tasks", IMG_TASK, PATH_TASKS);
            _explorer.RegisterContextMenu(NODE_TASK, PATH_SELECTED_TASK);
        }
    }
}
