using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    public interface IConsole
    {
        string TextContent { get; }
        void Clear();
    }
}