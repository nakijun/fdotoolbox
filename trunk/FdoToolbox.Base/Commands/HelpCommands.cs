using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Forms;
using System.Diagnostics;

namespace FdoToolbox.Base.Commands
{
    public class AboutCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            new AboutDialog().ShowDialog();
        }
    }

    public class HelpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Process.Start("userdoc.chm"); //TODO: Don't hardcode
        }
    }

    public class ApiHelpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Process.Start("FDO Toolbox Core API.chm"); //TODO: Don't hardcode
        }
    }
}
