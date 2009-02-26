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
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A view to maintain a list of coordinate system definitions
    /// </summary>
    public partial class CoordSysCatalog : ViewContent, IViewContent, ICoordSysCatalogView
    {
        private CoordSysCatalogPresenter _presenter;

        public CoordSysCatalog()
        {
            InitializeComponent();
            _presenter = new CoordSysCatalogPresenter(this, ServiceManager.Instance.GetService<FdoToolbox.Base.Services.CoordSysCatalog>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_COORDSYS_CATALOG"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public CoordinateSystemDefinition SelectedCS
        {
            get
            {
                if (grdCs.SelectedRows.Count == 1)
                    return grdCs.SelectedRows[0].DataBoundItem as CoordinateSystemDefinition;
                else if (grdCs.SelectedCells.Count == 1)
                    return grdCs.Rows[grdCs.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystemDefinition;
                else
                    return null;
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = CoordinateSystemDialog.NewCoordinateSystem();
            if (cs != null)
            {
                _presenter.AddNew(cs);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                string oldName = cs.Name;
                if (CoordinateSystemDialog.EditCooridinateSystem(cs))
                {
                    _presenter.Update(oldName, cs);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                _presenter.Delete(cs);
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            //_presenter.Filter(txtFilter.Text);
        }

        public BindingList<CoordinateSystemDefinition> CoordSysDefinitions
        {
            set 
            {
                grdCs.DataSource = value;
            }
        }

        public bool EditEnabled
        {
            set { btnEdit.Enabled = value; }
        }

        public bool DeleteEnabled
        {
            set { btnDelete.Enabled = value; }
        }

        private void grdCs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _presenter.CheckStatus();
        }
    }
}
