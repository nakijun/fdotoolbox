namespace FdoToolbox.Base.Controls
{
    partial class FdoDataPreviewCtl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.queryPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbQueryMode = new System.Windows.Forms.ToolStripComboBox();
            this.btnQuery = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.sDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resultTab = new System.Windows.Forms.TabControl();
            this.TAB_GRID = new System.Windows.Forms.TabPage();
            this.grdResults = new System.Windows.Forms.DataGridView();
            this.TAB_MAP = new System.Windows.Forms.TabPage();
            this.mapCtl = new FdoToolbox.Base.Controls.FdoMapPreview();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.resultTab.SuspendLayout();
            this.TAB_GRID.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).BeginInit();
            this.TAB_MAP.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.queryPanel);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultTab);
            this.splitContainer1.Size = new System.Drawing.Size(688, 480);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 0;
            // 
            // queryPanel
            // 
            this.queryPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.queryPanel.Location = new System.Drawing.Point(0, 25);
            this.queryPanel.Name = "queryPanel";
            this.queryPanel.Size = new System.Drawing.Size(684, 194);
            this.queryPanel.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cmbQueryMode,
            this.btnQuery,
            this.btnCancel,
            this.toolStripSeparator2,
            this.btnClear,
            this.btnSave,
            this.lblCount,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(684, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(66, 22);
            this.toolStripLabel1.Text = "Query Mode";
            // 
            // cmbQueryMode
            // 
            this.cmbQueryMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryMode.Name = "cmbQueryMode";
            this.cmbQueryMode.Size = new System.Drawing.Size(121, 25);
            this.cmbQueryMode.SelectedIndexChanged += new System.EventHandler(this.cmbQueryMode_SelectedIndexChanged);
            // 
            // btnQuery
            // 
            this.btnQuery.Image = global::FdoToolbox.Base.Images.table_go;
            this.btnQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(66, 22);
            this.btnQuery.Text = "Execute";
            this.btnQuery.ToolTipText = "Execute the query";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::FdoToolbox.Base.Images.cross;
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 22);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Cancel the running query";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::FdoToolbox.Base.Images.table_delete;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 22);
            this.btnClear.Text = "Clear Results";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sDFToolStripMenuItem});
            this.btnSave.Image = global::FdoToolbox.Base.Images.disk;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 22);
            this.btnSave.Text = "Save";
            // 
            // sDFToolStripMenuItem
            // 
            this.sDFToolStripMenuItem.Image = global::FdoToolbox.Base.Images.database;
            this.sDFToolStripMenuItem.Name = "sDFToolStripMenuItem";
            this.sDFToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.sDFToolStripMenuItem.Text = "SDF";
            // 
            // lblCount
            // 
            this.lblCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // resultTab
            // 
            this.resultTab.Controls.Add(this.TAB_GRID);
            this.resultTab.Controls.Add(this.TAB_MAP);
            this.resultTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultTab.Location = new System.Drawing.Point(0, 0);
            this.resultTab.Name = "resultTab";
            this.resultTab.SelectedIndex = 0;
            this.resultTab.Size = new System.Drawing.Size(684, 253);
            this.resultTab.TabIndex = 0;
            // 
            // TAB_GRID
            // 
            this.TAB_GRID.Controls.Add(this.grdResults);
            this.TAB_GRID.Location = new System.Drawing.Point(4, 22);
            this.TAB_GRID.Name = "TAB_GRID";
            this.TAB_GRID.Size = new System.Drawing.Size(676, 227);
            this.TAB_GRID.TabIndex = 0;
            this.TAB_GRID.Text = "Grid View";
            this.TAB_GRID.UseVisualStyleBackColor = true;
            // 
            // grdResults
            // 
            this.grdResults.AllowUserToAddRows = false;
            this.grdResults.AllowUserToDeleteRows = false;
            this.grdResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdResults.Location = new System.Drawing.Point(0, 0);
            this.grdResults.Name = "grdResults";
            this.grdResults.ReadOnly = true;
            this.grdResults.Size = new System.Drawing.Size(676, 227);
            this.grdResults.TabIndex = 0;
            // 
            // TAB_MAP
            // 
            this.TAB_MAP.Controls.Add(this.mapCtl);
            this.TAB_MAP.Location = new System.Drawing.Point(4, 22);
            this.TAB_MAP.Name = "TAB_MAP";
            this.TAB_MAP.Size = new System.Drawing.Size(676, 227);
            this.TAB_MAP.TabIndex = 1;
            this.TAB_MAP.Text = "Map View";
            this.TAB_MAP.UseVisualStyleBackColor = true;
            // 
            // mapCtl
            // 
            this.mapCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapCtl.Location = new System.Drawing.Point(0, 0);
            this.mapCtl.Name = "mapCtl";
            this.mapCtl.Size = new System.Drawing.Size(676, 227);
            this.mapCtl.TabIndex = 0;
            // 
            // FdoDataPreviewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FdoDataPreviewCtl";
            this.Size = new System.Drawing.Size(688, 480);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.resultTab.ResumeLayout(false);
            this.TAB_GRID.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).EndInit();
            this.TAB_MAP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl resultTab;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmbQueryMode;
        private System.Windows.Forms.ToolStripButton btnQuery;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnSave;
        private System.Windows.Forms.ToolStripMenuItem sDFToolStripMenuItem;
        private System.Windows.Forms.Panel queryPanel;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.TabPage TAB_GRID;
        private System.Windows.Forms.TabPage TAB_MAP;
        private System.Windows.Forms.DataGridView grdResults;
        private System.Windows.Forms.ToolStripLabel lblCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private FdoMapPreview mapCtl;
    }
}
