using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.ComponentModel;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoSpatialContextMgrView
    {
        IList<SpatialContextInfo> SpatialContexts { set; get; }

        SpatialContextInfo SelectedSpatialContext { get; }

        string Message { set; }

        bool CreateEnabled { set; }
        bool EditEnabled { set; }
        bool DeleteEnabled { set; }
    }

    public class FdoSpatialContextMgrPresenter
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
            int [] cmds = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
            bool canDelete = (Array.IndexOf<int>(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) >= 0);
            bool canCreate = (Array.IndexOf<int>(cmds, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) >= 0)
                            && _conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts).Value;
            bool canEdit = _view.SpatialContexts.Count > 0;

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

        public void SpatialContextSelected()
        {
            SpatialContextInfo sci = _view.SelectedSpatialContext;
            _view.DeleteEnabled = (sci != null);
            _view.EditEnabled = (sci != null);
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