using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class SpatialContextAssociationEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                GeometricPropertyDefinitionDesign design = (GeometricPropertyDefinitionDesign)context.Instance;
                if (design.Connection != null)
                {
                    SpatialContextInfo sci = FdoSpatialContextBrowserDlg.GetSpatialContext(design.Connection);
                    if (sci != null)
                        value = sci.Name;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
