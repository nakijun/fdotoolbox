using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A decorator interface that extends the Object Explorer with custom
    /// root nodes and context menus.
    /// </summary>
    public interface IObjectExplorerDecorator
    {
        void Decorate(IObjectExplorer explorer);
    }
}
