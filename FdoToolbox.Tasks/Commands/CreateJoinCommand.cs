using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base;
using FdoToolbox.Tasks.Controls;

namespace FdoToolbox.Tasks.Commands
{
    public class CreateJoinCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoJoinCtl ctl = new FdoJoinCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
