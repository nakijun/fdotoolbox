using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FdoToolbox.Express.Controls.Odbc
{
    public class OdbcSqlServer : IOdbcConnectionBuilder
    {
        private string _Server;

        [DefaultValue("(local)")]
        [Description("The named instance of the SQL server")]
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        private string _Database;

        [Description("The name of the SQL server database to connect to")]
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        private string _UserId;

        [Description("The user id to connect as")]
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private string _Password;

        [Description("The password for the user id")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private bool _TrustedConnection;

        [DefaultValue(false)]
        [Description("Indicates if this is a trusted connection. The user id and password properties are ignored if this is true")]
        public bool TrustedConnection
        {
            get { return _TrustedConnection; }
            set { _TrustedConnection = value; }
        }

        public string ToConnectionString()
        {
            if (this.TrustedConnection)
                return string.Format("Driver={{SQL Server}};Server={0};Database={1};Trusted_Connection=Yes", this.Server, this.Database);
            else
                return string.Format("Driver={{SQL Server}};Server={0};Database={1};Uid={2};Pwd={3}", this.Server, this.Database, this.UserId, this.Password);
        }
    }
}
