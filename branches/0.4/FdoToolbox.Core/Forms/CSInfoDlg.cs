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

namespace FdoToolbox.Core.Forms
{
    public partial class CSInfoDlg : Form
    {
        internal CSInfoDlg()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider1.SetError(txtName, "Required");
                return;
            }
            if (string.IsNullOrEmpty(txtWKT.Text))
            {
                errorProvider1.SetError(txtWKT, "Required");
                return;
            }
            if (HostApplication.Instance.CoordinateSystemCatalog.ProjectionExists(txtName.Text))
            {
                errorProvider1.SetError(txtName, "A coordinate system of that name already exists");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        public static CoordinateSystem NewCoordinateSystem()
        {
            CSInfoDlg diag = new CSInfoDlg();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return new CoordinateSystem(diag.txtName.Text, diag.txtDescription.Text, diag.txtWKT.Text);
            }
            return null;
        }

        public static bool EditCooridinateSystem(CoordinateSystem cs)
        {
            CSInfoDlg diag = new CSInfoDlg();
            diag.txtName.Text = cs.Name;
            diag.txtDescription.Text = cs.Description;
            diag.txtWKT.Text = cs.Wkt;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                cs.Name = diag.txtName.Text;
                cs.Description = diag.txtDescription.Text;
                cs.Wkt = diag.txtWKT.Text;
                return true;
            }
            return false;
        }
    }
}