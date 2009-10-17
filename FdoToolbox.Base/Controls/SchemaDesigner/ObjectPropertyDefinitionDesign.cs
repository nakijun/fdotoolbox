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
using FdoToolbox.Core.Feature;
using System.ComponentModel;
using OSGeo.FDO.Schema;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    internal class ObjectPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private ObjectPropertyDefinition _objDef;
        
        public ObjectPropertyDefinitionDesign(ObjectPropertyDefinition op, FdoConnection conn)
            : base(op, conn)
        {
            _objDef = op;
        }

        internal ClassDefinition GetParent()
        {
            return _objDef.Parent as ClassDefinition;
        }

        [Description("The reference to the class definition that defines the type of this property")]
        [Editor(typeof(ObjectClassBrowserEditor), typeof(UITypeEditor))]
        public ClassDefinition Class
        {
            get { return _objDef.Class; }
            set
            {
                _objDef.Class = value;
                this.FirePropertyChanged("Class");
            }
        }

        [Description("The referecnes to a Data Property Definition to use for uniquely identifying instances of the contained class within a single parent object instance. This value is only used for the ObjectType_Collection and ObjectType_OrderedCollection object property types. The Data Property Definition must belong to the Class Definition that defines the type of this property.")]
        [Editor(typeof(ObjectClassPropertyBrowserEditor), typeof(UITypeEditor))]
        public DataPropertyDefinition IdentityProperty
        {
            get { return _objDef.IdentityProperty; }
            set
            {
                _objDef.IdentityProperty = value;
                this.FirePropertyChanged("IdentityProperty");
            }
        }

        [Description("The type of this object property (value, collection or ordered collection)")]
        public ObjectType ObjectType
        {
            get { return _objDef.ObjectType; }
            set
            {
                _objDef.ObjectType = value;
                this.FirePropertyChanged("ObjectType");
            }
        }

        [Description("The order type of this object property (ascending or descending). This property is only applicable if the property type is set to ObjectType_OrderedCollection")]
        public OrderType OrderType
        {
            get { return _objDef.OrderType; }
            set
            {
                _objDef.OrderType = value;
                this.FirePropertyChanged("OrderType");
            }
        }
    }

    internal class ObjectClassBrowserEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ObjectPropertyDefinitionDesign ad = (ObjectPropertyDefinitionDesign)context.Instance;
                ClassDefinition cd = ad.GetParent();
                if (cd != null)
                {
                    FeatureSchema schema = cd.Parent as FeatureSchema;
                    if (schema != null)
                    {
                        ListBox lb = new ListBox();
                        lb.SelectionMode = SelectionMode.One;

                        foreach (ClassDefinition cls in schema.Classes)
                        {
                            if (cls.Name != cd.Name)
                                lb.Items.Add(cls.Name);
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
                            int idx = schema.Classes.IndexOf(lb.SelectedItem.ToString());
                            if (idx >= 0)
                                value = schema.Classes[idx];
                        }
                    }
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    internal class ObjectClassPropertyBrowserEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ObjectPropertyDefinitionDesign od = (ObjectPropertyDefinitionDesign)context.Instance;
                DataPropertyDefinitionCollection dpcol = (DataPropertyDefinitionCollection)value;
                ClassDefinition cd = od.Class;
                if (cd != null)
                {
                    CheckedListBox box = new CheckedListBox();

                    foreach (PropertyDefinition pd in cd.Properties)
                    {
                        DataPropertyDefinition dp = pd as DataPropertyDefinition;
                        if (dp != null)
                            box.Items.Add(dp.Name, dpcol.Contains(dp));
                    }

                    IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    editorService.DropDownControl(box);

                    if (box.CheckedItems.Count > 0)
                    {
                        DataPropertyDefinitionCollection dpdc = new DataPropertyDefinitionCollection(cd);
                        foreach (object obj in box.CheckedItems)
                        {
                            int pidx = cd.Properties.IndexOf(obj.ToString());
                            if (pidx >= 0)
                            {
                                PropertyDefinition pd = cd.Properties[pidx];
                                if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                                    dpdc.Add((DataPropertyDefinition)pd);
                            }
                        }
                        value = dpdc;
                    }
                    else
                    {
                        dpcol.Clear();
                    }
                }
                else
                {
                    ICSharpCode.Core.MessageService.ShowError("No referenced class specified");
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
