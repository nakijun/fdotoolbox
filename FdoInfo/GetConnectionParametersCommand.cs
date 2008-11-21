using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class GetConnectionParametersCommand : ConsoleCommand
    {
        private string _provider;

        public GetConnectionParametersCommand(string provider)
        {
            _provider = provider;
        }

        public override int Execute()
        {
            CommandStatus retCode;

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

            IConnectionPropertyDictionary dict = conn.ConnectionInfo.ConnectionProperties;

            Console.WriteLine("Connection properties for: {0}", _provider);
            foreach(string name in dict.PropertyNames)
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
            retCode = CommandStatus.E_OK;
            return (int)retCode;
        }
    }
}
