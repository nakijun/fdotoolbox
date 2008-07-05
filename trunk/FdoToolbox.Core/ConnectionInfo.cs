using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core
{
    public class ConnectionInfo
    {
        public ConnectionInfo(string name, IConnection conn)
        {
            this.Name = name;
            this.Connection = conn;
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private IConnection _Connection;

        public IConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }
    }
}
