using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core.Controls
{
    public interface IConnectionBoundCtl
    {
        IConnection BoundConnection { get; }
    }
}
