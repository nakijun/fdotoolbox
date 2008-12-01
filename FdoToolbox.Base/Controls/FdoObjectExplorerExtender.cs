using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using FdoToolbox.Base.Commands;
using System.Collections;

namespace FdoToolbox.Base.Controls
{
    public class FdoObjectExplorerExtender : IObjectExplorerExtender
    {
        private string RootNodeName = "NODE_FDO";

        private const string IMG_CONNECTION = "database_connect";
        private const string IMG_SCHEMA = "chart_organisation";
        private const string IMG_CLASS = "database_table";
        private const string IMG_DATA_PROPERTY = "table";
        private const string IMG_ID_PROPERTY = "key";
        private const string IMG_GEOM_PROPERTY = "shape_handles";
        private const string IMG_RASTER_PROPERTY = "image";
        private const string IMG_OBJECT_PROPERTY = "package";
        private const string IMG_ASSOC_PROPERTY = "table_relationship";

        private const string NODE_CONNECTION = "NODE_CONNECTION";
        private const string NODE_SCHEMA = "NODE_SCHEMA";
        private const string NODE_CLASS = "NODE_CLASS";
        //private const string NODE_PROPERTY = "NODE_PROPERTY";

        const string PATH_SELECTED_CONNECTION = "/ObjectExplorer/ContextMenus/SelectedConnection";
        const string PATH_SELECTED_SCHEMA = "/ObjectExplorer/ContextMenus/SelectedSchema";
        const string PATH_SELECTED_CLASS = "/ObjectExplorer/ContextMenus/SelectedClass";

        private IObjectExplorer _explorer;
        private FdoConnectionManager _connMgr;

        public void Decorate(IObjectExplorer explorer)
        {
            _explorer = explorer;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            _connMgr.ConnectionAdded += new ConnectionEventHandler(OnConnectionAdded);
            _connMgr.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
            _connMgr.ConnectionRemoved += new ConnectionEventHandler(OnConnectionRemoved);
            _connMgr.ConnectionRefreshed += new ConnectionEventHandler(OnConnectionRefreshed);

            _explorer.RegisterImage(IMG_ASSOC_PROPERTY);
            _explorer.RegisterImage(IMG_CLASS);
            _explorer.RegisterImage(IMG_CONNECTION);
            _explorer.RegisterImage(IMG_DATA_PROPERTY);
            _explorer.RegisterImage(IMG_GEOM_PROPERTY);
            _explorer.RegisterImage(IMG_ID_PROPERTY);
            _explorer.RegisterImage(IMG_OBJECT_PROPERTY);
            _explorer.RegisterImage(IMG_RASTER_PROPERTY);
            _explorer.RegisterImage(IMG_SCHEMA);

            _explorer.AfterSelection += new EventHandler(OnAfterSelection);

            _explorer.RegisterRootNode(RootNodeName, "FDO Data Sources", "database_connect", "/ObjectExplorer/ContextMenus/FdoConnections");
            _explorer.RegisterContextMenu(NODE_CONNECTION, PATH_SELECTED_CONNECTION);
            _explorer.RegisterContextMenu(NODE_SCHEMA, PATH_SELECTED_SCHEMA);
            _explorer.RegisterContextMenu(NODE_CLASS, PATH_SELECTED_CLASS);
            //_explorer.RegisterContextMenu(NODE_PROPERTY, "/ObjectExplorer/ContextMenus/SelectedProperty");
        }

        void OnAfterSelection(object sender, EventArgs e)
        {
            //We want to selectively disable commands if the connection in context does not support
            //such capability
            //TreeNode node = _explorer.GetSelectedNode();
            //FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            //FdoConnection conn = null;
            //ResetCommandState();
            //switch (node.Level)
            //{
            //    case 1: //Connections
            //        conn = mgr.GetConnection(node.Name);
            //        break;
            //    case 2: //Schemas
            //        conn = mgr.GetConnection(node.Parent.Name);
            //        break;
            //    case 3: //Classes
            //        conn = mgr.GetConnection(node.Parent.Parent.Name);
            //        break;
            //    case 4: //Property
            //        conn = mgr.GetConnection(node.Parent.Parent.Parent.Name);
            //        break;
            //    default: //Unknown node or root node. Do nothing
            //        return;
            //}

            //Toggle state of capability sensitive commands
            //CheckConnectionCommands(conn);
            //CheckSchemaCommands(conn);
            //CheckClassCommands(conn);
        }

        private void ResetCommandState()
        {
            //LoggingService.Debug("Resetting command state");
            //throw new Exception("The method or operation is not implemented.");
            ArrayList cmds = AddInTree.BuildItems(PATH_SELECTED_CLASS, this, false);
            foreach (object c in cmds)
            {
                ToolStripMenuItem t = c as ToolStripMenuItem;
                if (t != null)
                    t.Enabled = true;
            }
            cmds = AddInTree.BuildItems(PATH_SELECTED_CONNECTION, this, false);
            foreach (object c in cmds)
            {
                ToolStripMenuItem t = c as ToolStripMenuItem;
                if (t != null)
                    t.Enabled = true;
            }
            cmds = AddInTree.BuildItems(PATH_SELECTED_SCHEMA, this, false);
            foreach (object c in cmds)
            {
                ToolStripMenuItem t = c as ToolStripMenuItem;
                if (t != null)
                    t.Enabled = true;
            }
        }

        private void CheckClassCommands(FdoConnection conn)
        {
            //LoggingService.Debug("Checking class command state");
            //throw new Exception("The method or operation is not implemented.");
            ArrayList cmds = AddInTree.BuildItems(PATH_SELECTED_CLASS, this, false);
            foreach (object c in cmds)
            {
                //if (c is InsertDataCommand)
                //{
                //    int [] cmds = conn.Capabilities.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
                //    c.IsEnabled = Array.IndexOf<int>(cmds, (int)CommandType.CommandType_Insert) >= 0;
                //}
            }
        }

        private void CheckSchemaCommands(FdoConnection conn)
        {
            //LoggingService.Debug("Checking schema command state");
            //throw new Exception("The method or operation is not implemented.");
            //ArrayList cmds = AddInTree.BuildItems(PATH_SELECTED_SCHEMA, this, false);
            //foreach (object c in cmds)
            //{
            //    if (m.Command is DeleteSchemaCommand)
            //    {
            //        int[] cds = conn.Capabilities.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
            //        m.Enabled = Array.IndexOf<int>(cds, (int)CommandType.CommandType_Delete) >= 0;
            //    }
            //    //else if (c is ManageSchemaCommand)
            //    //{
            //    //    bool? value = conn.Capabilities.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSchemaModification);
            //    //    c.IsEnabled = value.HasValue ? value : false;
            //    //}
            //}
        }

        private void CheckConnectionCommands(FdoConnection conn)
        {
            //LoggingService.Debug("Checking connection command state");
            //throw new Exception("The method or operation is not implemented.");
        }

        void OnConnectionRefreshed(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.RemoveByKey(e.Data);
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = IMG_CONNECTION;
            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_CONNECTION);

            GetSchemaNodes(node);
            root.Nodes.Add(node);
            node.Expand();
            root.Expand();
        }

        void OnConnectionRemoved(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            _explorer.GetRootNode(RootNodeName).Nodes.RemoveByKey(e.Data);
        }

        void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e)
        {
            TreeNode node = _explorer.GetRootNode(RootNodeName).Nodes[e.OldName];
            node.Name = node.Text = e.NewName;
        }

        void OnConnectionAdded(object sender, FdoToolbox.Core.EventArgs<string> e)
        {
            string name = e.Data;
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = IMG_CONNECTION;
            node.ContextMenuStrip = _explorer.GetContextMenu(NODE_CONNECTION);

            GetSchemaNodes(node);
            TreeNode root = _explorer.GetRootNode(RootNodeName);
            root.Nodes.Add(node);
            node.Expand();
            root.Expand();
        }

        void GetSchemaNodes(TreeNode connNode)
        {
            FdoConnection conn = _connMgr.GetConnection(connNode.Name);
            if (conn != null)
            {
                FdoFeatureService service = conn.CreateFeatureService();
                connNode.ToolTipText = string.Format("Provider: {0}\nConnection String: {1}", conn.Provider, conn.ConnectionString);
                FeatureSchemaCollection schemas = service.DescribeSchema();
                foreach (FeatureSchema schema in schemas)
                {
                    TreeNode schemaNode = new TreeNode();
                    schemaNode.Name = schemaNode.Text = schema.Name;
                    schemaNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_SCHEMA);
                    schemaNode.ImageKey = schemaNode.SelectedImageKey = IMG_SCHEMA;
                    GetClassNodes(schema, schemaNode);
                    connNode.Nodes.Add(schemaNode);
                    schemaNode.Expand();
                }
            }
        }

        void GetClassNodes(FeatureSchema schema, TreeNode schemaNode)
        {
            foreach (ClassDefinition classDef in schema.Classes)
            {
                TreeNode classNode = new TreeNode();
                classNode.Name = classNode.Text = classDef.Name;
                classNode.ContextMenuStrip = _explorer.GetContextMenu(NODE_CLASS);
                classNode.ImageKey = classNode.SelectedImageKey = IMG_CLASS;
                classNode.ToolTipText = string.Format("Type: {0}", classDef.ClassType);
                GetPropertyNodes(classDef, classNode);
                schemaNode.Nodes.Add(classNode);
            }
        }

        void GetPropertyNodes(ClassDefinition classDef, TreeNode classNode)
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
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_DATA_PROPERTY;
                            if (classDef.IdentityProperties.Contains(dataDef))
                                propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_ID_PROPERTY;
                            propertyNode.ToolTipText = string.Format("Data Type: {0}\nLength: {1}\nAuto-Generated: {2}\nRead-Only: {3}", dataDef.DataType, dataDef.Length, dataDef.IsAutoGenerated, dataDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        {
                            GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_GEOM_PROPERTY;
                            propertyNode.ToolTipText = string.Format("Has Elevation: {0}\nHas Measure: {1}\nRead-Only: {2}", geomDef.HasElevation, geomDef.HasMeasure, geomDef.ReadOnly);
                        }
                        break;
                    case PropertyType.PropertyType_RasterProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_RASTER_PROPERTY;
                            propertyNode.ToolTipText = "Raster Property";
                        }
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_OBJECT_PROPERTY;
                            propertyNode.ToolTipText = "Object Property";
                        }
                        break;
                    case PropertyType.PropertyType_AssociationProperty:
                        {
                            propertyNode.ImageKey = propertyNode.SelectedImageKey = IMG_ASSOC_PROPERTY;
                            propertyNode.ToolTipText = "Association Property";
                        }
                        break;
                }
                classNode.Nodes.Add(propertyNode);
            }
        }
    }
}
