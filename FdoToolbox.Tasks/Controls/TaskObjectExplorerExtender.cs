#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Controls;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using System.Windows.Forms;

namespace FdoToolbox.Tasks.Controls
{
    public class TaskObjectExplorerExtender : IObjectExplorerExtender
    {
        private string RootNodeName = "NODE_TASKS";

        const string PATH_TASKS = "/ObjectExplorer/ContextMenus/Tasks";
        const string PATH_SELECTED_TASK = "/ObjectExplorer/ContextMenus/SelectedTask";

        const string NODE_TASK = "NODE_TASK";

        const string IMG_TASK = "application_go";
        const string IMG_COPY = "table_go";
        const string IMG_JOIN = "table_relationship";

        private IObjectExplorer _explorer;
        private TaskManager _taskMgr;

        public void Decorate(IObjectExplorer explorer)
        {
            _explorer = explorer;
            _taskMgr = ServiceManager.Instance.GetService<TaskManager>();
            _taskMgr.TaskAdded += new TaskEventHandler(OnTaskAdded);
            _taskMgr.TaskRemoved += new TaskEventHandler(OnTaskRemoved);
            _taskMgr.TaskRenamed += new TaskRenameEventHandler(OnTaskRenamed);

            _explorer.RegisterImage(IMG_TASK);
            _explorer.RegisterImage(IMG_JOIN);

            _explorer.RegisterRootNode(RootNodeName, "Tasks", IMG_TASK, PATH_TASKS);
            _explorer.RegisterContextMenu(NODE_TASK, PATH_SELECTED_TASK);
        }

        void OnTaskRenamed(object sender, TaskRenameEventArgs e)
        {
            TreeNode node = _explorer.GetRootNode(RootNodeName).Nodes[e.OldName];
            node.Name = node.Text = e.NewName;
        }

        void OnTaskRemoved(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            _explorer.GetRootNode(RootNodeName).Nodes.RemoveByKey(e.Data);
        }

        void OnTaskAdded(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = IMG_TASK;
            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_TASK);

            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.Add(node);
            node.Expand();
            root.Expand();
        }
    }
}
