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
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            mgr.Clear();
        }
    }

    public class RefreshConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
            {
                mgr.RefreshConnection(connNode.Name);
            }
        }
    }

    public class RemoveConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            mgr.RemoveConnection(connNode.Name);
        }
    }

    public class RenameConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            TreeNode connNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

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
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
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
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

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

                using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
                {
                    mgr.AddConnection(name, conn);
                }
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
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
                    {
                        service.LoadSchemasFromXml(path);
                    }
                    MessageService.ShowMessageFormatted(Res.GetString("MSG_SCHEMA_LOADED"), connNode.Name, path);
                    Log.InfoFormatted(Res.GetString("MSG_SCHEMA_LOADED"), connNode.Name, path);
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
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(connNode.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.WriteSchemaToXml(path);
                        Log.InfoFormatted(Res.GetString("LOG_SCHEMA_SAVED"), path);
                    }
                }
                else if (node.Level == 2) //Schema
                {
                    TreeNode schemaNode = node;
                    TreeNode connNode = node.Parent;
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(connNode.Name);
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.WriteSchemaToXml(schemaNode.Name, path);
                        Log.InfoFormatted(Res.GetString("LOG_SCHEMA_SAVED_2"), connNode.Name, path);
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
            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            FdoConnection conn = mgr.GetConnection(schemaNode.Parent.Name);
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                //TODO: This command should be preemptively disabled, as it was in 0.6 and before.
                try
                {
                    service.DestroySchema(schemaNode.Name);
                    Msg.ShowMessage(Res.GetString("MSG_SCHEMA_DELETED"), Res.GetString("TITLE_DELETE_SCHEMA"));
                    Log.InfoFormatted(Res.GetString("LOG_SCHEMA_DELETED"), schemaNode.Name, schemaNode.Parent.Name);
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    Msg.ShowError(ex);
                }
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
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                TreeNode connNode = wb.ObjectExplorer.GetSelectedNode();
                while (connNode.Level > 1)
                    connNode = connNode.Parent;
                
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(connNode.Name);
                FdoDataPreviewCtl ctl = null;
                if (node.Level > 1) //Class node
                    ctl = new FdoDataPreviewCtl(conn, node.Parent.Name, node.Name);
                else
                    ctl = new FdoDataPreviewCtl(conn);
                
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    public class EditSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode connNode = null;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if(node.Level == 2)
            {
                connNode = node.Parent;
                string name = connNode.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(name);

                if (conn != null)
                {
                    FdoSchemaDesignerCtl ctl = new FdoSchemaDesignerCtl(conn, node.Name);
                    ctl.SchemaApplied += delegate
                    {
                        mgr.RefreshConnection(name);
                    };
                    
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
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
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
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
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

    public class ManageSpatialContextsCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                if (node.Level == 1)
                {
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Name);

                    FdoSpatialContextMgrCtl ctl = new FdoSpatialContextMgrCtl(conn);
                    
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }

    public class ManageDataStoresCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                TreeNode node = wb.ObjectExplorer.GetSelectedNode();
                if (node.Level == 1)
                {
                    FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = mgr.GetConnection(node.Name);

                    FdoDataStoreMgrCtl ctl = new FdoDataStoreMgrCtl(conn);
                    
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }

    public class CreateSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1)
            {
                string name = node.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(name);

                if (conn != null)
                {
                    FdoSchemaDesignerCtl ctl = new FdoSchemaDesignerCtl(conn);
                    ctl.SchemaApplied += delegate
                    {
                        mgr.RefreshConnection(name);
                    };
                    
                    wb.ShowContent(ctl, ViewRegion.Document);
                }
            }
        }
    }

    public class ConfigureConnectionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 1)
            {
                string name = node.Name;
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(name);

                if (conn != null)
                {

                }
            }
        }
    }
}
