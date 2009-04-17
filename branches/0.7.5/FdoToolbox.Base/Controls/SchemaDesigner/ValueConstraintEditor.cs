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

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class ValueConstraintEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                DataPropertyDefinitionDesign design = (DataPropertyDefinitionDesign)context.Instance;
                FdoToolbox.Core.Feature.FdoConnection conn = design.Connection;
                if (conn == null)
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
                    bool supportsList = conn.Capability.GetBooleanCapability(FdoToolbox.Core.Feature.CapabilityType.FdoCapabilityType_SupportsValueConstraintsList).Value;
                    bool supportsRange = conn.Capability.GetBooleanCapability(FdoToolbox.Core.Feature.CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints).Value ||
                        conn.Capability.GetBooleanCapability(FdoToolbox.Core.Feature.CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints).Value;

                    if (supportsList || supportsRange)
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
                    else
                    {
                        ICSharpCode.Core.MessageService.ShowError("Value constraints not supported");
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
