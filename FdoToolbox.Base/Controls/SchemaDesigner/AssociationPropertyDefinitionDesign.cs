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

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class AssociationPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private OSGeo.FDO.Schema.AssociationPropertyDefinition _assocDef;

        public AssociationPropertyDefinitionDesign(OSGeo.FDO.Schema.AssociationPropertyDefinition ap)
            : base(ap)
        {
            _assocDef = ap;
        }

        public AssociationPropertyDefinitionDesign(OSGeo.FDO.Schema.AssociationPropertyDefinition ap, FdoConnection conn)
            : base(ap, conn)
        {
            _assocDef = ap;
        }

        [Description("A reference to the associated class. ")]
        public OSGeo.FDO.Schema.ClassDefinition AssociatedClass
        {
            get { return _assocDef.AssociatedClass; }
            set
            {
                _assocDef.AssociatedClass = value;
                this.FirePropertyChanged("AssociatedClass");
            }
        }

        [Description("The delete rule")]
        public OSGeo.FDO.Schema.DeleteRule DeleteRule
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
        public OSGeo.FDO.Schema.DataPropertyDefinitionCollection IdentityProperties
        {
            get { return _assocDef.IdentityProperties; }
            set
            {
                _assocDef.IdentityProperties.Clear();
                foreach (OSGeo.FDO.Schema.DataPropertyDefinition dp in value)
                {
                    _assocDef.IdentityProperties.Add(dp);
                }
                this.FirePropertyChanged("IdentityProperties");
            }
        }

        [Description("The collection of properties of the associated class that are used as key for this association. The number, order and types should match the IdentityProperties. If the reverse identity collection is empty, then the associated class identity properties will be used. The properties of the collection should already exist on the associated class.")]
        [Editor(typeof(AssocReverseIdentityPropertyLookupEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.DataPropertyDefinitionCollection ReverseIdentityProperties
        {
            get { return _assocDef.ReverseIdentityProperties; }
            set
            {
                _assocDef.ReverseIdentityProperties.Clear();
                foreach (OSGeo.FDO.Schema.DataPropertyDefinition dp in value)
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
    }

    internal class AssocReverseIdentityPropertyLookupEditor : UITypeEditor
    {
    }
}
