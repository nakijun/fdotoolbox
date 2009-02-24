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
using System.Windows.Forms.Design;
using OSGeo.FDO.Schema;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class IdentityPropertyLookupEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                ClassDefinitionDesign design = (ClassDefinitionDesign)context.Instance;
                ClassDefinition cls = design.ClassDefinition;
                CheckedListBox box = new CheckedListBox();

                //box.SelectionMode = SelectionMode.MultiExtended;
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        box.Items.Add(pd.Name, false);
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

                //box.SelectedIndexChanged += delegate
                //{
                //    if (box.CheckedItems.Count > 0)
                //        editorService.CloseDropDown();
                //};

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
