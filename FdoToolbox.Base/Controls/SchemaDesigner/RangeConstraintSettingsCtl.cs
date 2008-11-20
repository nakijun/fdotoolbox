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
    public partial class RangeConstraintSettingsCtl : UserControl
    {
        public RangeConstraintSettingsCtl()
        {
            InitializeComponent();
        }

        public DataValue MinValue
        {
            get { return (DataValue)DataValue.Parse(txtMinValue.Text); }
            set { txtMinValue.Text = value.ToString(); }
        }

        public DataValue MaxValue
        {
            get { return (DataValue)DataValue.Parse(txtMaxValue.Text); }
            set { txtMaxValue.Text = value.ToString(); }
        }

        public bool MinInclusive
        {
            get { return chkMinInclusive.Checked; }
            set { chkMinInclusive.Checked = value; }
        }

        public bool MaxInclusive
        {
            get { return chkMaxInclusive.Checked; }
            set { chkMaxInclusive.Checked = value; }
        }
    }
}
