using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls;

namespace FdoToolbox.OverrideManager.Commands
{
    internal class CreateSchemaCommand : AbstractMenuCommand
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

    internal class EditSchemaCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode connNode = null;
            TreeNode node = wb.ObjectExplorer.GetSelectedNode();
            if (node.Level == 2)
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
}
