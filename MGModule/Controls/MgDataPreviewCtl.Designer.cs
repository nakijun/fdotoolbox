namespace MGModule.Controls
{
    partial class MgDataPreviewCtl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MgDataPreviewCtl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkColumns = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbFeatureClass = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPreview = new System.Windows.Forms.TabControl();
            this.TAB_GRID = new System.Windows.Forms.TabPage();
            this.grdPreview = new System.Windows.Forms.DataGridView();
            this.TAB_MAP = new System.Windows.Forms.TabPage();
            this.mapPreviewPanel = new System.Windows.Forms.Panel();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.imgPreview = new System.Windows.Forms.ImageList(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPreview.SuspendLayout();
            this.TAB_GRID.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).BeginInit();
            this.TAB_MAP.SuspendLayout();
            this.mapPreviewPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblCount);
            this.splitContainer1.Panel1.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnGo);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabPreview);
            this.splitContainer1.Size = new System.Drawing.Size(622, 439);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Image = global::MGModule.Properties.Resources.cross;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(480, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(554, 184);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear Grid";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Image = global::MGModule.Properties.Resources.application_go;
            this.btnGo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGo.Location = new System.Drawing.Point(425, 184);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(49, 23);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkColumns);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtFilter);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbFeatureClass);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(612, 174);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Query Parameters";
            // 
            // chkColumns
            // 
            this.chkColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkColumns.FormattingEnabled = true;
            this.chkColumns.Location = new System.Drawing.Point(402, 60);
            this.chkColumns.Name = "chkColumns";
            this.chkColumns.Size = new System.Drawing.Size(192, 79);
            this.chkColumns.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Properties";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(72, 60);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(311, 93);
            this.txtFilter.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filter";
            // 
            // cmbFeatureClass
            // 
            this.cmbFeatureClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFeatureClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFeatureClass.FormattingEnabled = true;
            this.cmbFeatureClass.Location = new System.Drawing.Point(72, 26);
            this.cmbFeatureClass.Name = "cmbFeatureClass";
            this.cmbFeatureClass.Size = new System.Drawing.Size(311, 21);
            this.cmbFeatureClass.TabIndex = 2;
            this.cmbFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbFeatureClass_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Class";
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.TAB_GRID);
            this.tabPreview.Controls.Add(this.TAB_MAP);
            this.tabPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPreview.ImageList = this.imgPreview;
            this.tabPreview.Location = new System.Drawing.Point(0, 0);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.SelectedIndex = 0;
            this.tabPreview.Size = new System.Drawing.Size(620, 216);
            this.tabPreview.TabIndex = 0;
            // 
            // TAB_GRID
            // 
            this.TAB_GRID.Controls.Add(this.grdPreview);
            this.TAB_GRID.ImageKey = "application_view_columns.png";
            this.TAB_GRID.Location = new System.Drawing.Point(4, 23);
            this.TAB_GRID.Name = "TAB_GRID";
            this.TAB_GRID.Size = new System.Drawing.Size(612, 189);
            this.TAB_GRID.TabIndex = 0;
            this.TAB_GRID.Text = "Grid Preview";
            this.TAB_GRID.UseVisualStyleBackColor = true;
            // 
            // grdPreview
            // 
            this.grdPreview.AllowUserToAddRows = false;
            this.grdPreview.AllowUserToDeleteRows = false;
            this.grdPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdPreview.Location = new System.Drawing.Point(0, 0);
            this.grdPreview.Name = "grdPreview";
            this.grdPreview.ReadOnly = true;
            this.grdPreview.Size = new System.Drawing.Size(612, 189);
            this.grdPreview.TabIndex = 0;
            // 
            // TAB_MAP
            // 
            this.TAB_MAP.Controls.Add(this.mapPreviewPanel);
            this.TAB_MAP.ImageKey = "map.png";
            this.TAB_MAP.Location = new System.Drawing.Point(4, 23);
            this.TAB_MAP.Name = "TAB_MAP";
            this.TAB_MAP.Size = new System.Drawing.Size(612, 189);
            this.TAB_MAP.TabIndex = 1;
            this.TAB_MAP.Text = "Map Preview";
            this.TAB_MAP.UseVisualStyleBackColor = true;
            // 
            // mapPreviewPanel
            // 
            this.mapPreviewPanel.Controls.Add(this.webBrowser);
            this.mapPreviewPanel.Controls.Add(this.toolStrip1);
            this.mapPreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.mapPreviewPanel.Name = "mapPreviewPanel";
            this.mapPreviewPanel.Size = new System.Drawing.Size(612, 189);
            this.mapPreviewPanel.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 25);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(612, 164);
            this.webBrowser.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(612, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::MGModule.Properties.Resources.page_refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(65, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // imgPreview
            // 
            this.imgPreview.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgPreview.ImageStream")));
            this.imgPreview.TransparentColor = System.Drawing.Color.Transparent;
            this.imgPreview.Images.SetKeyName(0, "application_view_columns.png");
            this.imgPreview.Images.SetKeyName(1, "map.png");
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(18, 189);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 4;
            // 
            // MgDataPreviewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MgDataPreviewCtl";
            this.Size = new System.Drawing.Size(622, 439);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPreview.ResumeLayout(false);
            this.TAB_GRID.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).EndInit();
            this.TAB_MAP.ResumeLayout(false);
            this.mapPreviewPanel.ResumeLayout(false);
            this.mapPreviewPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabPreview;
        private System.Windows.Forms.TabPage TAB_GRID;
        private System.Windows.Forms.TabPage TAB_MAP;
        private System.Windows.Forms.ImageList imgPreview;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbFeatureClass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView grdPreview;
        private System.Windows.Forms.CheckedListBox chkColumns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel mapPreviewPanel;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.Label lblCount;
    }
}
