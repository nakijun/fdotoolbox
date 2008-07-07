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

namespace FdoToolbox.Core.Controls
{
    public partial class GenericCreateDataStoreCtl : BaseDocumentCtl
    {
        public GenericCreateDataStoreCtl()
        {
            InitializeComponent();
            InitializeGrid();
            StringCollection providerNames = new StringCollection();
            using (ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders())
            {
                foreach (Provider prov in providers)
                {
                    providerNames.Add(prov.Name);
                }
            }
            cmbProvider.DataSource = providerNames;
        }

        public override string Title
        {
            get
            {
                return "New Data Connection";
            }
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetConnectProperties(cmbProvider.SelectedItem.ToString());
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(cmbProvider.SelectedItem.ToString());
            using (conn)
            {
                using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    foreach (DataGridViewRow row in grdConnectProperties.Rows)
                    {
                        string name = row.Cells[0].Value.ToString();
                        string value = row.Cells[1].Value.ToString();
                        cmd.DataStoreProperties.SetProperty(name, value);
                    }
                    try
                    {
                        cmd.Execute();
                        AppConsole.Alert("Create Data Store", "Data Store successfully created");
                        this.Accept();
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
            this.Cancel();
        }

        private void InitializeGrid()
        {
            grdConnectProperties.Rows.Clear();
            grdConnectProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdConnectProperties.Columns.Add(colName);
            grdConnectProperties.Columns.Add(colValue);
        }

        private void AddRequiredProperty(string name, string defaultValue)
        {
            //TODO: Attach a validation scheme
            AddProperty(name, defaultValue);
        }

        private void AddRequiredEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
        {
            //TODO: Attach a validation scheme
            AddOptionalEnumerableProperty(name, defaultValue, values);
        }

        private void AddOptionalEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdConnectProperties.Rows.Add(row);
        }

        private void AddProperty(string name, string defaultValue)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdConnectProperties.Rows.Add(row);
        }

        public void SetConnectProperties(string provider)
        {
            try
            {
                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
                using (conn)
                {
                    //Supports ICreateDataStore
                    if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) > 0)
                    {
                        using (ICreateDataStore cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                        {
                            string[] propertyNames = cmd.DataStoreProperties.PropertyNames;
                            grdConnectProperties.Rows.Clear();
                            foreach (string name in propertyNames)
                            {
                                string localized = cmd.DataStoreProperties.GetLocalizedName(name);
                                bool required = cmd.DataStoreProperties.IsPropertyRequired(name);
                                bool enumerable = cmd.DataStoreProperties.IsPropertyEnumerable(name);
                                string defaultValue = cmd.DataStoreProperties.GetPropertyDefault(name);
                                string[] values = cmd.DataStoreProperties.EnumeratePropertyValues(name);
                                if (required && !enumerable)
                                    AddRequiredProperty(localized, defaultValue);
                                else if (required && enumerable)
                                    AddRequiredEnumerableProperty(localized, defaultValue, values);
                                else if (!required && enumerable)
                                    AddOptionalEnumerableProperty(localized, defaultValue, values);
                                else
                                    AddProperty(localized, defaultValue);
                            }
                        }
                        btnCreate.Enabled = true;
                    }
                    else
                    {
                        AppConsole.Alert("Error", "The selected provider does not support the creation of data stores");
                        btnCreate.Enabled = false;
                        grdConnectProperties.Rows.Clear();
                    }
                }
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
