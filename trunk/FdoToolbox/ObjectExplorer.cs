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
            HostApplication.Instance.TaskManager.TaskAdded += new TaskEventHandler(OnTaskAdded);
            HostApplication.Instance.TaskManager.TaskRemoved += new TaskEventHandler(OnTaskRemoved);
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
            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connNode.Name);
            if (conn != null)
            {
                connNode.ToolTipText = string.Format("Provider: {0}\nConnection String: {1}", conn.ConnectionInfo.ProviderName, conn.ConnectionString);
                using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    using (FeatureSchemaCollection schemas = cmd.Execute())
                    {
                        foreach (FeatureSchema schema in schemas)
                        {
                            TreeNode schemaNode = new TreeNode();
                            schemaNode.Name = schemaNode.Text = schema.Name;
                            schemaNode.ImageIndex = schemaNode.SelectedImageIndex = IMG_IDX_SCHEMA;
                            
                            GetClassNodes(schema, schemaNode);
                            connNode.Nodes.Add(schemaNode);
                        }
                    }
                }
            }
        }

        private void GetClassNodes(FeatureSchema schema, TreeNode schemaNode)
        {
            foreach (ClassDefinition classDef in schema.Classes)
            {
                TreeNode classNode = new TreeNode();
                classNode.Name = classNode.Text = classDef.Name;
                classNode.ImageIndex = classNode.SelectedImageIndex = IMG_IDX_CLASS;
                classNode.ToolTipText = string.Format("Type: {0}", classDef.ClassType);

                //GetPropertyNodes(classDef, classNode);
                schemaNode.Nodes.Add(classNode);
            }
        }

        TreeNode GetConnectionsNode() { return mTreeView.Nodes.Find("NODE_CONNECTIONS", false)[0]; }
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

        //public OSGeo.FDO.Connections.IConnection GetSelectedConnection(ref string name)
        //{
        //    TreeNode connNode = mTreeView.SelectedNode;
        //    if (connNode == null || connNode.Level != 1)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        name = connNode.Name;
        //        return HostApplication.Instance.ConnectionManager.GetConnection(connNode.Name);
        //    }
        //}

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

        private void LoadModule_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_EXTLOAD);
        }

        private void ConnectSDF_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(ExpressModule.CMD_SDFCONNECT);
        }

        private void ConnectSHP_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(ExpressModule.CMD_SHPCONNECT);
        }

        private void ConnectGeneric_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_CONNECT);
        }

        private void CreateSDF_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(ExpressModule.CMD_SDFCREATE);
        }

        private void CreateSHP_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(ExpressModule.CMD_SHPCREATE);
        }

        private void CreateCustom_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_CREATEDATASTORE);
        }

        private void CreateBulkCopy_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_CREATEBCP);
        }

        private void CreateJoin_Click(object sender, EventArgs e)
        {

        }

        private void RemoveSelectedConnection_Click(object sender, EventArgs e)
        {
            TreeNode connNode = mTreeView.SelectedNode;
            Debug.Assert(connNode.Parent == GetConnectionsNode());

            string name = connNode.Name;
            HostApplication.Instance.ConnectionManager.RemoveConnection(name);
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        private void ManageSchemas_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_MANSCHEMA);
        }


        public void RefreshConnection(string name)
        {
            TreeNode node = GetConnectionsNode().Nodes[name];
            Debug.Assert(node != null);
            node.Nodes.Clear();
            GetSchemaNodes(node);
        }

        private void SaveSchemas_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_SAVESCHEMA);
        }

        private void LoadSchemas_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_LOADSCHEMA);
        }

        private void ModuleInfo_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_MODINFO);
        }

        private void DeleteTask_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_DELETETASK);
        }

        private void ExecuteTask_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_EXECUTETASK);
        }

        private void EditTask_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_EDITTASK);
        }

        private void DataPreview_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_DATAPREVIEW);
        }

        private void LoadTask_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_LOADTASK);
        }

        private void SaveTask_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_SAVETASK);
        }

        public ConnectionInfo GetSelectedConnection()
        {
            TreeNode connNode = mTreeView.SelectedNode;
            if (connNode == null || connNode.Level != 1)
            {
                return null;
            }
            else
            {
                string name = connNode.Name;
                IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connNode.Name);
                if (conn != null)
                    return new ConnectionInfo(name, conn);
                else
                    return null;
            }
        }

        private void LoadConnection_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_LOADCONN);
        }

        private void SaveConnection_Click(object sender, EventArgs e)
        {
            HostApplication.Instance.ExecuteCommand(CoreModule.CMD_SAVECONN);
        }
    }
}