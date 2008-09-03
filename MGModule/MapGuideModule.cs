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
using System.IO;

namespace MGModule
{
    public class MapGuideModule : ModuleBase
    {
        #region Command Names

        const string CMD_MG_CONNECT_HTTP = "mg_connect_http";
        const string CMD_MG_CONNECT_LOCAL = "mg_connect_local";
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

        void OnConnectionRemoved(string host)
        {
            System.Windows.Forms.TreeNode mgNode = _App.Shell.ObjectExplorer.GetRootNode(MG_SERVERS);
            mgNode.Nodes.RemoveByKey(host.ToString());
            AppConsole.WriteLine("Disconnected from: {0}", host.ToString());
        }

        void OnConnectionAdded(string host)
        {
            System.Windows.Forms.TreeNode mgNode = _App.Shell.ObjectExplorer.GetRootNode(MG_SERVERS);
            System.Windows.Forms.TreeNode connNode = CreateConnectionNode(host);
            mgNode.Nodes.Add(connNode);
            ServerConnectionI conn = _ConnMgr.GetConnection(host);
            AppConsole.WriteLine("Connected to: {0}", host.ToString());
        }

        private System.Windows.Forms.TreeNode CreateConnectionNode(string host)
        {
            ServerConnectionI conn = _ConnMgr.GetConnection(host.ToString());
            System.Windows.Forms.TreeNode connNode = new System.Windows.Forms.TreeNode();
            connNode.Name = host;
            connNode.Text = conn.DisplayName;
            connNode.Tag = host;
            connNode.SelectedImageKey = connNode.ImageKey = MapGuideImages.MG_CONNECTION;
            connNode.ContextMenuStrip = _App.Shell.ObjectExplorer.GetContextMenu(MG_FEATURE_SOURCES);
            connNode.ToolTipText = string.Format("{0}\n\nVersion: {1}", conn.DisplayName, conn.SiteVersion);
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
                try
                {
                    PopulateSchemaNodes(fsNode, conn);
                    connNode.Nodes.Add(fsNode);
                }
                catch
                {
                    //TODO: Log this error
                }
            }
        }

        private void PopulateSchemaNodes(System.Windows.Forms.TreeNode fsNode, ServerConnectionI conn)
        {
            FeatureSourceDescription desc = conn.DescribeFeatureSource(fsNode.Name);
            Dictionary<string, List<FeatureSourceDescription.FeatureSourceSchema>> classes = new Dictionary<string, List<FeatureSourceDescription.FeatureSourceSchema>>();
            foreach (FeatureSourceDescription.FeatureSourceSchema schema in desc.Schemas)
            {
                if (!classes.ContainsKey(schema.Schema))
                    classes[schema.Schema] = new List<FeatureSourceDescription.FeatureSourceSchema>();

                classes[schema.Schema].Add(schema);
            }

            foreach (string schema in classes.Keys)
            {
                System.Windows.Forms.TreeNode schemaNode = new System.Windows.Forms.TreeNode();
                schemaNode.Text = schemaNode.Name = schema;
                schemaNode.ToolTipText = schema;
                schemaNode.SelectedImageKey = schemaNode.ImageKey = MapGuideImages.MG_SCHEMA;
                List<FeatureSourceDescription.FeatureSourceSchema> klasses = classes[schema];

                foreach (FeatureSourceDescription.FeatureSourceSchema klass in klasses)
                {
                    System.Windows.Forms.TreeNode classNode = new System.Windows.Forms.TreeNode();
                    classNode.Text = classNode.Name = klass.Name;
                    classNode.ToolTipText = klass.Name;
                    classNode.ImageKey = classNode.SelectedImageKey = MapGuideImages.MG_CLASS;
                    PopulatePropertyNodes(classNode, klass.Columns);
                    schemaNode.Nodes.Add(classNode);
                }

                fsNode.Nodes.Add(schemaNode);
            }
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

        protected override System.Resources.ResourceManager GetResourceManager()
        {
            //We want to use this assembly's Resources.resx
            return Properties.Resources.ResourceManager;
        }

        private IHostApplication _App;

        private MapGuideConnectionMgr _ConnMgr;

        [Command(MapGuideModule.CMD_MG_CONNECT_HTTP, "Connect to a remote MapGuide Server (HTTP)", ImageResourceName = "server_connect")]
        void ConnectHttp()
        {
            HttpConnectDlg dlg = new HttpConnectDlg();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Uri site = dlg.SiteUrl;
                string user = dlg.Username;
                string pass = dlg.Password;
                string siteStr = site.ToString();

                ServerConnectionI conn = _ConnMgr.GetConnection(siteStr);
                if (conn == null)
                {
                    conn = new HttpServerConnection(site, user, pass, "en", true);
                    _ConnMgr.AddConnection(siteStr, conn);
                }
            }
        }

        [Command(MapGuideModule.CMD_MG_CONNECT_LOCAL, "Connect to a local MapGuide Server", ImageResourceName = "server_connect")]
        void ConnectLocal()
        {
            LocalConnectDlg dlg = new LocalConnectDlg();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ip = dlg.ServerIP;
                string user = dlg.Username;
                string pass = dlg.Password;
                Version version = dlg.SiteVersion;

                ServerConnectionI conn = _ConnMgr.GetConnection(ip);
                if (conn == null)
                {
                    string confText = GetConfigForVersion(version);
                    string configFile = System.IO.Path.GetTempFileName();
                    confText = string.Format(confText, ip);

                    //Write out temp config file
                    File.WriteAllText(configFile, confText);
                    conn = new LocalNativeConnection(configFile, user, pass, "en");
                }
            }
        }

        private string GetConfigForVersion(Version version)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Command(MapGuideModule.CMD_MG_DISCONNECT, "Disconnect", ImageResourceName = "server_delete")]
        void Disconnect()
        {
            System.Windows.Forms.TreeNode connNode = _App.Shell.ObjectExplorer.GetSelectedNode();
            string host = connNode.Tag.ToString();
            _ConnMgr.RemoveConnection(host);
        }

        [Command(MapGuideModule.CMD_MG_DATAPREVIEW, "Data Preview", ImageResourceName = "zoom")]
        void DataPreview()
        {
            System.Windows.Forms.TreeNode fsNode = _App.Shell.ObjectExplorer.GetSelectedNode();
            //Feature Source level, walk back up to connections level
            System.Windows.Forms.TreeNode connNode = fsNode.Parent;
            ServerConnectionI conn = _ConnMgr.GetConnection(connNode.Tag.ToString());
            MgDataPreviewCtl ctl = new MgDataPreviewCtl(conn);
            ctl.FeatureSourceId = fsNode.Name;
            _App.Shell.ShowDocumentWindow(ctl);
        }

        [Command(MapGuideModule.CMD_MG_REFRESH, "Refresh", ImageResourceName = "page_refresh")]
        void Refresh()
        {
            System.Windows.Forms.TreeNode node = _App.Shell.ObjectExplorer.GetSelectedNode();
            string host = node.Tag.ToString();
            ServerConnectionI conn = _ConnMgr.GetConnection(host);
            PopulateFeatureSources(node, conn);
        }
    }
}
