using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Commands;
using OSGeo.MapGuide.MaestroAPI;
using FdoToolbox.Core;
using MGModule.Forms;
using FdoToolbox.Core.ClientServices;
using MGModule.Controls;

namespace MGModule
{
    public class MapGuideModule : ModuleBase
    {
        #region Command Names

        const string CMD_MG_CONNECT = "mg_connect";
        const string CMD_MG_DISCONNECT = "mg_disconnect";
        const string CMD_MG_DATAPREVIEW = "mg_datapreview";
        const string CMD_MG_REFRESH = "mg_refresh";
        
        #endregion

        public override string Name
        {
            get { return "mapguide"; }
        }

        public override string Description
        {
            get { return "MapGuide Extension Module"; }
        }

        public override void Initialize()
        {
            _App = AppGateway.RunningApplication;
            _ConnMgr = new MapGuideConnectionMgr();
            _ConnMgr.ConnectionAdded += new MgConnectionHandler(OnConnectionAdded);
            _ConnMgr.ConnectionRemoved += new MgConnectionHandler(OnConnectionRemoved);
            RegisterObjectExplorerImages();
            RegisterMapGuideNode();
            _App.Shell.ObjectExplorer.RegisterContextMenu(MG_FEATURE_SOURCES, new System.Windows.Forms.ContextMenuStrip());
            _App.Shell.ObjectExplorer.RegisterContextMenu(MG_SELECTED_FEATURE_SOURCE, new System.Windows.Forms.ContextMenuStrip());
            _App.Shell.ObjectExplorer.RegisterContextMenu(MG_SELECTED_FEATURE_CLASS, new System.Windows.Forms.ContextMenuStrip());
        }

        private void RegisterObjectExplorerImages()
        {
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_CLASS, Properties.Resources.database_table);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_CONNECTION, Properties.Resources.server_connect);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_DATA_PROPERTY, Properties.Resources.table);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_FEATURE_SOURCE, Properties.Resources.database);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_GEOMETRY_PROPERTY, Properties.Resources.shape_handles);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_IDENTITY_PROPERTY, Properties.Resources.table_key);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_SCHEMA, Properties.Resources.chart_organisation);
            _App.Shell.ObjectExplorer.RegisterImage(MapGuideImages.MG_SERVERS, Properties.Resources.server);
        }

        void OnConnectionRemoved(Uri host)
        {
            System.Windows.Forms.TreeNode mgNode = _App.Shell.ObjectExplorer.GetRootNode(MG_SERVERS);
            mgNode.Nodes.RemoveByKey(host.ToString());
        }

        void OnConnectionAdded(Uri host)
        {
            System.Windows.Forms.TreeNode mgNode = _App.Shell.ObjectExplorer.GetRootNode(MG_SERVERS);
            System.Windows.Forms.TreeNode connNode = CreateConnectionNode(host);
            mgNode.Nodes.Add(connNode);
        }

        private System.Windows.Forms.TreeNode CreateConnectionNode(Uri host)
        {
            ServerConnectionI conn = _ConnMgr.GetConnection(host);
            System.Windows.Forms.TreeNode connNode = new System.Windows.Forms.TreeNode();
            connNode.Name = MG_FEATURE_SOURCES;
            connNode.Text = host.ToString();
            connNode.Tag = host;
            connNode.SelectedImageKey = connNode.ImageKey = MapGuideImages.MG_CONNECTION;
            connNode.ContextMenuStrip = _App.Shell.ObjectExplorer.GetContextMenu(MG_FEATURE_SOURCES);
            connNode.ToolTipText = conn.DisplayName;
            PopulateFeatureSources(connNode, conn);
            return connNode;
        }

        private void PopulateFeatureSources(System.Windows.Forms.TreeNode connNode, ServerConnectionI conn)
        {
            ResourceList featureSources = conn.GetRepositoryResources("Library://", "FeatureSource");
            connNode.Nodes.Clear();
            foreach (object res in featureSources.Items)
            {
                ResourceListResourceDocument doc = res as ResourceListResourceDocument;
                System.Windows.Forms.TreeNode fsNode = new System.Windows.Forms.TreeNode();
                fsNode.Text = doc.ResourceId;
                fsNode.Name = doc.ResourceId;
                fsNode.Tag = doc;
                fsNode.ImageKey = fsNode.SelectedImageKey = MapGuideImages.MG_FEATURE_SOURCE;
                fsNode.ToolTipText = doc.ResourceId;
                fsNode.ContextMenuStrip = _App.Shell.ObjectExplorer.GetContextMenu(MG_SELECTED_FEATURE_SOURCE);
                PopulateSchemaNodes(fsNode, conn);
                connNode.Nodes.Add(fsNode);
            }
        }

        private void PopulateSchemaNodes(System.Windows.Forms.TreeNode fsNode, ServerConnectionI conn)
        {
            FeatureSourceDescription desc = conn.DescribeFeatureSource(fsNode.Name);
            foreach (OSGeo.MapGuide.MaestroAPI.FeatureSourceDescription.FeatureSourceSchema
                schema in desc.Schemas)
            {
                System.Windows.Forms.TreeNode schemaNode = new System.Windows.Forms.TreeNode();
                schemaNode.Text = schemaNode.Name = schema.Schema;
                schemaNode.ToolTipText = schema.Schema;
                schemaNode.SelectedImageKey = schemaNode.ImageKey = MapGuideImages.MG_SCHEMA;
                PopulateClassNodes(schemaNode, schema);
                fsNode.Nodes.Add(schemaNode);
            }
        }

        private void PopulateClassNodes(System.Windows.Forms.TreeNode schemaNode, FeatureSourceDescription.FeatureSourceSchema schema)
        {
            System.Windows.Forms.TreeNode classNode = new System.Windows.Forms.TreeNode();
            classNode.Text = classNode.Name = schema.Name;
            classNode.ToolTipText = schema.Name;
            classNode.ImageKey = classNode.SelectedImageKey = MapGuideImages.MG_CLASS;
            PopulatePropertyNodes(classNode, schema.Columns);
            schemaNode.Nodes.Add(classNode);
        }

        private void PopulatePropertyNodes(System.Windows.Forms.TreeNode classNode, FeatureSetColumn[] properties)
        {
            foreach (FeatureSetColumn prop in properties)
            {
                System.Windows.Forms.TreeNode propNode = new System.Windows.Forms.TreeNode();
                propNode.Text = propNode.Name = prop.Name;
                propNode.ToolTipText = prop.Name;
                propNode.SelectedImageKey = propNode.ImageKey = MapGuideImages.MG_DATA_PROPERTY;
                classNode.Nodes.Add(propNode);
            }
        }

        public override void Cleanup()
        {

        }

        #region Object Explorer Node Names

        const string MG_SERVERS = "MG_SERVERS";
        const string MG_FEATURE_SOURCES = "MG_FEATURE_SOURCES";
        const string MG_SELECTED_FEATURE_SOURCE = "MG_SELECTED_FEATURE_SOURCE";
        const string MG_SELECTED_FEATURE_CLASS = "MG_SELECTED_FEATURE_CLASS";

        #endregion

        private void RegisterMapGuideNode()
        {
            System.Windows.Forms.TreeNode mgNode = new System.Windows.Forms.TreeNode();
            mgNode.Text = "MapGuide Servers";
            mgNode.Name = MG_SERVERS;
            mgNode.ImageKey = mgNode.SelectedImageKey = MapGuideImages.MG_SERVERS;
            mgNode.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _App.Shell.ObjectExplorer.RegisterRootNode(MG_SERVERS, mgNode);
        }

        private IHostApplication _App;

        private MapGuideConnectionMgr _ConnMgr;

        [Command(MapGuideModule.CMD_MG_CONNECT, "Connect to a MapGuide Server")]
        void Connect()
        {
            ConnectDlg dlg = new ConnectDlg();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Uri site = dlg.SiteUrl;
                string user = dlg.Username;
                string pass = dlg.Password;

                ServerConnectionI conn = _ConnMgr.GetConnection(site);
                if (conn == null)
                {
                    conn = new HttpServerConnection(site, user, pass, "en", true);
                    _ConnMgr.AddConnection(site, conn);
                }
            }
        }

        [Command(MapGuideModule.CMD_MG_DISCONNECT, "Disconnect")]
        void Disconnect()
        {

        }

        [Command(MapGuideModule.CMD_MG_DATAPREVIEW, "Data Preview")]
        void DataPreview()
        {
            System.Windows.Forms.TreeNode fsNode = _App.Shell.ObjectExplorer.GetSelectedNode();
            //Feature Source level, walk back up to connections level
            System.Windows.Forms.TreeNode connNode = fsNode.Parent;
            ServerConnectionI conn = _ConnMgr.GetConnection(connNode.Tag as Uri);
            MgDataPreviewCtl ctl = new MgDataPreviewCtl(conn);
            ctl.FeatureSourceId = fsNode.Name;
            _App.Shell.ShowDocumentWindow(ctl);
        }

        [Command(MapGuideModule.CMD_MG_REFRESH, "Refresh")]
        void Refresh()
        {
            System.Windows.Forms.TreeNode node = _App.Shell.ObjectExplorer.GetSelectedNode();
            Uri host = node.Tag as Uri;
            ServerConnectionI conn = _ConnMgr.GetConnection(host);
            PopulateFeatureSources(node, conn);
        }
    }
}
