using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core
{
    public delegate void ConnectionEventHandler(string name);
    public delegate void ConnectionRenamedEventHandler(string oldName, string newName);

    public interface IConnectionMgr
    {
        string CreateUniqueName();
        void AddConnection(string name, IConnection conn);
        void RemoveConnection(string name);
        IConnection GetConnection(string name);
        ICollection<string> GetConnectionNames();
        void RenameConnection(string oldName, string newName);
        bool CanRenameConnection(string oldName, string newName, ref string reason);

        event ConnectionEventHandler ConnectionAdded;
        event ConnectionEventHandler ConnectionRemoved;
        event ConnectionRenamedEventHandler ConnectionRenamed;
    }
}
