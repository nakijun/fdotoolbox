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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Core;

namespace FdoToolbox.Lib.Forms
{
    public partial class CoordinateSystemPicker : Form
    {
        internal CoordinateSystemPicker()
        {
            InitializeComponent();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void grdProjections_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnOK.Enabled = (grdProjections.Rows.Count > 0);
        }

        public static CoordinateSystem GetCoordinateSystem()
        {
            CoordinateSystemPicker picker = new CoordinateSystemPicker();
            if (picker.ShowDialog() == DialogResult.OK)
            {
                return picker.SelectedCS;
            }
            return null;
        }
    }
}