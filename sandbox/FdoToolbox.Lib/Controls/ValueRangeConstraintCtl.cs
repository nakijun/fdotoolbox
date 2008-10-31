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
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Lib.Controls
{
    public partial class ValueRangeConstraintCtl : UserControl
    {
        public ValueRangeConstraintCtl()
        {
            InitializeComponent();
            this.InclusiveValuesEnabled = _incEnabled;
        }

        public bool IsMinInclusive
        {
            get { return chkMinInclusive.Checked; }
            set { chkMinInclusive.Checked = value; }
        }

        public bool IsMaxInclusive
        {
            get { return chkMaxInclusive.Checked; }
            set { chkMaxInclusive.Checked = value; }
        }

        private bool _incEnabled;

        public bool InclusiveValuesEnabled
        {
            get { return _incEnabled; }
            set { chkMaxInclusive.Enabled = chkMinInclusive.Enabled = _incEnabled = value; }
        }

        public DataValue MinValue
        {
            get { return (DataValue)DataValue.Parse(txtMinValue.Text); }
            set
            {
                switch (value.DataType)
                {
                    case DataType.DataType_Byte:
                        txtMinValue.Text = (value as ByteValue).Byte.ToString();
                        break;
                    case DataType.DataType_DateTime:
                        txtMinValue.Text = (value as DateTimeValue).DateTime.ToString();
                        break;
                    case DataType.DataType_Decimal:
                        txtMinValue.Text = (value as DecimalValue).Decimal.ToString();
                        break;
                    case DataType.DataType_Double:
                        txtMinValue.Text = (value as DoubleValue).Double.ToString();
                        break;
                    case DataType.DataType_Int16:
                        txtMinValue.Text = (value as Int16Value).Int16.ToString();
                        break;
                    case DataType.DataType_Int32:
                        txtMinValue.Text = (value as Int32Value).Int32.ToString();
                        break;
                    case DataType.DataType_Int64:
                        txtMinValue.Text = (value as Int64Value).Int64.ToString();
                        break;
                    case DataType.DataType_Single:
                        txtMinValue.Text = (value as SingleValue).Single.ToString();
                        break;
                    case DataType.DataType_String:
                        txtMinValue.Text = (value as StringValue).String;
                        break;
                }
            }
        }

        public DataValue MaxValue
        {
            get { return (DataValue)DataValue.Parse(txtMaxValue.Text); }
            set
            {
                switch (value.DataType)
                {
                    case DataType.DataType_Byte:
                        txtMaxValue.Text = (value as ByteValue).Byte.ToString();
                        break;
                    case DataType.DataType_DateTime:
                        txtMaxValue.Text = (value as DateTimeValue).DateTime.ToString();
                        break;
                    case DataType.DataType_Decimal:
                        txtMaxValue.Text = (value as DecimalValue).Decimal.ToString();
                        break;
                    case DataType.DataType_Double:
                        txtMaxValue.Text = (value as DoubleValue).Double.ToString();
                        break;
                    case DataType.DataType_Int16:
                        txtMaxValue.Text = (value as Int16Value).Int16.ToString();
                        break;
                    case DataType.DataType_Int32:
                        txtMaxValue.Text = (value as Int32Value).Int32.ToString();
                        break;
                    case DataType.DataType_Int64:
                        txtMaxValue.Text = (value as Int64Value).Int64.ToString();
                        break;
                    case DataType.DataType_Single:
                        txtMaxValue.Text = (value as SingleValue).Single.ToString();
                        break;
                    case DataType.DataType_String:
                        txtMaxValue.Text = (value as StringValue).String;
                        break;
                }
            }
        }
    }
}
