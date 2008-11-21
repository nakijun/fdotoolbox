using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;

namespace FdoUtil
{
    public class UnregisterProviderCommand : ConsoleCommand
    {
        private string _name;

        public UnregisterProviderCommand(string name)
        {
            _name = name;
        }

        public override int Execute()
        {
            FeatureAccessManager.GetProviderRegistry().UnregisterProvider(_name);
            WriteLine("Provider un-registered: {0}", _name);
            return (int)CommandStatus.E_OK;
        }
    }
}
