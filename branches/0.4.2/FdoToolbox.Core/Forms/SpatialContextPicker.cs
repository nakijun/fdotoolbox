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
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.SpatialContext;

namespace FdoToolbox.Core.Forms
{
    /// <summary>
    /// A dialog that allows a user to select a spatial context (by name)
    /// from a list of known spatial contexts in the current connection
    /// </summary>
    public partial class SpatialContextPicker : Form
    {
        internal SpatialContextPicker()
        {
            InitializeComponent();
        }
        
        public SpatialContextPicker(IConnection conn) : this()
        {
            FeatureService service = new FeatureService(conn);
            lstNames.Items.Clear();
            List<SpatialContextInfo> contexts = service.GetSpatialContexts();
            contexts.ForEach(delegate(SpatialContextInfo ctx) { lstNames.Items.Add(ctx.Name); });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lstNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (lstNames.SelectedIndex >= 0);
        }

        public static string GetName(SpatialConnectionInfo conn)
        {
            SpatialContextPicker picker = new SpatialContextPicker(conn.Connection);
            if (picker.ShowDialog() == DialogResult.OK)
            {
                return picker.lstNames.SelectedItem.ToString();
            }
            return null;
        }
    }
}