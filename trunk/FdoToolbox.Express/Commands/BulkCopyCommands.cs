using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Express.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Commands
{
    public class ExpressBulkCopyCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            FileToFileCtl ctl = new FileToFileCtl();
            Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}
