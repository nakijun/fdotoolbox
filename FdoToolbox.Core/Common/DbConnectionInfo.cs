using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace FdoToolbox.Core.Common
{
    public class DbConnectionInfo
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private IDbConnection _Connection;

        public IDbConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        private string _Driver;

        public string Driver
        {
            get { return _Driver; }
            set { _Driver = value; }
        }
	

        public DbConnectionInfo(string name, IDbConnection conn, string driver)
        {
            this.Name = name;
            this.Connection = conn;
            this.Driver = driver;
        }
    }
}
