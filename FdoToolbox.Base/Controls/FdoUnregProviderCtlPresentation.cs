using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoUnregProviderView
    {
        IList<FdoProviderInfo> ProviderList { set; }
        IList<string> SelectedProviders { get; }
        bool UnregEnabled { set; }
    }

    public class FdoUnregProviderPresenter
    {
        private readonly IFdoUnregProviderView _view;

        public FdoUnregProviderPresenter(IFdoUnregProviderView view)
        {
            _view = view;
            _view.UnregEnabled = false;
        }

        public void GetProviders()
        {
            _view.ProviderList = FdoFeatureService.GetProviders();
        }

        public void SelectionChanged()
        {
            _view.UnregEnabled = (_view.SelectedProviders.Count > 0);
        }

        public bool Unregister()
        {
            foreach (string prov in _view.SelectedProviders)
            {
                FdoFeatureService.UnregisterProvider(prov);
            }
            return true;
        }
    }
}
