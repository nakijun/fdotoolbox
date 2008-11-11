using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base.Commands
{
    public class RegisterProviderCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoRegProviderCtl ctl = new FdoRegProviderCtl();
                wb.ShowContent(ctl, ViewRegion.Dialog);
            }
        }
    }

    public class UnregisterProviderCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoUnregProviderCtl ctl = new FdoUnregProviderCtl();
                wb.ShowContent(ctl, ViewRegion.Dialog);
            }
        }
    }
}
