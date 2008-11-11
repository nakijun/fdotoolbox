using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Helper class to log common events to the console
    /// </summary>
    public sealed class EventWatcher
    {
        public static void Initialize()
        {
            FdoConnectionManager manager = ServiceManager.Services.GetService<FdoConnectionManager>();
            manager.ConnectionAdded += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection added: {0}", e.Data);
            };
            manager.ConnectionRemoved += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection removed: {0}", e.Data);
            };
            manager.ConnectionRenamed += delegate(object sender, ConnectionRenameEventArgs e)
            {
                LoggingService.InfoFormatted("Connection {0} renamed to {1}", e.OldName, e.NewName);
            };
            manager.ConnectionRefreshed += delegate(object sender, EventArgs<string> e)
            {
                LoggingService.InfoFormatted("Connection {0} refreshed", e.Data);
            };
        }
    }
}
