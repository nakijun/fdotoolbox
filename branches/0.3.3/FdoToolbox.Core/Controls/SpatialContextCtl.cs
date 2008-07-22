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
        private IConnection _BoundConnection;
        private FgfGeometryFactory _GeomFactory;
        private BindingSource _bsContexts;

        private bool _CanDelete;
        private bool _CanCreate;
        private bool _CanEdit;

        internal SpatialContextCtl()
        {
            InitializeComponent();
            this.Title = "Spatial Context Management";
            _GeomFactory = new FgfGeometryFactory();
            _bsContexts = new BindingSource();
            _bsContexts.DataSource = new List<SpatialContextInfo>();
            grdSpatialContexts.DataSource = _bsContexts;
            this.Disposed += delegate { _GeomFactory.Dispose(); };
        }

        public SpatialContextCtl(IConnection conn)
            : this()
        {
            _BoundConnection = conn;
        }

        private void ToggleUI()
        {
            _CanCreate = Array.Exists<int>(this.BoundConnection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext; });
            _CanDelete = Array.Exists<int>(this.BoundConnection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext; });
            _CanEdit = _CanCreate;

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
            using (IGetSpatialContexts cmd = this.BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                using (ISpatialContextReader reader = cmd.Execute())
                {
                    while (reader.ReadNext())
                    {
                        SpatialContextInfo info = GetSpatialContextFromReader(reader);
                        _bsContexts.Add(info);
                    }
                }
            }
        }

        private SpatialContextInfo GetSpatialContextFromReader(ISpatialContextReader reader)
        {
            SpatialContextInfo info = new SpatialContextInfo();
            info.Name = reader.GetName();
            info.Description = reader.GetDescription();
            info.CoordinateSystem = reader.GetCoordinateSystem();
            info.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
            info.ExtentType = reader.GetExtentType();
            info.XYTolerance = reader.GetXYTolerance();
            info.ZTolerance = reader.GetZTolerance();
            try
            {
                byte[] bGeom = reader.GetExtent();
                if (bGeom != null)
                {
                    using (IGeometry geom = _GeomFactory.CreateGeometryFromFgf(bGeom))
                    {
                        info.ExtentGeometryText = geom.Text;
                    }
                }
            }
            catch
            {
                info.ExtentGeometryText = null;
            }
            info.IsActive = reader.IsActive();
            return info;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteSpatialContext(SpatialContextInfo ctx)
        {
            using (IDestroySpatialContext destory = this.BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
            {
                destory.Name = ctx.Name;
                destory.Execute();
                AppConsole.Alert("Deleted", "Spatial context deleted");
                _bsContexts.Remove(ctx);
            }
        }

        private SpatialContextInfo GetSpatialContextByName(string name)
        {
            List<SpatialContextInfo> ctx = grdSpatialContexts.DataSource as List<SpatialContextInfo>;
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
                    CreateSpatialContext(ctx);
                }
            }
        }

        private void CreateSpatialContext(SpatialContextInfo ctx)
        {
            CreateSpatialContext(ctx, true);
        }

        private void CreateSpatialContext(SpatialContextInfo ctx, bool updateExisting)
        {
            using (FgfGeometryFactory factory = new FgfGeometryFactory())
            using (ICreateSpatialContext create = this.BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
            {
                create.CoordinateSystem = ctx.CoordinateSystem;
                create.CoordinateSystemWkt = ctx.CoordinateSystemWkt;
                create.Description = ctx.Description;
                IGeometry geom = null;
                create.ExtentType = ctx.ExtentType;
                if (ctx.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
                {
                    geom = factory.CreateGeometry(ctx.ExtentGeometryText);
                    create.Extent = factory.GetFgf(geom);
                }
                create.Name = ctx.Name;
                create.UpdateExisting = updateExisting;
                create.XYTolerance = ctx.XYTolerance;
                create.ZTolerance = ctx.ZTolerance;
                create.Execute();

                if (updateExisting)
                {
                    AppConsole.Alert("Updated", "Spatial Context updated: " + ctx.Name);
                }
                else
                {
                    AppConsole.Alert("Created", "New Spatial Context created: " + ctx.Name);
                    _bsContexts.Add(ctx);
                }
                btnDelete.Enabled = _CanDelete && (this.BoundConnection.ConnectionCapabilities.SupportsMultipleSpatialContexts() || (grdSpatialContexts.Rows.Count == 0));
                if (geom != null)
                    geom.Dispose();
            }
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

        public IConnection BoundConnection
        {
            get { return _BoundConnection; }
        }
    }
}
