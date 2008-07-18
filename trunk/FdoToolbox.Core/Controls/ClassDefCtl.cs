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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Forms;
using System.Diagnostics;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core.Controls
{
    /// <summary>
    /// User Control to create Class Definitions.
    /// </summary>
    public partial class ClassDefCtl : BaseDocumentCtl, IConnectionBoundCtl
    {
        private ClassDefinition _ClassDef;

        private BindingSource _BoundProperties;

        private BindingSource _BoundIdentityProperties;

        private FeatureSchema _ParentSchema;

        private IConnection _BoundConnection;

        public ClassDefCtl(ClassDefinition classDef, IConnection conn)
        {
            InitializeComponent();
            _BoundConnection = conn;
            _ClassDef = classDef;
            InitCommon(_ClassDef.ClassType);
            InitForm();
            ToggleUI();
        }

        public ClassDefCtl(FeatureSchema schema, ClassType ctype, IConnection conn)
        {
            InitializeComponent();
            switch (ctype)
            {
                case ClassType.ClassType_Class:
                    _ClassDef = new Class();
                    break;
                case ClassType.ClassType_FeatureClass:
                    _ClassDef = new FeatureClass();
                    break;
                case ClassType.ClassType_NetworkClass:
                    _ClassDef = new NetworkClass();
                    break;
                case ClassType.ClassType_NetworkLayerClass:
                    _ClassDef = new NetworkLayerClass();
                    break;
                case ClassType.ClassType_NetworkLinkClass:
                    _ClassDef = new NetworkLinkFeatureClass();
                    break;
                case ClassType.ClassType_NetworkNodeClass:
                    _ClassDef = new NetworkNodeFeatureClass();
                    break;
            }
            Debug.Assert(_ClassDef != null);
            _BoundConnection = conn;
            _ParentSchema = schema;
            InitCommon(ctype);
            ToggleUI();
        }

        private void ToggleUI()
        {
            associationPropertyToolStripMenuItem.Visible = this.BoundConnection.SchemaCapabilities.SupportsAssociationProperties;
            objectPropertyToolStripMenuItem.Visible = this.BoundConnection.SchemaCapabilities.SupportsObjectProperties;
            rasterPropertyToolStripMenuItem.Visible = this.BoundConnection.RasterCapabilities.SupportsRaster();
        }

        private void InitForm()
        {
            txtName.Text = _ClassDef.Name;
            txtDescription.Text = _ClassDef.Description;
            chkAbstract.Checked = _ClassDef.IsAbstract;
            chkComputed.Checked = _ClassDef.IsComputed;
        }

        private void InitCommon(ClassType ctype)
        {
            grdProperties.AutoGenerateColumns = false;
            _BoundProperties = new BindingSource();
            _BoundIdentityProperties = new BindingSource();
            _BoundProperties.DataSource = _ClassDef.Properties;
            _BoundIdentityProperties.DataSource = _ClassDef.IdentityProperties;
            grdProperties.DataSource = _BoundProperties;
            lstIdentityProperties.DataSource = _BoundIdentityProperties;
            InitializeExtendedInfo(ctype);
        }

        private void InitializeExtendedInfo(ClassType ctype)
        {
            switch (ctype)
            {
                case ClassType.ClassType_FeatureClass:
                    FeatureClassExtendedInfoCtl ctl = new FeatureClassExtendedInfoCtl(GetGeometryPropertyList());
                    //Hook on to binding source change event so that
                    //we can sync the property list on the extended info control
                    
                    ctl.GeometryPropertyList = GetGeometryPropertyList();
                    ListChangedEventHandler listChanged =  delegate(object sender, ListChangedEventArgs e)
                    {
                        if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted)
                        {
                            ctl.GeometryPropertyList = GetGeometryPropertyList();
                        }
                    };
                    _BoundProperties.ListChanged += listChanged;
                    ctl.Disposed += delegate
                    {
                        _BoundProperties.ListChanged -= listChanged;
                    };
                    this.ExtensionControl = ctl;
                    break;
                //TODO: Support other class types
            }
        }

        private List<GeometricPropertyDefinition> GetGeometryPropertyList()
        {
            List<GeometricPropertyDefinition> defs = new List<GeometricPropertyDefinition>();
            foreach (PropertyDefinition def in _ClassDef.Properties)
            {
                if (def.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    defs.Add((GeometricPropertyDefinition)def);
            }
            return defs;
        }

        /*
        public override string Title
        {
            get
            {
                string title = string.Empty;
                title = SetTitle(title);
                return title;
            }
        }*/

        private void SetTitle()
        {
            switch (_ClassDef.ClassType)
            {
                case ClassType.ClassType_Class:
                    this.Title = "Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_FeatureClass:
                    this.Title = "Feature Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkClass:
                    this.Title = "Network Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkLayerClass:
                    this.Title = "Network Layer Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkLinkClass:
                    this.Title = "Network Link Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkNodeClass:
                    this.Title = "Network Node Class Definition - " + _ClassDef.Name;
                    break;
            }
        }

        private int counter = 0;

        private string GetGeneratedPropertyName()
        {
            return "Property" + (counter++);
        }

        private void NewDataProperty_Click(object sender, EventArgs e)
        {
            DataPropertyDefinition def = DataPropertyDefinitionDlg.NewDataProperty(this.BoundConnection);
            if(def != null)
                _BoundProperties.Add(def);
        }

        private void NewGeometryProperty_Click(object sender, EventArgs e)
        {
            GeometricPropertyDefinition def = GeometryPropertyDefinitionDlg.NewGeometryProperty(this.BoundConnection);
            if(def != null)
                _BoundProperties.Add(def);
        }

        private void NewRasterProperty_Click(object sender, EventArgs e)
        {
            //RasterPropertyDefinition def = new RasterPropertyDefinition(GetGeneratedPropertyName(), "");
            //_BoundProperties.Add(def);
        }

        private void NewObjectProperty_Click(object sender, EventArgs e)
        {
            //ObjectPropertyDefinition def = new ObjectPropertyDefinition(GetGeneratedPropertyName(), "");
            //_BoundProperties.Add(def);
        }

        private void NewAssociationProperty_Click(object sender, EventArgs e)
        {
            //AssociationPropertyDefinition def = new AssociationPropertyDefinition(GetGeneratedPropertyName(), "");
            //_BoundProperties.Add(def);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            PropertyDefinition def = GetSelectedProperty();
            if (def != null)
            {
                if (def.PropertyType == PropertyType.PropertyType_DataProperty && _ClassDef.IdentityProperties.Contains((DataPropertyDefinition)def))
                    _ClassDef.IdentityProperties.Remove((DataPropertyDefinition)def);
                _BoundProperties.Remove(def);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            PropertyDefinition def = GetSelectedProperty();
            if (def != null)
            {
                Form frm = null;
                switch (def.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        frm = new DataPropertyDefinitionDlg((DataPropertyDefinition)def, this.BoundConnection);
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        frm = new GeometryPropertyDefinitionDlg((GeometricPropertyDefinition)def, this.BoundConnection);
                        break;
                }
                if (frm != null)
                    frm.ShowDialog();
            }
        }

        const int IDX_COL_IDENTITY = 2;

        private PropertyDefinition GetSelectedProperty()
        {
            if (grdProperties.SelectedRows.Count == 1)
                return grdProperties.SelectedRows[0].DataBoundItem as PropertyDefinition;
            else if (grdProperties.SelectedCells.Count == 1)
                return grdProperties.Rows[grdProperties.SelectedCells[0].RowIndex].DataBoundItem as PropertyDefinition;
            return null;
        }

        private void grdProperties_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnMakeIdentity.Enabled = btnEdit.Enabled = btnDelete.Enabled = (grdProperties.SelectedRows.Count == 1 || grdProperties.SelectedCells.Count > 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Cancel();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text)) 
            {
                errorProvider.SetError(txtName, "Required");
            }
            else if (ValidateClassType())
            {
                ApplyExtendedInfo();
                //This is a new class definition
                if (_ParentSchema != null)
                    _ParentSchema.Classes.Add(_ClassDef);
                this.Accept();
            }
        }

        private void ApplyExtendedInfo()
        {
            switch (_ClassDef.ClassType)
            {
                case ClassType.ClassType_FeatureClass:
                    Debug.Assert(this.ExtensionControl is FeatureClassExtendedInfoCtl);
                    Debug.Assert((this.ExtensionControl as FeatureClassExtendedInfoCtl).GeometryProperty != null);
                    ((FeatureClass)_ClassDef).GeometryProperty = (this.ExtensionControl as FeatureClassExtendedInfoCtl).GeometryProperty;
                    break;
            }
        }

        private Control ExtensionControl
        {
            get
            {
                return grpExtended.Controls[0];
            }
            set
            {
                grpExtended.Controls.Clear();
                value.Dock = DockStyle.Fill;
                grpExtended.Controls.Add(value);
            }
        }

        private bool ValidateClassType()
        {
            bool valid = true; 
            switch (_ClassDef.ClassType)
            {
                case ClassType.ClassType_FeatureClass:
                    Control ctl = this.ExtensionControl;
                    Debug.Assert(ctl != null);
                    Debug.Assert(ctl is FeatureClassExtendedInfoCtl);
                    GeometricPropertyDefinition def = ((FeatureClassExtendedInfoCtl)ctl).GeometryProperty;
                    if (def == null)
                    {
                        ((FeatureClassExtendedInfoCtl)ctl).FlagError(errorProvider, "Please select a geometry property");
                        valid = false;
                    }
                    break;
            }
            return valid;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _ClassDef.Name = txtName.Text;
            switch (_ClassDef.ClassType)
            {
                case ClassType.ClassType_Class:
                    this.Title = "Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_FeatureClass:
                    this.Title = "Feature Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkClass:
                    this.Title = "Network Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkLayerClass:
                    this.Title = "Network Layer Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkLinkClass:
                    this.Title = "Network Link Class Definition - " + _ClassDef.Name;
                    break;
                case ClassType.ClassType_NetworkNodeClass:
                    this.Title = "Network Node Class Definition - " + _ClassDef.Name;
                    break;
            }
        }

        private void IdentityProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = (_BoundIdentityProperties.Count > 0);
        }

        private void btnMakeIdentity_Click(object sender, EventArgs e)
        {
            PropertyDefinition def = GetSelectedProperty();
            if (def.PropertyType == PropertyType.PropertyType_DataProperty)
            {
                DataType[] supported = this.BoundConnection.SchemaCapabilities.SupportedIdentityPropertyTypes;
                if (Array.IndexOf<DataType>(supported, ((DataPropertyDefinition)def).DataType) >= 0)
                {
                    //Supports composite id (multiple identity properties)
                    if (this.BoundConnection.SchemaCapabilities.SupportsCompositeId)
                    {
                        if(!_BoundIdentityProperties.Contains(def))
                            _BoundIdentityProperties.Add(def);
                    }
                    else if(_BoundIdentityProperties.Count == 0) //Single identity
                    {
                        _BoundIdentityProperties.Add(def);
                    }
                }
                else
                {
                    AppConsole.Alert("Error", "Cannot make identity property: " + ((DataPropertyDefinition)def).DataType + " not supported");
                }
            }
            else
            {
                AppConsole.Alert("Error", "Cannot make identity property: Invalid property type");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DataPropertyDefinition def = lstIdentityProperties.SelectedItem as DataPropertyDefinition;
            if (def != null)
            {
                _BoundIdentityProperties.Remove(def);
            }
        }

        public IConnection BoundConnection
        {
            get { return _BoundConnection; }
        }
    }
}
