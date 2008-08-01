#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core;
using WeifenLuo.WinFormsUI.Docking;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using System.Diagnostics;
using System.Xml;

namespace FdoToolbox
{
    public partial class ObjectExplorer : DockContent, IObjectExplorer
    {
        public ObjectExplorer()
        {
            InitializeComponent();
            HostApplication.Instance.ModuleManager.ModuleLoaded += new ModuleEventHandler(OnModuleLoaded);
            HostApplication.Instance.ModuleManager.ModuleUnloaded += new ModuleEventHandler(OnModuleUnloaded);
            HostApplication.Instance.ConnectionManager.ConnectionAdded += new ConnectionEventHandler(OnConnectionAdded);
            HostApplication.Instance.ConnectionManager.ConnectionRemoved += new ConnectionEventHandler(OnConnectionRemoved);
            HostApplication.Instance.ConnectionManager.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
            HostApplication.Instance.TaskManager.TaskAdded += new TaskEventHandler(OnTaskAdded);
            HostApplication.Instance.TaskManager.TaskRemoved += new TaskEventHandler(OnTaskRemoved);
        }

        void OnConnectionRenamed(string oldName, string newName)
        {
            TreeNode node = GetConnectionsNode().Nodes[oldName];
            node.Name = node.Text = newName;
        }

        void OnTaskRemoved(string name)
        {
            GetTasksNode().Nodes.RemoveByKey(NODE_PREFIX_TASK + name);    
        }

        void OnTaskAdded(string name)
        {
            TreeNode node = new TreeNode();
            node.Name = NODE_PREFIX_TASK + name;
            node.Text = name;
            node.ImageIndex = node.SelectedImageIndex = IMG_IDX_TASK;
            node.ContextMenuStrip = ctxSelectedTask;
            GetTasksNode().Nodes.Add(node);
        }

        void OnConnectionRemoved(string name)
        {
            GetConnectionsNode().Nodes.RemoveByKey(name);
        }

        void OnConnectionAdded(string name)
        {
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageIndex = node.SelectedImageIndex = IMG_IDX_CONNECTION;
            node.ContextMenuStrip = ctxSelectedConnection;
            GetSchemaNodes(node);
            GetConnectionsNode().Nodes.Add(node);
        }

        private void GetSchemaNodes(TreeNode connNode)
        {
            FeatureService service = HostApplication.Instance.ConnectionManager.CreateService(connNode.Name);
            if (service != null)
            {
                connNode.ToolTipText = string.Format("Provider: {0}\nConnection String: {1}", service.Connection.ConnectionInfo.ProviderName, service.Connection.ConnectionString);
                FeatureSchemaCollection schemas = service.DescribeSchema();
                foreach (FeatureSchema schema in schemas)
                {
                    TreeNode schemaNode = new TreeNode();
                    schemaNode.Name = schemaNode.Text = schema.Name;
                    schemaNode.ContextMenuStrip = ctxSelectedSchema;
                    schemaNode.ImageIndex = schemaNode.SelectedImageIndex = IMG_IDX_SCHEMA;
                    GetClassNodes(schema, schemaNode);
                    connNode.Nodes.Add(schemaNode);
                    schemaNode.Expand();
                }
            }
        }

        private void GetClassNodes(FeatureSchema schema, TreeNode schemaNode)
        {
            foreach (ClassDefinition classDef in schema.Classes)
            {
                TreeNode classNode = new TreeNode();
                classNode.Name = classNode.Text = classDef.Name;
                classNode.ContextMenuStrip = ctxSelectedClass;
                classNode.ImageIndex = classNode.SelectedImageIndex = IMG_IDX_CLASS;
                classNode.ToolTipText = string.Format("Type: {0}", classDef.ClassType);
                GetPropertyNodes(classDef, classNode);
                schemaNode.Nodes.Add(classNode);
            }
        }

        private void GetPropertyNodes(ClassDefinition classDef, TreeNode classNode)
        {
            foreach (PropertyDefinition propDef in classDef.Properties)
            {
                TreeNode propertyNode = new TreeNode();
                propertyNode.Name = propertyNode.Text = propDef.Name;
                switch (propDef.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        {
                            DataPropertyDefinition dataDef = propDef as DataPropertyDefinition;
                            propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_DATA;
                            if (classDef.IdentityProperties.Contains(dataDef))
                                propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_IDENTITY;
                            propertyNode.ToolTipText = string.Format("Data Type: {0}\nLength: {1}\nAuto-Generated: {2}\nRead-Only: {3}", dataDef.DataType, dataDef.Length, dataDef.IsAutoGenerated, dataDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        {
                            GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                            propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_GEOMETRY;
                            propertyNode.ToolTipText = string.Format("Has Elevation: {0}\nHas Measure: {1}\nRead-Only: {2}", geomDef.HasElevation, geomDef.HasMeasure, geomDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_RasterProperty:
                        {
                            propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_RASTER;
                            propertyNode.ToolTipText = "Raster Property";
                        }
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        {
                            propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_OBJECT;
                            propertyNode.ToolTipText = "Object Property";
                        }
                        break;
                    case PropertyType.PropertyType_AssociationProperty:
                        {
                            propertyNode.ImageIndex = propertyNode.SelectedImageIndex = IMG_IDX_PROPERTY_ASSOCIATION;
                            propertyNode.ToolTipText = "Association Property";
                        }
                        break;
                }
                classNode.Nodes.Add(propertyNode);
            }
        }

        TreeNode GetConnectionsNode() { return mTreeView.Nodes.Find("NODE_FDO_CONNECTIONS", false)[0]; }
        TreeNode GetTasksNode() { return mTreeView.Nodes.Find("NODE_TASKS", false)[0]; }
        TreeNode GetModulesNode() { return mTreeView.Nodes.Find("NODE_MODULES", false)[0]; }

        const string NODE_PREFIX_CONNECTION = "CONN";
        const string NODE_PREFIX_TASK = "TASK";
        const string NODE_PREFIX_MODULE = "MOD";

        const int IMG_IDX_CONNECTION = 0;
        const int IMG_IDX_TASK = 1;
        const int IMG_IDX_MODULE = 2;
        const int IMG_IDX_SCHEMA = 3;
        const int IMG_IDX_CLASS = 4;
        const int IMG_IDX_PROPERTY_DATA = 5;
        const int IMG_IDX_PROPERTY_IDENTITY = 6;
        const int IMG_IDX_PROPERTY_RASTER = 7;
        const int IMG_IDX_PROPERTY_GEOMETRY = 8;
        const int IMG_IDX_PROPERTY_ASSOCIATION = 9;
        const int IMG_IDX_PROPERTY_OBJECT = 10;

        const int NODE_LEVEL_SCHEMA = 2;
        const int NODE_LEVEL_CLASS = 3;

        void OnModuleLoaded(IModule module) 
        {
            string key = NODE_PREFIX_MODULE + module.Name.GetHashCode();
            TreeNode modNode = new TreeNode();
            modNode.Name = key;
            modNode.Text = module.Name;
            modNode.ToolTipText = module.Description;
            modNode.ImageIndex = modNode.SelectedImageIndex = IMG_IDX_MODULE;
            modNode.ContextMenuStrip = ctxSelectedModule;
            
            TreeNode parent = GetModulesNode();
            parent.Nodes.Add(modNode);
            parent.Expand();
        }
        
        void OnModuleUnloaded(IModule module) 
        {
            string key = NODE_PREFIX_MODULE + module.Name.GetHashCode();
            GetModulesNode().Nodes.RemoveByKey(key);
        }

        public Form FormObj
        {
            get { return this; }
        }

        public ITask GetSelectedTask()
        {
            TreeNode node = mTreeView.SelectedNode;
            if (node == null || node.Level != 1)
            {
                return null;
            }
            else
            {
                Debug.Assert(node.Parent == GetTasksNode());
                return HostApplication.Instance.TaskManager.GetTask(node.Text);
            }
        }

        public IModule GetSelectedModule()
        {
            TreeNode node = mTreeView.SelectedNode;
            if (node == null || node.Level != 1)
            {
                return null;
            }
            else
            {
                Debug.Assert(node.Parent == GetModulesNode());
                return HostApplication.Instance.ModuleManager.GetLoadedModule(node.Text);
            }
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        public void RefreshConnection(string name)
        {
            TreeNode node = GetConnectionsNode().Nodes[name];
            Debug.Assert(node != null);
            node.Nodes.Clear();

            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(node.Name);
            if (conn != null)
            {
                conn.Close();
                conn.Open();
            }

            GetSchemaNodes(node);
        }

        public ConnectionInfo GetSelectedConnection()
        {
            TreeNode connNode = mTreeView.SelectedNode;
            if (connNode == null || connNode.Level == 0)
            {
                return null;
            }
            else
            {
                //If level > 1, walk back up the hierarchy
                while (connNode.Level > 1)
                {
                    connNode = connNode.Parent;
                }
                if (connNode.Parent == GetConnectionsNode())
                {
                    string name = connNode.Name;
                    IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connNode.Name);
                    if (conn != null)
                        return new ConnectionInfo(name, conn);
                    else
                        return null;
                }
                return null;
            }
        }

        public void InitializeMenus(string menuMapFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(menuMapFile);

            XmlNode toolbarNode = doc.SelectSingleNode("//ObjectExplorer/Toolbar");
            XmlNode connCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/Connections");
            XmlNode selConnCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/SelectedConnection");
            XmlNode taskCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/Tasks");
            XmlNode selTaskCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/SelectedTask");
            XmlNode moduleCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/Modules");
            XmlNode selModuleCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/SelectedModule");
            XmlNode selSchemaCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/SelectedSchema");
            XmlNode selClassCtxNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/SelectedClass");

            ProcessMenuNode(mToolStrip, toolbarNode);
            ProcessMenuNode(ctxFdoConnections, connCtxNode);
            ProcessMenuNode(ctxSelectedConnection, selConnCtxNode);
            ProcessMenuNode(ctxSelectedSchema, selSchemaCtxNode);
            ProcessMenuNode(ctxSelectedClass, selClassCtxNode);
            ProcessMenuNode(ctxTasks, taskCtxNode);
            ProcessMenuNode(ctxSelectedTask, selTaskCtxNode);
            ProcessMenuNode(ctxModules, moduleCtxNode);
            ProcessMenuNode(ctxSelectedModule, selModuleCtxNode);
        }

        private ToolStripMenuItem CreateMenuItem(Command cmd, XmlNode cmdNode)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = cmd.DisplayName;
            menuItem.Image = cmd.CommandImage;
            menuItem.ToolTipText = cmd.Description;
            menuItem.ShortcutKeys = cmd.ShortcutKeys;
            menuItem.Tag = cmd;
            menuItem.Click += delegate { cmd.Execute(); };
            if (cmdNode.Attributes["displayName"] != null)
                menuItem.Text = cmdNode.Attributes["displayName"].Value;
            HostApplication.Instance.MenuStateManager.RegisterMenuItem(cmd.Name, menuItem);
            return menuItem;
        }

        private void ProcessMenuNode(object toolstrip, XmlNode menuNode)
        {
            if (menuNode != null)
            {
                //TODO: Try to find the interface (one that exposes the .Items property)
                //so we can avoid this type testing nonsense
                ToolStrip tstrip = toolstrip as ToolStrip;
                ToolStripMenuItem tsmi = toolstrip as ToolStripMenuItem;
                foreach (XmlNode node in menuNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Command":
                            string cmdName = node.Attributes["name"].Value;
                            Command cmd = HostApplication.Instance.ModuleManager.GetCommand(cmdName);
                            if (cmd != null && (cmd.InvocationType != CommandInvocationType.Console))
                            {
                                if (tstrip != null)
                                    tstrip.Items.Add(CreateMenuItem(cmd, node));
                                else if (tsmi != null)
                                    tsmi.DropDown.Items.Add(CreateMenuItem(cmd, node));
                            }
                            else
                            {
                                AppConsole.Err.WriteLine("unrecognised menu command entry {0}", cmdName);
                            }
                            break;
                        case "Separator":
                            if (tstrip != null)
                                tstrip.Items.Add(new ToolStripSeparator());
                            else if (tsmi != null)
                                tsmi.DropDown.Items.Add(new ToolStripSeparator());
                            break;
                        case "SubMenu":
                            string subMenuName = node.Attributes["name"].Value;
                            ToolStripMenuItem subMenu = new ToolStripMenuItem();
                            subMenu.Text = subMenuName;
                            if (node.Attributes["resource"] != null)
                            {
                                object resource = Properties.Resources.ResourceManager.GetObject(node.Attributes["resource"].Value);
                                if (resource != null)
                                    subMenu.Image = (Image)resource;
                            }
                            ProcessMenuNode(subMenu, node);
                            if (tstrip != null)
                                tstrip.Items.Add(subMenu);
                            else if (tsmi != null)
                                tsmi.DropDown.Items.Add(subMenu);
                            break;
                        case "Menu":
                            string menuName = node.Attributes["name"].Value;
                            ToolStripMenuItem menu = new ToolStripMenuItem();
                            menu.Text = menuName;
                            ProcessMenuNode(menu, node);
                            if (node.Attributes["resource"] != null)
                            {
                                object resource = Properties.Resources.ResourceManager.GetObject(node.Attributes["resource"].Value);
                                if (resource != null)
                                    menu.Image = (Image)resource;
                            }
                            if (tstrip != null)
                                tstrip.Items.Add(menu);
                            else if (tsmi != null)
                                tsmi.DropDown.Items.Add(menu);
                            break;
                    }
                }
            }
        }

        public void ExtendUI(string uiExtensionFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(uiExtensionFile);

            XmlNode toolbarNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/Toolbar");
            XmlNode connCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/Connections");
            XmlNode selConnCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/SelectedConnection");
            XmlNode taskCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/Tasks");
            XmlNode selTaskCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/SelectedTask");
            XmlNode moduleCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/Modules");
            XmlNode selModuleCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/SelectedModule");
            XmlNode selSchemaCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/SelectedSchema");
            XmlNode selClassCtxNode = doc.SelectSingleNode("//UIExtension/ObjectExplorer/ContextMenus/SelectedClass");

            ProcessMenuNode(mToolStrip, toolbarNode);
            ProcessMenuNode(ctxFdoConnections, connCtxNode);
            ProcessMenuNode(ctxSelectedConnection, selConnCtxNode);
            ProcessMenuNode(ctxSelectedSchema, selSchemaCtxNode);
            ProcessMenuNode(ctxSelectedClass, selClassCtxNode);
            ProcessMenuNode(ctxTasks, taskCtxNode);
            ProcessMenuNode(ctxSelectedTask, selTaskCtxNode);
            ProcessMenuNode(ctxModules, moduleCtxNode);
            ProcessMenuNode(ctxSelectedModule, selModuleCtxNode);
        }

        public void UnHide()
        {
            this.Show();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ConnectionInfo connInfo = GetSelectedConnection();
            if (connInfo != null)
            {
                IModuleMgr modMgr = HostApplication.Instance.ModuleManager;
                ICollection<string> cmdNames = modMgr.GetCommandNames();
                foreach (string cmd in cmdNames)
                {
                    bool canExec = modMgr.IsCommandExecutable(cmd, connInfo.Connection);
                    HostApplication.Instance.MenuStateManager.EnableCommand(cmd, canExec);
                }
            }
        }

        public string GetSelectedSchema()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_SCHEMA)
                node = node.Parent;

            if (node.Level == NODE_LEVEL_SCHEMA && GetConnectionsNode() == node.Parent.Parent)
                return node.Name;

            return null;
        }

        public string GetSelectedClass()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_CLASS)
                node = node.Parent;
            
            if (node.Level == NODE_LEVEL_CLASS && GetConnectionsNode() == node.Parent.Parent.Parent)
                return node.Name;

            return null;
        }
    }
}