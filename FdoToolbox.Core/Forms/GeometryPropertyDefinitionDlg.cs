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

        private IConnection _BoundConnection;

        internal GeometryPropertyDefinitionDlg(IConnection conn)
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

        internal GeometryPropertyDefinitionDlg(GeometricPropertyDefinition def, IConnection conn)
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
 
        public static GeometricPropertyDefinition NewGeometryProperty(IConnection conn)
        {
            GeometryPropertyDefinitionDlg dlg = new GeometryPropertyDefinitionDlg(conn);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg._Definition;
            }
            return null;
        }

        public static void EditGeometryProperty(GeometricPropertyDefinition def, IConnection conn)
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