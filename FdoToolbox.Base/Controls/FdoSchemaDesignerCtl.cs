using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoSchemaDesignerCtl : UserControl, IFdoSchemaDesignerView, IViewContent
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

        public bool CanClose
        {
            get { return true; }
        }

        public bool Close()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }

        public bool SaveAs()
        {
            return true;
        }

        public event EventHandler ViewContentClosing;

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
    }
}
