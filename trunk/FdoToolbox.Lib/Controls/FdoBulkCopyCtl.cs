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
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.SpatialContext;
using FdoToolbox.Lib.Forms;
using FdoToolbox.Core.ETL;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Core.Common;
using OSGeo.FDO.Commands.Feature;
using System.Collections.ObjectModel;
using FdoToolbox.Core;
using FdoToolbox.Core.ClientServices;
#region overview
/*
 * The bulk copy control is the front-end to the BulkCopyTask class.
 * 
 * Class/Property mappings are done via a TreeView control. 
 * 
 * For each class node on the TreeView, it is assigned a dynamically populated 
 * ContextMenuStrip containing all the classes of the selected target schema. 
 * This is used to map the [source class] to the [target class]
 * 
 * When the target schema changes, this ContextMenuStrip is regenerated and all
 * mappings are reset.
 * 
 * When an option from this ContextMenuStrip is clicked, it assigns the selected
 * (class) node's Tag property with the target class's name. Anonymous delegates
 * are used to make this all work.
 * 
 * For reach property node it is initially assigned an empty ContextMenuStrip
 * until the parent (class) node has been assigned a target class. When the
 * parent (class) node has been assigned a target class, each property node
 * within is then assigned a dynamically populated ContextMenuStrip containing
 * all the mappable properties of the target class. The following types of 
 * properties are excluded from this menu.
 * 
 * 1. Auto-Generated properties
 * 2. Read-Only properties
 * 3. Raster properties
 * 4. Object properties
 * 5. Association properties
 * 
 * When an option from this ContextMenuStrip is clicked, it assigns the selected
 * (property) node's Tag property with the target property's name. Again, anonymous
 * delegates are used to make this all work.
 * 
 * When the user clicks "Save", all the class nodes are traversed. Only class and
 * property nodes whose [Tag] has been assigned are processed.
 */
#endregion
namespace FdoToolbox.Lib.Controls
{
    public partial class FdoBulkCopyCtl : BaseDocumentCtl 
    {
        private ContextMenuStrip ctxTargetClasses;

        private bool update;

        public FdoBulkCopyCtl()
        {
            InitializeComponent();
            this.Title = "Bulk Copy";
            ctxTargetClasses = new ContextMenuStrip();
            cmbSrcConn.DataSource = new List<string>(AppGateway.RunningApplication.FdoConnectionManager.GetConnectionNames());
            cmbDestConn.DataSource = new List<string>(AppGateway.RunningApplication.FdoConnectionManager.GetConnectionNames());
        }

        public FdoBulkCopyCtl(SpatialBulkCopyTask task) : this()
        {
            txtName.Text = task.Name;
            LoadSettings(task);
            update = true;
        }

        private void LoadSettings(SpatialBulkCopyTask task)
        {
            int srcidx = cmbSrcConn.Items.IndexOf(task.Options.Source.Name);
            int destidx = cmbDestConn.Items.IndexOf(task.Options.Target.Name);

            if (srcidx < 0)
                throw new TaskValidationException("Could not find source connection: " + task.Options.Source.Name);

            if (destidx < 0)
                throw new TaskValidationException("Could not find target connection: " + task.Options.Target.Name);

            cmbSrcConn.SelectedIndex = srcidx;
            cmbDestConn.SelectedIndex = destidx;

            numBatchSize.Value = task.Options.BatchInsertSize;

            cmbSrcSchema.SelectedText = task.Options.SourceSchemaName;
            cmbDestSchema.SelectedText = task.Options.TargetSchemaName;

            //Why is selected index changed not firing?????
            cmbSrcSchema_SelectedIndexChanged(this, EventArgs.Empty);

            txtGlobalFilter.Text = task.Options.GlobalSpatialFilter;
            chkCopySpatialContexts.Checked = task.Options.CopySpatialContexts;
            if (task.Options.CopySpatialContexts)
            {
                foreach (string name in task.Options.SourceSpatialContexts)
                {
                    int idx = chkSourceContextList.Items.IndexOf(name);
                    if (idx >= 0)
                        chkSourceContextList.SetItemChecked(idx, true);
                }
            }
            ReadOnlyCollection<ClassCopyOptions> copts = task.Options.ClassCopyOptions;
            foreach (ClassCopyOptions classCopyOption in copts)
            {
                //Map the class node
                TreeNode root = GetRootNode();
                TreeNode classNode = root.Nodes[classCopyOption.ClassName];
                MapClassNode(classCopyOption.TargetClassName, classNode);
                TreeNode optionsNode = classNode.Nodes[IDX_OPTIONS];
                TreeNode propertiesNode = classNode.Nodes[IDX_PROPERTIES];
                //Now map its properties
                ICollection<string> srcProperties = classCopyOption.PropertyNames;
                foreach (string sourceProp in srcProperties)
                {
                    string key = PREFIX_PROPERTY + sourceProp;
                    TreeNode propertyNode = propertiesNode.Nodes[key];
                    string targetProp = classCopyOption.GetTargetPropertyName(sourceProp);

                    MapPropertyNode(propertyNode, sourceProp, targetProp);
                }
                if (!string.IsNullOrEmpty(classCopyOption.AttributeFilter))
                {
                    optionsNode.Nodes[IDX_OPTION_FILTER].Tag = classCopyOption.AttributeFilter;
                    optionsNode.Nodes[IDX_OPTION_FILTER].Text = "Filter (set)";
                }
                TreeNode deleteNode = optionsNode.Nodes[IDX_OPTION_DELETE];
                deleteNode.Tag = classCopyOption.DeleteClassData;
                deleteNode.Text = deleteNode.Name + " (" + deleteNode.Tag + ")";
            }
        }

        private void cmbSrcConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbSrcConn.SelectedItem.ToString();
            FdoConnection conn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(connName);
            FeatureService service = conn.CreateFeatureService();
            FeatureSchemaCollection schemas = service.DescribeSchema();
            if(schemas.Count == 0)
            {
                AppConsole.Alert("Error", "The source connection is not a valid bulk copy source");
                btnSave.Enabled = false;
            }
            else
            {
                chkSourceContextList.Items.Clear();
                ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                foreach (SpatialContextInfo ctx in contexts)
                {
                    chkSourceContextList.Items.Add(ctx.Name, false);
                }
                btnSave.Enabled = true;
                cmbSrcSchema.DataSource = schemas;
            }
        }

        private void cmbDestConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbDestConn.SelectedItem.ToString();
            FdoConnection conn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(connName);
            using (FeatureService service = conn.CreateFeatureService())
            {
                FeatureSchemaCollection schemas = service.DescribeSchema();
                cmbDestSchema.DataSource = schemas;
                if (schemas.Count == 0)
                {
                    AppConsole.Alert("Warning", "There are no schemas in the target connection. If you save this task, all source classes will be copied");
                    ctxTargetClasses.Items.Clear();
                    ResetClassNodes();
                }
                // There is no actual capability atm to indicate support for batch insertion
                // 
                // So we'll only enable the batch size field under the two conditions:
                //  1. Connection support parameters
                //  2. IInserts created from this connection have a non-null BatchParameterValueCollection
                numBatchSize.Enabled = service.SupportsBatchInsertion();
            }
        }

        private void cmbSrcSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbSrcSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                PopulateClassNodes(schema.Classes);
            }
        }

        private TreeNode GetRootNode() { return mTreeView.Nodes["NODE_CLASSES"]; }

        const int IDX_OPTIONS = 0;
        const int IDX_PROPERTIES = 1;

        const int IDX_OPTION_FILTER = 0;
        const int IDX_OPTION_DELETE = 1;

        /// <summary>
        /// Fill the tree view with class nodes
        /// </summary>
        /// <param name="classCollection"></param>
        private void PopulateClassNodes(ClassCollection classCollection)
        {
            GetRootNode().Nodes.Clear();
            foreach (ClassDefinition classDef in classCollection)
            {
                TreeNode classNode = new TreeNode();
                classNode.Name = classDef.Name;
                classNode.Text = classDef.Name + " (unmapped)";
                classNode.Checked = false;
                classNode.ContextMenuStrip = ctxTargetClasses;

                TreeNode propertiesNode = new TreeNode();
                propertiesNode.Name = "Properties";
                propertiesNode.Text = "Properties";
                PopulatePropertyNodes(propertiesNode, classDef);

                TreeNode optionsNode = new TreeNode();
                optionsNode.Name = "Options";
                optionsNode.Text = "Options";
                PopulateOptionNodes(optionsNode);

                classNode.Nodes.Add(optionsNode);
                classNode.Nodes.Add(propertiesNode);

                GetRootNode().Nodes.Add(classNode);
            }
            GetRootNode().ExpandAll();
        }

        private void PopulateOptionNodes(TreeNode optionsNode)
        {
            TreeNode filterNode = new TreeNode();
            filterNode.Name = filterNode.Text = "Filter";
            filterNode.ToolTipText = "Apply an attribute filter to this (source) class";
            filterNode.ContextMenuStrip = ctxClassFilter;

            TreeNode deleteNode = new TreeNode();
            deleteNode.Name = "Delete before copy";
            deleteNode.ToolTipText = "Deletes all data in the target class before copying (may take a while)";
            deleteNode.Tag = false;
            deleteNode.Text = deleteNode.Name + " (" + deleteNode.Tag + ")";
            deleteNode.ContextMenuStrip = ctxDeleteBeforeCopy;

            optionsNode.Nodes.Add(filterNode);
            optionsNode.Nodes.Add(deleteNode);
        }

        private void setFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode filterNode = mTreeView.SelectedNode;
            string name = cmbSrcConn.SelectedItem.ToString();
            string schemaName = (cmbSrcSchema.SelectedItem as FeatureSchema).Name;
            string className = filterNode.Parent.Parent.Name;
            FdoConnection connInfo = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(name);
            FeatureService service = connInfo.CreateFeatureService();
            ClassDefinition classDef = service.GetClassByName(schemaName, className);
            if (classDef != null)
            {
                if (filterNode.Tag != null)
                {   
                    string expr = ExpressionDlg.EditExpression(connInfo, classDef, filterNode.Tag.ToString(), ExpressionMode.Filter);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        filterNode.Tag = expr;
                        filterNode.Text = "Filter (set)";
                    }
                    else
                    {
                        filterNode.Tag = null;
                        filterNode.Text = filterNode.Name;
                    }
                }
                else
                {
                    string expr = ExpressionDlg.NewExpression(connInfo, classDef, ExpressionMode.Filter);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        filterNode.Tag = expr;
                        filterNode.Text = "Filter (set)";
                    }
                    else
                    {
                        filterNode.Tag = null;
                        filterNode.Text = filterNode.Name;
                    }
                }
            }
        }

        const string PREFIX_PROPERTY = "PROP";

        private static void PopulatePropertyNodes(TreeNode classNode, ClassDefinition classDef)
        {
            foreach (PropertyDefinition property in classDef.Properties)
            {
                TreeNode node = new TreeNode();
                node.Name = PREFIX_PROPERTY + property.Name;
                node.Text = property.Name + " (unmapped)";
                string tooltip = "Property Type: " + property.PropertyType;
                if (property.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    tooltip += "\nData Type: " + (property as DataPropertyDefinition).DataType;
                }
                node.ToolTipText = tooltip;
                classNode.Nodes.Add(node);
            }
        }

        private void cmbDestSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctxTargetClasses.Items.Clear();
            ResetClassNodes();
            FeatureSchema schema = cmbDestSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {   
                foreach (ClassDefinition classDef in schema.Classes)
                {
                    ToolStripItem tsi = new ToolStripMenuItem();
                    string className = classDef.Name;
                    tsi.Name = className;
                    tsi.Text = "Map class to: " + className;
                    tsi.Click += delegate(object obj, EventArgs evt)
                    {
                        TreeNode classNode = mTreeView.SelectedNode;
                        MapClassNode(className, classNode);
                    };
                    ctxTargetClasses.Items.Add(tsi);
                }
            }
        }

        private void MapClassNode(string className, TreeNode classNode)
        {
            if (classNode != null)
            {
                classNode.Tag = className;
                classNode.Text = classNode.Name + " (=> " + className + ")";
                ResetPropertyNodes(classNode, className);
            }
        }

        /// <summary>
        /// Remove all tags from all class/property/option nodes
        /// </summary>
        private void ResetClassNodes()
        {
            foreach (TreeNode classNode in GetRootNode().Nodes)
            {
                TreeNode propertiesNode = classNode.Nodes[IDX_PROPERTIES];
                TreeNode optionsNode = classNode.Nodes[IDX_OPTIONS];
                classNode.Tag = null;
                classNode.Text = classNode.Name + " (unmapped)";
                foreach (TreeNode propNode in propertiesNode.Nodes)
                {
                    if (propNode.Name.StartsWith(PREFIX_PROPERTY))
                    {
                        propNode.Tag = null;
                        propNode.Text = GetPropertyName(propNode) + " (unmapped)";
                    }
                }
                foreach (TreeNode optNode in optionsNode.Nodes)
                {
                    optNode.Tag = null;
                    optNode.Text = optionsNode.Name;
                }
            }
        }

        /// <summary>
        /// Reset the context menu for the class node's property nodes
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="className"></param>
        private void ResetPropertyNodes(TreeNode sourceClassNode, string targetClassName)
        {
            TreeNode propertiesNode = sourceClassNode.Nodes[IDX_PROPERTIES];
            ClassDefinition targetClassDef = GetTargetClassByName(targetClassName);
            ContextMenuStrip ctxProperties = new ContextMenuStrip();
            ToolStripItem resetItem = new ToolStripMenuItem();
            resetItem.Text = "Remove Mapping";
            resetItem.Image = Properties.Resources.cross;
            resetItem.Click += delegate(object sender, EventArgs e)
            {
                TreeNode propNode = mTreeView.SelectedNode;
                propNode.Tag = null;
                propNode.Text = GetPropertyName(propNode) + " (unmapped)";
            };

            ctxProperties.Items.Add(resetItem);
            ctxProperties.Items.Add(new ToolStripSeparator());

            foreach (TreeNode node in propertiesNode.Nodes)
            {
                node.Tag = null;
            }
            foreach (PropertyDefinition property in targetClassDef.Properties)
            {
                DataPropertyDefinition dp = property as DataPropertyDefinition;
                // Skip the following:
                // - Auto-Generated properties
                // - Read-Only properties
                // - Raster properties
                // - Object properties
                // - Association properties
                if (dp != null)
                {
                    if (dp.IsAutoGenerated)
                        continue;
                    if (dp.ReadOnly)
                        continue;
                }
                else if (
                    (property.PropertyType == PropertyType.PropertyType_RasterProperty) ||
                    (property.PropertyType == PropertyType.PropertyType_AssociationProperty) ||
                    (property.PropertyType == PropertyType.PropertyType_ObjectProperty)
                )
                {
                    continue;
                }
                else if (property.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    if ((property as GeometricPropertyDefinition).ReadOnly)
                        continue;
                }
                ToolStripItem tsi = new ToolStripMenuItem();
                if (dp != null)
                {
                    tsi.Text = "Map property to: " + property.Name + " (" + dp.DataType + ")";
                }
                else
                {
                    tsi.Text = "Map property to: " + property.Name;
                }
                string srcClassName = sourceClassNode.Name;
                string destClassName = targetClassDef.Name;
                string newPropertyName = property.Name;
                switch (property.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        tsi.Image = Properties.Resources.table;
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        tsi.Image = Properties.Resources.shape_handles;
                        break;
                    /*
                    case PropertyType.PropertyType_RasterProperty:
                        tsi.Image = Properties.Resources.image;
                        break;
                    case PropertyType.PropertyType_ObjectProperty:
                        tsi.Image = Properties.Resources.package;
                        break;
                    case PropertyType.PropertyType_AssociationProperty:
                        tsi.Image = Properties.Resources.table_relationship;
                        break;
                     */
                }
                tsi.Click += delegate(object sender, EventArgs e)
                {
                    TreeNode propertyNode = mTreeView.SelectedNode;
                    string propertyName = GetPropertyName(propertyNode);
                    string reason = string.Empty;
                    if (IsMappable(srcClassName, propertyName, destClassName, newPropertyName, ref reason))
                    {
                        //Use the Tag property to store the mapped property name
                        MapPropertyNode(propertyNode, propertyName, newPropertyName);
                    }
                    else
                    {
                        AppConsole.Alert("Cannot map property", reason);
                    }
                };
                ctxProperties.Items.Add(tsi);
            }
            foreach (TreeNode node in propertiesNode.Nodes)
            {
                if (node.Name.StartsWith(PREFIX_PROPERTY))
                {
                    node.ContextMenuStrip = ctxProperties;
                }
            }
        }

        private static void MapPropertyNode(TreeNode propertyNode, string propertyName, string newPropertyName)
        {
            if (propertyNode != null)
            {
                propertyNode.Tag = newPropertyName;
                propertyNode.Text = propertyName + " (=> " + newPropertyName + ")";
            }
        }

        /// <summary>
        /// Checks if a source property can be mapped to a target property
        /// </summary>
        /// <param name="srcClassName"></param>
        /// <param name="srcProperty"></param>
        /// <param name="destClassName"></param>
        /// <param name="destProperty"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private bool IsMappable(string srcClassName, string srcProperty, string destClassName, string destProperty, ref string reason)
        {
            // [source] is mappable to [target] if any of the following is true
            // 1. They are of the same property type
            // 2. If they are data properties, they are of the same data type.
            ClassDefinition srcDef = GetSourceClassByName(srcClassName);
            ClassDefinition destDef = GetTargetClassByName(destClassName);

            if (srcDef != null && destDef != null)
            {
                PropertyDefinition srcPropDef = srcDef.Properties[srcDef.Properties.IndexOf(srcProperty)];
                PropertyDefinition destPropDef = destDef.Properties[destDef.Properties.IndexOf(destProperty)];

                if (srcPropDef.PropertyType == destPropDef.PropertyType)
                {
                    if (srcPropDef.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        DataType srcDataType = (srcPropDef as DataPropertyDefinition).DataType;
                        DataType destDataType = (destPropDef as DataPropertyDefinition).DataType;

                        if (SpatialBulkCopyTask.IsConvertable(srcDataType, destDataType))
                        {
                            return true;
                        }
                        else
                        {
                            //TODO: For type coercion feature to be implemented, be more lenient 
                            //here and allow for certain data type combinations to work
                            reason = "source data type cannot be converted to target data type";
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    reason = "source and target properties are different types";
                    return false;
                }
            }
            else
            {
                reason = "Unable to find source and/or target properties";
                return false;
            }
        }

        /// <summary>
        /// Get the property name from a property tree node
        /// </summary>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        private static string GetPropertyName(TreeNode propertyNode)
        {
            if (propertyNode.Name.StartsWith(PREFIX_PROPERTY))
                return propertyNode.Name.Substring(PREFIX_PROPERTY.Length);
            return null;
        }

        private void mTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider.SetError(txtName, "Required");
                return;
            }

            string srcConnName = cmbSrcConn.SelectedItem.ToString();
            string destConnName = cmbDestConn.SelectedItem.ToString();
            FdoConnection srcConn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(srcConnName);
            FdoConnection destConn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(destConnName);
            if (!destConn.InternalConnection.ConnectionCapabilities.SupportsMultipleSpatialContexts())
            {
                if (chkSourceContextList.CheckedItems.Count > 1)
                {
                    AppConsole.Alert("Spatial Contexts", "Target connection doesn't support multiple spatial contexts\n\nTherefore, please select at most ONE spatial context from the list");
                    return;
                }
            }
            
            SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(
                srcConn,
                destConn
            );
            if (numBatchSize.Enabled)
                options.BatchInsertSize = Convert.ToInt32(numBatchSize.Value);
            if (!string.IsNullOrEmpty(txtGlobalFilter.Text.Trim()))
                options.GlobalSpatialFilter = txtGlobalFilter.Text.Trim();
            options.CoerceDataTypes = chkCoerceDataTypes.Checked;
            options.CopySpatialContexts = chkCopySpatialContexts.Checked;
            if (options.CopySpatialContexts)
            {
                foreach (object item in chkSourceContextList.CheckedItems)
                {
                    options.AddSourceSpatialContext(item.ToString());
                }
            }
            foreach (TreeNode classNode in GetRootNode().Nodes)
            {
                //A tag set on the class node indicates this class is to be copied
                if (classNode.Tag != null)
                {
                    TreeNode optionsNode = classNode.Nodes[IDX_OPTIONS];
                    TreeNode propertiesNode = classNode.Nodes[IDX_PROPERTIES];

                    ClassCopyOptions cOptions = new ClassCopyOptions(GetSourceClassByName(classNode.Name.ToString()));
                    cOptions.TargetClassName = classNode.Tag.ToString();
                    foreach (TreeNode propertyNode in propertiesNode.Nodes)
                    {
                        if (propertyNode.Name.StartsWith(PREFIX_PROPERTY))
                        {
                            //A tag set on the property node indicates this property
                            //is to be mapped
                            if (propertyNode.Tag != null)
                            {
                                string srcPropertyName = GetPropertyName(propertyNode);
                                string destPropertyName = propertyNode.Tag.ToString();

                                PropertyDefinition propDef = cOptions.SourceClassDefinition.Properties[cOptions.SourceClassDefinition.Properties.IndexOf(srcPropertyName)];
                                cOptions.AddProperty(propDef, destPropertyName);
                            }
                        }
                    }

                    TreeNode filterNode = optionsNode.Nodes[IDX_OPTION_FILTER];
                    TreeNode deleteNode = optionsNode.Nodes[IDX_OPTION_DELETE];
                    if (filterNode.Tag != null)
                        cOptions.AttributeFilter = filterNode.Tag.ToString();
                    cOptions.DeleteClassData = Convert.ToBoolean(deleteNode.Tag);
                    options.AddClassCopyOption(cOptions);
                }
            }
            FeatureSchema srcSchema = cmbSrcSchema.SelectedItem as FeatureSchema;
            options.SourceSchemaName = srcSchema.Name;
            FeatureSchema destSchema = (cmbDestSchema.SelectedItem as FeatureSchema);
            if (destSchema != null)
                options.TargetSchemaName = destSchema.Name;

            //If copy full source schema, check that it can be applied to target.
            if (options.ApplySchemaToTarget)
            {
                using (FeatureService destService = destConn.CreateFeatureService())
                {
                    IncompatibleSchema incSchema = null;
                    if (!destService.CanApplySchema(srcSchema, out incSchema))
                    {
                        if (AppConsole.Confirm("Incompatible schema", "The source schema has incompatible elements:\n\n" + incSchema.ToString() + "\nThe source schema will be altered to be compatible with the target connection. Proceed?"))
                        {
                            try
                            {
                                FeatureSchema alteredSchema = destService.AlterSchema(srcSchema, incSchema);
                                if (alteredSchema != null)
                                {
                                    //Alter works. So do it again when task executes.
                                    options.AlterSchema = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                AppConsole.Alert("Alter Schema Error", ex.Message + "\nCannot save this task");
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
           
            SpatialBulkCopyTask bcptask = new SpatialBulkCopyTask(txtName.Text, options);
            if(update)
            {
                ITask task = AppGateway.RunningApplication.TaskManager.GetTask(txtName.Text);
                if (task != null)
                    AppGateway.RunningApplication.TaskManager.UpdateTask(txtName.Text, bcptask);
                else
                    AppGateway.RunningApplication.TaskManager.AddTask(bcptask);
            }
            else
            {
                AppGateway.RunningApplication.TaskManager.AddTask(bcptask);
            }
            this.Close();
        }

        private ClassDefinition GetTargetClassByName(string className)
        {
            FeatureSchema schema = (FeatureSchema)cmbDestSchema.SelectedItem;
            return schema.Classes[schema.Classes.IndexOf(className)]; 
        }

        private ClassDefinition GetSourceClassByName(string className)
        {
            FeatureSchema schema = (FeatureSchema)cmbSrcSchema.SelectedItem;
            return schema.Classes[schema.Classes.IndexOf(className)];
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.Title = "Bulk Copy - " + txtName.Text;
        }

        private void chkCopySpatialContexts_CheckedChanged(object sender, EventArgs e)
        {
            chkSourceContextList.Enabled = chkCopySpatialContexts.Checked;
            if (chkCopySpatialContexts.Checked)
            {
                string connName = cmbDestConn.SelectedItem.ToString();
                FdoConnection conn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(connName);
                if (!conn.InternalConnection.ConnectionCapabilities.SupportsMultipleSpatialContexts())
                {
                    AppConsole.Alert("Warning", "The target connection does not support multiple spatial contexts\nSelect at most ONE spatial context from the list\nThe target spatial context and any data stored in that context will also be destroyed!");
                }
            }
        }

        private void DeleteBeforeCopyEnable_Click(object sender, EventArgs e)
        {
            string name = cmbDestConn.SelectedItem.ToString();
            FdoConnection conn = AppGateway.RunningApplication.FdoConnectionManager.GetConnection(name);
            using (FeatureService service = conn.CreateFeatureService())
            {
                TreeNode delNode = mTreeView.SelectedNode;
                if (service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Delete))
                {
                    delNode.Tag = true;
                }
                else
                {
                    AppConsole.Alert("Unsupported", "The target connection does not support deleting");
                    delNode.Tag = false;
                }
                delNode.Text = delNode.Name + " (" + delNode.Tag + ")";
            }
        }

        private void DeleteBeforeCopyDisable_Click(object sender, EventArgs e)
        {
            TreeNode delNode = mTreeView.SelectedNode;
            delNode.Tag = false;
            delNode.Text = delNode.Name + " (" + delNode.Tag + ")";
        }
    }
}
