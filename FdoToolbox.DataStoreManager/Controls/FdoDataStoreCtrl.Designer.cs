namespace FdoToolbox.DataStoreManager.Controls
{
    partial class FdoDataStoreCtrl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.schemaView = new FdoToolbox.DataStoreManager.Controls.FdoSchemaView();
            this.spatialContextView = new FdoToolbox.DataStoreManager.Controls.FdoSpatialContextView();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSaveXmlConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.sDFFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLiteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnApply = new System.Windows.Forms.ToolStripDropDownButton();
            this.featureSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allFeatureSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialContextsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.everythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImport,
            this.btnExport,
            this.toolStripSeparator3,
            this.btnApply,
            this.btnReload});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(596, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.schemaView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spatialContextView);
            this.splitContainer1.Size = new System.Drawing.Size(596, 400);
            this.splitContainer1.SplitterDistance = 292;
            this.splitContainer1.TabIndex = 1;
            // 
            // fdoSchemaView1
            // 
            this.schemaView.CanCollapse = false;
            this.schemaView.CanExpand = false;
            this.schemaView.ContentBackgroundColor = System.Drawing.SystemColors.Control;
            this.schemaView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schemaView.HeaderBackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.schemaView.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.schemaView.HeaderText = "Feature Schemas";
            this.schemaView.Location = new System.Drawing.Point(0, 0);
            this.schemaView.Name = "fdoSchemaView1";
            this.schemaView.Size = new System.Drawing.Size(596, 292);
            this.schemaView.TabIndex = 0;
            // 
            // fdoSpatialContextView1
            // 
            this.spatialContextView.CanCollapse = false;
            this.spatialContextView.CanExpand = false;
            this.spatialContextView.ContentBackgroundColor = System.Drawing.SystemColors.Control;
            this.spatialContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spatialContextView.HeaderBackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.spatialContextView.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spatialContextView.HeaderText = "Spatial Contexts";
            this.spatialContextView.Location = new System.Drawing.Point(0, 0);
            this.spatialContextView.Name = "fdoSpatialContextView1";
            this.spatialContextView.Size = new System.Drawing.Size(596, 104);
            this.spatialContextView.TabIndex = 0;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnImport
            // 
            this.btnImport.Image = global::FdoToolbox.DataStoreManager.Images.folder;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(63, 22);
            this.btnImport.Text = "Import";
            this.btnImport.ToolTipText = "Import an existing XML configuration";
            // 
            // btnExport
            // 
            this.btnExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveXmlConfig,
            this.toolStripSeparator1,
            this.sDFFileToolStripMenuItem,
            this.sQLiteFileToolStripMenuItem});
            this.btnExport.Image = global::FdoToolbox.DataStoreManager.Images.disk;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(69, 22);
            this.btnExport.Text = "Export";
            // 
            // btnSaveXmlConfig
            // 
            this.btnSaveXmlConfig.Image = global::FdoToolbox.DataStoreManager.Images.page_white_code;
            this.btnSaveXmlConfig.Name = "btnSaveXmlConfig";
            this.btnSaveXmlConfig.Size = new System.Drawing.Size(234, 22);
            this.btnSaveXmlConfig.Text = "XML Configuration Document";
            this.btnSaveXmlConfig.ToolTipText = "Export this current configuration to a XML file";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(231, 6);
            // 
            // sDFFileToolStripMenuItem
            // 
            this.sDFFileToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.database;
            this.sDFFileToolStripMenuItem.Name = "sDFFileToolStripMenuItem";
            this.sDFFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.sDFFileToolStripMenuItem.Text = "SDF File";
            this.sDFFileToolStripMenuItem.ToolTipText = "Apply current schema and spatial context to a new SDF file";
            // 
            // sQLiteFileToolStripMenuItem
            // 
            this.sQLiteFileToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.database;
            this.sQLiteFileToolStripMenuItem.Name = "sQLiteFileToolStripMenuItem";
            this.sQLiteFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.sQLiteFileToolStripMenuItem.Text = "SQLite File";
            this.sQLiteFileToolStripMenuItem.ToolTipText = "Apply this current schema and spatial context to a SQLite file";
            // 
            // btnApply
            // 
            this.btnApply.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featureSchemaToolStripMenuItem,
            this.allFeatureSchemaToolStripMenuItem,
            this.spatialContextsToolStripMenuItem,
            this.toolStripSeparator2,
            this.everythingToolStripMenuItem});
            this.btnApply.Image = global::FdoToolbox.DataStoreManager.Images.application_edit;
            this.btnApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(67, 22);
            this.btnApply.Text = "Apply";
            // 
            // featureSchemaToolStripMenuItem
            // 
            this.featureSchemaToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.chart_organisation;
            this.featureSchemaToolStripMenuItem.Name = "featureSchemaToolStripMenuItem";
            this.featureSchemaToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.featureSchemaToolStripMenuItem.Text = "Selected Feature Schema";
            this.featureSchemaToolStripMenuItem.ToolTipText = "Apply the currently selected feature schema";
            // 
            // allFeatureSchemaToolStripMenuItem
            // 
            this.allFeatureSchemaToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.chart_organisation;
            this.allFeatureSchemaToolStripMenuItem.Name = "allFeatureSchemaToolStripMenuItem";
            this.allFeatureSchemaToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.allFeatureSchemaToolStripMenuItem.Text = "All Feature Schemas";
            this.allFeatureSchemaToolStripMenuItem.ToolTipText = "Apply all feature schemas";
            // 
            // spatialContextsToolStripMenuItem
            // 
            this.spatialContextsToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.world;
            this.spatialContextsToolStripMenuItem.Name = "spatialContextsToolStripMenuItem";
            this.spatialContextsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.spatialContextsToolStripMenuItem.Text = "Spatial Contexts";
            this.spatialContextsToolStripMenuItem.ToolTipText = "Apply all spatial contexts";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // everythingToolStripMenuItem
            // 
            this.everythingToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.disk_multiple;
            this.everythingToolStripMenuItem.Name = "everythingToolStripMenuItem";
            this.everythingToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.everythingToolStripMenuItem.Text = "Everything";
            this.everythingToolStripMenuItem.ToolTipText = "Apply all feature schemas and spatial contexts";
            // 
            // btnReload
            // 
            this.btnReload.Image = global::FdoToolbox.DataStoreManager.Images.arrow_refresh;
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(63, 22);
            this.btnReload.Text = "Reload";
            this.btnReload.ToolTipText = "Reloads all schemas, mappings and spatial contexts from the current connection";
            // 
            // FdoDataStoreCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FdoDataStoreCtrl";
            this.Size = new System.Drawing.Size(596, 425);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private FdoSchemaView schemaView;
        private FdoSpatialContextView spatialContextView;
        private System.Windows.Forms.ToolStripDropDownButton btnExport;
        private System.Windows.Forms.ToolStripMenuItem btnSaveXmlConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem sDFFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLiteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton btnApply;
        private System.Windows.Forms.ToolStripMenuItem featureSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFeatureSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spatialContextsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem everythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripButton btnReload;
    }
}
