using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using FdoToolbox.Core.ClientServices;
using System.Data.Common;
using System.Data.OleDb;

namespace FdoToolbox.Core.Common
{
    public class DatabaseConnection
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private OleDbConnection _Connection;

        public OleDbConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

#if TEST
        public DatabaseConnection() { }
#endif
        public DatabaseConnection(string name, OleDbConnection conn)
        {
            this.Name = name;
            this.Connection = conn;

            DiscoverSchema();
        }

        public void Refresh()
        {
            DiscoverSchema();
        }

        private void DiscoverSchema()
        {
            OleDbConnection conn = this.Connection;
            if (conn.State != ConnectionState.Open)
                conn.Open();

            DatabaseInfo db = new DatabaseInfo(string.IsNullOrEmpty(conn.Database) ? conn.DataSource : conn.Database);
            using (DataTable tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new string [] { null, null, null, "TABLE"}))
            {
                foreach (DataRow tableRow in tables.Rows)
                {
                    string tableName = tableRow["TABLE_NAME"].ToString();
                    TableInfo tbl = new TableInfo(tableName);

                    List<string> pkNames = new List<string>();
                    using (DataTable pks = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new string[] { null, null, tableName }))
                    {
                        foreach (DataRow pkRow in pks.Rows)
                        {
                            string colName = pkRow["COLUMN_NAME"].ToString();
                            ColumnInfo col = new ColumnInfo(colName);
                            col.IsPrimaryKey = true;
                            tbl.AddColumn(col);
                            pkNames.Add(col.Name);
                        }
                    }

                    using (DataTable columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new string[] { null, null, tableName, null }))
                    {
                        foreach (DataRow colRow in columns.Rows)
                        {
                            string colName = colRow["COLUMN_NAME"].ToString();
                            if (!pkNames.Contains(colName))
                            {
                                ColumnInfo col = new ColumnInfo(colName);
                                tbl.AddColumn(col);
                            }
                        }
                    }

                    db.AddTable(tbl);
                }
            }
            _Database = db;
        }

        private DatabaseInfo _Database;

        public DatabaseInfo Database
        {
            get { return _Database; }
        }

        public TableInfo GetTable(string tableName)
        {
            foreach (TableInfo tbl in this.Database.Tables)
            {
                if (tbl.Name == tableName)
                    return tbl;
            }
            return null;
        }
    }

    public class DatabaseInfo
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
        }

        private List<TableInfo> _Tables;

        public IEnumerable<TableInfo> Tables
        {
            get { return _Tables; }
        }

        public DatabaseInfo(string name)
        {
            _Tables = new List<TableInfo>();
            _Name = name;
        }

        public void AddTable(TableInfo table)
        {
            _Tables.Add(table);
        }
    }

    public class TableInfo
    {
        private List<ColumnInfo> _Columns;

        public IEnumerable<ColumnInfo> Columns
        {
            get { return _Columns; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
	
        private bool _IsView;

        public bool IsView
        {
            get { return _IsView; }
            set { _IsView = value; }
        }
	
        public TableInfo(string name)
        {
            _Name = name;
            _Columns = new List<ColumnInfo>();
        }

        public void AddColumn(ColumnInfo column)
        {
            _Columns.Add(column);
        }
    }

    public class ColumnInfo
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
        }

        private bool _IsPrimaryKey;

        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }
	
        public ColumnInfo(string name)
        {
            _Name = name;
        }
    }
}
