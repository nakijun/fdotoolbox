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
using FdoToolbox.Core.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Common.Io;
using System.IO;

namespace FdoToolbox.Core.Controls
{
    public partial class SchemaMgrCtl : BaseDocumentCtl, IConnectionBoundCtl
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

        private ConnectionInfo _BoundConnection;

        private FeatureSchemaCollection _Schemas;
        private FeatureService _Service;

        public SchemaMgrCtl(ConnectionInfo conn)
            : this()
        {
            _BoundConnection = conn;
            _Service = HostApplication.Instance.ConnectionManager.CreateService(_BoundConnection.Name);
            _Schemas = _Service.DescribeSchema();
            _bsSchemas.DataSource = _Schemas;
            lstSchemas.DataSource = _bsSchemas;

            ToggleUI();
        }

        /// <summary>
        /// Toggle ui elements based on capabilities
        /// </summary>
        private void ToggleUI()
        {
            //Supported class types
            ClassType[] ctypes = this.BoundConnection.Connection.SchemaCapabilities.ClassTypes;
            featureClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_FeatureClass) >= 0;
            classNonFeatureToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_Class) >= 0;
            networkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkClass) >= 0;
            networkLayerClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLayerClass) >= 0;
            networkLinkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLinkClass) >= 0;

            //Schema modification
            btnDeleteSchema.Visible = btnDeleteClass.Visible = btnEditClass.Visible = this.BoundConnection.Connection.SchemaCapabilities.SupportsSchemaModification;
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
                    this.Accept();
                }
                else
                {
                    AppConsole.Alert("Schema Management", "No schema(s) to apply. Closing");
                    this.Cancel();
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
            }
        }

        private void btnAddSchema_Click(object sender, EventArgs e)
        {
            bool canAdd = this.BoundConnection.Connection.SchemaCapabilities.SupportsMultipleSchemas ||
                        (!this.BoundConnection.Connection.SchemaCapabilities.SupportsMultipleSchemas && _bsSchemas.Count == 0);
            if (canAdd)
            {
                FeatureSchema schema = SchemaInfoDlg.NewSchema();
                _bsSchemas.Add(schema);
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
        }

        private void btnEditClass_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = (ClassDefinition)lstClasses.SelectedItem;
            if (classDef.Capabilities.SupportsWrite)
                EditClass(classDef);
            else
                AppConsole.Alert("Error", "Selected class is read-only and cannot be edited");
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
        }

        public ConnectionInfo BoundConnection
        {
            get { return _BoundConnection; }
        }

        private void SaveSchemaAsXML_Click(object sender, EventArgs e)
        {
            FeatureSchema selectedSchema = lstSchemas.SelectedItem as FeatureSchema;
            if (selectedSchema != null)
            {
                string fileName = HostApplication.Instance.SaveFile("Save schema to XML", "Feature Schema Definition (*.schema)|*.schema");
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
                string sdfFile = HostApplication.Instance.SaveFile("Save schema to SDF", "SDF File (*.sdf)|*.sdf");
                if(sdfFile != null)
                    ExpressUtility.ApplySchemaToNewSDF(selectedSchema, sdfFile);
            }
        }

        public void SetName(string name)
        {
            this.BoundConnection.Name = name;
            this.Title = "Schema Management - " + this.BoundConnection.Name;
        }
    }
}
