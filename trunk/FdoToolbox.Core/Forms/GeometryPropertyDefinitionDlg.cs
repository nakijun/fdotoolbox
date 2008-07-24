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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core.Forms
{
    public partial class GeometryPropertyDefinitionDlg : Form
    {
        private GeometricPropertyDefinition _Definition;

        private ConnectionInfo _BoundConnection;

        internal GeometryPropertyDefinitionDlg(ConnectionInfo conn)
        {
            InitializeComponent();
            _BoundConnection = conn;
            _Definition = new GeometricPropertyDefinition(txtName.Text, txtDescription.Text);
            
            Array gtypes = Enum.GetValues(typeof(GeometricType));
            foreach (object val in gtypes)
            {
                chkGeometryTypes.Items.Add(val, false);
            }
        }

        internal GeometryPropertyDefinitionDlg(GeometricPropertyDefinition def, ConnectionInfo conn)
            : this(conn)
        {
            _Definition = def;
            txtDescription.Text = def.Description;
            txtName.Text = def.Name;
            txtSpatialContext.Text = def.SpatialContextAssociation;
            chkElevation.Checked = def.HasElevation;
            chkMeasure.Checked = def.HasMeasure;
            chkReadOnly.Checked = def.ReadOnly;
            chkSystem.Checked = def.IsSystem;
            chkGeometryTypes.ClearSelected();
            if ((def.GeometryTypes & (int)GeometricType.GeometricType_All) == (int)GeometricType.GeometricType_All)
                chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometricType.GeometricType_All), true);
            if ((def.GeometryTypes & (int)GeometricType.GeometricType_Curve) == (int)GeometricType.GeometricType_Curve)
                chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometricType.GeometricType_Curve), true);
            if ((def.GeometryTypes & (int)GeometricType.GeometricType_Point) == (int)GeometricType.GeometricType_Point)
                chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometricType.GeometricType_Point), true);
            if ((def.GeometryTypes & (int)GeometricType.GeometricType_Solid) == (int)GeometricType.GeometricType_Solid)
                chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometricType.GeometricType_Solid), true);
            if ((def.GeometryTypes & (int)GeometricType.GeometricType_Surface) == (int)GeometricType.GeometricType_Surface)
                chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometricType.GeometricType_Surface), true);    
        }

        public static GeometricPropertyDefinition NewGeometryProperty(ConnectionInfo conn)
        {
            GeometryPropertyDefinitionDlg dlg = new GeometryPropertyDefinitionDlg(conn);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg._Definition;
            }
            return null;
        }

        public static void EditGeometryProperty(GeometricPropertyDefinition def, ConnectionInfo conn)
        {
            GeometryPropertyDefinitionDlg dlg = new GeometryPropertyDefinitionDlg(def, conn);
            dlg.ShowDialog();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
                errorProvider.SetError(txtName, "Required");

            //set class
            _Definition.Name = txtName.Text;
            _Definition.Description = txtDescription.Text;
            _Definition.IsSystem = chkSystem.Checked;
            _Definition.HasElevation = chkElevation.Checked;
            _Definition.HasMeasure = chkMeasure.Checked;
            _Definition.ReadOnly = chkReadOnly.Checked;
            _Definition.GeometryTypes = GetGeometryTypes();
            _Definition.SpatialContextAssociation = txtSpatialContext.Text;
            this.DialogResult = DialogResult.OK;
        }

        private int GetGeometryTypes()
        {
            GeometricType gtype = default(GeometricType);
            foreach (int idx in chkGeometryTypes.SelectedIndices)
            {
                if (chkGeometryTypes.GetItemChecked(idx))
                    gtype |= (GeometricType)Enum.Parse(typeof(GeometricType), chkGeometryTypes.Items[idx].ToString());
            }
            return (int)gtype;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnPickContext_Click(object sender, EventArgs e)
        {
            string name = SpatialContextPicker.GetName(_BoundConnection);
            if (!string.IsNullOrEmpty(name))
                txtSpatialContext.Text = name;
        }
    }
}