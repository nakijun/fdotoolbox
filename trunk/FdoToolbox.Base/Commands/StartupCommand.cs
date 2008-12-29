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
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base.Commands
{
    public class StartupCommand : AbstractCommand
    {
        public override void Run()
        {
            EventWatcher.Initialize();
            ServiceManager svcMgr = ServiceManager.Instance;
            
            Res.RegisterNeutralStrings(FdoToolbox.Base.Strings.ResourceManager);
            Res.RegisterNeutralImages(FdoToolbox.Base.Images.ResourceManager);
            Res.RegisterNeutralStrings(ResourceUtil.StringResourceManager);

            Workbench.WorkbenchInitialized += delegate
            {
                Workbench wb = Workbench.Instance;
                List<IObjectExplorerExtender> extenders = AddInTree.BuildItems<IObjectExplorerExtender>("/ObjectExplorer/Extenders", this);
                if (extenders != null)
                {
                    foreach (IObjectExplorerExtender dec in extenders)
                    {
                        dec.Decorate(wb.ObjectExplorer);
                    }
                }

                svcMgr.RestoreSession();
                Msg.MainForm = wb;
                wb.SetTitle(Res.GetString("UI_TITLE"));

                wb.FormClosing += delegate
                {
                    svcMgr.UnloadAllServices();
                };
            };
        }
    }
}
