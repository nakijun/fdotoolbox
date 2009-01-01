using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL;
using FdoToolbox.Base;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Tasks.Services
{
    public class TaskLoader : BaseDefinitionLoader
    {
        protected override FdoConnection CreateConnection(string provider, string connStr)
        {
            //Try to find matching open connection first
            IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();

            FdoConnection conn = connMgr.GetConnection(provider, connStr);
            if (conn == null)
                conn = new FdoConnection(provider, connStr);

            return conn;
        }
    }
}
