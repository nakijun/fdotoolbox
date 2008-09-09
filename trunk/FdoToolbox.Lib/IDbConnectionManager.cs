using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Lib
{
    public interface IDbConnectionManager : IDisposable
    {
        string CreateUniqueName();
        void AddConnection(DbConnectionInfo conn);
        void RemoveConnection(string name);
        DbConnectionInfo GetConnection(string name);
        ICollection<string> GetConnectionNames();
        void RenameConnection(string oldName, string newName);
        bool CanRenameConnection(string oldName, string newName, ref string reason);

        event ConnectionBeforeRemoveHandler BeforeConnectionRemove;
        event ConnectionEventHandler ConnectionAdded;
        event ConnectionEventHandler ConnectionRemoved;
        event ConnectionRenamedEventHandler ConnectionRenamed;
    }
}
