namespace FdoToolbox.DataStoreManager.Controls
{
    partial class FdoSchemaDesignerCtl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnApply = new System.Windows.Forms.ToolStripButton();
            this.btnFix = new System.Windows.Forms.ToolStripButton();
            this.btnAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveAsXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsNewSDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.schemaTree = new System.Windows.Forms.TreeView();
            this.imgTree = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.lnkClear = new System.Windows.Forms.LinkLabel();
            this.lnkApply = new System.Windows.Forms.LinkLabel();
            this.grdAttributes = new System.Windows.Forms.DataGridView();
            this.COL_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_VALUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.ctxProperty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletePropertyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxClass = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddNewProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteClassItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToNewSDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAttributes)).BeginInit();
            this.ctxProperty.SuspendLayout();
            this.ctxClass.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnApply,
            this.btnFix,
            this.btnAdd,
            this.toolStripSeparator3,
            this.btnLoad,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(678, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnApply
            // 
            this.btnApply.Image = global::FdoToolbox.DataStoreManager.Images.application_edit;
            this.btnApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(94, 22);
            this.btnApply.Text = "Apply Schema";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnFix
            // 
            this.btnFix.Image = global::FdoToolbox.DataStoreManager.Images.wrench_orange;
            this.btnFix.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(120, 22);
            this.btnFix.Text = "Fix Incompatibilities";
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::FdoToolbox.DataStoreManager.Images.add;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(79, 22);
            this.btnAdd.Text = "Add New";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = global::FdoToolbox.DataStoreManager.Images.folder;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(50, 22);
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsXMLToolStripMenuItem,
            this.saveAsNewSDFToolStripMenuItem});
            this.btnSave.Image = global::FdoToolbox.DataStoreManager.Images.disk;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 22);
            this.btnSave.Text = "Save";
            // 
            // saveAsXMLToolStripMenuItem
            // 
            this.saveAsXMLToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.page_white_code;
            this.saveAsXMLToolStripMenuItem.Name = "saveAsXMLToolStripMenuItem";
            this.saveAsXMLToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveAsXMLToolStripMenuItem.Text = "Save as XML";
            this.saveAsXMLToolStripMenuItem.Click += new System.EventHandler(this.saveToXMLToolStripMenuItem_Click);
            // 
            // saveAsNewSDFToolStripMenuItem
            // 
            this.saveAsNewSDFToolStripMenuItem.Image = global::FdoToolbox.DataStoreManager.Images.database;
            this.saveAsNewSDFToolStripMenuItem.Name = "saveAsNewSDFToolStripMenuItem";
            this.saveAsNewSDFToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveAsNewSDFToolStripMenuItem.Text = "Save as new SDF";
            this.saveAsNewSDFToolStripMenuItem.Click += new System.EventHandler(this.saveToNewSDFToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.schemaTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(678, 475);
            this.splitContainer1.SplitterDistance = 331;
            this.splitContainer1.TabIndex = 1;
            // 
            // schemaTree
            // 
            this.schemaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schemaTree.ImageIndex = 0;
            this.schemaTree.ImageList = this.imgTree;
            this.schemaTree.Location = new System.Drawing.Point(0, 0);
            this.schemaTree.Name = "schemaTree";
            this.schemaTree.SelectedImageIndex = 0;
            this.schemaTree.Size = new System.Drawing.Size(331, 475);
            this.schemaTree.TabIndex = 0;
            this.schemaTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.schemaTree_AfterSelect);
            this.schemaTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.schemaTree_MouseDown);
            // 
            // imgTree
            // 
            this.imgTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgTree.ImageSize = new System.Drawing.Size(16, 16);
            this.imgTree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lnkClear);
            this.splitContainer2.Panel2.Controls.Add(this.lnkApply);
            this.splitContainer2.Panel2.Controls.Add(this.grdAttributes);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Size = new System.Drawing.Size(343, 475);
            this.splitContainer2.SplitterDistance = 321;
            this.splitContainer2.TabIndex = 1;
            // 
            // propGrid
            // 
            this.propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGrid.Location = new System.Drawing.Point(0, 0);
            this.propGrid.Name = "propGrid";
            this.propGrid.Size = new System.Drawing.Size(343, 321);
            this.propGrid.TabIndex = 0;
            // 
            // lnkClear
            // 
            this.lnkClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkClear.AutoSize = true;
            this.lnkClear.Location = new System.Drawing.Point(300, 10);
            this.lnkClear.Name = "lnkClear";
            this.lnkClear.Size = new System.Drawing.Size(31, 13);
            this.lnkClear.TabIndex = 3;
            this.lnkClear.TabStop = true;
            this.lnkClear.Text = "Clear";
            this.lnkClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClear_LinkClicked);
            // 
            // lnkApply
            // 
            this.lnkApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkApply.AutoSize = true;
            this.lnkApply.Enabled = false;
            this.lnkApply.Location = new System.Drawing.Point(262, 10);
            this.lnkApply.Name = "lnkApply";
            this.lnkApply.Size = new System.Drawing.Size(32, 13);
            this.lnkApply.TabIndex = 2;
            this.lnkApply.TabStop = true;
            this.lnkApply.Text = "Save";
            this.lnkApply.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkApply_LinkClicked);
            // 
            // grdAttributes
            // 
            this.grdAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_NAME,
            this.COL_VALUE});
            this.grdAttributes.Location = new System.Drawing.Point(15, 36);
            this.grdAttributes.Name = "grdAttributes";
            this.grdAttributes.Size = new System.Drawing.Size(316, 98);
            this.grdAttributes.TabIndex = 1;
            this.grdAttributes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdAttributes_CellValueChanged);
            // 
            // COL_NAME
            // 
            this.COL_NAME.HeaderText = "Name";
            this.COL_NAME.Name = "COL_NAME";
            // 
            // COL_VALUE
            // 
            this.COL_VALUE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_VALUE.HeaderText = "Value";
            this.COL_VALUE.Name = "COL_VALUE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Attributes";
            // 
            // ctxProperty
            // 
            this.ctxProperty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletePropertyItem});
            this.ctxProperty.Name = "ctxProperty";
            this.ctxProperty.Size = new System.Drawing.Size(106, 26);
            // 
            // deletePropertyItem
            // 
            this.deletePropertyItem.Name = "deletePropertyItem";
            this.deletePropertyItem.Size = new System.Drawing.Size(105, 22);
            this.deletePropertyItem.Text = "Delete";
            this.deletePropertyItem.Click += new System.EventHandler(this.deletePropertyItem_Click);
            // 
            // ctxClass
            // 
            this.ctxClass.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddNewProperty,
            this.toolStripSeparator2,
            this.deleteClassItem});
            this.ctxClass.Name = "ctxClass";
            this.ctxClass.Size = new System.Drawing.Size(118, 54);
            // 
            // mnuAddNewProperty
            // 
            this.mnuAddNewProperty.Name = "mnuAddNewProperty";
            this.mnuAddNewProperty.Size = new System.Drawing.Size(117, 22);
            this.mnuAddNewProperty.Text = "Add New";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(114, 6);
            // 
            // deleteClassItem
            // 
            this.deleteClassItem.Name = "deleteClassItem";
            this.deleteClassItem.Size = new System.Drawing.Size(117, 22);
            this.deleteClassItem.Text = "Delete";
            this.deleteClassItem.Click += new System.EventHandler(this.deleteClassItem_Click);
            // 
            // saveToXMLToolStripMenuItem
            // 
            this.saveToXMLToolStripMenuItem.Name = "saveToXMLToolStripMenuItem";
            this.saveToXMLToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.saveToXMLToolStripMenuItem.Text = "Save to XML";
            this.saveToXMLToolStripMenuItem.Click += new System.EventHandler(this.saveToXMLToolStripMenuItem_Click);
            // 
            // saveToNewSDFToolStripMenuItem
            // 
            this.saveToNewSDFToolStripMenuItem.Name = "saveToNewSDFToolStripMenuItem";
            this.saveToNewSDFToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.saveToNewSDFToolStripMenuItem.Text = "Save to new SDF";
            this.saveToNewSDFToolStripMenuItem.Click += new System.EventHandler(this.saveToNewSDFToolStripMenuItem_Click);
            // 
            // FdoSchemaDesignerCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FdoSchemaDesignerCtl";
            this.Size = new System.Drawing.Size(678, 500);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdAttributes)).EndInit();
            this.ctxProperty.ResumeLayout(false);
            this.ctxClass.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

}

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView schemaTree;
        private System.Windows.Forms.PropertyGrid propGrid;
        private System.Windows.Forms.ImageList imgTree;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip ctxProperty;
        private System.Windows.Forms.ContextMenuStrip ctxClass;
        private System.Windows.Forms.ToolStripMenuItem deletePropertyItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNewProperty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteClassItem;
        private System.Windows.Forms.ToolStripMenuItem saveToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToNewSDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnApply;
        private System.Windows.Forms.ToolStripDropDownButton btnAdd;
        private System.Windows.Forms.ToolStripDropDownButton btnSave;
        private System.Windows.Forms.ToolStripMenuItem saveAsXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsNewSDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnFix;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView grdAttributes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel lnkClear;
        private System.Windows.Forms.LinkLabel lnkApply;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_VALUE;

    }
}
