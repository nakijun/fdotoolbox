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
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.Common;

namespace FdoToolbox
{
    public partial class ObjectExplorer : DockContent, IObjectExplorer
    {
        private Dictionary<string, TreeNode> _RootNodes;
        private Dictionary<string, ContextMenuStrip> _ContextMenus;

        public ObjectExplorer()
        {
            InitializeComponent();
            _RootNodes = new Dictionary<string, TreeNode>();
            _ContextMenus = new Dictionary<string, ContextMenuStrip>();
            AppGateway.RunningApplication.ModuleManager.ModuleLoaded += new ModuleEventHandler(OnModuleLoaded);
            AppGateway.RunningApplication.ModuleManager.ModuleUnloaded += new ModuleEventHandler(OnModuleUnloaded);
            AppGateway.RunningApplication.SpatialConnectionManager.ConnectionAdded += new ConnectionEventHandler(OnSpatialConnectionAdded);
            AppGateway.RunningApplication.SpatialConnectionManager.ConnectionRemoved += new ConnectionEventHandler(OnSpatialConnectionRemoved);
            AppGateway.RunningApplication.SpatialConnectionManager.ConnectionRenamed += new ConnectionRenamedEventHandler(OnSpatialConnectionRenamed);
            AppGateway.RunningApplication.DatabaseConnectionManager.ConnectionAdded += new ConnectionEventHandler(OnDatabaseConnectionAdded);
            AppGateway.RunningApplication.DatabaseConnectionManager.ConnectionRemoved += new ConnectionEventHandler(OnDatabaseConnectionRemoved);
            AppGateway.RunningApplication.DatabaseConnectionManager.ConnectionRenamed += new ConnectionRenamedEventHandler(OnDatabaseConnectionRenamed);
            AppGateway.RunningApplication.TaskManager.TaskAdded += new TaskEventHandler(OnTaskAdded);
            AppGateway.RunningApplication.TaskManager.TaskRemoved += new TaskEventHandler(OnTaskRemoved);
            RegisterContextMenus();
            RegisterRootNodes();
            RegisterImages();
        }

        private void RegisterImages()
        {
            RegisterImage(ObjectExplorerImages.IMG_CONNECTION, Properties.Resources.database_connect);
            RegisterImage(ObjectExplorerImages.IMG_TASK, Properties.Resources.application_go);
            RegisterImage(ObjectExplorerImages.IMG_MODULE, Properties.Resources.plugin);
            RegisterImage(ObjectExplorerImages.IMG_SCHEMA, Properties.Resources.chart_organisation);
            RegisterImage(ObjectExplorerImages.IMG_CLASS, Properties.Resources.database_table);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_DATA, Properties.Resources.table);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_IDENTITY, Properties.Resources.key);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_RASTER, Properties.Resources.image);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_GEOMETRY, Properties.Resources.shape_handles);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_ASSOCIATION, Properties.Resources.table_relationship);
            RegisterImage(ObjectExplorerImages.IMG_PROPERTY_OBJECT, Properties.Resources.package);
            RegisterImage(ObjectExplorerImages.IMG_DATABASE, Properties.Resources.database);
        }

        private TreeNode _FdoConnectionsNode;
        private TreeNode _DbConnectionsNode;
        private TreeNode _TasksNode;
        private TreeNode _ModulesNode;

        private void RegisterRootNodes()
        {
            _FdoConnectionsNode = new TreeNode();
            _DbConnectionsNode = new TreeNode();
            _TasksNode = new TreeNode();
            _ModulesNode = new TreeNode();

            _FdoConnectionsNode.Text = "FDO Data Sources";
            _DbConnectionsNode.Text = "ADO.net Data Sources";
            _TasksNode.Text = "Tasks";
            _ModulesNode.Text = "Modules";

            _FdoConnectionsNode.ImageKey = _FdoConnectionsNode.SelectedImageKey = ObjectExplorerImages.IMG_CONNECTION;
            _DbConnectionsNode.ImageKey = _DbConnectionsNode.SelectedImageKey = ObjectExplorerImages.IMG_CONNECTION;
            _TasksNode.ImageKey = _TasksNode.SelectedImageKey = ObjectExplorerImages.IMG_TASK;
            _ModulesNode.ImageKey = _TasksNode.SelectedImageKey = ObjectExplorerImages.IMG_MODULE;

            _FdoConnectionsNode.ContextMenuStrip = new ContextMenuStrip();
            _DbConnectionsNode.ContextMenuStrip = new ContextMenuStrip();
            _TasksNode.ContextMenuStrip = new ContextMenuStrip();
            _ModulesNode.ContextMenuStrip = new ContextMenuStrip();

            RegisterRootNode(ObjectExplorerNodeNames.FDO_CONNECTIONS, GetSpatialConnectionsNode());
            RegisterRootNode(ObjectExplorerNodeNames.DB_CONNECTIONS, GetDatabaseConnectionsNode());
            RegisterRootNode(ObjectExplorerNodeNames.TASKS, GetTasksNode());
            RegisterRootNode(ObjectExplorerNodeNames.MODULES, GetModulesNode());
        }

        private void RegisterContextMenus()
        {
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_CLASS] = ctxSelectedClass;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_DB] = ctxSelectedDatabase;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_DB_CONNECTION] = ctxSelectedDatabaseConnection;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_FDO_CONNECTION] = ctxSelectedSpatialConnection;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_MODULE] = ctxSelectedModule;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_SCHEMA] = ctxSelectedSchema;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_TABLE] = ctxSelectedTable;
            _ContextMenus[ObjectExplorerNodeNames.SELECTED_TASK] = ctxSelectedTask;
        }

        void OnDatabaseConnectionRenamed(string oldName, string newName)
        {
            TreeNode node = GetDatabaseConnectionsNode().Nodes[oldName];
            node.Name = node.Text = newName;
        }

        void OnDatabaseConnectionRemoved(string name)
        {
            TreeNode node = GetDatabaseConnectionsNode().Nodes[name];
            if (node != null)
            {
                GetDatabaseConnectionsNode().Nodes.RemoveByKey(name);
            }
        }

        void OnDatabaseConnectionAdded(string name)
        {
            DbConnectionInfo connInfo = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(name);
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = ObjectExplorerImages.IMG_CONNECTION;
            node.ContextMenuStrip = ctxSelectedDatabaseConnection;

            GetDatabaseNodes(node, connInfo.Database);
            GetDatabaseConnectionsNode().Nodes.Add(node);
            
            string tooltip = string.Format("Connection String: {0}", connInfo.Connection.ConnectionString);
            node.ToolTipText = tooltip;
            node.Expand();
            GetDatabaseConnectionsNode().Expand();
        }

        private void GetDatabaseNodes(TreeNode node, DatabaseInfo db)
        {
            TreeNode dbNode = new TreeNode();
            dbNode.Name = dbNode.Text = db.Name;
            dbNode.ImageKey = dbNode.SelectedImageKey = ObjectExplorerImages.IMG_DATABASE;
            dbNode.ContextMenuStrip = ctxSelectedDatabase;
            GetTableNodes(dbNode, db);
            node.Nodes.Add(dbNode);
        }

        private void GetTableNodes(TreeNode node, DatabaseInfo db)
        {
            foreach (TableInfo table in db.Tables)
            {
                TreeNode tableNode = new TreeNode();
                tableNode.Name = tableNode.Text = table.Name;
                tableNode.ImageKey = tableNode.SelectedImageKey = ObjectExplorerImages.IMG_CLASS;
                tableNode.ContextMenuStrip = ctxSelectedTable;
                GetColumnNodes(tableNode, table);
                string tooltip = string.Format(table.IsView ? "View\nDescription: {0}" : "Table\nDescription: {0}", table.Description);
                tableNode.ToolTipText = tooltip;
                node.Nodes.Add(tableNode);
            }
        }

        private void GetColumnNodes(TreeNode tableNode, TableInfo table)
        {
            foreach (ColumnInfo column in table.Columns)
            {
                TreeNode colNode = new TreeNode();
                colNode.Name = colNode.Text = column.Name;
                string key = ObjectExplorerImages.IMG_PROPERTY_IDENTITY;
                if (column.IsPrimaryKey)
                    key = ObjectExplorerImages.IMG_PROPERTY_IDENTITY;
                colNode.ImageKey = colNode.SelectedImageKey = key;
                tableNode.Nodes.Add(colNode);
            }
        }

        void OnSpatialConnectionRenamed(string oldName, string newName)
        {
            TreeNode node = GetSpatialConnectionsNode().Nodes[oldName];
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
            node.ImageKey = node.SelectedImageKey = ObjectExplorerImages.IMG_TASK;
            node.ContextMenuStrip = ctxSelectedTask;
            GetTasksNode().Nodes.Add(node);
        }

        void OnSpatialConnectionRemoved(string name)
        {
            GetSpatialConnectionsNode().Nodes.RemoveByKey(name);
        }

        void OnSpatialConnectionAdded(string name)
        {
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = ObjectExplorerImages.IMG_CONNECTION;
            node.ContextMenuStrip = ctxSelectedSpatialConnection;
            GetSchemaNodes(node);
            GetSpatialConnectionsNode().Nodes.Add(node);
            node.Expand();
            GetSpatialConnectionsNode().Expand();
        }

        private void GetSchemaNodes(TreeNode connNode)
        {
            FdoConnectionInfo connInfo = AppGateway.RunningApplication.SpatialConnectionManager.GetConnection(connNode.Name);
            if (connInfo != null)
            {
                FeatureService service = connInfo.CreateFeatureService();
                connNode.ToolTipText = string.Format("Provider: {0}\nConnection String: {1}", service.Connection.ConnectionInfo.ProviderName, service.Connection.ConnectionString);
                FeatureSchemaCollection schemas = service.DescribeSchema();
                foreach (FeatureSchema schema in schemas)
                {
                    TreeNode schemaNode = new TreeNode();
                    schemaNode.Name = schemaNode.Text = schema.Name;
                    schemaNode.ContextMenuStrip = ctxSelectedSchema;
                    schemaNode.ImageKey = schemaNode.SelectedImageKey = ObjectExplorerImages.IMG_SCHEMA;
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
                classNode.ImageKey = classNode.SelectedImageKey = ObjectExplorerImages.IMG_CLASS;
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
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_DATA;
                            if (classDef.IdentityProperties.Contains(dataDef))
                                propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_IDENTITY;
                            propertyNode.ToolTipText = string.Format("Data Type: {0}\nLength: {1}\nAuto-Generated: {2}\nRead-Only: {3}", dataDef.DataType, dataDef.Length, dataDef.IsAutoGenerated, dataDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        {
                            GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_GEOMETRY;
                            propertyNode.ToolTipText = string.Format("Has Elevation: {0}\nHas Measure: {1}\nRead-Only: {2}", geomDef.HasElevation, geomDef.HasMeasure, geomDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_RasterProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_RASTER;
                            propertyNode.ToolTipText = "Raster Property";
                        }
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_OBJECT;
                            propertyNode.ToolTipText = "Object Property";
                        }
                        break;
                    case PropertyType.PropertyType_AssociationProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = ObjectExplorerImages.IMG_PROPERTY_ASSOCIATION;
                            propertyNode.ToolTipText = "Association Property";
                        }
                        break;
                }
                classNode.Nodes.Add(propertyNode);
            }
        }

        TreeNode GetDatabaseConnectionsNode() { return _DbConnectionsNode; }
        TreeNode GetSpatialConnectionsNode() { return _FdoConnectionsNode; }
        TreeNode GetTasksNode() { return _TasksNode; }
        TreeNode GetModulesNode() { return _ModulesNode; }

        const string NODE_PREFIX_CONNECTION = "CONN";
        const string NODE_PREFIX_TASK = "TASK";
        const string NODE_PREFIX_MODULE = "MOD";

        const int NODE_LEVEL_SCHEMA = 2;
        const int NODE_LEVEL_CLASS = 3;

        const int NODE_LEVEL_DATABASE = 2;
        const int NODE_LEVEL_TABLE = 3;

        void OnModuleLoaded(IModule module) 
        {
            string key = NODE_PREFIX_MODULE + module.Name.GetHashCode();
            TreeNode modNode = new TreeNode();
            modNode.Name = key;
            modNode.Text = module.Name;
            modNode.ToolTipText = module.Description;
            modNode.ImageKey = modNode.SelectedImageKey = ObjectExplorerImages.IMG_MODULE;
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
                return AppGateway.RunningApplication.TaskManager.GetTask(node.Text);
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
                return AppGateway.RunningApplication.ModuleManager.GetLoadedModule(node.Text);
            }
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        public void RefreshDatabaseConnection(string name)
        {
            TreeNode node = GetDatabaseConnectionsNode().Nodes[name];
            if (node != null)
            {
                node.Nodes.Clear();
                DbConnectionInfo connInfo = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(name);
                connInfo.Refresh();
                GetDatabaseNodes(node, connInfo.Database);
            }
        }

        public void RefreshSpatialConnection(string name)
        {
            TreeNode node = GetSpatialConnectionsNode().Nodes[name];
            node.Nodes.Clear();

            FdoConnectionInfo conn = AppGateway.RunningApplication.SpatialConnectionManager.GetConnection(node.Name);
            if (conn != null)
            {
                conn.Refresh();
            }

            GetSchemaNodes(node);
        }

        public DbConnectionInfo GetSelectedDatabaseConnection()
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
                if (connNode.Parent == GetDatabaseConnectionsNode())
                {
                    string name = connNode.Name;
                    DbConnectionInfo connInfo = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(connNode.Name);
                    if (connInfo != null)
                        return connInfo;
                    else
                        return null;
                }
                return null;
            }
        }

        public FdoConnectionInfo GetSelectedSpatialConnection()
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
                if (connNode.Parent == GetSpatialConnectionsNode())
                {
                    string name = connNode.Name;
                    return AppGateway.RunningApplication.SpatialConnectionManager.GetConnection(connNode.Name);
                }
                return null;
            }
        }

        public void InitializeMenus(string menuMapFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(menuMapFile);

            XmlNode toolbarNode = doc.SelectSingleNode("//ObjectExplorer/Toolbar");

            ProcessMenuNode(mToolStrip, toolbarNode);

            XmlNodeList contextMenuNodes = doc.SelectNodes("//ObjectExplorer/ContextMenus/ContextMenu");
            foreach (XmlNode node in contextMenuNodes)
            {
                string name = node.Attributes["attachTo"].Value;
                ProcessNode(doc, name);
            }
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
            AppGateway.RunningApplication.MenuStateManager.RegisterMenuItem(cmd.Name, menuItem);
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
                            Command cmd = AppGateway.RunningApplication.ModuleManager.GetCommand(cmdName);
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

        private void ProcessNode(XmlDocument doc, string name)
        {
            XmlNode menuNode = doc.SelectSingleNode("//ObjectExplorer/ContextMenus/ContextMenu[@attachTo='" + name + "']");
            if (_ContextMenus.ContainsKey(name))
                ProcessMenuNode(_ContextMenus[name], menuNode);
            else
                throw new ArgumentException("No context menu has been registered under: " + name);
        }

        public void ExtendUI(string uiExtensionFile)
        {
            InitializeMenus(uiExtensionFile);
        }

        public void UnHide()
        {
            this.Show();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FdoConnectionInfo connInfo = GetSelectedSpatialConnection();
            if (connInfo != null)
            {
                IModuleMgr modMgr = AppGateway.RunningApplication.ModuleManager;
                ICollection<string> cmdNames = modMgr.CommandNames;
                foreach (string cmd in cmdNames)
                {
                    bool canExec = modMgr.IsCommandExecutable(cmd, connInfo.InternalConnection);
                    AppGateway.RunningApplication.MenuStateManager.EnableCommand(cmd, canExec);
                }
            }
        }

        public string GetSelectedSchema()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_SCHEMA)
                node = node.Parent;

            if (node.Level == NODE_LEVEL_SCHEMA && GetSpatialConnectionsNode() == node.Parent.Parent)
                return node.Name;

            return null;
        }

        public string GetSelectedClass()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_CLASS)
                node = node.Parent;
            
            if (node.Level == NODE_LEVEL_CLASS && GetSpatialConnectionsNode() == node.Parent.Parent.Parent)
                return node.Name;

            return null;
        }

        public string GetSelectedDatabase()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_DATABASE)
                node = node.Parent;

            if (node.Level == NODE_LEVEL_DATABASE && GetDatabaseConnectionsNode() == node.Parent.Parent)
                return node.Name;

            return null;
        }

        public string GetSelectedTable()
        {
            TreeNode node = mTreeView.SelectedNode;
            while (node.Level > NODE_LEVEL_TABLE)
                node = node.Parent;

            if (node.Level == NODE_LEVEL_TABLE && GetDatabaseConnectionsNode() == node.Parent.Parent.Parent)
                return node.Name;

            return null;
        }

        public void RegisterRootNode(string nodeName, TreeNode node)
        {
            if(_RootNodes.ContainsKey(nodeName) && _ContextMenus.ContainsKey(nodeName))
                throw new ArgumentException("A root node named " + nodeName + " already has been registered");

            if (node.ContextMenuStrip == null)
                throw new ArgumentException("The given node has no context menu attached");

            _RootNodes[nodeName] = node;
            _ContextMenus[nodeName] = node.ContextMenuStrip;

            mTreeView.Nodes.Add(node);
        }

        public TreeNode GetRootNode(string nodeName)
        {
            if (_RootNodes.ContainsKey(nodeName))
                return _RootNodes[nodeName];

            return null;
        }

        public void RegisterContextMenu(string nodeName, ContextMenuStrip contextMenu)
        {
            if (_ContextMenus.ContainsKey(nodeName))
                throw new ArgumentException("A context menu has already been registered under: " + nodeName);

            _ContextMenus[nodeName] = contextMenu;
        }

        public ContextMenuStrip GetContextMenu(string nodeName)
        {
            if (_ContextMenus.ContainsKey(nodeName))
                return _ContextMenus[nodeName];

            return null;
        }

        public TreeNode GetSelectedNode()
        {
            return mTreeView.SelectedNode;
        }

        public void RegisterImage(string key, Image image)
        {
            if (mImageList.Images.ContainsKey(key))
                throw new ArgumentException("The Object Explorer image list already contains an image under the key: " + key);

            mImageList.Images.Add(key, image);
        }
    }
}