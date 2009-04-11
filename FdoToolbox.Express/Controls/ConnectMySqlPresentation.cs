#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
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
