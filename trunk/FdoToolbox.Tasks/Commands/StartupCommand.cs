using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base;
using FdoToolbox.Tasks.Controls;

namespace FdoToolbox.Tasks.Commands
{
    public class StartupCommand : AbstractCommand
    {
        public override void Run()
        {
            ResourceService.RegisterNeutralStrings(Strings.ResourceManager);
            EventWatcher.Initialize();
        }
    }
}
