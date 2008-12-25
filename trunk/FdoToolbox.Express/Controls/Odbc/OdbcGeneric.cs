using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcGeneric : IOdbcConnectionBuilder
    {
        private string _ConnectionString;

        [Description("The ODBC connection string")]
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
	
        public string ToConnectionString()
        {
            return this.ConnectionString;
        }
    }
}
