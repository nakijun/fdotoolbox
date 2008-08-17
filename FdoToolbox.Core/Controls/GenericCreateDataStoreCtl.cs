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
using OSGeo.FDO.ClientServices;
using System.Collections.Specialized;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.DataStore;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Controls
{
    public partial class GenericCreateDataStoreCtl : BaseDocumentCtl
    {
        public GenericCreateDataStoreCtl()
        {
            InitializeComponent();
            InitializeGrid(grdConnectProperties);
            InitializeGrid(grdDataStoreProperties);
            this.Title = "New Data Store";
        }

        private bool _FlatFile;

        public bool IsFlatFile
        {
            get { return _FlatFile; }
            set { _FlatFile = value; }
        }
	
        protected override void OnLoad(EventArgs e)
        {
            ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders();
            List<OSGeo.FDO.ClientServices.Provider> bProviders = new List<OSGeo.FDO.ClientServices.Provider>();
            foreach (OSGeo.FDO.ClientServices.Provider prov in providers)
            {
                bProviders.Add(prov);
            }
            cmbProvider.DataSource = bProviders;
            base.OnLoad(e);
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            OSGeo.FDO.ClientServices.Provider prov = cmbProvider.SelectedItem as OSGeo.FDO.ClientServices.Provider;
            if (prov != null)
            {
                using (IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(prov.Name))
                {
                    this.IsFlatFile = (conn.ConnectionInfo.ProviderDatastoreType == ProviderDatastoreType.ProviderDatastoreType_File);
                    if (SetDataStoreProperties(conn))
                    {   
                        grpConnect.Enabled = !this.IsFlatFile;
                        if (!this.IsFlatFile)
                            SetConnectProperties(conn);
                    }
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            OSGeo.FDO.ClientServices.Provider prov = cmbProvider.SelectedItem as OSGeo.FDO.ClientServices.Provider;
            if (prov != null)
            {
                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(prov.Name);
                using (conn)
                {
                    try
                    {
                        //Non-flatfile providers require a connection be made first
                        if (!this.IsFlatFile)
                        {
                            foreach (DataGridViewRow row in grdConnectProperties.Rows)
                            {
                                string name = row.Cells[0].Value.ToString();
                                string value = row.Cells[1].Value.ToString();
                                conn.ConnectionInfo.ConnectionProperties.SetProperty(name, value);
                            }
                            conn.Open();
                        }

                        using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                        {
                            foreach (DataGridViewRow row in grdDataStoreProperties.Rows)
                            {
                                string name = row.Cells[0].Value.ToString();
                                string value = row.Cells[1].Value.ToString();
                                cmd.DataStoreProperties.SetProperty(name, value);
                            }
                            cmd.Execute();
                            AppConsole.Alert("Create Data Store", "Data Store successfully created");
                            this.Close();
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns.Add(colName);
            grid.Columns.Add(colValue);
        }

        private DataGridViewRow AddRequiredProperty(DataGridView grid, string name, string defaultValue)
        {
            //TODO: Attach a validation scheme
            return AddProperty(grid, name, defaultValue);
        }

        private DataGridViewRow AddRequiredEnumerableProperty(DataGridView grid, string name, string defaultValue, IEnumerable<string> values)
        {
            //TODO: Attach a validation scheme
            return AddOptionalEnumerableProperty(grid, name, defaultValue, values);
        }

        private DataGridViewRow AddOptionalEnumerableProperty(DataGridView grid, string name, string defaultValue, IEnumerable<string> values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grid.Rows.Add(row);
            return row;
        }

        private DataGridViewRow AddProperty(DataGridView grid, string name, string defaultValue)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grid.Rows.Add(row);
            return row;
        }

        public bool SetDataStoreProperties(IConnection conn)
        {
            bool canCreate = false;
            try
            {  
                //Supports ICreateDataStore
                if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) > 0)
                {
                    canCreate = true;
                    using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                    {
                        string[] propertyNames = cmd.DataStoreProperties.PropertyNames;
                        grdDataStoreProperties.Rows.Clear();
                        foreach (string name in propertyNames)
                        {
                            string localized = cmd.DataStoreProperties.GetLocalizedName(name);
                            bool required = cmd.DataStoreProperties.IsPropertyRequired(name);
                            bool enumerable = cmd.DataStoreProperties.IsPropertyEnumerable(name);
                            string defaultValue = cmd.DataStoreProperties.GetPropertyDefault(name);
                            if (enumerable)
                            {
                                string[] values = cmd.DataStoreProperties.EnumeratePropertyValues(name);
                                if (required)
                                {
                                    DataGridViewRow row = AddRequiredEnumerableProperty(grdDataStoreProperties, localized, defaultValue, values);
                                    if (values.Length > 0)
                                        row.Cells[1].Value = values[0];
                                }
                                else
                                {
                                    AddOptionalEnumerableProperty(grdDataStoreProperties, localized, defaultValue, values);
                                }
                            }
                            else
                            {
                                if (required)
                                {
                                    AddRequiredProperty(grdDataStoreProperties, localized, defaultValue);
                                }
                                else
                                {
                                    AddProperty(grdDataStoreProperties, localized, defaultValue);
                                }
                            }
                        }
                    }
                    btnCreate.Enabled = true;
                }
                else
                {
                    canCreate = false;
                    AppConsole.Alert("Error", "The selected provider does not support the creation of data stores");
                    btnCreate.Enabled = false;
                    grdDataStoreProperties.Rows.Clear();
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
                btnCreate.Enabled = false;
                grdDataStoreProperties.Rows.Clear();
            }
            return canCreate;
        }

        private void SetConnectProperties(IConnection conn)
        {
            try
            {
                string[] propertyNames = conn.ConnectionInfo.ConnectionProperties.PropertyNames;
                grdConnectProperties.Rows.Clear();
                foreach (string name in propertyNames)
                {
                    string localized = conn.ConnectionInfo.ConnectionProperties.GetLocalizedName(name);
                    bool required = conn.ConnectionInfo.ConnectionProperties.IsPropertyRequired(name);
                    bool enumerable = conn.ConnectionInfo.ConnectionProperties.IsPropertyEnumerable(name);
                    string defaultValue = conn.ConnectionInfo.ConnectionProperties.GetPropertyDefault(name);
                    
                    if (enumerable)
                    {
                        bool canGetValues = true;
                        string[] values = null;
                        try
                        {
                            values = conn.ConnectionInfo.ConnectionProperties.EnumeratePropertyValues(name);
                        }
                        catch
                        {
                            canGetValues = false;
                        }
                        if (canGetValues)
                        {
                            if (required)
                            {
                                DataGridViewRow row = AddRequiredEnumerableProperty(grdConnectProperties, localized, defaultValue, values);
                                if (values.Length > 0)
                                    row.Cells[1].Value = values[0];
                            }
                            else
                            {
                                AddOptionalEnumerableProperty(grdConnectProperties, localized, defaultValue, values);
                            }
                        }
                    }
                    else
                    {
                        if (required)
                        {
                            AddRequiredProperty(grdConnectProperties, localized, defaultValue);
                        }
                        else
                        {
                            AddProperty(grdConnectProperties, localized, defaultValue);
                        }
                    }
                }
                btnCreate.Enabled = true;
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
                btnCreate.Enabled = false;
                grdConnectProperties.Rows.Clear();
            }
        }
    }
}
