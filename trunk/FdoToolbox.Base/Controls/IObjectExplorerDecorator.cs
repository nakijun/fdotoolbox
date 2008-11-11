using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    public interface IObjectExplorerDecorator
    {
        void Decorate(IObjectExplorer explorer);
    }
}
