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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Forms
{
    public partial class PartialSchemaSaveDialog : Form
    {
        const int IDX_CLASS = 0;
        const int IDX_FEATURECLASS = 1;
        const int IDX_KEY = 2;
        const int IDX_DATA = 3;
        const int IDX_GEOMETRY = 4;
        const int IDX_OBJECT = 5;
        const int IDX_ASSOCIATION = 6;
        const int IDX_RASTER = 7;

        private PartialSchemaSaveDialog()
        {
            InitializeComponent();
        }

        private FeatureSchema _schema;

        public PartialSchemaSaveDialog(FeatureSchema schema)
            : this()
        {
            _schema = schema;
            InitTree();
        }

        protected override void OnLoad(EventArgs e)
        {
            btnSave.Enabled = CanSave();
            base.OnLoad(e);
        }

        private void InitTree()
        {
            foreach (ClassDefinition cls in _schema.Classes)
            {
                TreeNode clsNode = new TreeNode();
                clsNode.Name = cls.Name;
                clsNode.Text = cls.Name;
                clsNode.Checked = true;

                if (cls.ClassType == ClassType.ClassType_FeatureClass)
                    clsNode.ImageIndex = clsNode.SelectedImageIndex = IDX_FEATURECLASS;
                else
                    clsNode.ImageIndex = clsNode.SelectedImageIndex = IDX_CLASS;

                foreach (PropertyDefinition pd in cls.Properties)
                {
                    TreeNode propNode = new TreeNode();
                    propNode.Name = pd.Name;
                    propNode.Text = pd.Name;
                    propNode.Checked = true;

                    switch (pd.PropertyType)
                    {
                        case PropertyType.PropertyType_AssociationProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_ASSOCIATION;
                            break;
                        case PropertyType.PropertyType_DataProperty:
                            if (cls.IdentityProperties.Contains(pd.Name))
                                propNode.ImageIndex = propNode.SelectedImageIndex = IDX_KEY;
                            else
                                propNode.ImageIndex = propNode.SelectedImageIndex = IDX_DATA;
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_GEOMETRY;
                            break;
                        case PropertyType.PropertyType_ObjectProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_OBJECT;
                            break;
                        case PropertyType.PropertyType_RasterProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_RASTER;
                            break;
                    }

                    clsNode.Nodes.Add(propNode);
                }

                treeSchema.Nodes.Add(clsNode);
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            CheckAll(treeSchema.Nodes, true);
        }

        private static void CheckAll(TreeNodeCollection nodes, bool state)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = state;
                if (node.Nodes.Count > 0)
                {
                    CheckAll(node.Nodes, state);
                }
            }
        }

        private void btnCheckNone_Click(object sender, EventArgs e)
        {
            CheckAll(treeSchema.Nodes, false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Operate on a clone
            FeatureSchema schema = FdoFeatureService.CloneSchema(_schema);

            //Remove elements that have been unchecked. 
            foreach (TreeNode clsNode in treeSchema.Nodes)
            {
                string className = clsNode.Name;
                int index = schema.Classes.IndexOf(className);
                if (!clsNode.Checked)
                {
                    if (index >= 0)
                        schema.Classes.RemoveAt(index);
                }
                else
                {
                    if (index >= 0)
                    {
                        ClassDefinition clsDef = schema.Classes[index];
                        foreach (TreeNode propNode in clsNode.Nodes)
                        {
                            if (!propNode.Checked)
                            {
                                string propName = propNode.Text;
                                int pidx = clsDef.Properties.IndexOf(propName);
                                if (pidx >= 0)
                                {
                                    clsDef.Properties.RemoveAt(pidx);
                                    if (clsDef.IdentityProperties.Contains(propName))
                                    {
                                        int idpdx = clsDef.IdentityProperties.IndexOf(propName);
                                        clsDef.IdentityProperties.RemoveAt(idpdx);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (rdXml.Checked)
            {
                schema.WriteXml(txtXml.Text);
                MessageService.ShowMessage("Schema saved to: " + txtXml.Text);
                this.DialogResult = DialogResult.OK;
            }
            else if (rdFile.Checked)
            {
                string fileName = txtFile.Text;
                if (ExpressUtility.CreateFlatFileDataSource(fileName))
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection(fileName);
                    bool disposeConn = true;
                    using (FdoFeatureService svc = conn.CreateFeatureService())
                    {
                        svc.ApplySchema(schema);
                        if (MessageService.AskQuestion("Schema saved to: " + txtFile.Text + " connect to it?", "Saved"))
                        {
                            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                            string name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_CONNECTION_NAME"), ResourceService.GetString("PROMPT_ENTER_CONNECTION"), "");
                            if (name == null)
                                return;

                            while (name == string.Empty || mgr.NameExists(name))
                            {
                                MessageService.ShowError(ResourceService.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                                name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_CONNECTION_NAME"), ResourceService.GetString("PROMPT_ENTER_CONNECTION"), name);

                                if (name == null)
                                    return;
                            }
                            disposeConn = false;
                            mgr.AddConnection(name, conn);
                        }
                    }
                    if (disposeConn)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool CanSave()
        {
            if (rdXml.Checked)
            {
                return !string.IsNullOrEmpty(txtXml.Text);
            }
            else if (rdFile.Checked)
            {
                return !string.IsNullOrEmpty(txtFile.Text);
            }
            return false;
        }

        private void treeSchema_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //Uncheck all children if unchecking class node
            if (!e.Node.Checked)
            {
                if (e.Node.Level == 0) //Class
                {
                    foreach (TreeNode pNode in e.Node.Nodes)
                    {
                        pNode.Checked = false;
                    }
                }
            }
            else 
            {
                if (e.Node.Level == 1 && !e.Node.Parent.Checked) //Property
                {
                    e.Node.Parent.Checked = true;   
                }
            }

            btnSave.Enabled = CanSave();
        }

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "FDO Feature Schemas (*.schema)|*.schema";
            diag.FileName = _schema.Name + ".schema";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                txtXml.Text = diag.FileName;
            }
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "SDF Files (*.sdf)|*.sdf|SQLite files (*.db;*.sqlite;*.slt)|*.db;*.sqlite;*.slt";
            diag.FileName = _schema.Name;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = diag.FileName;
            }
        }

        private void txtXml_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void rdXml_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void rdFile_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }
    }
}