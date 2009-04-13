#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using FdoToolbox.Core.Feature;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using System.Windows.Forms.Design;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class DataPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private OSGeo.FDO.Schema.DataPropertyDefinition _dataDef;
        
        public DataPropertyDefinitionDesign(OSGeo.FDO.Schema.DataPropertyDefinition dp, FdoConnection conn) : base(dp, conn)
        {
            _dataDef = dp;
        }

        [Description("The FDO data type of this property")]
        [Editor(typeof(DataPropertyTypeEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.DataType DataType
        { 
            get { return _dataDef.DataType; } 
            set 
            {
                _dataDef.DataType = value;
                FirePropertyChanged("DataType");
            } 
        }

        [Description("The default value of this property")]
        public string DefaultValue
        { 
            get { return _dataDef.DefaultValue; } 
            set 
            { 
                _dataDef.DefaultValue = value;
                FirePropertyChanged("DefaultValue");
            } 
        }

        [Description("Indicates if this is an auto-generated property")]
        public bool IsAutoGenerated
        { 
            get { return _dataDef.IsAutoGenerated; } 
            set 
            { 
                _dataDef.IsAutoGenerated = value;
                FirePropertyChanged("IsAutoGenerated");
            } 
        }

        [Description("The length of this property. Only applies to string, BLOB or CLOB properties")]
        public int Length
        { 
            get { return _dataDef.Length; } 
            set 
            { 
                _dataDef.Length = value;
                FirePropertyChanged("Length");
            } 
        }

        [Description("Indicates if this property accepts null values")]
        public bool Nullable
        { 
            get { return _dataDef.Nullable; } 
            set 
            { 
                _dataDef.Nullable = value;
                FirePropertyChanged("Nullable");
            } 
        }

        [Description("The precision (total number of digits) of this property. Only applies if this property is a decimal")]
        public int Precision
        {
            get
            {
                return _dataDef.Precision;
            }
            set 
            { 
                _dataDef.Precision = value;
                FirePropertyChanged("Precision");
            }
        }

        [Description("Indicates if this property is read-only")]
        public bool ReadOnly
        { 
            get { return _dataDef.ReadOnly; } 
            set 
            { 
                _dataDef.ReadOnly = value;
                FirePropertyChanged("ReadOnly");
            } 
        }

        [Description("The scale (number of digits to the right of the decimal point) of this property. Only applies if tis property is a decimal")]
        public int Scale
        { 
            get { return _dataDef.Scale; } 
            set 
            { 
                _dataDef.Scale = value;
                FirePropertyChanged("Scale");
            } 
        }

        [Description("The value constraint of this data property")]
        [Editor(typeof(ValueConstraintEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.PropertyValueConstraint ValueConstraint
        { 
            get { return _dataDef.ValueConstraint; } 
            set 
            { 
                _dataDef.ValueConstraint = value;
                FirePropertyChanged("ValueConstraint");
            } 
        }
    }

    internal class DataPropertyTypeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                DataPropertyDefinitionDesign dp = (DataPropertyDefinitionDesign)context.Instance;
                ListBox lb = new ListBox();
                lb.SelectionMode = SelectionMode.One;
                FdoConnection conn = dp.Connection;
                DataType[] values = null;
                if (conn != null)
                    values = (DataType[])conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_DataTypes);
                else
                    values = (DataType[])Enum.GetValues(typeof(DataType));
                
                foreach(DataType dt in values)
                {
                    lb.Items.Add(dt);
                }

                if (value != null)
                {
                    int idx = lb.Items.IndexOf(value);
                    lb.SelectedIndex = idx;
                }

                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                lb.SelectedIndexChanged += delegate
                {
                    if (lb.SelectedItem != null)
                        editorService.CloseDropDown();
                };

                editorService.DropDownControl(lb);

                if (lb.SelectedItem != null)
                {
                    value = lb.SelectedItem;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
