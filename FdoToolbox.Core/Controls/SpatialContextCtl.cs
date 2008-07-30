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
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Geometry;
using FdoToolbox.Core.Forms;

namespace FdoToolbox.Core.Controls
{
    /// <summary>
    /// A control to manage spatial contexts for a given connection
    /// </summary>
    public partial class SpatialContextCtl : BaseDocumentCtl, IConnectionBoundCtl
    {
        private ConnectionInfo _BoundConnection;
        private FgfGeometryFactory _GeomFactory;
        private BindingSource _bsContexts;

        private bool _CanDelete;
        private bool _CanCreate;
        private bool _CanEdit;

        internal SpatialContextCtl()
        {
            InitializeComponent();
            _GeomFactory = new FgfGeometryFactory();
            _bsContexts = new BindingSource();
            _bsContexts.DataSource = new List<SpatialContextInfo>();
            grdSpatialContexts.DataSource = _bsContexts;
            this.Disposed += delegate { _GeomFactory.Dispose(); };
        }

        private FeatureService _Service;

        public SpatialContextCtl(ConnectionInfo conn)
            : this()
        {
            _BoundConnection = conn;
            _Service = new FeatureService(conn.Connection);
            this.Disposed += delegate { _Service.Dispose(); };
        }

        private void ToggleUI()
        {
            _CanCreate = Array.Exists<int>(this.BoundConnection.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext; })
                        && this.BoundConnection.Connection.ConnectionCapabilities.SupportsMultipleSpatialContexts();
            _CanDelete = Array.Exists<int>(this.BoundConnection.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext; });
            _CanEdit = (_bsContexts.Count > 0);

            btnCreate.Enabled = _CanCreate;
            btnDelete.Enabled = _CanDelete;
            btnEdit.Enabled = _CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadSpatialContexts();
            ToggleUI();
            base.OnLoad(e);
        }

        private void LoadSpatialContexts()
        {
            _bsContexts.Clear();
            List<SpatialContextInfo> context = _Service.GetSpatialContexts();
            context.ForEach(delegate(SpatialContextInfo ctx) { _bsContexts.Add(ctx); });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteSpatialContext(SpatialContextInfo ctx)
        {
            _Service.DestroySpatialContext(ctx);
            AppConsole.Alert("Deleted", "Spatial context deleted");
            _bsContexts.Remove(ctx);
        }

        private SpatialContextInfo GetSpatialContextByName(string name)
        {
            List<SpatialContextInfo> ctx = _bsContexts.DataSource as List<SpatialContextInfo>;
            if (ctx != null)
            {
                return ctx.Find(delegate(SpatialContextInfo sci) { return sci.Name == name; });
            }
            return null;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SpatialContextInfo ctx = SpatialContextInfoDlg.CreateNew(this.BoundConnection);
            if (ctx != null)
            {
                SpatialContextInfo search = GetSpatialContextByName(ctx.Name);
                if (search != null)
                {
                    if (AppConsole.Confirm("Overwrite", "A spatial context with name " + ctx.Name + " already exists. Overwrite?"))
                        CreateSpatialContext(ctx, true);
                    else
                        AppConsole.Alert("Error", "Unable to create spatial context. A spatial context named " + ctx.Name + " already exists");
                }
                else
                {
                    CreateSpatialContext(ctx, false);
                }
            }
        }

        private void CreateSpatialContext(SpatialContextInfo ctx, bool updateExisting)
        {
            FeatureService service = new FeatureService(this.BoundConnection.Connection);
            service.CreateSpatialContext(ctx, updateExisting);

            if (updateExisting)
            {
                AppConsole.Alert("Updated", "Spatial Context updated: " + ctx.Name);
                _bsContexts.DataSource = service.GetSpatialContexts();
                //grdSpatialContexts.DataSource = _bsContexts;
            }
            else
            {
                AppConsole.Alert("Created", "New Spatial Context created: " + ctx.Name);
                _bsContexts.Add(ctx);
            }
            btnDelete.Enabled = _CanDelete && (this.BoundConnection.Connection.ConnectionCapabilities.SupportsMultipleSpatialContexts() || (grdSpatialContexts.Rows.Count == 0));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SpatialContextInfo info = GetSelectedSpatialContext();
            if (info != null)
            {
                if(AppConsole.Confirm("Confirm Delete", "Deleting this spatial context will delete *all* data stored in this context. Are you sure you want to do this?"))
                    DeleteSpatialContext(info);
            }
        }

        private SpatialContextInfo GetSelectedSpatialContext()
        {
            if (grdSpatialContexts.SelectedRows.Count == 1)
                return grdSpatialContexts.SelectedRows[0].DataBoundItem as SpatialContextInfo;
            else if (grdSpatialContexts.SelectedCells.Count == 1)
                return grdSpatialContexts.Rows[grdSpatialContexts.SelectedCells[0].RowIndex].DataBoundItem as SpatialContextInfo;

            return null;
        }

        private void grdSpatialContexts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = _CanDelete;
            btnEdit.Enabled = _CanEdit;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SpatialContextInfo ctx = GetSelectedSpatialContext();
            if (ctx != null)
            {
                ctx = SpatialContextInfoDlg.Edit(this.BoundConnection, ctx);
                if (ctx != null)
                {
                    CreateSpatialContext(ctx, true);
                }
            }
        }

        public ConnectionInfo BoundConnection
        {
            get { return _BoundConnection; }
        }

        public void SetName(string name)
        {
            this.BoundConnection.Name = name;
            this.Title = "Spatial Context Management - " + this.BoundConnection.Name;
        }
    }
}
