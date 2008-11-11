using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls
{
    public interface IObjectExplorer : IViewContent
    {
        void RegisterRootNode(string name, string text, string imgResource, string addInTreePath);
        void RegisterImage(string imgResource);
        void RegisterContextMenu(string nodeType, string addInTreePath);
        TreeNode GetSelectedNode();
        TreeNode GetRootNode(string name);
        ContextMenuStrip GetContextMenu(string nodeType);

        event EventHandler AfterSelection;
    }
}
