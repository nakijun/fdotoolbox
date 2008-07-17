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
namespace FdoToolbox.Core.Controls
{
    public partial class BulkCopyCtl : BaseDocumentCtl 
    {
        private ContextMenuStrip ctxTargetClasses;

        public BulkCopyCtl()
        {
            InitializeComponent();
            this.Title = "Bulk Copy";
            ctxTargetClasses = new ContextMenuStrip();
            cmbSrcConn.DataSource = new List<string>(HostApplication.Instance.ConnectionManager.GetConnectionNames());
            cmbDestConn.DataSource = new List<string>(HostApplication.Instance.ConnectionManager.GetConnectionNames());
        }

        private void cmbSrcConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbSrcConn.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connName);
            using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = cmd.Execute();
                if(schemas.Count == 0)
                {
                    AppConsole.Alert("Error", "The source connection is not a valid bulk copy source");
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                    cmbSrcSchema.DataSource = schemas;
                }
            }
        }

        private void cmbDestConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connName = cmbDestConn.SelectedItem.ToString();
            IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connName);
            using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = cmd.Execute();
                cmbDestSchema.DataSource = schemas;
                if (schemas.Count == 0)
                {
                    AppConsole.Alert("Warning", "There are no schemas in the target connection. If you save this task, all source classes will be copied");
                    ctxTargetClasses.Items.Clear();
                    ResetClassNodes();
                }
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
                PopulatePropertyNodes(classNode, classDef);
                GetRootNode().Nodes.Add(classNode);
            }
            GetRootNode().ExpandAll();
        }

        const string PREFIX_PROPERTY = "PROP";

        private void PopulatePropertyNodes(TreeNode classNode, ClassDefinition classDef)
        {
            foreach (PropertyDefinition property in classDef.Properties)
            {
                TreeNode node = new TreeNode();
                node.Name = PREFIX_PROPERTY + property.Name;
                node.Text = property.Name + " (unmapped)";
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
                        classNode.Tag = className;
                        classNode.Text = classNode.Name + " (=> " + className + ")";
                        ResetPropertyNodes(classNode, tsi.Name);
                    };
                    ctxTargetClasses.Items.Add(tsi);
                }
            }
        }

        /// <summary>
        /// Remove all tags from all class/property nodes
        /// </summary>
        private void ResetClassNodes()
        {
            foreach (TreeNode classNode in GetRootNode().Nodes)
            {
                classNode.Tag = null;
                classNode.Text = classNode.Name + " (unmapped)";
                foreach (TreeNode propNode in classNode.Nodes)
                {
                    if (propNode.Name.StartsWith(PREFIX_PROPERTY))
                    {
                        propNode.Tag = null;
                        propNode.Text = GetPropertyName(propNode) + " (unmapped)";
                    }
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
            ClassDefinition targetClassDef = GetTargetClassByName(targetClassName);
            ContextMenuStrip ctxProperties = new ContextMenuStrip();
            
            foreach (TreeNode node in sourceClassNode.Nodes)
            {
                node.Tag = null;
            }
            foreach (PropertyDefinition property in targetClassDef.Properties)
            {
                // Skip the following:
                // - Auto-Generated properties
                // - Read-Only properties
                // - Raster properties
                // - Object properties
                // - Association properties
                if (property.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    if ((property as DataPropertyDefinition).IsAutoGenerated)
                        continue;
                    if ((property as DataPropertyDefinition).ReadOnly)
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
                tsi.Text = "Map property to: " + property.Name;
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
                        propertyNode.Tag = newPropertyName;
                        propertyNode.Text = propertyName + "(=> " + newPropertyName + ")";
                    }
                    else
                    {
                        AppConsole.Alert("Cannot map property", reason);
                    }
                };
                ctxProperties.Items.Add(tsi);
            }
            foreach (TreeNode node in sourceClassNode.Nodes)
            {
                if (node.Name.StartsWith(PREFIX_PROPERTY))
                {
                    node.ContextMenuStrip = ctxProperties;
                }
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

                        if (srcDataType == destDataType)
                        {
                            return true;
                        }
                        else
                        {
                            //TODO: For type coercion feature to be implemented, be more lenient 
                            //here and allow for certain data type combinations to work
                            reason = "source and target properties are of different data types";
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
        private string GetPropertyName(TreeNode propertyNode)
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
            BulkCopyOptions options = new BulkCopyOptions(
                new ConnectionInfo(
                    srcConnName,
                    HostApplication.Instance.ConnectionManager.GetConnection(srcConnName)
                ),
                new ConnectionInfo(
                    destConnName,
                    HostApplication.Instance.ConnectionManager.GetConnection(destConnName)
                )
            );
            options.CoerceDataTypes = chkCoerceDataTypes.Checked;
            options.CopySpatialContexts = chkCopySpatialContexts.Checked;
            foreach (TreeNode classNode in GetRootNode().Nodes)
            {
                //A tag set on the class node indicates this class is to be copied
                if (classNode.Tag != null)
                {
                    ClassCopyOptions cOptions = new ClassCopyOptions(GetSourceClassByName(classNode.Tag.ToString()));
                    cOptions.TargetClassName = classNode.Tag.ToString();
                    foreach (TreeNode propertyNode in classNode.Nodes)
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
                    options.AddClassCopyOption(cOptions);
                }
            }
            options.SourceSchemaName = (cmbSrcSchema.SelectedItem as FeatureSchema).Name;
            FeatureSchema destSchema = (cmbDestSchema.SelectedItem as FeatureSchema);
            if (destSchema != null)
                options.TargetSchemaName = destSchema.Name;
           
            BulkCopyTask task = new BulkCopyTask(txtName.Text, options);
            HostApplication.Instance.TaskManager.AddTask(task);
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
            if (chkCopySpatialContexts.Checked)
            {
                string connName = cmbDestConn.SelectedItem.ToString();
                IConnection conn = HostApplication.Instance.ConnectionManager.GetConnection(connName);
                if (!conn.ConnectionCapabilities.SupportsMultipleSpatialContexts())
                {
                    AppConsole.Alert("Warning", "The target connection does not support multiple spatial contexts\nWhen the bulk copy begins you will be prompted for the spatial context to copy\nThe target spatial context and any data stored in that context will also be destroyed!");
                }
            }
        }
    }
}
