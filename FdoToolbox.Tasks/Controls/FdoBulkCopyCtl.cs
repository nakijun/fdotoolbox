using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using FdoToolbox.Core;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoBulkCopyCtl : UserControl, IViewContent, IFdoBulkCopyView
    {
        private FdoBulkCopyPresenter _presenter;

        public FdoBulkCopyCtl()
        {
            InitializeComponent();
            _presenter = new FdoBulkCopyPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public string Title
        {
            get { return "Bulk Copy Settings"; }
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

        public event EventHandler ViewContentClosing = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        public string TaskName
        {
            get
            {
                return txtName.Text;
            }
            set
            {
                txtName.Text = value;
            }
        }

        public List<string> SourceConnections
        {
            set { cmbSrcConnection.DataSource = value; }
        }

        public List<string> TargetConnections
        {
            set { cmbTargetConnection.DataSource = value; }
        }

        public List<string> SourceSchemas
        {
            set { cmbSrcSchema.DataSource = value; }
        }

        public List<string> TargetSchemas
        {
            set { cmbTargetSchema.DataSource = value; }
        }

        public string SelectedSourceConnection
        {
            get
            {
                return cmbSrcConnection.SelectedItem as string;
            }
            set
            {
                cmbSrcConnection.SelectedItem = value;
            }
        }

        public string SelectedTargetConnection
        {
            get
            {
                return cmbTargetConnection.SelectedItem as string;
            }
            set
            {
                cmbTargetConnection.SelectedItem = value;
            }
        }

        public string SelectedSourceSchema
        {
            get
            {
                return cmbSrcSchema.SelectedItem as string;
            }
            set
            {
                cmbSrcSchema.SelectedItem = value;
            }
        }

        public string SelectedTargetSchema
        {
            get
            {
                return cmbTargetSchema.SelectedItem as string;
            }
            set
            {
                cmbTargetSchema.SelectedItem = value;
            }
        }

        public List<string> SourceSpatialContexts
        {
            set 
            {
                chkListSpatialContexts.Items.Clear();
                foreach (string str in value)
                {
                    chkListSpatialContexts.Items.Add(str, false);
                }
            }
        }

        public string SpatialFilter
        {
            get { return txtSpatialFilter.Text; }
        }

        public int BatchInsertSize
        {
            get
            {
                return Convert.ToInt32(numBatchSize.Value);
            }
            set
            {
                numBatchSize.Value = Convert.ToDecimal(value);
            }
        }

        public bool BatchEnabled
        {
            set 
            {
                if (!value)
                    numBatchSize.Value = 0;
                numBatchSize.Enabled = value; 
            }
        }

        public bool CopySpatialContexts
        {
            get
            {
                return chkCopySpatialContexts.Checked;
            }
            set
            {
                chkCopySpatialContexts.Checked = value;
                chkListSpatialContexts.Enabled = value;
                if (!value)
                {
                    foreach (int idx in chkListSpatialContexts.CheckedIndices)
                    {
                        chkListSpatialContexts.SetItemChecked(idx, false);
                    }
                }
            }
        }

        public void ClearMappings()
        {
            ClassesNode.Nodes.Clear();
        }

        public void RemoveAllMappings()
        {
            //Null the tag property of every class and property node
            foreach (TreeNode classNode in ClassesNode.Nodes)
            {
                _presenter.UnmapClass(classNode.Name);
            }
        }

        private TreeNode ClassesNode
        {
            get
            {
                return treeMappings.Nodes[0];
            }
        }

        public void AddClass(string className)
        {
            TreeNode classNode = new TreeNode();
            classNode.Name = className;
            classNode.Text = className + " (Unmapped)";
            classNode.ContextMenuStrip = ctxSelectedClass;
            classNode.Nodes.Add(PREFIX_CLASS_OPTIONS, ResourceService.GetString("NODE_OPTIONS"));
            classNode.Nodes.Add(PREFIX_CLASS_PROPERTIES, ResourceService.GetString("NODE_PROPERTIES"));
            TreeNode exprNode = new TreeNode();
            exprNode.Text = ResourceService.GetString("NODE_EXPRESSION");
            exprNode.Name = PREFIX_CLASS_EXPRESSION;
            exprNode.ContextMenuStrip = ctxExpressions;
            classNode.Nodes.Add(exprNode);
            ClassesNode.Nodes.Add(classNode);
            treeMappings.ExpandAll();
        }

        private const string PREFIX_CLASS_OPTIONS = "CLASS_OPTIONS";
        private const string PREFIX_CLASS_PROPERTIES = "CLASS_PROPERTIES";
        private const string PREFIX_CLASS_EXPRESSION = "CLASS_EXPRESSION";

        public void AddClassSourceFilterOption(string className)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode filterNode = new TreeNode();
                filterNode.Name = filterNode.Text = ResourceService.GetString("LBL_SOURCE_FILTER");
                filterNode.ContextMenuStrip = ctxFilter;
                classNode.Nodes[PREFIX_CLASS_OPTIONS].Nodes.Add(filterNode);
            }
        }

        public void AddClassDeleteOption(string className)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode deleteNode = new TreeNode();
                deleteNode.Name = deleteNode.Text = ResourceService.GetString("LBL_DELETE_TARGET");
                deleteNode.ContextMenuStrip = ctxDelete;
                classNode.Nodes[PREFIX_CLASS_OPTIONS].Nodes.Add(deleteNode);
            }
        }

        public void AddClassProperty(string className, string propName, string imgKey)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode propertyNode = new TreeNode();
                propertyNode.Name = propName;
                propertyNode.Text = propName + " (Unmapped)";
                propertyNode.ImageKey = propertyNode.SelectedImageKey = imgKey;
                classNode.Nodes[PREFIX_CLASS_PROPERTIES].Nodes.Add(propertyNode);
                treeMappings.ExpandAll();
            }
        }

        public void MapClassProperty(string className, string propName, string targetProp)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode propertyNode = classNode.Nodes[PREFIX_CLASS_PROPERTIES].Nodes[propName];
                if (propertyNode != null)
                {
                    SetPropertyNode(propertyNode, targetProp);
                }
            }
        }

        private static void SetPropertyNode(TreeNode propertyNode, string targetProp)
        {
            propertyNode.Tag = targetProp;
            if (targetProp != null)
                propertyNode.Text = propertyNode.Name + " => " + targetProp;
            else
                propertyNode.Text = propertyNode.Name + " (Unmapped)";
        }

        public void MapClass(string className, string targetClass)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                classNode.Tag = targetClass;
                if (targetClass != null)
                {
                    classNode.Text = className + " => " + targetClass;
                }
                else
                {
                    classNode.Text = className + " (Unmapped)";
                    //Un-map the properties
                    foreach (TreeNode propNode in classNode.Nodes[PREFIX_CLASS_PROPERTIES].Nodes)
                    {
                        _presenter.UnmapProperty(className, propNode.Name);
                    }
                }
            }
        }

        public List<string> MappableClasses
        {
            set 
            {
                ctxSelectedClass.Items.Clear();
                foreach (string cls in value)
                {
                    ctxSelectedClass.Items.Add(
                        "Map to: " + cls,
                        null,
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                _presenter.MapClass(treeMappings.SelectedNode.Name, cls);
                            }
                            catch (MappingException ex)
                            {
                                MessageService.ShowError(ex);
                            }
                        });
                }
            }
        }

        public void SetMappableProperties(string className, List<string> properties)
        {
            if (properties == null || properties.Count == 0)
                return;

            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                mapExpressionItem.DropDown.Items.Clear();
                ContextMenuStrip ctxProperties = new ContextMenuStrip();
                ctxProperties.Items.Add("Remove Mapping", ResourceService.GetBitmap("cross"), delegate
                {
                    SetPropertyNode(treeMappings.SelectedNode, null);
                });
                ctxProperties.Items.Add(new ToolStripSeparator());
                foreach (string p in properties)
                {
                    string prop = p;
                    ctxProperties.Items.Add(
                        "Map property to: " + prop,
                        ResourceService.GetBitmap("table"),
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                string sp = treeMappings.SelectedNode.Name;
                                _presenter.MapProperty(className, sp, classNode.Tag.ToString(), prop);
                            }
                            catch (MappingException ex)
                            {
                                MessageService.ShowError(ex);
                            }
                        });
                    mapExpressionItem.DropDown.Items.Add(
                        prop,
                        ResourceService.GetBitmap("table"),
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                string alias = treeMappings.SelectedNode.Name;
                                ExpressionMappingInfo map = (ExpressionMappingInfo)treeMappings.SelectedNode.Tag;
                                _presenter.MapExpression(className, alias, classNode.Tag.ToString(), prop);
                            }
                            catch (MappingException ex)
                            {
                                MessageService.ShowError(ex);
                            }
                        });
                }
                foreach (TreeNode propNode in classNode.Nodes[PREFIX_CLASS_PROPERTIES].Nodes)
                {
                    propNode.ContextMenuStrip = ctxProperties;
                }
            }
        }

        private void cmbSrcConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SourceConnectionChanged();
        }

        private void cmbSrcSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.SourceSchemaChanged();
        }

        private void cmbTargetConnection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.TargetConnectionChanged();
        }

        private void cmbTargetSchema_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _presenter.TargetSchemaChanged();
        }

        public void AddExpression(string className, string alias, string expr)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode exprNode = new TreeNode();
                exprNode.Text = alias;
                exprNode.ToolTipText = "Expression: " + expr;
                exprNode.Name = alias;
                ExpressionMappingInfo map = new ExpressionMappingInfo();
                map.Expression = expr;
                exprNode.Tag = map;
                exprNode.ContextMenuStrip = ctxSelectedExpression;
                classNode.Nodes[PREFIX_CLASS_EXPRESSION].Nodes.Add(exprNode);
                treeMappings.ExpandAll();
            }
        }

        public void EditExpression(string className, string alias, string expr)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode exprNode = classNode.Nodes[PREFIX_CLASS_EXPRESSION].Nodes[alias];
                if (exprNode.Tag != null)
                {
                    ((ExpressionMappingInfo)exprNode.Tag).Expression = expr;
                }
            }
        }

        public void RemoveExpression(string className, string alias)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                classNode.Nodes[PREFIX_CLASS_EXPRESSION].Nodes.RemoveByKey(alias);
            }
        }

        public void MapExpression(string className, string alias, string targetProp)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode exprNode = classNode.Nodes[PREFIX_CLASS_EXPRESSION].Nodes[alias];
                if (exprNode.Tag != null)
                {
                    ExpressionMappingInfo map = (ExpressionMappingInfo)exprNode.Tag;
                    map.TargetProperty = targetProp;
                    if (targetProp != null)
                        exprNode.Text = exprNode.Name + " => " + targetProp;
                    else
                        exprNode.Text = exprNode.Name;
                }
            }
        }

        private void removeExpressionItem_Click(object sender, EventArgs e)
        {
            treeMappings.Nodes.Remove(treeMappings.SelectedNode);
        }

        private void editExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode exprNode = treeMappings.SelectedNode;
            if (exprNode.Tag != null)
            {
                TreeNode classNode = exprNode.Parent.Parent;
                ExpressionMappingInfo map = (ExpressionMappingInfo)exprNode.Tag;
                FdoConnection conn = _presenter.GetSourceConnection();
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    ClassDefinition cd = service.GetClassByName(this.SelectedSourceSchema, classNode.Name);
                    if (cd != null)
                    {
                        string expr = ExpressionEditor.EditExpression(conn, cd, map.Expression, ExpressionMode.Normal);
                        if (expr != null)
                        {
                            map.Expression = expr;
                        }
                    }
                }
            }
        }

        private void treeMappings_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeMappings.SelectedNode = treeMappings.GetNodeAt(e.X, e.Y);
            }
        }

        private void addComputedExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode exprNode = treeMappings.SelectedNode;
            TreeNode classNode = exprNode.Parent;
            FdoConnection conn = _presenter.GetSourceConnection();
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                ClassDefinition cd = service.GetClassByName(this.SelectedSourceSchema, classNode.Name);
                if (cd != null)
                {
                    string expr = ExpressionEditor.NewExpression(conn, cd, ExpressionMode.Normal);
                    if (expr != null)
                    {
                        string alias = MessageService.ShowInputBox("Expression Alias", "Enter the alias for this expression", "MyExpression");
                        while (exprNode.Nodes[alias] != null)
                        {
                            alias = MessageService.ShowInputBox("Expression Alias", "Enter the alias for this expression", alias);
                        }
                        AddExpression(classNode.Name, alias, expr);
                    }
                }
            }
        }

        private void chkCopySpatialContexts_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.CopySpatialContextChanged();
        }

        private void removeMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode exprNode = treeMappings.SelectedNode;
            TreeNode classNode = exprNode.Parent.Parent;
            MapExpression(classNode.Name, exprNode.Name, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _presenter.SaveTask();
                ViewContentClosing(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }


        public bool GetClassDeleteOption(string className)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode deleteNode = classNode.Nodes[PREFIX_CLASS_OPTIONS].Nodes[1];
                if (deleteNode.Tag != null)
                {
                    return (bool)deleteNode.Tag;
                }
            }
            return false;
        }

        public string GetClassFilterOption(string className)
        {
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                TreeNode filterNode = classNode.Nodes[PREFIX_CLASS_OPTIONS].Nodes[0];
                if (filterNode.Tag != null)
                {
                    return filterNode.Tag.ToString();
                }
            }
            return null;
        }

        private void clearFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode filterNode = treeMappings.SelectedNode;
            filterNode.Tag = null;
            filterNode.ToolTipText = null;
            filterNode.Text = ResourceService.GetString("LBL_SOURCE_FILTER");
        }

        private void setFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode filterNode = treeMappings.SelectedNode;
            TreeNode classNode = filterNode.Parent.Parent;
            FdoConnection conn = _presenter.GetSourceConnection();
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                ClassDefinition cd = service.GetClassByName(this.SelectedSourceSchema, classNode.Name);
                if (cd != null)
                {
                    string expr = ExpressionEditor.NewExpression(conn, cd, ExpressionMode.Filter);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        filterNode.Tag = filterNode.ToolTipText = expr;
                        filterNode.Text = ResourceService.GetString("LBL_SOURCE_FILTER") + " (set)";
                    }
                }
            }
        }

        private void deleteTrueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode delNode = treeMappings.SelectedNode;
            FdoConnection conn = _presenter.GetTargetConnection();
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                if (service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Delete))
                {
                    delNode.Tag = true;
                }
                else
                {
                    MessageService.ShowMessage(ResourceService.GetString("MSG_DELETE_UNSUPPORTED"));
                    delNode.Tag = false;
                }
                delNode.Text = delNode.Name + " (" + delNode.Tag + ")";
            }
        }

        private void deleteFalseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode delNode = treeMappings.SelectedNode;
            delNode.Tag = false;
            delNode.Text = delNode.Name + " (" + delNode.Tag + ")";
        }


        public System.Collections.Specialized.NameValueCollection GetExpressions(string className)
        {
            System.Collections.Specialized.NameValueCollection exprs = new System.Collections.Specialized.NameValueCollection();
            TreeNode classNode = ClassesNode.Nodes[className];
            if (classNode != null)
            {
                foreach (TreeNode exprNode in classNode.Nodes[PREFIX_CLASS_EXPRESSION].Nodes)
                {
                    if (exprNode.Tag != null)
                    {
                        ExpressionMappingInfo map = (ExpressionMappingInfo)exprNode.Tag;
                        exprs.Add(exprNode.Name, map.Expression);
                    }
                }
            }
            return exprs;
        }
    }
}
