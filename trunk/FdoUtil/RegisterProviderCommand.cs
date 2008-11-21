using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.AppFramework;

namespace FdoUtil
{
    public class RegisterProviderCommand : ConsoleCommand
    {
        private string _name;
        private string _displayName;
        private string _description;
        private string _libraryPath;
        private string _version;
        private string _fdoVersion;
        private bool _isManaged;

        public RegisterProviderCommand(string name, string displayName, string description, string libraryPath, string version, string fdoVersion, bool isManaged)
        {
            _name = name;
            _displayName = displayName;
            _description = description;
            _libraryPath = libraryPath;
            _version = version;
            _fdoVersion = fdoVersion;
            _isManaged = isManaged;
        }

        public override int Execute()
        {
            FeatureAccessManager.GetProviderRegistry().RegisterProvider(
                _name,
                _displayName,
                _description,
                _version,
                _fdoVersion,
                _libraryPath,
                _isManaged);
            WriteLine("New provider registered: {0}", _name);
            return (int)CommandStatus.E_OK;
        }
    }
}
