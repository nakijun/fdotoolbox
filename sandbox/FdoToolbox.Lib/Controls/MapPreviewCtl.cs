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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpMap;
using SharpMap.Layers;
using FdoToolbox.Lib.ClientServices;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Feature;
using FdoToolbox.Lib.SharpMapProvider;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Lib.Controls
{
    public partial class MapPreviewCtl : UserControl
    {
        public MapPreviewCtl()
        {
            InitializeComponent();
            mapImg.Map = new Map();
        }

        private FdoInMemoryProvider _provider = new FdoInMemoryProvider();

        public FdoDataTable DataSource
        {
            get { return _provider.DataSource; }
            set 
            { 
                _provider.DataSource = value;
                if (value != null && mapImg.Map.Layers.Count == 0)
                {
                    VectorLayer layer = new VectorLayer("Preview", _provider);
                    layer.Style.Fill = Brushes.Transparent;
                    layer.Style.EnableOutline = true;
                    mapImg.Map.Layers.Add(layer);
                }
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            ZoomExtents();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshMap();
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            Pan();
        }

        private void ZoomIn()
        {
            ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomIn);
        }

        private void ChangeTool(SharpMap.Forms.MapImage.Tools tool)
        {
            mapImg.ActiveTool = tool;

            foreach (ToolStripItem tsi in mapToolStrip.Items)
            {
                ToolStripButton btn = tsi as ToolStripButton;
                if (btn != null)
                {
                    btn.Checked = false;
                }
            }

            switch (tool)
            {
                case SharpMap.Forms.MapImage.Tools.Pan:
                    btnPan.Checked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomIn:
                    btnZoomIn.Checked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomOut:
                    btnZoomOut.Checked = true;
                    break;
            }
        }

        private void ZoomOut()
        {
            ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomOut);
        }

        public void ZoomExtents()
        {
            mapImg.Map.ZoomToExtents();
            RefreshMap();
        }

        public void RefreshMap()
        {
            mapImg.Refresh();
        }

        private void Pan()
        {
            ChangeTool(SharpMap.Forms.MapImage.Tools.Pan);
        }

        private void mapImg_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        public void Reset()
        {
            mapImg.Map.Layers.Clear();
            RefreshMap();
        }

        private void mapImg_MouseMove(SharpMap.Geometries.Point WorldPos, MouseEventArgs ImagePos)
        {
            positionLabel.Text = string.Format("X: {0} Y: {0}", WorldPos.X, WorldPos.Y);
        }
    }
}
