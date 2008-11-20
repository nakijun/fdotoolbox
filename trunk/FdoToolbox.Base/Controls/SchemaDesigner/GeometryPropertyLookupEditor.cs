using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using OSGeo.FDO.Schema;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class GeometryPropertyLookupEditor : UITypeEditor
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
