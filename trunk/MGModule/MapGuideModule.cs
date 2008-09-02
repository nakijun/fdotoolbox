using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Commands;
using OSGeo.MapGuide.MaestroAPI;
using FdoToolbox.Core;
using MGModule.Forms;
using FdoToolbox.Core.ClientServices;

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
            RegisterMapGuideNode();
            _App.Shell.ObjectExplorer.RegisterContextMenu(MG_FEATURE_SOURCES, new System.Windows.Forms.ContextMenuStrip());
            _App.Shell.ObjectExplorer.RegisterContextMenu(MG_SELECTED_FEATURE_SOURCE, new System.Windows.Forms.ContextMenuStrip());
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
            System.Windows.Forms.TreeNode connNode = new System.Windows.Forms.TreeNode();
            connNode.Name = MG_FEATURE_SOURCES;
            connNode.Text = host.ToString();
            connNode.Tag = host;
            connNode.ContextMenuStrip = _App.Shell.ObjectExplorer.GetContextMenu(MG_FEATURE_SOURCES);
            connNode.ToolTipText = "HTTP connection: " + host.ToString();
            PopulateFeatureSources(connNode, host);
            return connNode;
        }

        private void PopulateFeatureSources(System.Windows.Forms.TreeNode connNode, Uri host)
        {
            ServerConnectionI conn = _ConnMgr.GetConnection(host);
            ResourceList featureSources = conn.GetRepositoryResources("Library://", "FeatureSource");
            connNode.Nodes.Clear();
            foreach (object res in featureSources.Items)
            {
                ResourceListResourceDocument doc = res as ResourceListResourceDocument;
                System.Windows.Forms.TreeNode fsNode = new System.Windows.Forms.TreeNode();
                fsNode.Text = doc.ResourceId;
                fsNode.Name = doc.ResourceId;
                fsNode.Tag = doc;
                fsNode.ToolTipText = doc.ResourceId;
                fsNode.ContextMenuStrip = _App.Shell.ObjectExplorer.GetContextMenu(MG_SELECTED_FEATURE_SOURCE);

                connNode.Nodes.Add(fsNode);
            }
        }

        public override void Cleanup()
        {

        }

        #region Object Explorer Node Names

        const string MG_SERVERS = "MG_SERVERS";
        const string MG_FEATURE_SOURCES = "MG_FEATURE_SOURCES";
        const string MG_SELECTED_FEATURE_SOURCE = "MG_SELECTED_FEATURE_SOURCE";

        #endregion

        private void RegisterMapGuideNode()
        {
            System.Windows.Forms.TreeNode mgNode = new System.Windows.Forms.TreeNode();
            mgNode.Text = "MapGuide Servers";
            mgNode.Name = MG_SERVERS;
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

        }

        [Command(MapGuideModule.CMD_MG_REFRESH, "Refresh")]
        void Refresh()
        {
            System.Windows.Forms.TreeNode node = _App.Shell.ObjectExplorer.GetSelectedNode();
            Uri host = node.Tag as Uri;
            PopulateFeatureSources(node, host);
        }
    }
}
