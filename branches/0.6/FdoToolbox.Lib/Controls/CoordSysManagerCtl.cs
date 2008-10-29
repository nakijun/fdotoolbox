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
using FdoToolbox.Lib.Forms;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Core;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Lib.Controls
{
    public partial class CoordSysManagerCtl : BaseDocumentCtl
    {
        public CoordSysManagerCtl()
        {
            InitializeComponent();
            this.Title = "Coordinate System Catalog";
        }

        protected override void OnLoad(EventArgs e)
        {
            grdProjections.DataSource = AppGateway.RunningApplication.CoordinateSystemCatalog.GetAllProjections();
            base.OnLoad(e);
        }

        public CoordinateSystem SelectedCS
        {
            get
            {
                if (grdProjections.SelectedRows.Count == 1)
                    return grdProjections.SelectedRows[0].DataBoundItem as CoordinateSystem;
                else if (grdProjections.SelectedCells.Count == 1)
                    return grdProjections.Rows[grdProjections.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystem;
                else
                    return null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CoordinateSystem cs = CSInfoDlg.NewCoordinateSystem();
            if (cs != null)
            {
                AppGateway.RunningApplication.CoordinateSystemCatalog.AddProjection(cs);
                AppConsole.Alert("Added", "Coordinate System Added");
                CheckButtonStatus();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CoordinateSystem cs = this.SelectedCS;
            if (cs != null)
            {
                string oldName = cs.Name;
                if(CSInfoDlg.EditCooridinateSystem(cs))
                {
                    if (AppGateway.RunningApplication.CoordinateSystemCatalog.UpdateProjection(cs, oldName))
                    {
                        AppConsole.Alert("Updated", "Coordinate System Updated");
                        CheckButtonStatus();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CoordinateSystem cs = this.SelectedCS;
            if (cs != null)
            {
                if (AppGateway.RunningApplication.CoordinateSystemCatalog.DeleteProjection(cs))
                {
                    AppConsole.Alert("Deleted", "Coordinate System Deleted");
                    CheckButtonStatus();
                }
            }
        }

        private void grdProjections_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CheckButtonStatus();
        }

        private void CheckButtonStatus()
        {
            btnEdit.Enabled = btnDelete.Enabled = (grdProjections.Rows.Count > 0);
        }
    }
}
