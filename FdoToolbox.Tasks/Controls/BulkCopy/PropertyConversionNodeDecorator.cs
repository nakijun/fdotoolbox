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
using System.Windows.Forms;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    public class PropertyConversionNodeDecorator
    {
        public TreeNode _propertyNode;

        public PropertyConversionNodeDecorator(TreeNode propertyNode)
        {
            _propertyNode = propertyNode;
            _propertyNode.Nodes.Clear();
            Init();
        }

        private void Init()
        {
            _propertyNode.Nodes.Add("Set to NULL on failed conversion");
            _propertyNode.Nodes.Add("Truncate value");

            this.NullOnFailedConversion = true;
            this.Truncate = true;
        }

        public bool NullOnFailedConversion
        {
            get 
            {
                return Convert.ToBoolean(_propertyNode.Nodes[0].Tag);
            }
            set 
            {
                _propertyNode.Nodes[0].Tag = value;
                _propertyNode.Nodes[0].Text = "Set to NULL on failed conversion: " + value;
            } 
        }

        public bool Truncate
        {
            get
            {
                return Convert.ToBoolean(_propertyNode.Nodes[1].Tag);
            }
            set
            {
                _propertyNode.Nodes[1].Tag = value;
                _propertyNode.Nodes[1].Text = "Truncate value: " + value;
            }  
        }
    }
}
