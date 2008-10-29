namespace FdoToolbox.Lib.Controls
{
    partial class MapPreviewCtl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SharpMap.Map map1 = new SharpMap.Map();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapPreviewCtl));
            System.Drawing.Drawing2D.Matrix matrix1 = new System.Drawing.Drawing2D.Matrix();
            this.mapToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.btnZoomExtents = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mapImg = new SharpMap.Forms.MapImage();
            this.mapToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapImg)).BeginInit();
            this.SuspendLayout();
            // 
            // mapToolStrip
            // 
            this.mapToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnPan,
            this.btnZoomExtents,
            this.toolStripSeparator1,
            this.btnRefresh});
            this.mapToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mapToolStrip.Name = "mapToolStrip";
            this.mapToolStrip.Size = new System.Drawing.Size(525, 25);
            this.mapToolStrip.TabIndex = 0;
            this.mapToolStrip.Text = "toolStrip1";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::FdoToolbox.Lib.Properties.Resources.zoom_in;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::FdoToolbox.Lib.Properties.Resources.zoom_out;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnPan
            // 
            this.btnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPan.Image = global::FdoToolbox.Lib.Properties.Resources.icon_pan;
            this.btnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(23, 22);
            this.btnPan.Text = "Pan";
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
            // 
            // btnZoomExtents
            // 
            this.btnZoomExtents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomExtents.Image = global::FdoToolbox.Lib.Properties.Resources.icon_fitwindow;
            this.btnZoomExtents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomExtents.Name = "btnZoomExtents";
            this.btnZoomExtents.Size = new System.Drawing.Size(23, 22);
            this.btnZoomExtents.Text = "Zoom Extents";
            this.btnZoomExtents.Click += new System.EventHandler(this.btnZoomExtents_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::FdoToolbox.Lib.Properties.Resources.page_refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(65, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 404);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(525, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mapImg
            // 
            this.mapImg.ActiveTool = SharpMap.Forms.MapImage.Tools.None;
            this.mapImg.BackColor = System.Drawing.Color.White;
            this.mapImg.Cursor = System.Windows.Forms.Cursors.Cross;
            this.mapImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapImg.Location = new System.Drawing.Point(0, 25);
            map1.BackColor = System.Drawing.Color.Transparent;
            map1.Center = ((SharpMap.Geometries.Point)(resources.GetObject("map1.Center")));
            map1.Layers = ((System.Collections.Generic.List<SharpMap.Layers.ILayer>)(resources.GetObject("map1.Layers")));
            map1.MapTransform = matrix1;
            map1.MaximumZoom = 1.7976931348623157E+308;
            map1.MinimumZoom = 0;
            map1.PixelAspectRatio = 1;
            map1.Size = new System.Drawing.Size(100, 50);
            map1.Zoom = 1;
            this.mapImg.Map = map1;
            this.mapImg.Name = "mapImg";
            this.mapImg.QueryLayerIndex = 0;
            this.mapImg.Size = new System.Drawing.Size(525, 379);
            this.mapImg.TabIndex = 2;
            this.mapImg.TabStop = false;
            this.mapImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapImg_MouseClick);
            // 
            // MapPreviewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapImg);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mapToolStrip);
            this.Name = "MapPreviewCtl";
            this.Size = new System.Drawing.Size(525, 426);
            this.mapToolStrip.ResumeLayout(false);
            this.mapToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mapToolStrip;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnZoomExtents;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private SharpMap.Forms.MapImage mapImg;
    }
}
