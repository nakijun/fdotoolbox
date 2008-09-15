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
using System.Diagnostics;
using FdoToolbox.Lib.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Common.Io;
using System.IO;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Lib.Modules;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Lib.Controls
{
    public partial class SchemaMgrCtl : FdoConnectionBoundControl
    {
        public SchemaMgrCtl()
        {
            InitializeComponent();
            _bsClasses = new BindingSource();
            _bsSchemas = new BindingSource();
        }

        private BindingSource _bsSchemas;
        private BindingSource _bsClasses;

        public event EventHandler OnSchemasApplied;

        private FeatureSchemaCollection _Schemas;
        private FeatureService _Service;

        public SchemaMgrCtl(FdoConnection conn, string key)
            : base(conn, key)
        {
            InitializeComponent();
            _bsClasses = new BindingSource();
            _bsSchemas = new BindingSource();
            _BoundConnection = conn;
            _Service = conn.CreateFeatureService();
            _Schemas = _Service.DescribeSchema();
            _bsSchemas.DataSource = _Schemas;
            lstSchemas.DataSource = _bsSchemas;

            ToggleUI();
        }

        public void SetInitialSchema(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
            {
                int idx = _Schemas.IndexOf(schemaName);
                if (idx >= 0)
                {
                    lstSchemas.SelectedIndex = idx;
                }
            }
        }

        /// <summary>
        /// Toggle ui elements based on capabilities
        /// </summary>
        private void ToggleUI()
        {
            //Supported class types
            ClassType[] ctypes = this.BoundConnection.InternalConnection.SchemaCapabilities.ClassTypes;
            featureClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_FeatureClass) >= 0;
            classNonFeatureToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_Class) >= 0;
            networkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkClass) >= 0;
            networkLayerClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLayerClass) >= 0;
            networkLinkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLinkClass) >= 0;

            //Schema modification
            btnDeleteSchema.Visible = btnDeleteClass.Visible = btnEditClass.Visible = this.BoundConnection.InternalConnection.SchemaCapabilities.SupportsSchemaModification;
        }

        private void CheckDirtyStatus()
        {
            btnApply.Enabled = false;
            foreach (FeatureSchema schema in _Schemas)
            {
                if (schema.ElementState != SchemaElementState.SchemaElementState_Unchanged)
                    btnApply.Enabled = true;
            }
        }

        private void lstSchemas_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema selectedSchema = lstSchemas.SelectedItem as FeatureSchema;
            if (selectedSchema != null)
            {
                _bsClasses.DataSource = selectedSchema.Classes;
                lstClasses.DataSource = _bsClasses;
                btnAddClass.Enabled = btnEditClass.Enabled = btnDeleteClass.Enabled = true;
                btnSaveSchema.Visible = true;
            }
            else
            {
                btnSaveSchema.Visible = false;
                btnAddClass.Enabled = btnEditClass.Enabled = btnDeleteClass.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (FeatureSchema schema in _Schemas)
            {
                schema.RejectChanges();
            }
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (_Schemas.Count > 0)
                {
                    foreach (FeatureSchema schema in _Schemas)
                    {
                        _Service.ApplySchema(schema);
                    }
                    AppConsole.Alert("Schema Management", "Schema(s) applied");
                    if (this.OnSchemasApplied != null)
                        this.OnSchemasApplied(this, EventArgs.Empty);
                    this.Close();
                }
                else
                {
                    AppConsole.Alert("Schema Management", "No schema(s) to apply. Closing");
                    this.Close();
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
            }
        }

        private void btnAddSchema_Click(object sender, EventArgs e)
        {
            bool canAdd = this.BoundConnection.InternalConnection.SchemaCapabilities.SupportsMultipleSchemas ||
                        (!this.BoundConnection.InternalConnection.SchemaCapabilities.SupportsMultipleSchemas && _bsSchemas.Count == 0);
            if (canAdd)
            {
                FeatureSchema schema = SchemaInfoDlg.NewSchema();
                if(schema != null)
                    _bsSchemas.Add(schema);
                CheckDirtyStatus();
            }
            else
            {
                AppConsole.Alert("Add Schema", "This connection does not support multiple schemas");
            }
        }

        private void btnDeleteSchema_Click(object sender, EventArgs e)
        {
            FeatureSchema schema = (FeatureSchema)lstSchemas.SelectedItem;
            _bsSchemas.Remove(schema);
            CheckDirtyStatus();
        }

        private void btnEditClass_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = (ClassDefinition)lstClasses.SelectedItem;
            if (classDef.Capabilities.SupportsWrite)
                EditClass(classDef);
            else
                AppConsole.Alert("Error", "Selected class is read-only and cannot be edited");
            CheckDirtyStatus();
        }

        private void EditClass(ClassDefinition classDef)
        {
            BaseDocumentCtl ctl = null;
            switch (classDef.ClassType)
            {
                case ClassType.ClassType_Class:
                    ctl = new ClassDefCtl((Class)classDef, this.BoundConnection);
                    break;
                case ClassType.ClassType_FeatureClass:
                    ctl = new ClassDefCtl((FeatureClass)classDef, this.BoundConnection);
                    break;
            }
            if (ctl != null)
            {
                Form frm = FormFactory.CreateFormForControl(ctl);
                frm.ShowDialog();
            }
        }

        private void btnDeleteClass_Click(object sender, EventArgs e)
        {
            _bsClasses.Remove((ClassDefinition)lstClasses.SelectedItem);
            CheckDirtyStatus();
        }

        private void featureClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewClass(ClassType.ClassType_FeatureClass);
        }

        private void classNonFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewClass(ClassType.ClassType_Class);
        }

        private void NewClass(ClassType classType)
        {
            FeatureSchema schema = (FeatureSchema)lstSchemas.SelectedItem;
            BaseDocumentCtl ctl = new ClassDefCtl(schema, classType, this.BoundConnection);
            Form frm = FormFactory.CreateFormForControl(ctl, 500, 400);
            frm.ShowDialog();
            CheckDirtyStatus();
        }

        private void SaveSchemaAsXML_Click(object sender, EventArgs e)
        {
            FeatureSchema selectedSchema = lstSchemas.SelectedItem as FeatureSchema;
            if (selectedSchema != null)
            {
                string fileName = AppGateway.RunningApplication.SaveFile("Save schema to XML", "Feature Schema Definition (*.schema)|*.schema");
                if(fileName != null)
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    selectedSchema.WriteXml(fileName);
                    AppConsole.Alert("Schema saved", "Schema saved to " + fileName);
                }
            }
        }

        private void SaveSchemaAsSDF_Click(object sender, EventArgs e)
        {
            FeatureSchema selectedSchema = lstSchemas.SelectedItem as FeatureSchema;
            if (selectedSchema != null)
            {
                string sdfFile = AppGateway.RunningApplication.SaveFile("Save schema to SDF", "SDF File (*.sdf)|*.sdf");
                if (sdfFile != null)
                {
                    try
                    {
                        IConnection conn = ExpressUtility.ApplySchemaToNewSDF(selectedSchema, sdfFile);
                        if (AppConsole.Confirm("Save Schema to SDF", "Schema saved to SDF file: " + sdfFile + ". Connect to it?"))
                        {
                            string name = AppGateway.RunningApplication.FdoConnectionManager.CreateUniqueName();
                            name = StringInputDlg.GetInput("Connection name", "Enter a name for this connection", name);
                            CoreModule.AddConnection(conn, name);
                        }
                        else
                        {
                            conn.Dispose();
                        } 
                    }
                    catch (Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                        AppConsole.WriteException(ex);
                    }
                }
            }
        }

        public override void SetName(string name)
        {
            this.BoundConnection.Name = name;
            this.Title = "Schema Management - " + this.BoundConnection.Name;
        }

        public override string GetTabType()
        {
            return CoreModule.TAB_SCHEMA_MGMT;
        }
    }
}
