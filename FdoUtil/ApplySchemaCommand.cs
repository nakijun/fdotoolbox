using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using FdoToolbox.Core;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class ApplySchemaCommand : ConsoleCommand
    {
        private string _schemaFile;
        private string _provider;
        private string _connstr;

        public ApplySchemaCommand(string provider, string connStr, string schemaFile)
        {
            _schemaFile = schemaFile;
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            CommandStatus retCode;

            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_CONNECT;
                return (int)retCode;
            }

            using (conn)
            {
                FdoFeatureService service = new FdoFeatureService(conn);
                using (service)
                {
                    try
                    {
                        service.LoadSchemasFromXml(_schemaFile);
                        WriteLine("Schema(s) applied");
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                        return (int)retCode;
                    }
                }
            }
            return (int)retCode;
        }
    }
}
