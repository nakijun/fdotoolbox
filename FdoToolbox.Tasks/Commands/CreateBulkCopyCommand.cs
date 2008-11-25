using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Tasks.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Tasks.Commands
{
    public class CreateBulkCopyCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoBulkCopyCtl ctl = new FdoBulkCopyCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
