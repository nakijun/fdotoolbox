using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectMySqlView
    {
        string Service { get; }
        string Username { get; }
        string Password { get; }

        bool DataStoreEnabled { set; }
        bool SubmitEnabled { set; }

        string[] DataStores { set; }
        string SelectedDataStore { get; }
        string ConnectionName { get; }

        void AlertError(string msg);
    }

    public class ConnectMySqlPresenter
    {
        private readonly IConnectMySqlView _view;
        private readonly IFdoConnectionManager _connMgr;
        private FdoConnection _conn;

        public ConnectMySqlPresenter(IConnectMySqlView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
            _conn = new FdoConnection("OSGeo.MySQL");
            _view.DataStoreEnabled = false;
            _view.SubmitEnabled = false; 
        }

        private void SetDataStore(string[] values)
        {
            _view.DataStores = values;
            _view.DataStoreEnabled = values.Length > 0;
            _view.SubmitEnabled = values.Length > 0;
        }

        public void PendingConnect()
        {
            try
            {
                if (_conn.State != FdoConnectionState.Closed)
                    _conn.Close();

                _conn.ConnectionString = string.Format("Service={0};Username={1};Password={2}", _view.Service, _view.Username, _view.Password);
                if (_conn.Open() == FdoConnectionState.Pending)
                {
                    List<string> datstores = new List<string>();
                    using (FdoFeatureService service = _conn.CreateFeatureService())
                    {
                        ICollection<DataStoreInfo> dstores = service.ListDataStores(false);
                        foreach (DataStoreInfo info in dstores)
                        {
                            datstores.Add(info.Name);
                        }
                    }
                    SetDataStore(datstores.ToArray());
                }
            }
            catch (Exception ex)
            {
                _view.AlertError(ex.Message);
            }
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(_view.ConnectionName) || _connMgr.NameExists(_view.ConnectionName))
            {
                _view.AlertError(ResourceService.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                return false;
            }

            try
            {
                _conn.ConnectionString = string.Format("Service={0};Username={1};Password={2};DataStore={3}", _view.Service, _view.Username, _view.Password, _view.SelectedDataStore);
                if (_conn.Open() == FdoConnectionState.Open)
                {
                    _connMgr.AddConnection(_view.ConnectionName, _conn);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _view.AlertError(ex.Message);
            }

            return false;
        }
    }
}
