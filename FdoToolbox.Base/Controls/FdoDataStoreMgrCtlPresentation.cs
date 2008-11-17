using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using System.Collections.Specialized;
using FdoToolbox.Base.Forms;
using ICSharpCode.Core;
using System.Collections.ObjectModel;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoDataStoreMgrView
    {
        bool AddEnabled { set; }
        bool DestroyEnabled { set; }
        IList<DataStoreInfo> DataStores { set; }
        string Message { set; }
    }

    public class FdoDataStoreMgrPresenter
    {
        private readonly IFdoDataStoreMgrView _view;
        private FdoConnection _conn;

        public FdoConnection Connection
        {
            get { return _conn; }
        }

        public FdoDataStoreMgrPresenter(IFdoDataStoreMgrView view, FdoConnection conn)
        {
            _view = view;
            _conn = conn;
            _view.Message = ResourceService.GetString("MSG_LISTING_DATA_STORES");
        }

        public void Init()
        {
            GetDataStores();
        }

        private void GetDataStores()
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                ReadOnlyCollection<DataStoreInfo> dstores = service.ListDataStores(true);
                _view.DataStores = dstores;
                _view.DestroyEnabled = (dstores.Count > 0) && canDestroy;
            }
        }

        private bool canDestroy;
        private bool canAdd;

        private void ToggleUI()
        {
            int[] cmds = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
            _view.AddEnabled = canAdd = Array.IndexOf<int>(cmds, (int)CommandType.CommandType_CreateDataStore) >= 0;
            _view.DestroyEnabled = canDestroy = Array.IndexOf<int>(cmds, (int)CommandType.CommandType_DestroyDataStore) >= 0;
        }

        public void DestroyDataStore(NameValueCollection props)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                using (IDestroyDataStore destroy = service.CreateCommand<IDestroyDataStore>(CommandType.CommandType_DestroyDataStore))
                {
                    foreach (string key in props.AllKeys)
                    {
                        destroy.DataStoreProperties.SetProperty(key, props[key]);
                    }
                    destroy.Execute();
                }
            }
        }

        public void CreateDataStore(NameValueCollection props)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                using (ICreateDataStore create = service.CreateCommand<ICreateDataStore>(CommandType.CommandType_CreateDataStore))
                {
                    foreach (string key in props.AllKeys)
                    {
                        create.DataStoreProperties.SetProperty(key, props[key]);
                    }
                    create.Execute();
                }
            }
        }
    }
}
