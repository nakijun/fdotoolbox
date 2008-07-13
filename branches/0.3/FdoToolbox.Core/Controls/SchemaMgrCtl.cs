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

namespace FdoToolbox.Core.Controls
{
    public partial class SchemaMgrCtl : BaseDocumentCtl
    {
        public SchemaMgrCtl()
        {
            InitializeComponent();
            this.Title = "Schema Management";
            _bsClasses = new BindingSource();
            _bsSchemas = new BindingSource();
        }

        private BindingSource _bsSchemas;
        private BindingSource _bsClasses;

        public event EventHandler OnSchemasApplied;
        
        private IConnection _BoundConnection;
         
        public SchemaMgrCtl(IConnection conn)
            : this()
        {
            _BoundConnection = conn;
            using (IDescribeSchema cmd = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                _Schemas = cmd.Execute();
            }
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
            ClassType[] ctypes = _BoundConnection.SchemaCapabilities.ClassTypes;
            featureClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_FeatureClass) >= 0;
            classNonFeatureToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_Class) >= 0;
            networkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkClass) >= 0;
            networkLayerClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLayerClass) >= 0;
            networkLinkClassToolStripMenuItem.Visible = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_NetworkLinkClass) >= 0;

            //Schema modification
            btnDeleteSchema.Visible = btnDeleteClass.Visible = _BoundConnection.SchemaCapabilities.SupportsSchemaModification;
        }

        private FeatureSchemaCollection _Schemas;

        private void lstSchemas_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema selectedSchema = lstSchemas.SelectedItem as FeatureSchema;
            if (selectedSchema != null)
            {
                _bsClasses.DataSource = selectedSchema.Classes;
                lstClasses.DataSource = _bsClasses;
                btnAddClass.Enabled = btnEditClass.Enabled = btnDeleteClass.Enabled = true;
            }
            else
            {
                btnAddClass.Enabled = btnEditClass.Enabled = btnDeleteClass.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Cancel();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (_Schemas.Count > 0)
                {
                    using (IApplySchema cmd = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                    {
                        foreach (FeatureSchema schema in _Schemas)
                        {
                            cmd.FeatureSchema = schema;
                            cmd.Execute();
                        }
                        AppConsole.Alert("Schema Management", "Schema(s) applied");
                        if (this.OnSchemasApplied != null)
                            this.OnSchemasApplied(this, EventArgs.Empty);
                        this.Accept();
                    }
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
            bool canAdd = _BoundConnection.SchemaCapabilities.SupportsMultipleSchemas ||
                        (!_BoundConnection.SchemaCapabilities.SupportsMultipleSchemas && _bsSchemas.Count == 0);
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
                    ctl = new ClassDefCtl((Class)classDef, _BoundConnection);
                    break;
                case ClassType.ClassType_FeatureClass:
                    ctl = new ClassDefCtl((FeatureClass)classDef, _BoundConnection);
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
            BaseDocumentCtl ctl = new ClassDefCtl(schema, classType, _BoundConnection);
            Form frm = FormFactory.CreateFormForControl(ctl, 500, 400);
            frm.ShowDialog();
        }
    }
}
