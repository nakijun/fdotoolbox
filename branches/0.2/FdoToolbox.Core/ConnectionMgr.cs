using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core
{
    /// <summary>
    /// FDO Connection Manager
    /// </summary>
    public class ConnectionMgr : IConnectionMgr, IDisposable
    {
        private int counter = 0;

        private Dictionary<string, IConnection> _ConnectionDict;

        public ConnectionMgr() { _ConnectionDict = new Dictionary<string, IConnection>(); }

        public void AddConnection(string name, OSGeo.FDO.Connections.IConnection conn)
        {
            if (_ConnectionDict.ContainsKey(name))
                throw new FdoConnectionException("Unable to add connection named " + name + " to the connection manager");
            if (conn.ConnectionState != ConnectionState.ConnectionState_Open)
                conn.Open();
            _ConnectionDict.Add(name, conn);
            AppConsole.WriteLine("New connection added: {0}", name);
            if (this.ConnectionAdded != null)
                this.ConnectionAdded(name);
        }

        public void RemoveConnection(string name)
        {
            if (_ConnectionDict.ContainsKey(name))
            {
                IConnection conn = _ConnectionDict[name];
                if (conn.ConnectionState == ConnectionState.ConnectionState_Open)
                    conn.Close();
                
                _ConnectionDict.Remove(name);
                conn.Dispose();
                AppConsole.WriteLine("Connection removed: {0}", name);
                if (this.ConnectionRemoved != null)
                    this.ConnectionRemoved(name);
            }
        }

        public IConnection GetConnection(string name)
        {
            if (_ConnectionDict.ContainsKey(name))
                return _ConnectionDict[name];
            return null;
        }
        
        public ICollection<string> GetConnectionNames()
        {
            return _ConnectionDict.Keys;
        }

        public string CreateUniqueName()
        {
            return "Connection" + (counter++);
        }
        
        public event ConnectionEventHandler ConnectionAdded;

        public event ConnectionEventHandler ConnectionRemoved;

        public void Dispose()
        {
            foreach (string name in GetConnectionNames())
            {
                _ConnectionDict[name].Close();
                _ConnectionDict[name].Dispose();
            }
            _ConnectionDict.Clear();
        }
    }
}
