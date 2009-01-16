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
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public partial class ObjectExplorer : ViewContent, IObjectExplorer
    {
        private ToolStrip objToolStrip;
        private TreeView objTreeView;
        private ImageList imgList;

        public ObjectExplorer()
        {
            imgList = new ImageList();
            InitTreeView();
            objToolStrip = ToolbarService.CreateToolStrip(this, "/ObjectExplorer/Toolbar");
            
            this.Controls.Add(objTreeView);
            this.Controls.Add(objToolStrip);
        }

        private void InitTreeView()
        {
            objTreeView = new TreeView();
            objTreeView.ShowLines = false;
            objTreeView.ShowNodeToolTips = true;
            objTreeView.ShowPlusMinus = true;
            objTreeView.ShowRootLines = true;
            objTreeView.ImageList = imgList;
            objTreeView.Dock = DockStyle.Fill;
            objTreeView.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    objTreeView.SelectedNode = objTreeView.GetNodeAt(e.X, e.Y);
                }
            };
            objTreeView.AfterSelect += new TreeViewEventHandler(OnAfterSelect);
        }

        void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterSelection(this, EventArgs.Empty);
        }

        public override bool CanClose
        {
            get
            {
                return false;
            }
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("UI_OBJECT_EXPLORER"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public void RegisterImage(string imgResource)
        {
            if (!imgList.Images.ContainsKey(imgResource))
                imgList.Images.Add(imgResource, ResourceService.GetBitmap(imgResource));
        }

        private Dictionary<string, ContextMenuStrip> _ContextMenus = new Dictionary<string, ContextMenuStrip>();

        public void RegisterContextMenu(string nodeType, string addInTreePath)
        {
            if(_ContextMenus.ContainsKey(nodeType))
                throw new ArgumentException("A context menu has already been registered under: " + nodeType);

            _ContextMenus[nodeType] = MenuService.CreateContextMenu(this, addInTreePath);
        }

        public TreeNode GetSelectedNode()
        {
            return objTreeView.SelectedNode;
        }

        public void RegisterRootNode(string name, string text, string imgResource, string addInTreePath)
        {
            if (!imgList.Images.ContainsKey(imgResource))
            {
                imgList.Images.Add(imgResource, ResourceService.GetBitmap(imgResource));
            }

            TreeNode node = new TreeNode();
            node.Name = name;
            node.Text = text;
            node.ImageKey = node.SelectedImageKey = imgResource;
            node.ContextMenuStrip = MenuService.CreateContextMenu(node, addInTreePath);
            objTreeView.Nodes.Add(node);
        }

        public TreeNode GetRootNode(string name)
        {
            return objTreeView.Nodes[name];
        }

        public ContextMenuStrip GetContextMenu(string nodeType)
        {
            if (_ContextMenus.ContainsKey(nodeType))
                return _ContextMenus[nodeType];
            return null;
        }

        public event EventHandler AfterSelection = delegate { };
    }
}
