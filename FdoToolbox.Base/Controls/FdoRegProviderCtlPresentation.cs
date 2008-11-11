using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoRegProviderView
    {
        string ProviderName { get; }
        string DisplayName { get; }
        string Description { get; }
        string Version { get; }
        string FdoVersion { get; }
        string LibraryPath { get; }
        bool IsManaged { get; }
    }

    public class FdoRegProviderPresentation
    {
        private readonly IFdoRegProviderView _view;

        public FdoRegProviderPresentation(IFdoRegProviderView view)
        {
            _view = view;
        }

        public bool Register()
        {
            FdoFeatureService.RegisterProvider(
                _view.ProviderName,
                _view.DisplayName,
                _view.Description,
                _view.Version,
                _view.FdoVersion,
                _view.LibraryPath,
                _view.IsManaged);
            return true;
        }
    }
}
