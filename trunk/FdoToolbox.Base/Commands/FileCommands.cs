using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base.Commands
{
    public class ExitCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                wb.Close();
            }
        }
    }

    public class ConnectCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoConnectCtl ctl = new FdoConnectCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }

    public class CreateDataStoreCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                FdoCreateDataStoreCtl ctl = new FdoCreateDataStoreCtl();
                wb.ShowContent(ctl, ViewRegion.Document);
            }
        }
    }
}
