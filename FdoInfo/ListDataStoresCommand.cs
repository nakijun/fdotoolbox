using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands;
using System.Collections.ObjectModel;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class ListDataStoresCommand : ConsoleCommand
    {
        private bool _FdoOnly;
        private string _provider;
        private string _connstr;

        public ListDataStoresCommand(string provider, string connStr, bool fdoOnly)
        {
            _FdoOnly = fdoOnly;
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_ListDataStores) < 0)
                return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                ReadOnlyCollection<DataStoreInfo> datastores = service.ListDataStores(_FdoOnly);
                Console.WriteLine("Listing datastores:\n");
                foreach (DataStoreInfo dstore in datastores)
                {
                    Console.WriteLine("\n\tName:{0}\n\tDescription:{1}", dstore.Name, dstore.Description);
                }
            }

            conn.Close();
            return (int)CommandStatus.E_OK;
        }
    }
}
