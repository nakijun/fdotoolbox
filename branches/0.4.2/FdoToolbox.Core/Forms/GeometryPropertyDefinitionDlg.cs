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
using OSGeo.FDO.Common;

namespace FdoToolbox.Core.Forms
{
    public partial class GeometryPropertyDefinitionDlg : Form
    {
        private GeometricPropertyDefinition _Definition;

        private SpatialConnectionInfo _BoundConnection;

        internal GeometryPropertyDefinitionDlg(SpatialConnectionInfo conn)
        {
            InitializeComponent();
            _BoundConnection = conn;
            _Definition = new GeometricPropertyDefinition(txtName.Text, txtDescription.Text);
            
            GeometryType[] gtypes = _BoundConnection.Connection.GeometryCapabilities.GeometryTypes;

            //Array gtypes = Enum.GetValues(typeof(GeometricType));
            foreach (object val in gtypes)
            {
                chkGeometryTypes.Items.Add(val, false);
            }
        }

        internal GeometryPropertyDefinitionDlg(GeometricPropertyDefinition def, SpatialConnectionInfo conn)
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

            if (def.GeometryTypes != (int)GeometryType.GeometryType_None)
            {
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_CurvePolygon) == (int)GeometryType.GeometryType_CurvePolygon)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_CurvePolygon), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_CurveString) == (int)GeometryType.GeometryType_CurveString)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_CurveString), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_LineString) == (int)GeometryType.GeometryType_LineString)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_LineString), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiCurvePolygon) == (int)GeometryType.GeometryType_MultiCurvePolygon)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiCurvePolygon), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiCurveString) == (int)GeometryType.GeometryType_MultiCurveString)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiCurveString), true);
                //if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiGeometry) == (int)GeometryType.GeometryType_MultiGeometry)
                //    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiGeometry), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiLineString) == (int)GeometryType.GeometryType_MultiLineString)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiLineString), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiPoint) == (int)GeometryType.GeometryType_MultiPoint)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiPoint), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiPolygon) == (int)GeometryType.GeometryType_MultiPolygon)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_MultiPolygon), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_Point) == (int)GeometryType.GeometryType_Point)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_Point), true);
                if ((def.GeometryTypes & (int)GeometryType.GeometryType_Polygon) == (int)GeometryType.GeometryType_Polygon)
                    chkGeometryTypes.SetItemChecked(chkGeometryTypes.Items.IndexOf(GeometryType.GeometryType_Polygon), true);
            }
        }

        public static GeometricPropertyDefinition NewGeometryProperty(SpatialConnectionInfo conn)
        {
            GeometryPropertyDefinitionDlg dlg = new GeometryPropertyDefinitionDlg(conn);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg._Definition;
            }
            return null;
        }

        public static void EditGeometryProperty(GeometricPropertyDefinition def, SpatialConnectionInfo conn)
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
            GeometryType gtype = GeometryType.GeometryType_None;
            foreach (int idx in chkGeometryTypes.SelectedIndices)
            {
                if (chkGeometryTypes.GetItemChecked(idx))
                    gtype |= (GeometryType)Enum.Parse(typeof(GeometryType), chkGeometryTypes.Items[idx].ToString());
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