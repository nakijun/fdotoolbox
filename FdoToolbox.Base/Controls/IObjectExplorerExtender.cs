using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// An extender interface that extends the Object Explorer with custom
    /// root nodes and context menus.
    /// </summary>
    public interface IObjectExplorerExtender
    {
        void Decorate(IObjectExplorer explorer);
    }
}
