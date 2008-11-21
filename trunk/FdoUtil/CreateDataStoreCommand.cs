using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class CreateDataStoreCommand : ConsoleCommand
    {
        private string _dstoreStr;
        private string _connStr;
        private string _provider;

        public CreateDataStoreCommand(string provider, string connStr, string dstoreStr)
        {
            _connStr = connStr;
            _provider = provider;
            _dstoreStr = dstoreStr;
        }

        private IConnection CreateConnection()
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(_provider);
            return conn;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connStr);
                if (!string.IsNullOrEmpty(_connStr))
                {
                    conn.ConnectionString = _connStr;
                    conn.Open();
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_CONNECT;
                return (int)retCode;
            }

            using (conn)
            {
                using (FdoFeatureService service = new FdoFeatureService(conn))
                {
                    try
                    {
                        service.CreateDataStore(_dstoreStr);
                        WriteLine("Data Store Created!");
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                        return (int)retCode;
                    }
                }
                if (conn.ConnectionState != ConnectionState.ConnectionState_Closed)
                    conn.Close();
            }
            return (int)retCode;
        }
    }
}
