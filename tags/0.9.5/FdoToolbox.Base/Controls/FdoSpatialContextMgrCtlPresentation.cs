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
using FdoToolbox.Core.Feature;
using System.ComponentModel;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoSpatialContextMgrView
    {
        IList<SpatialContextInfo> SpatialContexts { set; get; }

        SpatialContextInfo SelectedSpatialContext { get; }

        string Message { set; }

        bool CreateEnabled { set; }
        bool EditEnabled { set; }
        bool DeleteEnabled { set; }
    }

    internal class FdoSpatialContextMgrPresenter
    {
        private readonly IFdoSpatialContextMgrView _view;
        private FdoConnection _conn;

        public FdoConnection Connection
        {
            get { return _conn; }
        }

        public FdoSpatialContextMgrPresenter(IFdoSpatialContextMgrView view, FdoConnection conn)
        {
            _view = view;
            _view.Message = ResourceService.GetString("MSG_LISTING_SPATIAL_CONTEXTS"); ;
            _conn = conn;
        }

        public void Init()
        {
            GetSpatialContexts();
            ToggleUI();
        }

        private void ToggleUI()
        {
            Array cmds = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
            bool canDelete = (Array.IndexOf(cmds, OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) >= 0);
            bool canCreate = (Array.IndexOf(cmds, OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) >= 0);
            bool canEdit = _view.SpatialContexts.Count > 0 && (Array.IndexOf(cmds, OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) >= 0);

            if (canCreate)
            {
                //One spatial context exists, and this doesn't support multiple spatial contexts
                if (_view.SpatialContexts.Count > 0 && !_conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts))
                    canCreate = false;
            }

            _view.CreateEnabled = canCreate;
            _view.DeleteEnabled = canDelete;
            _view.EditEnabled = canEdit;
        }

        public void GetSpatialContexts()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                _view.SpatialContexts = new BindingList<SpatialContextInfo>(service.GetSpatialContexts());
            }
        }

        public void AddSpatialContext(SpatialContextInfo sci)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                service.CreateSpatialContext(sci, false);
            }
        }

        public void UpdateSpatialContext(SpatialContextInfo sci)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                service.CreateSpatialContext(sci, true);
            }
        }

        public void DeleteSpatialContext(SpatialContextInfo sci)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                service.DestroySpatialContext(sci);
            }
        }
    }
}
