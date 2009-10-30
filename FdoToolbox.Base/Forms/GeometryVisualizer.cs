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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Forms
{
    public partial class GeometryVisualizer : Form
    {
        private SharpMap.Layers.VectorLayer _layer;
        private SharpMap.Forms.MapImage _mapCtl;

        public GeometryVisualizer()
        {
            InitializeComponent();
            _layer = new SharpMap.Layers.VectorLayer("Preview");
            _mapCtl = new SharpMap.Forms.MapImage();
            _mapCtl.Map = new SharpMap.Map();
            _mapCtl.Map.Layers.Add(_layer);
            _mapCtl.Dock = DockStyle.Fill;
            mapPanel.Controls.Add(_mapCtl);
        }

        public string GeometryText
        {
            get { return txtGeometry.Text; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                SharpMap.Geometries.Geometry geom = SharpMap.Geometries.Geometry.GeomFromText(txtGeometry.Text);
                _layer.DataSource = new SharpMap.Data.Providers.GeometryProvider(geom);
                SharpMap.Geometries.BoundingBox bbox = _mapCtl.Map.GetExtents();
                bbox = bbox.Grow(2.0, 2.0);
                _mapCtl.Map.ZoomToBox(bbox);
                _mapCtl.Refresh();
                btnOK.Enabled = true;
            }
            catch (Exception)
            {
                MessageService.ShowError("The specified text does not constitute a valid geometry");
                btnOK.Enabled = false;
            }
        }
    }
}