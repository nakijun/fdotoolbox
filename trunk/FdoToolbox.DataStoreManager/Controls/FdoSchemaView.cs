#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Schema;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using ICSharpCode.Core;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class FdoSchemaView : CollapsiblePanel
    {
        private FdoSchemaViewTreePresenter _presenter;

        internal FdoSchemaView()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        public SchemaDesignContext Context
        {
            get { return _context; }
            set
            {
                _context = value;
                _presenter = new FdoSchemaViewTreePresenter(this, _context);
                this.RightPaneVisible = false;

                if (!_context.IsConnected)
                {
                    this.PhysicalMappingsVisible = false;
                    btnFix.Enabled = false;
                }
                else
                {
                    btnFix.Enabled = true;
                    btnImport.Enabled = _context.CanModifyExistingSchemas;
                    this.PhysicalMappingsVisible = _context.CanOverrideSchemas;
                }

                _presenter.Initialize();
            }
        }

        internal bool PhysicalMappingsVisible
        {
            get { return tabControl1.TabPages.Contains(TAB_PHYSICAL); }
            set
            {
                if (value)
                {
                    if (!tabControl1.TabPages.Contains(TAB_PHYSICAL))
                        tabControl1.TabPages.Add(TAB_PHYSICAL);
                }
                else
                {
                    tabControl1.TabPages.Remove(TAB_PHYSICAL);
                }
            }
        }

        internal bool RightPaneVisible
        {
            get { return !splitContainer1.Panel2Collapsed; }
            set { splitContainer1.Panel2Collapsed = !value; }
        }

        internal void SetLogicalControl(Control c)
        {
            TAB_LOGICAL.Controls.Clear();

            if (c != null)
            {
                c.Dock = DockStyle.Fill;
                TAB_LOGICAL.Controls.Add(c);
            }
        }

        internal void SetPhysicalControl(Control c)
        {
            TAB_PHYSICAL.Controls.Clear();

            if (c != null)
            {
                c.Dock = DockStyle.Fill;
                TAB_PHYSICAL.Controls.Add(c);
            }
        }


        class FdoSchemaViewTreePresenter
        {
            private SchemaDesignContext _context;
            private readonly FdoSchemaView _view;

            const int LEVEL_SCHEMA = 0;
            const int LEVEL_CLASS = 1;
            const int LEVEL_PROPERTY = 2;

            const int IDX_SCHEMA = 0;
            const int IDX_CLASS = 1;
            const int IDX_FEATURE_CLASS = 2;
            const int IDX_KEY = 3;
            const int IDX_DATA_PROPERTY = 4;
            const int IDX_GEOMETRY_PROPERTY = 5;
            const int IDX_ASSOCIATION_PROPERTY = 6;
            const int IDX_OBJECT_PROPERTY = 7;
            const int IDX_RASTER_PROPERTY = 8;

            private ContextMenuStrip ctxSchema;
            private ContextMenuStrip ctxClass;
            private ContextMenuStrip ctxProperty;

            public FdoSchemaViewTreePresenter(FdoSchemaView view, SchemaDesignContext context)
            {
                _view = view;
                _context = context;
                _view.schemaTree.AfterSelect += new TreeViewEventHandler(OnAfterSelect);

                InitContextMenus();
            }

            private void InitContextMenus()
            {
                ctxClass = new ContextMenuStrip();
                ctxProperty = new ContextMenuStrip();
                ctxSchema = new ContextMenuStrip();

                //Add delete item to all
                ctxClass.Items.Add("Delete", ResourceService.GetBitmap("cross"), OnDeleteClass);
                ctxProperty.Items.Add("Delete", ResourceService.GetBitmap("cross"), OnDeleteProperty);
                ctxSchema.Items.Add("Delete", ResourceService.GetBitmap("cross"), OnDeleteSchema);
            }

            void OnDeleteClass(object sender, EventArgs e)
            {

            }

            void OnDeleteProperty(object sender, EventArgs e)
            {

            }

            void OnDeleteSchema(object sender, EventArgs e)
            {

            }

            void OnAfterSelect(object sender, TreeViewEventArgs e)
            {
                switch (e.Node.Level)
                {
                    case LEVEL_SCHEMA:
                        {
                            _view.TAB_LOGICAL.Text = "Logical Schema";
                            var schema = (FeatureSchema)e.Node.Tag;
                            OnSchemaSelected(schema);
                        }
                        break;
                    case LEVEL_CLASS:
                        {
                            var cls = (ClassDefinition)e.Node.Tag;
                            OnClassSelected(cls);
                        }
                        break;
                    case LEVEL_PROPERTY:
                        {
                            var prop = (PropertyDefinition)e.Node.Tag;
                            OnPropertySelected(prop);
                        }
                        break;
                }
            }

            private void OnPropertySelected(PropertyDefinition prop)
            {
                Control c = null;
                if (prop.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Data Property";
                    c = new DataPropertyCtrl(new DataPropertyDefinitionDecorator((DataPropertyDefinition)prop), _context);
                }
                else if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    _view.TAB_LOGICAL.Text = "Logical Geometric Property";
                    c = new GeometricPropertyCtrl(new GeometricPropertyDefinitionDecorator((GeometricPropertyDefinition)prop), _context);
                }

                if (c != null)
                {
                    _view.SetLogicalControl(c);
                    _view.PhysicalMappingsVisible = this.ShowPhysicalMappings;
                    _view.RightPaneVisible = true;
                }
                else
                {
                    _view.RightPaneVisible = false;
                }
            }

            private void OnClassSelected(ClassDefinition cls)
            {
                Control c = null;
                if (cls.ClassType == ClassType.ClassType_Class)
                {
                    _view.TAB_LOGICAL.Text = "Logical Class";
                    c = new ClassDefinitionCtrl(new ClassDecorator((Class)cls), _context);
                }
                else if (cls.ClassType == ClassType.ClassType_FeatureClass)
                {
                    _view.TAB_LOGICAL.Text = "Logical Feature Class";
                    c = new ClassDefinitionCtrl(new FeatureClassDecorator((FeatureClass)cls), _context);
                }

                if (c != null)
                {
                    _view.SetLogicalControl(c);
                    _view.PhysicalMappingsVisible = this.ShowPhysicalMappings;
                    _view.RightPaneVisible = true;
                }
                else
                {
                    _view.RightPaneVisible = false;
                }
            }

            internal bool ShowPhysicalMappings
            {
                get { return _context.Connection != null && _context.CanShowPhysicalMapping; }
            }

            private void OnSchemaSelected(FeatureSchema schema)
            {
                var c = new SchemaCtrl(new FeatureSchemaDecorator(schema));
                _view.SetLogicalControl(c);
                _view.PhysicalMappingsVisible = false;
                _view.RightPaneVisible = true;
            }

            public void Initialize()
            {
                _view.schemaTree.Nodes.Clear();
                foreach (FeatureSchema schema in _context.Schemas)
                {
                    var snode = CreateSchemaNode(schema);
                    _view.schemaTree.Nodes.Add(snode);

                    foreach (ClassDefinition cls in schema.Classes)
                    {
                        var cnode = CreateClassNode(cls);
                        snode.Nodes.Add(cnode);

                        foreach (PropertyDefinition prop in cls.Properties)
                        {
                            var pnode = CreatePropertyNode(prop);
                            cnode.Nodes.Add(pnode);
                        }
                    }
                }
            }

            private TreeNode CreateSchemaNode(FeatureSchema schema)
            {
                return new TreeNode()
                {
                    Name = schema.Name,
                    Text = schema.Name,
                    Tag = schema,
                    ImageIndex = IDX_SCHEMA,
                    SelectedImageIndex = IDX_SCHEMA,
                    ContextMenuStrip = ctxSchema
                };
            }

            private TreeNode CreateClassNode(ClassDefinition cls)
            {
                int idx = IDX_CLASS;
                if (cls.ClassType == ClassType.ClassType_FeatureClass)
                    idx = IDX_FEATURE_CLASS;

                return new TreeNode()
                {
                    Name = cls.Name,
                    Text = cls.Name,
                    Tag = cls,
                    ImageIndex = idx,
                    SelectedImageIndex = idx,
                    ContextMenuStrip = ctxClass
                };
            }

            private TreeNode CreatePropertyNode(PropertyDefinition prop)
            {
                int idx = IDX_DATA_PROPERTY;
                if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    idx = IDX_GEOMETRY_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_AssociationProperty)
                    idx = IDX_ASSOCIATION_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_ObjectProperty)
                    idx = IDX_OBJECT_PROPERTY;
                else if (prop.PropertyType == PropertyType.PropertyType_RasterProperty)
                    idx = IDX_RASTER_PROPERTY;

                return new TreeNode()
                {
                    Name = prop.Name,
                    Text = prop.Name,
                    Tag = prop,
                    ImageIndex = idx,
                    SelectedImageIndex = idx,
                    ContextMenuStrip = ctxProperty
                };
            }

            internal void AddSchema()
            {
                FeatureSchema schema = new FeatureSchema(_context.GetName("Schema"), "");
                _context.AddSchema(schema);
            }

            internal void AddFeatureClass()
            {
                string schema = GetSelectedSchema();
                if (!string.IsNullOrEmpty(schema))
                {
                    FeatureClass cls = new FeatureClass(_context.GetName("FeatureClass"), "");
                    _context.AddClass(schema, cls);
                }
            }

            private string GetSelectedSchema()
            {
                var node = _view.schemaTree.SelectedNode;
                if (node != null)
                    return node.Level == LEVEL_SCHEMA ? node.Text : null;
                else
                    return null;
            }

            internal void FixIncompatibilities()
            {
                _context.FixIncompatibilities();
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            _presenter.FixIncompatibilities();
        }

        private void btnAddSchema_Click(object sender, EventArgs e)
        {
            _presenter.AddSchema();
        }

        private void btnAddFeatureClass_Click(object sender, EventArgs e)
        {
            _presenter.AddFeatureClass();
        }

        private void btnAddClass_Click(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }
    }
}
