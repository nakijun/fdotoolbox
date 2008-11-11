using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

using Msg = ICSharpCode.Core.MessageService;
using Res = ICSharpCode.Core.ResourceService;
using Log = ICSharpCode.Core.LoggingService;
using OSGeo.FDO.Schema;
using FdoToolbox.Base.Forms;
using System.Collections.Specialized;

namespace FdoToolbox.Base.Commands
{
    public class RemoveAllConnectionsCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            mgr.Clear();
        }
    }

    public class RefreshConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            mgr.RefreshConnection(connNode.Name);
        }
    }

    public class RemoveConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            mgr.RemoveConnection(connNode.Name);
        }
    }

    public class RenameConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();

            string name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), connNode.Name);
            if (name == null)
                return;

            while (name == string.Empty || mgr.NameExists(name))
            {
                name = Msg.ShowInputBox(Res.GetString("TITLE_RENAME_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), connNode.Name);
                if (name == null)
                    return;
            }

            mgr.RenameConnection(connNode.Name, name);
        }
    }

    public class SaveConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.SaveFile(Res.GetString("TITLE_SAVE_CONNECTION"), Res.GetString("FILTER_CONNECTION_FILE"));
            if (!string.IsNullOrEmpty(path))
            {
                TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                conn.Save(path);
                Log.InfoFormatted("Connection saved to: {0}", path);
            }
        }
    }

    public class LoadConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.OpenFile(Res.GetString("TITLE_LOAD_CONNECTION"), Res.GetString("FILTER_CONNECTION_FILE"));
            if (FileService.FileExists(path))
            {
                FdoConnection conn = FdoConnection.LoadFromFile(path);
                FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();

                string name = string.Empty;
                name = Msg.ShowInputBox(Res.GetString("TITLE_NEW_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
                if (name == null)
                    return;

                while (name == string.Empty || mgr.NameExists(name))
                {
                    name = Msg.ShowInputBox(Res.GetString("TITLE_NEW_CONNECTION"), Res.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
                    if (name == null)
                        return;
                }

                mgr.AddConnection(name, conn);
            }
        }
    }

    public class LoadSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.OpenFile(Res.GetString("TITLE_LOAD_SCHEMA"), Res.GetString("FILTER_SCHEMA_FILE"));
            if (FileService.FileExists(path))
            {
                TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    service.LoadSchemasFromXml(path);
                    Log.InfoFormatted("Schemas loaded into connection {0} from {1}", connNode.Name, path);
                }
            }
        }
    }

    public class SaveSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.SaveFile(Res.GetString("TITLE_SAVE_SCHEMA"), Res.GetString("FILTER_SCHEMA_FILE"));
            if (!string.IsNullOrEmpty(path))
            {
                TreeNode node = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                if (node.Level == 1) //Connection
                {
                    TreeNode connNode = node;
                    FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(connNode.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.WriteSchemaToXml(path);
                        Log.InfoFormatted("Schemas saved to {0}", path);
                    }
                }
                else if (node.Level == 2) //Schema
                {
                    TreeNode schemaNode = node;
                    TreeNode connNode = node.Parent;
                    FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(connNode.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.WriteSchemaToXml(schemaNode.Name, path);
                        Log.InfoFormatted("Schema {0} saved to {1}", connNode.Name, path);
                    }
                }
            }
        }
    }
    
    public class DeleteSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode schemaNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            FdoConnection conn = mgr.GetConnection(schemaNode.Parent.Name);
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                service.DestroySchema(schemaNode.Name);
                Msg.ShowMessage("Schema Deleted", "Delete Schema");
                Log.InfoFormatted("Schema {0} delete from connection: {1}", schemaNode.Name, schemaNode.Parent.Name);
            }
        }
    }

    public class DataPreviewCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode connNode = wb.ObjectExplorer.GetSelectedNode();
                while (connNode.Level > 1)
                    connNode = connNode.Parent;
                
                FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);

                FdoDataPreviewCtl ctl = new FdoDataPreviewCtl(conn);
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    public class ManageSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode connNode = null;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1) //Connection
            {
                connNode = node;
            }
            else if (node.Level >= 2) //Schema or lower
            {
                while (node.Level > 2)
                {
                    node = node.Parent;
                }
                connNode = node.Parent;
            }
            if (connNode != null)
            {
                FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);

                if (conn != null)
                {
                    FdoSchemaMgrCtl ctl = new FdoSchemaMgrCtl(conn);
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }

    public class EditClassAttributesCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                if (node.Level == 3)
                {
                    FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Parent.Parent.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        string sName = node.Parent.Name;
                        string cName = node.Name;
                        ClassDefinition cd = service.GetClassByName(sName, cName);
                        if (cd != null)
                        {
                            NameValueCollection nvc = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_EDIT_CLASS_ATTRIBUTES"), cd.Attributes);
                            if (nvc != null && nvc.Count > 0)
                            {
                                foreach (string key in nvc.AllKeys)
                                {
                                    cd.Attributes.SetAttributeValue(key, nvc[key]);
                                }
                                service.ApplySchema(cd.FeatureSchema);
                                LoggingService.Info("Class attributes saved");
                            }
                        }
                    }
                }
            }
        }
    }

    public class EditSchemaAttributesCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                if (node.Level == 2)
                {
                    FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Parent.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        string sName = node.Name;
                        FeatureSchema fs = service.GetSchemaByName(sName);
                        if (fs != null)
                        {
                            NameValueCollection nvc = DictionaryDialog.GetParameters(ResourceService.GetString("TITLE_EDIT_SCHEMA_ATTRIBUTES"), fs.Attributes);
                            if (nvc != null && nvc.Count > 0)
                            {
                                foreach (string key in nvc.AllKeys)
                                {
                                    fs.Attributes.SetAttributeValue(key, nvc[key]);
                                }
                                service.ApplySchema(fs);
                                LoggingService.Info("Class attributes saved");
                            }
                        }
                    }
                }
            }
        }
    }
}
