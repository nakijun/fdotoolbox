using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Connections;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoCreateDataStoreView
    {
        IList<FdoProviderInfo> ProviderList { set; }
        FdoProviderInfo SelectedProvider { get; }

        NameValueCollection ConnectProperties { get; }
        NameValueCollection DataStoreProperties { get; }

        void InitializeConnectGrid();
        void InitializeDataStoreGrid();

        bool CreateEnabled { set; }

        void ResetConnectGrid();
        void AddConnectProperty(DictionaryProperty p);
        void AddEnumerableConnectProperty(string name, string defaultValue, string[] values);

        void ResetDataStoreGrid();
        void AddDataStoreProperty(DictionaryProperty p);
        void AddEnumerableDataStoreProperty(string name, string defaultValue, string[] values);
    }

    public class FdoCreateDataStorePresenter
    {
        public readonly IFdoCreateDataStoreView _view;
        private readonly IFdoConnectionManager _manager;

        public FdoCreateDataStorePresenter(IFdoCreateDataStoreView view, IFdoConnectionManager manager)
        {
            _view = view;
            _manager = manager;
        }

        public void LoadProviders()
        {
            _view.InitializeConnectGrid();
            _view.InitializeDataStoreGrid();
            _view.ProviderList = FdoFeatureService.GetProviders();
        }

        public void ProviderChanged()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                _view.ResetDataStoreGrid();

                IList<DictionaryProperty> dprops = FdoFeatureService.GetDataStoreProperties(prov.Name);
                if (dprops != null)
                {
                    foreach (DictionaryProperty p in dprops)
                    {
                        if (p.Enumerable)
                        {
                            EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                            if(!ep.RequiresConnection)
                                _view.AddEnumerableDataStoreProperty(ep.Name, ep.DefaultValue, ep.Values);
                        }
                        else
                        {
                            _view.AddDataStoreProperty(p);
                        }
                    }
                    _view.CreateEnabled = true;
                }
                else
                {
                    MessageService.ShowError("Selected provider does not support creation of data stores");
                    _view.ResetDataStoreGrid();
                    _view.ResetConnectGrid();
                    _view.CreateEnabled = false;
                    return;
                }

                if (!prov.IsFlatFile)
                {
                    _view.ResetConnectGrid();
                    IList<DictionaryProperty> cprops = FdoFeatureService.GetConnectProperties(prov.Name);
                    if (cprops != null)
                    {
                        foreach (DictionaryProperty p in cprops)
                        {
                            if (p.Enumerable)
                            {
                                EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                                if(!ep.RequiresConnection)
                                    _view.AddEnumerableConnectProperty(ep.Name, ep.DefaultValue, ep.Values);
                            }
                            else
                            {
                                _view.AddConnectProperty(p);
                            }
                        }
                    }
                }
            }
        }

        public bool CreateDataStore()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                NameValueCollection dp = _view.DataStoreProperties;
                NameValueCollection cp = _view.ConnectProperties;
                FdoFeatureService.CreateDataStore(prov.Name, dp, cp);
                MessageService.ShowMessage(ResourceService.GetString("MSG_DATA_STORE_CREATED"), ResourceService.GetString("TITLE_CREATE_DATA_STORE"));
                return true;
            }
            return false;
        }
    }
}
