#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Forms
{
    public partial class ClassTypePicker : Form
    {
        internal ClassTypePicker()
        {
            InitializeComponent();
        }

        public ClassTypePicker(ClassType[] types)
            : this()
        {
            foreach (ClassType type in types)
            {
                switch (type)
                {
                    case ClassType.ClassType_Class:
                        rdClass.Enabled = true;
                        break;
                    case ClassType.ClassType_FeatureClass:
                        rdFeature.Enabled = true;
                        break;
                    case ClassType.ClassType_NetworkClass:
                        rdNetwork.Enabled = true;
                        break;
                    case ClassType.ClassType_NetworkLayerClass:
                        rdNetworkLayer.Enabled = true;
                        break;
                    case ClassType.ClassType_NetworkLinkClass:
                        rdNetworkLink.Enabled = true;
                        break;
                    case ClassType.ClassType_NetworkNodeClass:
                        rdNetworkNode.Enabled = true;
                        break;
                }
            }
        }

        public ClassType? SelectedType
        {
            get 
            {
                if (rdClass.Checked)
                    return ClassType.ClassType_Class;
                else if (rdFeature.Checked)
                    return ClassType.ClassType_FeatureClass;
                else if (rdNetwork.Checked)
                    return ClassType.ClassType_NetworkClass;
                else if (rdNetworkLayer.Checked)
                    return ClassType.ClassType_NetworkLayerClass;
                else if (rdNetworkLink.Checked)
                    return ClassType.ClassType_NetworkLinkClass;
                else if (rdNetworkNode.Checked)
                    return ClassType.ClassType_NetworkNodeClass;
                else
                    return null;
            }
        }

        public static ClassType? GetClassType(ConnectionInfo connInfo)
        {
            ClassTypePicker diag = new ClassTypePicker(connInfo.Connection.SchemaCapabilities.ClassTypes);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.SelectedType;
            }
            return null;
        }

        private void Class_CheckChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}