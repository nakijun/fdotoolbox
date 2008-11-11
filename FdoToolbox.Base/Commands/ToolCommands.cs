using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base.Commands
{
    public class CoordSysCatalogCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                CoordSysCatalog cat = new CoordSysCatalog();
                wb.ShowContent(cat, ViewRegion.Document);
            }
        }
    }

    public class SchemaEditorCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoSchemaMgrCtl ctl = new FdoSchemaMgrCtl();
                ctl.Standalone = true;
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
