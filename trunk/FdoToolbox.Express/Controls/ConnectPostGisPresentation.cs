using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    public interface IConnectPostGisView
    {
        string Service { get; }
        string Username { get; }
        string Password { get; }

        string DataStore { get; }
        string ConnectionName { get; }

        void AlertError(string msg);
    }

    public class ConnectPostGisPresenter
    {
        private readonly IConnectPostGisView _view;
        private readonly IFdoConnectionManager _connMgr;
        private FdoConnection _conn;

        public ConnectPostGisPresenter(IConnectPostGisView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
            _conn = new FdoConnection("OSGeo.PostGIS");
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
                _conn.ConnectionString = string.Format("Service={0};Username={1};Password={2};DataStore={3}", _view.Service, _view.Username, _view.Password, _view.DataStore);
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
