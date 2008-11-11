using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.Connections;
using System.Collections.Specialized;

namespace FdoToolbox.Base.Forms
{
    public partial class PendingParameterDialog : Form
    {
        public PendingParameterDialog()
        {
            InitializeComponent();
            InitializeGrid();
            this.Text = ResourceService.GetString("TITLE_PENDING_PARAMETERS");
        }

        public PendingParameterDialog(IEnumerable<DictionaryProperty> properties)
            : this()
        {
            foreach (DictionaryProperty dp in properties)
            {
                if (dp.Enumerable)
                {
                    EnumerableDictionaryProperty ep = dp as EnumerableDictionaryProperty;
                    if (ep.Required)
                        this.AddRequiredEnumerableProperty(ep.Name, ep.DefaultValue, ep.Values);
                    else
                        this.AddOptionalEnumerableProperty(ep.Name, ep.DefaultValue, ep.Values);
                }
            }
        }

        private void InitializeGrid()
        {
            grdPendingProperties.Rows.Clear();
            grdPendingProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdPendingProperties.Columns.Add(colName);
            grdPendingProperties.Columns.Add(colValue);
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
            grdPendingProperties.Rows.Add(row);
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

            grdPendingProperties.Rows.Add(row);
            return row;
        }

        public static System.Collections.Specialized.NameValueCollection GetExtraParameters(IEnumerable<DictionaryProperty> properties)
        {
            PendingParameterDialog diag = new PendingParameterDialog(properties);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.GetPendingParameters();
            }
            return null;
        }

        public NameValueCollection GetPendingParameters()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (DataGridViewRow row in grdPendingProperties.Rows)
            {
                string name = row.Cells[0].Value.ToString();
                string value = row.Cells[1].Value.ToString();
                nvc.Add(name, value);
            }
            return nvc;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}