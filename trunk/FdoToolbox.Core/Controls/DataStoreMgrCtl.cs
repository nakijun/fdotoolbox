#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Commands.DataStore;
using System.Collections.Specialized;
using FdoToolbox.Core.Forms;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Common;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.Controls
{
    public partial class DataStoreMgrCtl : SpatialConnectionBoundControl
    {
        private FeatureService _Service;

        internal DataStoreMgrCtl()
        {
            InitializeComponent();
        }

        public DataStoreMgrCtl(FdoConnectionInfo connInfo, string key)
            : base(connInfo, key)
        {
            InitializeComponent();
            _BoundConnection = connInfo;
            _Service = new FeatureService(connInfo.InternalConnection);
            ToggleUI();
            this.Disposed += delegate { _Service.Dispose(); };
        }

        private void ToggleUI()
        {
            btnAdd.Enabled = (Array.IndexOf<int>(this.BoundConnection.InternalConnection.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) >= 0);
            btnDestroy.Enabled = (Array.IndexOf<int>(this.BoundConnection.InternalConnection.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore) >= 0);
        }

        protected override void OnLoad(EventArgs e)
        {
            ListDataStores();
            base.OnLoad(e);
        }

        private void ListDataStores()
        {
            FeatureService service = new FeatureService(this.BoundConnection.InternalConnection);
            ReadOnlyCollection<DataStoreInfo> stores = service.ListDataStores(true);
            grdDataStores.DataSource = stores;
        }


        public override void SetName(string name)
        {
            this.Title = "Data Store Management - " + name;
        }

        public DataStoreInfo SelectedDataStore
        {
            get
            {
                if (grdDataStores.SelectedRows.Count == 1)
                    return grdDataStores.SelectedRows[0].DataBoundItem as DataStoreInfo;
                if (grdDataStores.SelectedCells.Count == 1)
                    return grdDataStores.Rows[grdDataStores.SelectedCells[0].RowIndex].DataBoundItem as DataStoreInfo;

                return null;
            }
        }

        private void grdDataStores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDestroy.Enabled = (grdDataStores.Rows.Count > 0);
        }

        private void btnDestroy_Click(object sender, EventArgs e)
        {
            if (AppConsole.Confirm("Destroy Datastore", "Are you sure you want to destroy a datastore?\nYou will lose all data inside the datastore"))
            {
                using (IDestroyDataStore destroy = this.BoundConnection.InternalConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
                {
                    NameValueCollection props = DictionaryDialog.GetParameters("Destroy parameters", destroy.DataStoreProperties);
                    if (props != null)
                    {
                        foreach (string key in props.AllKeys)
                        {
                            destroy.DataStoreProperties.SetProperty(key, props[key]);
                        }
                    }
                    destroy.Execute();
                    AppConsole.Alert("Destroy datastore", "Datastore destroyed");
                    ListDataStores();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (ICreateDataStore create = this.BoundConnection.InternalConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
            {
                NameValueCollection props = DictionaryDialog.GetParameters("Create parameters", create.DataStoreProperties);
                if (props != null)
                {
                    foreach (string key in props.AllKeys)
                    {
                        create.DataStoreProperties.SetProperty(key, props[key]);
                    }
                }
                create.Execute();
                AppConsole.Alert("Create datastore", "Datastore created");
                ListDataStores();
            }
        }

        public override string GetTabType()
        {
            return CoreModule.TAB_DATASTORE_MGMT;
        }
    }
}
