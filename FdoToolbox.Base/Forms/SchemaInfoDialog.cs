using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Forms
{
    public partial class SchemaInfoDialog : Form
    {
        public SchemaInfoDialog()
        {
            InitializeComponent();
            this.Text = ResourceService.GetString("TITLE_SCHEMA_INFORMATION");
        }

        public static OSGeo.FDO.Schema.FeatureSchema NewSchema()
        {
            SchemaInfoDialog dlg = new SchemaInfoDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string name = dlg.txtName.Text;
                string desc = dlg.txtDescription.Text;
                return new OSGeo.FDO.Schema.FeatureSchema(name, desc);
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider1.SetError(txtName, ResourceService.GetString("ERR_FIELD_REQUIRED"));
                return;
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}