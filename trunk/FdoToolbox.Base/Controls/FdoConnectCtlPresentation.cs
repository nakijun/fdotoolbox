using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using FdoToolbox.Core.Utility;
using ICSharpCode.Core;
using FdoToolbox.Core.Connections;
using FdoToolbox.Core;
using FdoToolbox.Base.Forms;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Generic Connect View
    /// </summary>
    public interface IFdoConnectView : IViewContent
    {
        string ConnectionName { get; }
        IList<FdoProviderInfo> ProviderList { set; }
        FdoProviderInfo SelectedProvider { get; }
        NameValueCollection ConnectProperties { get; }
        void FlagNameError(string msg);

        void ResetGrid();
        void AddEnumerableProperty(string name, string defaultValue, string[] values);

        void AddProperty(DictionaryProperty p);
    }

    /// <summary>
    /// Generic Connect presenter
    /// </summary>
    public class FdoConnectCtlPresenter
    {
        private readonly IFdoConnectView _view;
        private readonly IFdoConnectionManager _manager;

        public FdoConnectCtlPresenter(IFdoConnectView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _manager = connMgr;
        }

        public void GetProviderList()
        {
            _view.ProviderList = FdoFeatureService.GetProviders();
        }

        public void TestConnection()
        {
            FdoProviderInfo provider = _view.SelectedProvider;
            string connStr = ExpressUtility.ConvertFromNameValueCollection(_view.ConnectProperties);

            FdoConnection conn = new FdoConnection(provider.Name, connStr);
            try
            {
                FdoConnectionState state = conn.Open();
                if (state == FdoConnectionState.Open)
                {
                    MessageService.ShowMessage("Test successful");
                    conn.Close();
                }
                else
                {
                    MessageService.ShowError("Connection test failed");
                }
            }
            catch (FdoException ex)
            {
                MessageService.ShowError(ex.InnerException.Message);
            }
        }

        private List<DictionaryProperty> _pendingProperties = new List<DictionaryProperty>();

        public void ProviderChanged()
        {
            FdoProviderInfo prov = _view.SelectedProvider;
            if (prov != null)
            {
                _view.ResetGrid();
                _pendingProperties.Clear();
                IList<DictionaryProperty> props = FdoFeatureService.GetConnectProperties(prov.Name);
                if (props != null)
                {
                    foreach (DictionaryProperty p in props)
                    {
                        if (p.Enumerable)
                        {
                            EnumerableDictionaryProperty ep = p as EnumerableDictionaryProperty;
                            if (!ep.RequiresConnection)
                                _view.AddEnumerableProperty(ep.Name, ep.DefaultValue, ep.Values);
                            else
                                _pendingProperties.Add(ep);
                        }
                        else
                        {
                            _view.AddProperty(p);
                        }
                    }
                }
            }
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName))
            {
                _view.FlagNameError("Required");
                return false;
            }

            FdoConnection conn = _manager.GetConnection(_view.ConnectionName);
            if (conn != null)
            {
                _view.FlagNameError("A connection named " + _view.ConnectionName + " already exists");
                return false;
            }

            FdoProviderInfo provider = _view.SelectedProvider;
            //string connStr = ExpressUtility.ConvertFromNameValueCollection(_view.ConnectProperties);

            NameValueCollection cp = new NameValueCollection(_view.ConnectProperties);
            if (_pendingProperties.Count > 0)
            {
                NameValueCollection extra = new NameValueCollection();
                cp.Add(extra);
            }
            string connStr = ExpressUtility.ConvertFromNameValueCollection(cp);

            conn = new FdoConnection(provider.Name, connStr);
            FdoConnectionState state = conn.Open();
            if (state == FdoConnectionState.Open)
            {
                _manager.AddConnection(_view.ConnectionName, conn);
                return true;
            }
            else if (state == FdoConnectionState.Pending)
            {
                //Re-query the pending parameters and re-prompt in a new dialog
                if (_pendingProperties.Count > 0)
                {
                    List<DictionaryProperty> pend = new List<DictionaryProperty>();
                    foreach (DictionaryProperty p in _pendingProperties)
                    {
                        pend.Add(conn.GetConnectTimeProperty(p.Name));
                    }
                    NameValueCollection extra = PendingParameterDialog.GetExtraParameters(pend);
                    //Cancelled action
                    if (extra == null)
                        return false;

                    cp.Add(extra);
                    conn.ConnectionString = ExpressUtility.ConvertFromNameValueCollection(cp);
                    if (conn.Open() == FdoConnectionState.Open)
                    {
                        _manager.AddConnection(_view.ConnectionName, conn);
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }
    }
}
