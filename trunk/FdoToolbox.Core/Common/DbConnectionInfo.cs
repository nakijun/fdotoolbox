using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using FdoToolbox.Core.ClientServices;

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

        private MyMeta.dbRoot _Meta;

        public MyMeta.dbRoot MetaData
        {
            get { return _Meta; }
        }
#if TEST
        public DbConnectionInfo() { }
#endif
        public DbConnectionInfo(string name, IDbConnection conn, string driver)
        {
            this.Name = name;
            this.Connection = conn;
            this.Driver = driver;
            _Meta = new MyMeta.dbRoot();
            _Meta.LanguageMappingFileName = AppGateway.RunningApplication.LanguageMappingFile;
            _Meta.DbTargetMappingFileName = AppGateway.RunningApplication.DbTargetsFile;
            _Meta.Language = "C#";
            _Meta.DbTarget = driver.ToUpper();
            _Meta.Connect(this.Driver, this.Connection.ConnectionString);
        }

        public MyMeta.IDatabase GetDatabase(string name)
        {
            foreach (MyMeta.IDatabase db in _Meta.Databases)
            {
                if (db.Name == name)
                    return db;
            }
            return null;
        }

        public MyMeta.ITable GetTable(string dbName, string tableName)
        {
            MyMeta.IDatabase db = GetDatabase(dbName);
            if (db != null)
            {
                foreach (MyMeta.ITable table in db.Tables)
                {
                    if (table.Name == tableName)
                        return table;
                }
            }
            return null;
        }
    }
}
