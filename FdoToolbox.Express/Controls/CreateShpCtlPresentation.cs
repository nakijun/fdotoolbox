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
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using System.IO;
using ICSharpCode.Core;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateShpView : IViewContent
    {
        string ShpDirectory { get; }
        string FeatureSchemaDefinition { get; }
        bool CreateConnection { get; }
        string ConnectionName { get; set; }
        bool ConnectionEnabled { set; }
        bool FixIncompatibilities { get; }
    }

    public class CreateShpPresenter
    {
        private readonly ICreateShpView _view;
        private FdoConnectionManager _connMgr;

        public CreateShpPresenter(ICreateShpView view)
        {
            _view = view;
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            CheckConnect();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        public bool CreateShp()
        {
            //Creating SHP files is as follows
            //
            // 1. Connect to the *parent* directory of the shape file we want to create
            // 2. Apply the schema to this connection

            if (FileService.FileExists(_view.FeatureSchemaDefinition))
            {
                try
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", _view.ShpDirectory);
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_view.FeatureSchemaDefinition, _view.FixIncompatibilities);
                    }
                    conn.Dispose();
                    if (_view.CreateConnection)
                    {
                        conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", _view.ShpDirectory);
                        conn.Open();
                        _connMgr.AddConnection(_view.ConnectionName, conn);
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    LoggingService.Error("Failed to create SHP", ex);
                    return false;
                }
            }
            return true;
        }

        public void CheckConnect()
        {
            if (!_view.CreateConnection)
                _view.ConnectionName = "";

            _view.ConnectionEnabled = _view.CreateConnection;
        }
    }
}
