#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    public partial class GeometricPropertyCtrl : UserControl
    {
        public GeometricPropertyCtrl()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;

        public GeometricPropertyCtrl(GeometricPropertyDefinitionDecorator p, SchemaDesignContext context)
            : this()
        {
            _context = context;

            txtName.DataBindings.Add("Text", p, "Name");
            txtDescription.DataBindings.Add("Text", p, "Description");
            chkElevation.DataBindings.Add("Checked", p, "HasElevation");
            chkMeasure.DataBindings.Add("Checked", p, "HasMeasure");

            cmbSpatialContext.DisplayMember = "Name";
            cmbSpatialContext.DataSource = _context.SpatialContexts;

            var allTypes = (GeometricType[])Enum.GetValues(typeof(GeometricType));
            //Fill the list
            foreach (GeometricType gt in allTypes)
            {
                chkGeometryTypes.Items.Add(gt, false);
            }

            //Now check the ones which are identity
            foreach (GeometricType gt in allTypes)
            {
                if ((p.GeometryTypes & (int)gt) > 0)
                {
                    var idx = chkGeometryTypes.Items.IndexOf(gt);
                    if (idx >= 0)
                        chkGeometryTypes.SetItemChecked(idx, true);
                }
            }

            //Now wire up change listener
            chkGeometryTypes.ItemCheck += (s, e) =>
            {
                p.GeometryTypes = GetUpdatedGeometryMask();
            };
        }

        public int GetUpdatedGeometryMask()
        {
            GeometricType mask = default(GeometricType);
            if (chkGeometryTypes.CheckedItems.Count > 0)
            {
                foreach (GeometricType gt in chkGeometryTypes.CheckedItems)
                {
                    mask |= gt;
                }
            }
            return (int)mask;
        }
    }
}
