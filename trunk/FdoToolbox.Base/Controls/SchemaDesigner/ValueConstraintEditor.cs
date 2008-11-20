using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class ValueConstraintEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                DataPropertyDefinitionDesign design = (DataPropertyDefinitionDesign)context.Instance;
                if (design.Connection == null)
                {
                    if (value == null)
                    {
                        PropertyValueConstraint constraint = ValueConstraintDialog.GetConstraint();
                        if(constraint != null)
                            value = constraint;
                    }
                    else
                    {
                        PropertyValueConstraint constraint = ValueConstraintDialog.GetConstraint((PropertyValueConstraint)value);
                        if (constraint != null)
                            value = constraint;
                    }
                }
                else
                {
                    if (value == null)
                    {
                        PropertyValueConstraint constraint = ValueConstraintDialog.GetConstraint(design.Connection);
                        if (constraint != null)
                            value = constraint;
                    }
                    else
                    {
                        PropertyValueConstraint constraint = ValueConstraintDialog.GetConstraint((PropertyValueConstraint)value, design.Connection);
                        if (constraint != null)
                            value = constraint;
                    }
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
