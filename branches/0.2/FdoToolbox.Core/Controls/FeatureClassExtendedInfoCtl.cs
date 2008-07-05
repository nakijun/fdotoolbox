using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Controls
{
    public partial class FeatureClassExtendedInfoCtl : UserControl
    {
        public FeatureClassExtendedInfoCtl()
        {
            InitializeComponent();
        }

        public FeatureClassExtendedInfoCtl(List<GeometricPropertyDefinition> defs)
            : this()
        {
            this.GeometryPropertyList = defs;
        }

        public List<GeometricPropertyDefinition> GeometryPropertyList
        {
            set { cmbGeometryProperty.DataSource = value; }
        }

        public GeometricPropertyDefinition GeometryProperty
        {
            get { return cmbGeometryProperty.SelectedItem as GeometricPropertyDefinition; }
        }

        internal void FlagError(ErrorProvider errorProvider, string p)
        {
            errorProvider.SetError(cmbGeometryProperty, p);
        }
    }
}
