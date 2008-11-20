using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class GeometryTypeEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                GeometricPropertyDefinitionDesign design = (GeometricPropertyDefinitionDesign)context.Instance;
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                GeometryTypeCtl ctl = null;
                if (design.Connection == null)
                    ctl = new GeometryTypeCtl();
                else
                    ctl = new GeometryTypeCtl(design.Connection);

                ctl.GeometryTypes = Convert.ToInt32(value);
                editorService.DropDownControl(ctl);
                value = ctl.GeometryTypes;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
