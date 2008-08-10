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
using FdoToolbox.Core.Forms;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Controls
{
    public partial class GenericSpatialConnectCtl : BaseDocumentCtl
    {
        public GenericSpatialConnectCtl()
        {
            InitializeComponent();
            InitializeGrid();
            this.Title = "New Data Connection";
            _PendingProperties = new List<string>();
            StringCollection providerNames = new StringCollection();
            using (ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders())
            {
                foreach (OSGeo.FDO.ClientServices.Provider prov in providers)
                {
                    providerNames.Add(prov.Name);
                }
            }
            cmbProvider.DataSource = providerNames;
        }

        private string ParseConnectionString()
        {
            string str = "";
            foreach (DataGridViewRow row in grdConnectProperties.Rows)
            {
                if (string.IsNullOrEmpty(str))
                    str += string.Format("{0}={1}", row.Cells[0].Value, row.Cells[1].Value);
                else
                    str += string.Format(";{0}={1}", row.Cells[0].Value, row.Cells[1].Value);
            }
            return str;
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetConnectProperties(cmbProvider.SelectedItem.ToString());
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(cmbProvider.SelectedItem.ToString());
            using (conn) 
            {
                conn.ConnectionString = this.ParseConnectionString();
                try
                {
                    if (conn.Open() == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open)
                    {
                        MessageBox.Show("Connection Successful");
                        conn.Close();
                    }
                    else
                        MessageBox.Show("Connection failed");
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message);
                }
            }

        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider1.SetError(txtName, "Required");
                return;
            }
            if (HostApplication.Instance.SpatialConnectionManager.GetConnection(txtName.Text) != null)
            {
                errorProvider1.SetError(txtName, "The specified connection name already exists. Please choose another");
                return;
            }
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(cmbProvider.SelectedItem.ToString());
           
            conn.ConnectionString = this.ParseConnectionString();
            try
            {
                if (conn.Open() == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Pending)
                {
                    //Pending. Further parameters required
                    NameValueCollection pendingParams = PendingParameterDialog.GetParameters(_PendingProperties, conn);
                    if (pendingParams != null)
                    {
                        foreach (string name in pendingParams.AllKeys)
                        {
                            conn.ConnectionInfo.ConnectionProperties.SetProperty(name, pendingParams[name]);
                        }
                    }
                    else
                    {
                        throw new FdoConnectionException("Pending Parameters were not filled in");
                    }
                }
                if (conn.ConnectionState == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open || conn.Open() == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open)
                {
                    HostApplication.Instance.SpatialConnectionManager.AddConnection(txtName.Text, conn);
                    this.Close();
                }
                else
                {
                    throw new FdoConnectionException("Opening the connection did not move it to the \"opened\" state");
                }
            }
            catch (FdoConnectionException ex)
            {
                AppConsole.Alert("Error", ex.Message);
                conn.Dispose();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", "Connection failed: " + ex.Message);
                conn.Dispose();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private DataGridViewRow AddRequiredProperty(string name, string defaultValue)
        {
            //TODO: Attach a validation scheme
            return AddProperty(name, defaultValue);
        }

        private DataGridViewRow AddRequiredEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
        {
            //TODO: Attach a validation scheme
            return AddOptionalEnumerableProperty(name, defaultValue, values);
        }

        private DataGridViewRow AddOptionalEnumerableProperty(string name, string defaultValue, IEnumerable<string> values)
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
            return row;
        }

        private DataGridViewRow AddProperty(string name, string defaultValue)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdConnectProperties.Rows.Add(row);
            return row;
        }

        private List<string> _PendingProperties;

        public void SetConnectProperties(string provider)
        {
            _PendingProperties.Clear();
            try
            {
                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
                using (conn)
                {
                    string[] propertyNames = conn.ConnectionInfo.ConnectionProperties.PropertyNames;
                    grdConnectProperties.Rows.Clear();
                    foreach (string name in propertyNames)
                    {
                        string localized = conn.ConnectionInfo.ConnectionProperties.GetLocalizedName(name);
                        bool required = conn.ConnectionInfo.ConnectionProperties.IsPropertyRequired(name);
                        bool enumerable = conn.ConnectionInfo.ConnectionProperties.IsPropertyEnumerable(name);
                        string defaultValue = conn.ConnectionInfo.ConnectionProperties.GetPropertyDefault(name);
                        bool canGetValues = true;
                        string[] values = null;
                        try
                        {
                            values = conn.ConnectionInfo.ConnectionProperties.EnumeratePropertyValues(name);
                        }
                        catch
                        {
                            _PendingProperties.Add(name);
                            canGetValues = false;
                        }
                        if (required)
                        {
                            if (enumerable)
                            {
                                if (canGetValues)
                                {
                                    DataGridViewRow row = AddRequiredEnumerableProperty(localized, defaultValue, values);
                                    if(values.Length > 0)
                                        row.Cells[1].Value = values[0];
                                }
                            }
                            else
                                AddRequiredProperty(localized, defaultValue);
                        }
                        else
                        {
                            if (enumerable)
                            {
                                if(canGetValues)
                                    AddOptionalEnumerableProperty(localized, defaultValue, values);
                            }
                            else
                                AddProperty(localized, defaultValue);
                        }
                            
                        /*
                        string localized = conn.ConnectionInfo.ConnectionProperties.GetLocalizedName(name);
                        bool required = conn.ConnectionInfo.ConnectionProperties.IsPropertyRequired(name);
                        bool enumerable = conn.ConnectionInfo.ConnectionProperties.IsPropertyEnumerable(name);
                        string defaultValue = conn.ConnectionInfo.ConnectionProperties.GetPropertyDefault(name);
                        string[] values = conn.ConnectionInfo.ConnectionProperties.EnumeratePropertyValues(name);
                        if (required && !enumerable)
                            AddRequiredProperty(localized, defaultValue);
                        else if (required && enumerable)
                            AddRequiredEnumerableProperty(localized, defaultValue, values);
                        else if (!required && enumerable)
                            AddOptionalEnumerableProperty(localized, defaultValue, values);
                        else
                            AddProperty(localized, defaultValue);
                         */
                    }
                    btnTest.Enabled = btnConnect.Enabled = true;
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
                btnTest.Enabled = btnConnect.Enabled = false;
                grdConnectProperties.Rows.Clear();
            }
        }
    }
}
