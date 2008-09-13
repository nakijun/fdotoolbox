using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Commands;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Commands.SQL;
using FdoToolbox.Core.Common;
using System.Data;

namespace FdoQuery
{
    public class SqlQueryCommand : FdoConnectionConsoleCommand
    {
        private string _sql;

        public SqlQueryCommand(string provider, string connStr, string sql)
            : base(provider, connStr)
        {
            _sql = sql;
        }

        public override int Execute()
        {
            CommandStatus stat = CommandStatus.E_OK;
            IConnection conn = CreateConnection();
            conn.Open();
            using (conn)
            {
                using (FeatureService service = new FeatureService(conn))
                {
                    if (!service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
                    {
                        WriteError("This connection does not support SQL queries. Please use the standard feature query");
                        return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                    }
                    string nullString = "(NULL)";
                    using (FdoSqlReader reader = service.ExecuteSQLQuery(_sql))
                    {
                        using (DataTable table = new DataTable())
                        {
                            try
                            {
                                table.Load(reader);
                            }
                            catch (Exception ex)
                            {
                                WriteException(ex);
                                return (int)CommandStatus.E_FAIL_LOAD_QUERY_RESULTS;
                            }

                            AppConsole.WriteLine("");
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                DataColumn col = table.Columns[i];
                                int len = col.MaxLength;
                                AppConsole.Write("{0} ", col.ColumnName);
                                if (i == table.Columns.Count - 1)
                                    AppConsole.WriteLine("");
                                else
                                    AppConsole.Write("| ");
                            }

                            int headerLength = 0;
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                DataColumn col = table.Columns[i];
                                headerLength += col.ColumnName.Length;
                            }
                            AppConsole.WriteLine("{0}", RepeatString("-", headerLength));
                            foreach (DataRow row in table.Rows)
                            {
                                for (int i = 0; i < table.Columns.Count; i++)
                                {
                                    DataColumn col = table.Columns[i];
                                    string str = null;
                                    if (row[col] != null || row[col] != DBNull.Value)
                                    {
                                        str = row[col].ToString();
                                    }
                                    else
                                    {
                                        str = nullString;
                                    }
                                    AppConsole.Write("{0} ", str);
                                    if (i == table.Columns.Count - 1)
                                        AppConsole.WriteLine("");
                                    else
                                        AppConsole.Write("| ");
                                }
                            }
                            AppConsole.WriteLine("\n{0} rows returned", table.Rows.Count);
                        }
                    }
                }
            }

            return (int)stat;
        }
    }
}
