using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;
using FdoToolbox.Core.AppFramework;

namespace FdoInfo
{
    public class GetCreateDataStoreParametersCommand : ConsoleCommand
    {
        private string _provider;

        public GetCreateDataStoreParametersCommand(string provider)
        {
            _provider = provider;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = FeatureAccessManager.GetConnectionManager().CreateConnection(_provider);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CREATE_CONNECTION;
            }

            if(Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) < 0)
            {
                Console.Error.WriteLine("This provider does not support creating data stores");
                return (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
            }

            using (ICreateDataStore create = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
            {
                IDataStorePropertyDictionary dict = create.DataStoreProperties;
                Console.WriteLine("Data Store Properties:");
                WriteProperties(dict);
            }

            if (conn.ConnectionState != ConnectionState.ConnectionState_Closed)
                conn.Close();
            return (int)CommandStatus.E_OK;
        }

        private void WriteProperties(IDataStorePropertyDictionary dict)
        {
            foreach (string name in dict.PropertyNames)
            {
                Console.WriteLine("\nProperty Name: {0}\n\n\tLocalized Name: {1}", name, dict.GetLocalizedName(name));
                Console.WriteLine("\tRequired: {0}\n\tProtected: {1}\n\tEnumerable: {2}",
                    dict.IsPropertyRequired(name),
                    dict.IsPropertyProtected(name),
                    dict.IsPropertyEnumerable(name));
                if (dict.IsPropertyEnumerable(name))
                {
                    Console.WriteLine("\tValues for property:");
                    try
                    {
                        string[] values = dict.EnumeratePropertyValues(name);
                        foreach (string str in values)
                        {
                            Console.WriteLine("\t\t- {0}", str);
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception)
                    {
                        Console.Error.WriteLine("\t\tProperty values not available");
                    }
                }
            }
        }
    }
}
