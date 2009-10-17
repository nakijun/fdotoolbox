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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    internal class AssociationPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private AssociationPropertyDefinition _assocDef;

        public AssociationPropertyDefinitionDesign(AssociationPropertyDefinition ap, FdoConnection conn)
            : base(ap, conn)
        {
            _assocDef = ap;
        }

        internal ClassDefinition GetParent()
        {
            return _assocDef.Parent as ClassDefinition;
        }

        [Description("A reference to the associated class.")]
        [Editor(typeof(AssocClassBrowserEditor), typeof(UITypeEditor))]
        public ClassDefinition AssociatedClass
        {
            get { return _assocDef.AssociatedClass; }
            set
            {
                _assocDef.AssociatedClass = value;
                this.FirePropertyChanged("AssociatedClass");
            }
        }

        [Description("The delete rule")]
        public DeleteRule DeleteRule
        {
            get { return _assocDef.DeleteRule; }
            set
            {
                _assocDef.DeleteRule = value;
                this.FirePropertyChanged("DeleteRule");
            }
        }

        [Description("The collection of properties of the current class that are used as key for this association. Initially, this collection is empty. The user can optionally add any number of properties. If the collection is left empty, the identity properties of the associated class are added to the current class. The number, order and types should match the property of the ReverseIdentityProperties collection. All properties in the collection should already exist in the containing class. This is needed in case the current class already has properties (foreign keys) that are used to reference the associated feature.")]
        [Editor(typeof(AssocIdentityPropertyLookupEditor), typeof(UITypeEditor))]
        public DataPropertyDefinitionCollection IdentityProperties
        {
            get { return _assocDef.IdentityProperties; }
            set
            {
                _assocDef.IdentityProperties.Clear();
                foreach (DataPropertyDefinition dp in value)
                {
                    _assocDef.IdentityProperties.Add(dp);
                }
                this.FirePropertyChanged("IdentityProperties");
            }
        }

        [Description("The collection of properties of the associated class that are used as key for this association. The number, order and types should match the IdentityProperties. If the reverse identity collection is empty, then the associated class identity properties will be used. The properties of the collection should already exist on the associated class.")]
        [Editor(typeof(AssocReverseIdentityPropertyLookupEditor), typeof(UITypeEditor))]
        public DataPropertyDefinitionCollection ReverseIdentityProperties
        {
            get { return _assocDef.ReverseIdentityProperties; }
            set
            {
                _assocDef.ReverseIdentityProperties.Clear();
                foreach (DataPropertyDefinition dp in value)
                {
                    _assocDef.ReverseIdentityProperties.Add(dp);
                }
                this.FirePropertyChanged("ReverseIdentityProperties");
            }
        }

        [Description("Determines if this property is read-only")]
        public bool IsReadOnly
        {
            get { return _assocDef.IsReadOnly; }
        }

        [Description("The lock cascade option")]
        public bool LockCascade
        {
            get { return _assocDef.LockCascade; }
            set
            {
                _assocDef.LockCascade = value;
                this.FirePropertyChanged("LockCascade");
            }
        }

        [Description("The association multiplicity from the property owner class side. The only possible values are 1 or m. If the multiplicity is set to 1, then only one instance of the owning class can be associated to a given instance of the associated class. If the multiplicity is set to m, then many instances of the owning class can be associated to the same instance of the associated class.")]
        [Editor(typeof(MultiplicityValueEditor), typeof(UITypeEditor))]
        public string Multiplicity
        {
            get { return _assocDef.Multiplicity; }
            set
            {
                _assocDef.Multiplicity = value;
                this.FirePropertyChanged("Multiplicity");
            }
        }

        [Description("The association multiplicity from the associated class side. The only possible values are 0 or 1. If the multiplicity is set to 0, then it is not mandatory to initialize the association property when a new object is created. If the multiplicity is set to 1, then the association property must be initialized when a new object is created.")]
        [Editor(typeof(ReverseMultiplicityValueEditor), typeof(UITypeEditor))]
        public string ReverseMultiplicity
        {
            get { return _assocDef.ReverseMultiplicity; }
            set
            {
                _assocDef.ReverseMultiplicity = value;
                this.FirePropertyChanged("ReverseMultiplicity");
            }
        }

        [Description("The name of this association as seen by the associated class. This is an optional parameter that can be provided if the navigation back from the associated class is needed. This property will appear as read-only property on the associated class. For example let's assume that we have an Accident class that associated to a Road class by an association called Road? If we wanted to find all the accidents associated to a given road, we will need to provide a reverse property name of the road association property. In this case an appropriate value for such a name would be accident. The filter that can be used to find all the accidents would look like: accident not null.")]
        public string ReverseName
        {
            get { return _assocDef.ReverseName; }
            set
            {
                _assocDef.ReverseName = value;
                this.FirePropertyChanged("ReverseName");
            }
        }
    }

    internal class AssocClassBrowserEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                AssociationPropertyDefinitionDesign ad = (AssociationPropertyDefinitionDesign)context.Instance;
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

    internal class MultiplicityValueEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ListBox cb = new ListBox();
                cb.Items.Add("1");
                cb.Items.Add("m");
                cb.SelectionMode = SelectionMode.One;

                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                editorService.DropDownControl(cb);

                if (cb.SelectedItem != null)
                {
                    value = cb.SelectedItem;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    internal class ReverseMultiplicityValueEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ListBox cb = new ListBox();
                cb.Items.Add("0");
                cb.Items.Add("1");
                cb.SelectionMode = SelectionMode.One;

                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                editorService.DropDownControl(cb);

                if (cb.SelectedItem != null)
                {
                    value = cb.SelectedItem;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    internal class AssocIdentityPropertyLookupEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                DataPropertyDefinitionCollection dpcol = (DataPropertyDefinitionCollection)value;
                AssociationPropertyDefinitionDesign ad = (AssociationPropertyDefinitionDesign)context.Instance;
                ClassDefinition cd = ad.GetParent();
                if (cd != null)
                {
                    CheckedListBox box = new CheckedListBox();

                    //Grab all the "foreign key" candidates
                    foreach (PropertyDefinition pd in cd.Properties)
                    {
                        DataPropertyDefinition dp = pd as DataPropertyDefinition;
                        if(dp != null)
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
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    internal class AssocReverseIdentityPropertyLookupEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                DataPropertyDefinitionCollection dpcol = (DataPropertyDefinitionCollection)value;
                AssociationPropertyDefinitionDesign ad = (AssociationPropertyDefinitionDesign)context.Instance;
                ClassDefinition cd = ad.AssociatedClass;
                if (cd == null)
                {
                    ICSharpCode.Core.MessageService.ShowError("Please specify the associated class");
                    return value;
                }
                else
                {
                    CheckedListBox box = new CheckedListBox();
                    foreach (DataPropertyDefinition dp in cd.IdentityProperties)
                    {
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
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
