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
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoSchemaDesignerCtl : ViewContent, IFdoSchemaDesignerView, IViewContent, IConnectionDependentView
    {
        private FdoSchemaDesignerPresenter _presenter;

        public FdoSchemaDesignerCtl()
        {
            InitializeComponent();
            _presenter = new FdoSchemaDesignerPresenter(this);
        }

        public FdoSchemaDesignerCtl(FdoConnection conn)
        {
            InitializeComponent();
            _presenter = new FdoSchemaDesignerPresenter(this, conn);
        }

        public FdoSchemaDesignerCtl(FdoConnection conn, string schemaName)
        {
            InitializeComponent();
            _presenter = new FdoSchemaDesignerPresenter(this, conn, schemaName);
        }

        public void AddImage(string name, Bitmap bmp)
        {
            imgTree.Images.Add(name, bmp);
        }

        public void SetSchemaNode(string name)
        {
            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = FdoSchemaDesignerPresenter.RES_SCHEMA;
            schemaTree.Nodes.Clear();
            schemaTree.Nodes.Add(node);
            schemaTree.SelectedNode = node;
        }

        public void AddClassNode(string name, string imageKey)
        {
            TreeNode schemaNode = schemaTree.Nodes[0];
            if (schemaNode == null)
                throw new InvalidOperationException("No schema node defined");

            TreeNode node = new TreeNode();
            node.Name = node.Text = name;
            node.ImageKey = node.SelectedImageKey = imageKey;
            node.ContextMenuStrip = ctxClass;
            schemaNode.Nodes.Add(node);
            schemaNode.Expand();
        }

        public void AddPropertyNode(string className, string propName, string imageKey)
        {
            TreeNode schemaNode = schemaTree.Nodes[0];
            if (schemaNode == null)
                throw new InvalidOperationException("No schema node defined");

            TreeNode classNode = schemaNode.Nodes[className];
            if (classNode == null)
                throw new InvalidOperationException("Class node not found");

            TreeNode node = new TreeNode();
            node.Name = node.Text = propName;
            node.ImageKey = node.SelectedImageKey = imageKey;
            node.ContextMenuStrip = ctxProperty;
            classNode.Nodes.Add(node);
            classNode.Expand();
        }

        public string SelectedSchema
        {
            get 
            {
                TreeNode node = schemaTree.Nodes[0];
                return node.Name;
            }
        }

        public string SelectedClass
        {
            get
            {
                TreeNode node = schemaTree.SelectedNode;
                if (node.Level < 1)
                    return null;

                while (node.Level > 1)
                    node = node.Parent;

                return node.Name;
            }
        }

        public string SelectedProperty
        {
            get 
            {
                TreeNode node = schemaTree.SelectedNode;
                if (node.Level == 2)
                    return node.Name;

                return null;
            }
        }

        public void RefreshTree()
        {
            schemaTree.Refresh();
        }

        public object SelectedObject
        {
            set { propGrid.SelectedObject = value; }
        }


        public OSGeo.FDO.Schema.ClassType[] SupportedClassTypes
        {
            set 
            {
                btnAdd.DropDown.Items.Clear();
                foreach (OSGeo.FDO.Schema.ClassType ctype in value)
                {
                    switch(ctype)
                    {
                        case OSGeo.FDO.Schema.ClassType.ClassType_Class:
                            btnAdd.DropDown.Items.Add(ResourceService.GetString("LBL_CLASS"), ResourceService.GetBitmap(FdoSchemaDesignerPresenter.RES_CLASS), delegate
                            {
                                _presenter.AddClass();
                            });
                            break;
                        case OSGeo.FDO.Schema.ClassType.ClassType_FeatureClass:
                            btnAdd.DropDown.Items.Add(ResourceService.GetString("LBL_FEATURE_CLASS"), ResourceService.GetBitmap(FdoSchemaDesignerPresenter.RES_CLASS), delegate 
                            {
                                _presenter.AddFeatureClass();
                            });
                            break;
                    }
                    
                }
            }
        }

        public OSGeo.FDO.Schema.PropertyType[] SupportedPropertyTypes
        {
            set 
            {
                mnuAddNewProperty.DropDown.Items.Clear();
                foreach (OSGeo.FDO.Schema.PropertyType ptype in value)
                {
                    switch (ptype)
                    {
                        case OSGeo.FDO.Schema.PropertyType.PropertyType_GeometricProperty:
                            mnuAddNewProperty.DropDown.Items.Add(ResourceService.GetString("LBL_GEOMETRIC_PROPERTY"), ResourceService.GetBitmap("shape_handles"), delegate
                            {
                                _presenter.AddGeometricProperty();
                            });
                            break;
                        case OSGeo.FDO.Schema.PropertyType.PropertyType_DataProperty:
                            mnuAddNewProperty.DropDown.Items.Add(ResourceService.GetString("LBL_DATA_PROPERTY"), ResourceService.GetBitmap("table"), delegate
                            {
                                _presenter.AddDataProperty();
                            });
                            break;
                    }
                }
            }
        }

        public string SelectedName
        {
            set { schemaTree.SelectedNode.Name = schemaTree.SelectedNode.Text = value; }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; TitleChanged(this, EventArgs.Empty); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        private void schemaTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Level)
            {
                case 0:
                    _presenter.SchemaSelected();
                    break;
                case 1:
                    _presenter.ClassSelected();
                    break;
                case 2:
                    _presenter.PropertySelected();
                    break;
            }
        }

        private void schemaTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                schemaTree.SelectedNode = schemaTree.GetNodeAt(e.X, e.Y);
            }
        }

        public bool ApplyEnabled
        {
            get { return btnApply.Enabled; }
            set { btnApply.Enabled = value; }
        }

        private void saveToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlFile = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_SCHEMA"), ResourceService.GetString("FILTER_SCHEMA_FILE"));
            if (!string.IsNullOrEmpty(xmlFile))
            {
                try
                {
                    _presenter.SaveSchemaToXml(xmlFile);
                    MessageService.ShowMessage(ResourceService.GetString("MSG_SCHEMA_SAVED"));
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex);
                }
            }
        }

        private void saveToNewSDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sdfFile = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_SCHEMA"), ResourceService.GetString("FILTER_SDF_FILE"));
            if (!string.IsNullOrEmpty(sdfFile))
            {
                try
                {
                    _presenter.SaveSchemaToSdf(sdfFile);
                    MessageService.ShowMessage(ResourceService.GetString("MSG_SCHEMA_SAVED"));
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex);
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                _presenter.ApplySchema();
                MessageService.ShowMessage(ResourceService.GetString("MSG_SCHEMA_APPLIED"));
                this.SchemaApplied(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }


        public event EventHandler SchemaApplied = delegate { };

        private void deletePropertyItem_Click(object sender, EventArgs e)
        {
            _presenter.RemoveProperty();
        }

        private void deleteClassItem_Click(object sender, EventArgs e)
        {
            _presenter.RemoveClass();
        }


        public void RemoveClassNode(string className)
        {
            TreeNode schemaNode = schemaTree.Nodes[0];
            if (schemaNode == null)
                throw new InvalidOperationException("No schema node defined");

            schemaNode.Nodes.RemoveByKey(className);
        }

        public void RemovePropertyNode(string className, string propName)
        {
            TreeNode schemaNode = schemaTree.Nodes[0];
            if (schemaNode == null)
                throw new InvalidOperationException("No schema node defined");

            TreeNode classNode = schemaNode.Nodes[className];
            if (classNode != null)
            {
                classNode.Nodes.RemoveByKey(propName);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_LOAD_SCHEMA"), ResourceService.GetString("FILTER_SCHEMA_FILE"));
            if (FileService.FileExists(file))
            {
                _presenter.Load(file);
            }
        }

        public bool LoadEnabled
        {
            set { btnLoad.Enabled = value; }
        }

        public bool DependsOnConnection(FdoConnection conn)
        {
            return _presenter.MatchesConnection(conn);
        }
    }
}
