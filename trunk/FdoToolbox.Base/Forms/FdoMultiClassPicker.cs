using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Forms
{
    public partial class FdoMultiClassPicker : Form, IFdoMultiClassPickerView
    {
        private FdoMultiClassPickerPresenter _presenter;
        private OSGeo.FDO.Schema.FeatureSchemaCollection _schemas;

        internal FdoMultiClassPicker()
        {
            InitializeComponent();
        }

        internal FdoMultiClassPicker(string title, string message, OSGeo.FDO.Schema.FeatureSchemaCollection schemas)
            : this()
        {
            _schemas = schemas; 
            _presenter = new FdoMultiClassPickerPresenter(this, title, message);
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init(_schemas);
        }

        public OSGeo.FDO.Schema.FeatureSchemaCollection SchemaList
        {
            set 
            {
                cmbSchema.DisplayMember = "Name";
                cmbSchema.DataSource = value;
            }
        }

        public OSGeo.FDO.Schema.FeatureSchema SelectedSchema
        {
            get { return cmbSchema.SelectedItem as OSGeo.FDO.Schema.FeatureSchema; }
        }

        public OSGeo.FDO.Schema.ClassCollection ClassList
        {
            set 
            {
                lstClasses.SelectionMode = SelectionMode.MultiSimple;
                lstClasses.DisplayMember = "Name";
                lstClasses.DataSource = value;
            }
        }

        public IList<OSGeo.FDO.Schema.ClassDefinition> SelectedClasses
        {
            get 
            {
                List<OSGeo.FDO.Schema.ClassDefinition> classes = new List<OSGeo.FDO.Schema.ClassDefinition>();
                foreach (object item in lstClasses.SelectedItems)
                {
                    classes.Add((OSGeo.FDO.Schema.ClassDefinition)item);
                }
                return classes;
            }
        }

        public string Title
        {
            set { this.Text = value;  }
        }

        public string Message
        {
            set { lblMessage.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public static IList<OSGeo.FDO.Schema.ClassDefinition> GetClasses(string title, string message, FdoConnection conn)
        {
            OSGeo.FDO.Schema.FeatureSchemaCollection schemas = null;
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                schemas = service.DescribeSchema();
            }
            if (schemas != null)
            {
                FdoMultiClassPicker diag = new FdoMultiClassPicker(title, message, schemas);
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    return diag.SelectedClasses;
                }
            }
            return null;
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.SchemaChanged();
        }
    }
}