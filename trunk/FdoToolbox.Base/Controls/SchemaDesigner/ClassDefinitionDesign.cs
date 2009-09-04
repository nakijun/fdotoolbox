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
using OSGeo.FDO.Schema;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public abstract class ClassDefinitionDesign : INotifyPropertyChanged
    {
        private OSGeo.FDO.Schema.ClassDefinition _classDef;
        private FdoConnection _conn;

        public ClassDefinitionDesign(OSGeo.FDO.Schema.ClassDefinition cd, FdoConnection conn)
        {
            _classDef = cd;
            _conn = conn;
        }

        [Browsable(false)]
        public FdoConnection Connection
        {
            get { return _conn; }
        }
        
        [Browsable(false)]
        public OSGeo.FDO.Schema.ClassDefinition ClassDefinition
        {
            get { return _classDef; }
        }

        [Browsable(false)]
        public OSGeo.FDO.Schema.ClassDefinition BaseClass
        {
            get { return _classDef.BaseClass; }
            set 
            { 
                _classDef.BaseClass = value;
                FirePropertyChanged("BaseClass");
            }
        }

        [Description("The type of class")]
        public ClassType ClassType
        {
            get { return _classDef.ClassType; }
        }

        [Description("The identity properties of this class")]
        [Editor(typeof(ClassIdentityPropertyLookupEditor), typeof(UITypeEditor))]
        public DataPropertyDefinitionCollection IdentityProperties 
        {
            get { return _classDef.IdentityProperties; }
            set 
            {
                _classDef.IdentityProperties.Clear();
                foreach (DataPropertyDefinition dp in value)
                {
                    _classDef.IdentityProperties.Add(dp);
                }
                FirePropertyChanged("IdentityProperties");
            }
        }

        [Description("Indicates whether this class is abstract")]
        public bool IsAbstract
        {
            get { return _classDef.IsAbstract; }
            set 
            { 
                _classDef.IsAbstract = value;
                FirePropertyChanged("IsAbstract");
            }
        }

        [Description("Indicates whether this class is computed")]
        public bool IsComputed
        {
            get { return _classDef.IsComputed; }
            set 
            { 
                _classDef.IsComputed = value;
                FirePropertyChanged("IsComputed");
            }
        }

        [Description("The name of this class")]
        public string Name
        {
            get { return _classDef.Name; }
            set
            {
                _classDef.Name = value;
                FirePropertyChanged("Name");
            }
        }

        [Description("The unique constraints of this class")]
        [Editor(typeof(ClassUniqueConstraintEditor), typeof(UITypeEditor))]
        public UniqueConstraintCollection UniqueConstraints
        {
            get { return _classDef.UniqueConstraints; }
            set
            {
                _classDef.UniqueConstraints.Clear();
                foreach (UniqueConstraint uniq in value)
                {
                    _classDef.UniqueConstraints.Add(uniq);
                }
                FirePropertyChanged("UniqueConstraints");
            }
        }

        internal void FirePropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    public class ClassIdentityPropertyLookupEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ClassDefinitionDesign design = (ClassDefinitionDesign)context.Instance;
                ClassDefinition cls = design.ClassDefinition;
                CheckedListBox box = new CheckedListBox();
                FdoConnection conn = design.Connection;
                DataType[] idTypes = null;
                if (conn != null)
                    idTypes = (DataType[])conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes);

                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        if (idTypes != null)
                        {
                            //Only add if its data type matches any one in the array
                            if(Array.IndexOf<DataType>(idTypes, (pd as DataPropertyDefinition).DataType) >= 0)
                                box.Items.Add(pd.Name, false);
                        }
                        else
                        {
                            box.Items.Add(pd.Name, false);
                        }
                    }
                }

                if (value != null)
                {
                    foreach (DataPropertyDefinition dp in (DataPropertyDefinitionCollection)value)
                    {
                        int idx = box.Items.IndexOf(dp.Name);
                        if (idx >= 0)
                            box.SetItemChecked(idx, true);
                    }
                }

                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                editorService.DropDownControl(box);
                if (box.CheckedItems.Count > 0)
                {
                    DataPropertyDefinitionCollection dpdc = new DataPropertyDefinitionCollection(cls);
                    foreach (object obj in box.CheckedItems)
                    {
                        int pidx = cls.Properties.IndexOf(obj.ToString());
                        if (pidx >= 0)
                        {
                            PropertyDefinition pd = cls.Properties[pidx];
                            if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                                dpdc.Add((DataPropertyDefinition)pd);
                        }
                    }
                    value = dpdc;
                    design.FirePropertyChanged("IdentityProperties");
                }
                else
                {
                    (value as DataPropertyDefinitionCollection).Clear();
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    internal class ClassUniqueConstraintEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ClassDefinitionDesign cd = (ClassDefinitionDesign)context.Instance;
                ClassDefinition cls = cd.ClassDefinition;
                FdoConnection conn = cd.Connection;
                if (conn != null && !conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints))
                {
                    ICSharpCode.Core.MessageService.ShowError("Unique constraints not supported");
                    return value;
                }

                UniqueConstraintDialog diag = new UniqueConstraintDialog();
                List<string> dataProps = new List<string>();
                List<UniqueConstraintInfo> ucs = new List<UniqueConstraintInfo>();
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                        dataProps.Add(pd.Name);
                }
                diag.PropertyNames = dataProps;
                foreach (UniqueConstraint uc in cls.UniqueConstraints)
                {
                    List<string> tuple = new List<string>();
                    foreach (DataPropertyDefinition dp in uc.Properties)
                    {
                        tuple.Add(dp.Name);
                    }
                    ucs.Add(new UniqueConstraintInfo(tuple.ToArray()));
                }
                diag.Constraints = ucs;
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    //Rebuild unique constraint collection
                    cls.UniqueConstraints.Clear();
                    foreach (UniqueConstraintInfo uc in diag.Constraints)
                    {
                        UniqueConstraint constraint = new UniqueConstraint();
                        foreach (string name in uc.PropertyNames)
                        {
                            int idx = cls.Properties.IndexOf(name);
                            if (idx >= 0)
                                constraint.Properties.Add((DataPropertyDefinition)cls.Properties[idx]);
                        }
                        cls.UniqueConstraints.Add(constraint);
                    }
                    value = cls.UniqueConstraints;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
