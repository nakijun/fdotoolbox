using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Forms
{
    public partial class CoordinateSystemDialog : Form
    {
        private readonly CoordSysCatalog catalog;

        internal CoordinateSystemDialog()
        {
            InitializeComponent();
            catalog = ServiceManager.Instance.GetService<CoordSysCatalog>();
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
                errorProvider1.SetError(txtName, "Required");
                return;
            }
            if (string.IsNullOrEmpty(txtWKT.Text))
            {
                errorProvider1.SetError(txtWKT, "Required");
                return;
            }
            if (catalog.ProjectionExists(txtName.Text))
            {
                errorProvider1.SetError(txtName, "A coordinate system of that name already exists");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        public static CoordinateSystemDefinition NewCoordinateSystem()
        {
            CoordinateSystemDialog diag = new CoordinateSystemDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return new CoordinateSystemDefinition(diag.txtName.Text, diag.txtDescription.Text, diag.txtWKT.Text);
            }
            return null;
        }

        public static bool EditCooridinateSystem(CoordinateSystemDefinition cs)
        {
            CoordinateSystemDialog diag = new CoordinateSystemDialog();
            diag.txtName.Text = cs.Name;
            diag.txtDescription.Text = cs.Description;
            diag.txtWKT.Text = cs.Wkt;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                cs.Name = diag.txtName.Text;
                cs.Description = diag.txtDescription.Text;
                cs.Wkt = diag.txtWKT.Text;
                return true;
            }
            return false;
        }
    }
}