using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoSqlQueryView : IQuerySubView
    {
        string SQLString { get; }
    }
}
