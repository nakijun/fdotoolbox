using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpMap.Forms;
using SharpMap;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.SharpMapProvider;
using SharpMap.Layers;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoMapPreview : UserControl, IFdoMapView
    {
        private FdoMapPreviewPresenter _presenter;
        private MapImage img;


        public FdoMapPreview()
        {
            InitializeComponent();
            img = new MapImage();
            img.Map = new Map();
            img.Dock = DockStyle.Fill;
            mapContentPanel.Controls.Add(img);

            _presenter = new FdoMapPreviewPresenter(this, img);
        }

        private FdoInMemoryProvider _provider = new FdoInMemoryProvider();

        public FdoFeatureTable DataSource
        {
            set
            {
                _provider.DataSource = value;
                if (value != null && img.Map.Layers.Count == 0)
                {
                    VectorLayer layer = new VectorLayer("Preview", _provider);
                    layer.Style.Fill = Brushes.Transparent;
                    layer.Style.EnableOutline = true;
                    img.Map.Layers.Add(layer);
                }
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _presenter.ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _presenter.ZoomOut();
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            _presenter.Pan();
        }

        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            _presenter.ZoomExtents();
        }

        public bool ZoomInChecked
        {
            set { btnZoomIn.Checked = value; }
        }

        public bool ZoomOutChecked
        {
            set { btnZoomOut.Checked = value; }
        }

        public bool SelectChecked
        {
            set { btnSelect.Checked = value; }
        }

        public bool PanChecked
        {
            set { btnPan.Checked = value; }
        }

        public string StatusText
        {
            set { lblStatus.Text = value; }
        }
    }
}
