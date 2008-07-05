using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Forms
{
    public partial class SchemaInfoDlg : Form
    {
        public SchemaInfoDlg()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
                errorProvider1.SetError(txtName, "Required");
            this.DialogResult = DialogResult.OK;
        }

        public static FeatureSchema NewSchema()
        {
            SchemaInfoDlg dlg = new SchemaInfoDlg();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return new FeatureSchema(dlg.txtName.Text, dlg.txtDescription.Text);
            }
            return null;
        }
    }
}