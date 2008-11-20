namespace FdoToolbox.Base.Controls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FdoSchemaDesignerCtl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnApply = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSave = new System.Windows.Forms.ToolStripDropDownButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.schemaTree = new System.Windows.Forms.TreeView();
            this.imgTree = new System.Windows.Forms.ImageList(this.components);
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.ctxProperty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxClass = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddNewProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ctxProperty.SuspendLayout();
            this.ctxClass.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnApply,
            this.toolStripSeparator1,
            this.btnAdd,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(678, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnApply
            // 
            this.btnApply.Image = ((System.Drawing.Image)(resources.GetObject("btnApply.Image")));
            this.btnApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(94, 22);
            this.btnApply.Text = "Apply Schema";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(55, 22);
            this.btnAdd.Text = "Add";
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 22);
            this.btnSave.Text = "Save";
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
            this.splitContainer1.Panel2.Controls.Add(this.propGrid);
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
            // propGrid
            // 
            this.propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGrid.Location = new System.Drawing.Point(0, 0);
            this.propGrid.Name = "propGrid";
            this.propGrid.Size = new System.Drawing.Size(343, 475);
            this.propGrid.TabIndex = 0;
            // 
            // ctxProperty
            // 
            this.ctxProperty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem1});
            this.ctxProperty.Name = "ctxProperty";
            this.ctxProperty.Size = new System.Drawing.Size(117, 26);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            // 
            // ctxClass
            // 
            this.ctxClass.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddNewProperty,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem});
            this.ctxClass.Name = "ctxClass";
            this.ctxClass.Size = new System.Drawing.Size(129, 54);
            // 
            // mnuAddNewProperty
            // 
            this.mnuAddNewProperty.Name = "mnuAddNewProperty";
            this.mnuAddNewProperty.Size = new System.Drawing.Size(128, 22);
            this.mnuAddNewProperty.Text = "Add New";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(125, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
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
        private System.Windows.Forms.ToolStripDropDownButton btnAdd;
        private System.Windows.Forms.ToolStripDropDownButton btnSave;
        private System.Windows.Forms.ToolStripButton btnApply;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip ctxProperty;
        private System.Windows.Forms.ContextMenuStrip ctxClass;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNewProperty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;

    }
}
