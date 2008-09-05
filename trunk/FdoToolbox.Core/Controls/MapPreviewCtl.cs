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
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Feature;
using FdoToolbox.Core.SharpMapProvider;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core.Controls
{
    public partial class MapPreviewCtl : UserControl
    {
        public MapPreviewCtl()
        {
            InitializeComponent();
            mapImg.Map = new Map();
            this.Disposed += delegate 
            {
                if (_conn.ConnectionState != ConnectionState.ConnectionState_Closed)
                    _conn.Close();
                _conn.Dispose(); 
            };
        }

        private FdoInMemoryProvider _provider;

        private IConnection _conn;

        public void Initialize(IConnection conn)
        {
            if (_conn == null)
            {
                //Clone the connection
                _conn = FeatureAccessManager.GetConnectionManager().CreateConnection(conn.ConnectionInfo.ProviderName);
                _conn.ConnectionString = conn.ConnectionString;
            }
        }

        public void LoadQuery(FeatureQueryOptions options)
        {
            Reset();
            _conn.Open();
            using (FeatureService service = new FeatureService(_conn))
            {
                using (IFeatureReader reader = service.SelectFeatures(options))
                {
                    _provider = new FdoInMemoryProvider(reader);
                }
            }
            _conn.Close();
            VectorLayer layer = new VectorLayer("Preview", _provider);
            layer.Style.Fill = Brushes.Transparent;
            layer.Style.EnableOutline = true;
            mapImg.Map.Layers.Add(layer);
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
    }
}
