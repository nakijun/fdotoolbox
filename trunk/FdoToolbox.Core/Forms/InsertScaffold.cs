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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Forms
{
    /// <summary>
    /// A dynamic data-entry form for feature data. 
    /// </summary>
    public partial class InsertScaffold : Form
    {
        internal InsertScaffold()
        {
            InitializeComponent();
        }

        public InsertScaffold(ClassDefinition classDef)
            : this()
        {
            this.Text = "Insert Data: " + classDef.QualifiedName;

            SetupGrid();

            foreach (DataPropertyDefinition dataDef in classDef.IdentityProperties)
            {
                AddIdentityField(dataDef);
            }

            foreach (PropertyDefinition propDef in classDef.Properties)
            {
                switch (propDef.PropertyType)
                {
                    case PropertyType.PropertyType_DataProperty:
                        DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                        if(!classDef.IdentityProperties.Contains(dp))
                            AddDataField(dp);
                        break;
                    case PropertyType.PropertyType_GeometricProperty:
                        AddGeometryField(propDef as GeometricPropertyDefinition);
                        break;
                    default:
                        throw new NotSupportedException("Unsupported property type");
                }
            }
        }

        private void SetupGrid()
        {
            grdProperties.Rows.Clear();
            grdProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdProperties.Columns.Add(colName);
            grdProperties.Columns.Add(colValue);
        }

        private void AddGeometryField(GeometricPropertyDefinition geomDef)
        {
            if (geomDef.ReadOnly)
                return;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = geomDef.Name;
            nameCell.Tag = geomDef;

            DataGridViewCell valueCell = new DataGridViewTextBoxCell();
            valueCell.ToolTipText = "Enter the FGF or WKB geometry text";
            
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            nameCell.ReadOnly = true;

            grdProperties.Rows.Add(row);
        }

        private void AddDataField(DataPropertyDefinition dataPropertyDefinition)
        {
            AddIdentityField(dataPropertyDefinition);
        }

        public Dictionary<string, ValueExpression> GetValues()
        {
            Dictionary<string, ValueExpression> values = new Dictionary<string, ValueExpression>();
            foreach (DataGridViewRow row in grdProperties.Rows)
            {
                string name = row.Cells[0].Value.ToString();
                PropertyDefinition propDef = row.Cells[0].Tag as PropertyDefinition;
                if (row.Cells[1].Value != null)
                {
                    string str = row.Cells[1].Value.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        ValueExpression expr = null;
                        if (propDef.PropertyType == PropertyType.PropertyType_DataProperty)
                        {
                            DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                            switch(dp.DataType)
                            {
                                case DataType.DataType_Boolean:
                                    expr = new BooleanValue(Convert.ToBoolean(str));
                                    break;
                                case DataType.DataType_Byte:
                                    expr = new ByteValue(Convert.ToByte(str));
                                    break;
                                case DataType.DataType_DateTime:
                                    expr = new DateTimeValue(Convert.ToDateTime(str));
                                    break;
                                case DataType.DataType_Decimal:
                                    expr = new DecimalValue(Convert.ToDouble(str));
                                    break;
                                case DataType.DataType_Double:
                                    expr = new DoubleValue(Convert.ToDouble(str));
                                    break;
                                case DataType.DataType_Int16:
                                    expr = new Int16Value(Convert.ToInt16(str));
                                    break;
                                case DataType.DataType_Int32:
                                    expr = new Int32Value(Convert.ToInt32(str));
                                    break;
                                case DataType.DataType_Int64:
                                    expr = new Int64Value(Convert.ToInt64(str));
                                    break;
                                case DataType.DataType_Single:
                                    expr = new SingleValue(Convert.ToSingle(str));
                                    break;
                                case DataType.DataType_String:
                                    expr = new StringValue(str);
                                    break;
                                default:
                                    throw new NotSupportedException("Unsupported data type: " + dp.DataType);
                            }
                        }
                        else if (propDef.PropertyType == PropertyType.PropertyType_GeometricProperty)
                        {
                            expr = new GeometryValue(FdoGeometryUtil.GetFgf(str));
                        }

                        if(expr != null)
                            values.Add(name, expr);
                    }
                }
            }
            return values;
        }

        public bool UseTransaction
        {
            get { return chkUseTransaction.Checked; }
        }

        private void AddIdentityField(DataPropertyDefinition dataDef)
        {
            if (dataDef.IsAutoGenerated)
                return;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = dataDef.Name;
            nameCell.ToolTipText = "Type: " + dataDef.DataType;
            nameCell.Tag = dataDef;

            DataGridViewCell valueCell = null;
            if (dataDef.ValueConstraint != null && dataDef.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_List)
            {
                PropertyValueConstraintList list = (dataDef.ValueConstraint as PropertyValueConstraintList);
                DataGridViewComboBoxCell cc = new DataGridViewComboBoxCell();
                List<string> values = new List<string>();
                if (dataDef.Nullable)
                    values.Add("");
                foreach (DataValue value in list.ConstraintList)
                {
                    switch (value.DataType)
                    {
                        case DataType.DataType_Byte:
                            values.Add((value as ByteValue).Byte.ToString());
                            break;
                        case DataType.DataType_DateTime:
                            values.Add((value as DateTimeValue).DateTime.ToString());
                            break;
                        case DataType.DataType_Decimal:
                            values.Add((value as DecimalValue).Decimal.ToString());
                            break;
                        case DataType.DataType_Double:
                            values.Add((value as DoubleValue).Double.ToString());
                            break;
                        case DataType.DataType_Int16:
                            values.Add((value as Int16Value).Int16.ToString());
                            break;
                        case DataType.DataType_Int32:
                            values.Add((value as Int32Value).Int32.ToString());
                            break;
                        case DataType.DataType_Int64:
                            values.Add((value as Int64Value).Int64.ToString());
                            break;
                        case DataType.DataType_Single:
                            values.Add((value as SingleValue).Single.ToString());
                            break;
                        case DataType.DataType_String:
                            values.Add((value as StringValue).String);
                            break;
                    }
                }
                cc.DataSource = values;
                valueCell = cc;
            }
            else
            {
                switch (dataDef.DataType)
                {
                    case DataType.DataType_BLOB:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
                            valueCell = tc;
                        }
                        break;
                    case DataType.DataType_Boolean:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Byte:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_CLOB:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
                            valueCell = tc;
                        }
                        break;
                    case DataType.DataType_DateTime:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Decimal:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Double:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int16:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int32:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Int64:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_Single:
                        valueCell = new DataGridViewTextBoxCell();
                        break;
                    case DataType.DataType_String:
                        {
                            DataGridViewTextBoxCell tc = new DataGridViewTextBoxCell();
                            tc.MaxInputLength = dataDef.Length;
                            valueCell = tc;
                        }
                        break;
                }
            }
            valueCell.Style.BackColor = dataDef.Nullable ? Color.YellowGreen : Color.White;
            valueCell.Value = dataDef.DefaultValue;
            valueCell.ToolTipText = dataDef.Description;

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            nameCell.ReadOnly = true;

            grdProperties.Rows.Add(row);
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