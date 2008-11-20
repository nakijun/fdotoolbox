using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Expression;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public partial class ListConstraintSettingsCtl : UserControl
    {
        public ListConstraintSettingsCtl()
        {
            InitializeComponent();
            CheckDeleteState();
        }

        private void lstValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDeleteState();
        }

        private void CheckDeleteState()
        {
            btnDelete.Enabled = (lstValues.SelectedItem != null);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lstValues.Items.RemoveAt(lstValues.SelectedIndex);
            CheckDeleteState();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewValue.Text))
            {
                //TODO: How smart is this list constraint? Is a list of 
                //number strings treated as a list of numbers?
                DataValue val = new StringValue(txtNewValue.Text);
                
                txtNewValue.Text = string.Empty;
                lstValues.Items.Add(val);
            }
        }

        public DataValueCollection ListValues
        {
            get
            {
                DataValueCollection values = new DataValueCollection();
                foreach (object obj in lstValues.Items)
                {
                    values.Add((DataValue)obj);
                }
                return values;
            }
            set
            {
                lstValues.Items.Clear();
                foreach (DataValue val in value)
                {
                    lstValues.Items.Add(val);
                }
            }
        }
    }
}
