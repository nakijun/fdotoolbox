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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view to allow for the management of Spatial Contexts
    /// </summary>
    public partial class FdoSpatialContextMgrCtl : ViewContent, IFdoSpatialContextMgrView, IViewContent, IConnectionDependentView
    {
        private FdoSpatialContextMgrPresenter _presenter;

        internal FdoSpatialContextMgrCtl()
        {
            InitializeComponent();
        }

        public FdoSpatialContextMgrCtl(FdoConnection conn)
            : this()
        {
            _presenter = new FdoSpatialContextMgrPresenter(this, conn);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public IList<SpatialContextInfo> SpatialContexts
        {
            set { grdContexts.DataSource = value; }
            get { return (IList<SpatialContextInfo>)grdContexts.DataSource; }
        }

        public SpatialContextInfo SelectedSpatialContext
        {
            get 
            {
                SpatialContextInfo sci = null;
                if (grdContexts.SelectedRows.Count == 1)
                    sci = grdContexts.SelectedRows[0].DataBoundItem as SpatialContextInfo;
                else if (grdContexts.SelectedCells.Count == 1)
                    sci = grdContexts.Rows[grdContexts.SelectedCells[0].RowIndex].DataBoundItem as SpatialContextInfo;
                return sci;
            }
        }

        public string Message
        {
            set { lblMessage.Text = value; }
        }

        public bool EditEnabled
        {
            set { btnEdit.Enabled = value; }
        }

        public bool DeleteEnabled
        {
            set { btnDelete.Enabled = value; }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SpatialContextInfo sci = FdoSpatialContextDialog.CreateNew(_presenter.Connection);
            if (sci != null)
            {
                _presenter.AddSpatialContext(sci);
                MessageService.ShowMessage(ResourceService.GetString("MSG_SPATIAL_CONTEXT_CREATED"));
                _presenter.GetSpatialContexts();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SpatialContextInfo sci = this.SelectedSpatialContext;
            if (sci != null)
            {
                SpatialContextInfo sci2 = FdoSpatialContextDialog.Edit(_presenter.Connection, sci);
                if (sci2 != null)
                {
                    _presenter.UpdateSpatialContext(sci2);
                    MessageService.ShowMessage(ResourceService.GetString("MSG_SPATIAL_CONTEXT_UPDATED"));
                    _presenter.GetSpatialContexts();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SpatialContextInfo sci = this.SelectedSpatialContext;
            if (sci != null)
            {
                if (MessageService.AskQuestion(
                    ResourceService.GetString("MSG_CONFIRM_DELETE_SPATIAL_CONTEXT"),
                    ResourceService.GetString("TITLE_CONFIRM_DELETE_SPATIAL_CONTEXT")))
                {
                    _presenter.DeleteSpatialContext(sci);
                    _presenter.GetSpatialContexts();
                }
            }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_SPATIAL_CONTEXT_MGMT"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        public bool CreateEnabled
        {
            set { btnCreate.Enabled = value; }
        }

        public bool DependsOnConnection(FdoConnection conn)
        {
            return _presenter.Connection == conn;
        }
    }
}
