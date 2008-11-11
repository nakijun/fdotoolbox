using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Commands
{
    public class SaveConsoleLogCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_LOG"), ResourceService.GetString("FILTER_LOG_FILE"));
                if (FileService.FileExists(file))
                {
                    System.IO.File.WriteAllText(file, wb.Console.TextContent);
                    MessageService.ShowMessage("Log saved to " + file, "Log saved");
                }
            }
        }
    }

    public class ClearConsoleCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            if (wb != null)
            {
                wb.Console.Clear();
            }
        }
    }
}
