using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;

namespace FdoInfo
{
    public class ListProvidersCommand : ConsoleCommand
    {
        public override int Execute()
        {
            ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders();
            using (providers)
            {
                foreach (Provider provider in providers)
                {
                    Console.WriteLine("\nProvider Name: {0}\n", provider.Name);
                    Console.WriteLine("\tDisplay Name: {0}\n\tDescription: {1}\n\tLibrary Path: {2}\n\tVersion: {3}\n\tFDO Version: {4}\n\tIs Managed: {5}",
                        provider.DisplayName,
                        provider.Description,
                        provider.LibraryPath,
                        provider.Version,
                        provider.FeatureDataObjectsVersion,
                        provider.IsManaged);
                }
            }
            return (int)CommandStatus.E_OK;
        }
    }
}
