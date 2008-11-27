namespace FdoToolbox.Tasks.Controls
{
    partial class FdoBulkCopyCtl
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
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Classes");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeMappings = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbSrcSchema = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSrcConnection = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbTargetSchema = new System.Windows.Forms.ComboBox();
            this.cmbTargetConnection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.ctxSelectedClass = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxFilter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxDelete = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxExpressions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addComputedExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSelectedExpression = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeExpressionItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapExpressionItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSpatialFilter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numBatchSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.chkListSpatialContexts = new System.Windows.Forms.CheckedListBox();
            this.chkCopySpatialContexts = new System.Windows.Forms.CheckBox();
            this.removeMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.ctxExpressions.SuspendLayout();
            this.ctxSelectedExpression.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(448, 20);
            this.txtName.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.treeMappings);
            this.groupBox2.Location = new System.Drawing.Point(4, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 356);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mappings";
            // 
            // treeMappings
            // 
            this.treeMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMappings.Location = new System.Drawing.Point(3, 16);
            this.treeMappings.Name = "treeMappings";
            treeNode6.Name = "NODE_CLASSES";
            treeNode6.Text = "Classes";
            this.treeMappings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6});
            this.treeMappings.Size = new System.Drawing.Size(282, 337);
            this.treeMappings.TabIndex = 0;
            this.treeMappings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeMappings_MouseDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cmbSrcSchema);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cmbSrcConnection);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(298, 60);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 73);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source";
            // 
            // cmbSrcSchema
            // 
            this.cmbSrcSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcSchema.FormattingEnabled = true;
            this.cmbSrcSchema.Location = new System.Drawing.Point(73, 45);
            this.cmbSrcSchema.Name = "cmbSrcSchema";
            this.cmbSrcSchema.Size = new System.Drawing.Size(141, 21);
            this.cmbSrcSchema.TabIndex = 3;
            this.cmbSrcSchema.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcSchema_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Schema";
            // 
            // cmbSrcConnection
            // 
            this.cmbSrcConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSrcConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcConnection.FormattingEnabled = true;
            this.cmbSrcConnection.Location = new System.Drawing.Point(73, 18);
            this.cmbSrcConnection.Name = "cmbSrcConnection";
            this.cmbSrcConnection.Size = new System.Drawing.Size(141, 21);
            this.cmbSrcConnection.TabIndex = 1;
            this.cmbSrcConnection.SelectionChangeCommitted += new System.EventHandler(this.cmbSrcConnection_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Connection";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.cmbTargetSchema);
            this.groupBox4.Controls.Add(this.cmbTargetConnection);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(298, 139);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(220, 72);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Target";
            // 
            // cmbTargetSchema
            // 
            this.cmbTargetSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetSchema.FormattingEnabled = true;
            this.cmbTargetSchema.Location = new System.Drawing.Point(73, 46);
            this.cmbTargetSchema.Name = "cmbTargetSchema";
            this.cmbTargetSchema.Size = new System.Drawing.Size(141, 21);
            this.cmbTargetSchema.TabIndex = 7;
            this.cmbTargetSchema.SelectionChangeCommitted += new System.EventHandler(this.cmbTargetSchema_SelectionChangeCommitted);
            // 
            // cmbTargetConnection
            // 
            this.cmbTargetConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetConnection.FormattingEnabled = true;
            this.cmbTargetConnection.Location = new System.Drawing.Point(73, 19);
            this.cmbTargetConnection.Name = "cmbTargetConnection";
            this.cmbTargetConnection.Size = new System.Drawing.Size(141, 21);
            this.cmbTargetConnection.TabIndex = 5;
            this.cmbTargetConnection.SelectionChangeCommitted += new System.EventHandler(this.cmbTargetConnection_SelectionChangeCommitted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Schema";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Connection";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.panel1);
            this.groupBox5.Location = new System.Drawing.Point(298, 217);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(220, 199);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Bulk Copy Options";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(443, 422);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ctxSelectedClass
            // 
            this.ctxSelectedClass.Name = "ctxSelectedClass";
            this.ctxSelectedClass.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxFilter
            // 
            this.ctxFilter.Name = "ctxFilter";
            this.ctxFilter.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxDelete
            // 
            this.ctxDelete.Name = "ctxDelete";
            this.ctxDelete.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxExpressions
            // 
            this.ctxExpressions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addComputedExpressionToolStripMenuItem});
            this.ctxExpressions.Name = "ctxExpressions";
            this.ctxExpressions.Size = new System.Drawing.Size(212, 26);
            // 
            // addComputedExpressionToolStripMenuItem
            // 
            this.addComputedExpressionToolStripMenuItem.Name = "addComputedExpressionToolStripMenuItem";
            this.addComputedExpressionToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.addComputedExpressionToolStripMenuItem.Text = "Add Computed Expression";
            this.addComputedExpressionToolStripMenuItem.Click += new System.EventHandler(this.addComputedExpressionToolStripMenuItem_Click);
            // 
            // ctxSelectedExpression
            // 
            this.ctxSelectedExpression.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeExpressionItem,
            this.removeMappingToolStripMenuItem,
            this.toolStripSeparator1,
            this.editExpressionToolStripMenuItem,
            this.mapExpressionItem});
            this.ctxSelectedExpression.Name = "ctxSelectedExpression";
            this.ctxSelectedExpression.Size = new System.Drawing.Size(168, 98);
            // 
            // removeExpressionItem
            // 
            this.removeExpressionItem.Name = "removeExpressionItem";
            this.removeExpressionItem.Size = new System.Drawing.Size(158, 22);
            this.removeExpressionItem.Text = "Remove";
            this.removeExpressionItem.Click += new System.EventHandler(this.removeExpressionItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // editExpressionToolStripMenuItem
            // 
            this.editExpressionToolStripMenuItem.Name = "editExpressionToolStripMenuItem";
            this.editExpressionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.editExpressionToolStripMenuItem.Text = "Edit Expression";
            this.editExpressionToolStripMenuItem.Click += new System.EventHandler(this.editExpressionToolStripMenuItem_Click);
            // 
            // mapExpressionItem
            // 
            this.mapExpressionItem.Name = "mapExpressionItem";
            this.mapExpressionItem.Size = new System.Drawing.Size(158, 22);
            this.mapExpressionItem.Text = "Map To";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.txtSpatialFilter);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.numBatchSize);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.chkListSpatialContexts);
            this.panel1.Controls.Add(this.chkCopySpatialContexts);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 180);
            this.panel1.TabIndex = 0;
            // 
            // txtSpatialFilter
            // 
            this.txtSpatialFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpatialFilter.Location = new System.Drawing.Point(6, 128);
            this.txtSpatialFilter.Multiline = true;
            this.txtSpatialFilter.Name = "txtSpatialFilter";
            this.txtSpatialFilter.Size = new System.Drawing.Size(205, 48);
            this.txtSpatialFilter.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Global Spatial Filter";
            // 
            // numBatchSize
            // 
            this.numBatchSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numBatchSize.Location = new System.Drawing.Point(99, 81);
            this.numBatchSize.Name = "numBatchSize";
            this.numBatchSize.Size = new System.Drawing.Size(112, 20);
            this.numBatchSize.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Batch Insert Size";
            // 
            // chkListSpatialContexts
            // 
            this.chkListSpatialContexts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkListSpatialContexts.FormattingEnabled = true;
            this.chkListSpatialContexts.Location = new System.Drawing.Point(6, 27);
            this.chkListSpatialContexts.Name = "chkListSpatialContexts";
            this.chkListSpatialContexts.Size = new System.Drawing.Size(205, 49);
            this.chkListSpatialContexts.TabIndex = 7;
            // 
            // chkCopySpatialContexts
            // 
            this.chkCopySpatialContexts.AutoSize = true;
            this.chkCopySpatialContexts.Location = new System.Drawing.Point(6, 4);
            this.chkCopySpatialContexts.Name = "chkCopySpatialContexts";
            this.chkCopySpatialContexts.Size = new System.Drawing.Size(129, 17);
            this.chkCopySpatialContexts.TabIndex = 6;
            this.chkCopySpatialContexts.Text = "Copy Spatial Contexts";
            this.chkCopySpatialContexts.UseVisualStyleBackColor = true;
            this.chkCopySpatialContexts.CheckedChanged += new System.EventHandler(this.chkCopySpatialContexts_CheckedChanged);
            // 
            // removeMappingToolStripMenuItem
            // 
            this.removeMappingToolStripMenuItem.Name = "removeMappingToolStripMenuItem";
            this.removeMappingToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.removeMappingToolStripMenuItem.Text = "Remove Mapping";
            this.removeMappingToolStripMenuItem.Click += new System.EventHandler(this.removeMappingToolStripMenuItem_Click);
            // 
            // FdoBulkCopyCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FdoBulkCopyCtl";
            this.Size = new System.Drawing.Size(521, 453);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ctxExpressions.ResumeLayout(false);
            this.ctxSelectedExpression.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeMappings;
        private System.Windows.Forms.ComboBox cmbSrcSchema;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSrcConnection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTargetSchema;
        private System.Windows.Forms.ComboBox cmbTargetConnection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedClass;
        private System.Windows.Forms.ContextMenuStrip ctxFilter;
        private System.Windows.Forms.ContextMenuStrip ctxDelete;
        private System.Windows.Forms.ContextMenuStrip ctxExpressions;
        private System.Windows.Forms.ToolStripMenuItem addComputedExpressionToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedExpression;
        private System.Windows.Forms.ToolStripMenuItem removeExpressionItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editExpressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapExpressionItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSpatialFilter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numBatchSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckedListBox chkListSpatialContexts;
        private System.Windows.Forms.CheckBox chkCopySpatialContexts;
        private System.Windows.Forms.ToolStripMenuItem removeMappingToolStripMenuItem;
    }
}
