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
using System.Drawing.Design;
using OSGeo.FDO.Schema;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal class GeometryPropertyLookupEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                FeatureClassDesign design = context.Instance as FeatureClassDesign;
                ClassDefinition cls = design.ClassDefinition;
                ListBox box = new ListBox();
                
                box.SelectionMode = SelectionMode.One;
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        box.Items.Add(pd.Name);
                    }
                }

                if (value != null)
                    box.SelectedItem = ((GeometricPropertyDefinition)value).Name;

                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                box.SelectedIndexChanged += delegate
                {
                    if (box.SelectedItem != null)
                        editorService.CloseDropDown();
                };

                editorService.DropDownControl(box);
                if (box.SelectedItem != null)
                {
                    int pidx = cls.Properties.IndexOf(box.SelectedItem.ToString());
                    if (pidx >= 0)
                    {
                        PropertyDefinition pd = cls.Properties[pidx];
                        if (pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
                            value = (GeometricPropertyDefinition)pd;
                    }
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
