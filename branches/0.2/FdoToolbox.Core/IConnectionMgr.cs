using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core
{
    public delegate void ConnectionEventHandler(string name);

    public interface IConnectionMgr
    {
        string CreateUniqueName();
        void AddConnection(string name, IConnection conn);
        void RemoveConnection(string name);
        IConnection GetConnection(string name);
        ICollection<string> GetConnectionNames();

        event ConnectionEventHandler ConnectionAdded;
        event ConnectionEventHandler ConnectionRemoved;
    }
}
