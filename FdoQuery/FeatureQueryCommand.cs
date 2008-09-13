using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Commands;
using System.Collections.ObjectModel;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;
using OSGeo.FDO.Schema;
using System.Data;

namespace FdoQuery
{
    public class FeatureQueryCommand : FdoConnectionConsoleCommand
    {
        private ReadOnlyCollection<string> _propertyNames;
        private string _className;
        private string _filter;
        private int _limit;

        public FeatureQueryCommand(string provider, string connStr, string className, string filter, ReadOnlyCollection<string> names, int limit)
            : base(provider, connStr)
        {
            _propertyNames = names;
            _className = className;
            _filter = filter;
            _limit = limit;
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
                    FeatureQueryOptions qry = new FeatureQueryOptions(_className);
                    if (!string.IsNullOrEmpty(_filter))
                        qry.Filter = _filter;
                    if (_propertyNames.Count > 0)
                        qry.AddFeatureProperty(_propertyNames);
                    string nullString = "(NULL)";
                    using (FdoFeatureReader reader = service.SelectFeatures(qry, _limit))
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
