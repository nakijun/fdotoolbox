using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using System.Resources;
using System.Reflection;
using FdoToolbox.Base.Services;

using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Core;

namespace FdoToolbox.Base.Commands
{
    public class StartupCommand : AbstractCommand
    {
        public override void Run()
        {
            ServiceManager svcMgr = ServiceManager.Instance;
            EventWatcher.Initialize();
            Res.RegisterNeutralStrings(FdoToolbox.Base.Strings.ResourceManager);
            Res.RegisterNeutralImages(FdoToolbox.Base.Images.ResourceManager);

            Workbench.WorkbenchInitialized += delegate
            {
                Workbench wb = Workbench.Instance;
                Msg.MainForm = wb;
                wb.SetTitle(Res.GetString("UI_TITLE"));
            };
        }
    }
}
