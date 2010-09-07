using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.DataStoreManager.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.DataStoreManager.Commands
{
    internal class SchemaEditorCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoSchemaDesignerCtl ctl = new FdoSchemaDesignerCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
