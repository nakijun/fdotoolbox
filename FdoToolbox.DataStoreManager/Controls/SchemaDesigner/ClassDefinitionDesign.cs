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

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    /// <summary>
    /// Designer object for FDO class definitions
    /// </summary>
    internal abstract class ClassDefinitionDesign : INotifyPropertyChanged
    {
        private OSGeo.FDO.Schema.ClassDefinition _classDef;
        private FdoConnection _conn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDefinitionDesign"/> class.
        /// </summary>
        /// <param name="cd">The cd.</param>
        /// <param name="conn">The conn.</param>
        public ClassDefinitionDesign(OSGeo.FDO.Schema.ClassDefinition cd, FdoConnection conn)
        {
            _classDef = cd;
            _conn = conn;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [Browsable(false)]
        public FdoConnection Connection
        {
            get { return _conn; }
        }

        /// <summary>
        /// Gets the class definition.
        /// </summary>
        /// <value>The class definition.</value>
        [Browsable(false)]
        public OSGeo.FDO.Schema.ClassDefinition ClassDefinition
        {
            get { return _classDef; }
        }

        /// <summary>
        /// Gets or sets the base class.
        /// </summary>
        /// <value>The base class.</value>
        [Browsable(true)]
        [Description("The base class definition which this class is derived from")]
        [Editor(typeof(BaseClassBrowserEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.ClassDefinition BaseClass
        {
            get { return _classDef.BaseClass; }
            set 
            { 
                if (value != null)
                {
                    _classDef.BaseClass = value;
                    _classDef.SetBaseProperties(value.Properties);
                }
                else
                {
                    _classDef.BaseClass = null;
                    _classDef.SetBaseProperties(null);
                }
                FirePropertyChanged("BaseClass");
            }
        }

        /// <summary>
        /// Gets the type of the class.
        /// </summary>
        /// <value>The type of the class.</value>
        [Description("The type of class")]
        public ClassType ClassType
        {
            get { return _classDef.ClassType; }
        }

        /// <summary>
        /// Gets or sets the identity properties.
        /// </summary>
        /// <value>The identity properties.</value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is abstract.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is abstract; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is computed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is computed; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
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

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Description("The description of this class")]
        public string Description
        {
            get { return _classDef.Description; }
            set
            {
                _classDef.Description = value;
                FirePropertyChanged("Description");
            }
        }

        /// <summary>
        /// Gets or sets the unique constraints.
        /// </summary>
        /// <value>The unique constraints.</value>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    internal class BaseClassBrowserEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ClassDefinitionDesign ad = (ClassDefinitionDesign)context.Instance;
                ClassDefinition cd = ad.ClassDefinition;
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

    /// <summary>
    /// Designer object for edition class identity properties
    /// </summary>
    internal class ClassIdentityPropertyLookupEditor : UITypeEditor
    {
        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
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
