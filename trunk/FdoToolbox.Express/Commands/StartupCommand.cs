using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Core;
using FdoToolbox.Base;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Express.Commands
{
    public class StartupCommand : AbstractCommand
    {
        public override void Run()
        {
            ResourceService.RegisterNeutralStrings(FdoToolbox.Express.Strings.ResourceManager);
            ResourceService.RegisterNeutralImages(FdoToolbox.Express.Images.ResourceManager);
        }
    }
}
