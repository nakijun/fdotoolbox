using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Express.Controls.Odbc
{
    public interface IOdbcConnectionBuilder
    {
        string ToConnectionString();
    }
}
