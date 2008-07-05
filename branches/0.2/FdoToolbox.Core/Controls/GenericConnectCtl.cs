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

namespace FdoToolbox.Core.Controls
{
    public partial class GenericConnectCtl : BaseDocumentCtl
    {
        public GenericConnectCtl()
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
            if (HostApplication.Instance.ConnectionManager.GetConnection(txtName.Text) != null)
            {
                errorProvider1.SetError(txtName, "The specified connection name already exists. Please choose another");
                return;
            }
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(cmbProvider.SelectedItem.ToString());
            using (conn)
            {
                conn.ConnectionString = this.ParseConnectionString();
                try
                {
                    if (conn.Open() == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open)
                    {
                        HostApplication.Instance.ConnectionManager.AddConnection(txtName.Text, conn);
                        this.Close();
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
                    string[] propertyNames = conn.ConnectionInfo.ConnectionProperties.PropertyNames;
                    grdConnectProperties.Rows.Clear();
                    foreach (string name in propertyNames)
                    {
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
