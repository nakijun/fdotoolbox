namespace FdoToolbox.Core.Controls
{
    partial class SchemaMgrCtl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstSchemas = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddSchema = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteSchema = new System.Windows.Forms.ToolStripButton();
            this.btnSaveSchema = new System.Windows.Forms.ToolStripDropDownButton();
            this.asXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asNewSDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstClasses = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnAddClass = new System.Windows.Forms.ToolStripDropDownButton();
            this.featureClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classNonFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkLayerClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkLinkClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditClass = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteClass = new System.Windows.Forms.ToolStripButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(485, 248);
            this.splitContainer1.SplitterDistance = 220;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstSchemas);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 248);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Schemas";
            // 
            // lstSchemas
            // 
            this.lstSchemas.DisplayMember = "Name";
            this.lstSchemas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSchemas.FormattingEnabled = true;
            this.lstSchemas.Location = new System.Drawing.Point(3, 41);
            this.lstSchemas.Name = "lstSchemas";
            this.lstSchemas.Size = new System.Drawing.Size(214, 199);
            this.lstSchemas.TabIndex = 1;
            this.lstSchemas.SelectedIndexChanged += new System.EventHandler(this.lstSchemas_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddSchema,
            this.btnDeleteSchema,
            this.btnSaveSchema});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(214, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddSchema
            // 
            this.btnAddSchema.Image = global::FdoToolbox.Core.Properties.Resources.chart_organisation_add;
            this.btnAddSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddSchema.Name = "btnAddSchema";
            this.btnAddSchema.Size = new System.Drawing.Size(46, 22);
            this.btnAddSchema.Text = "Add";
            this.btnAddSchema.Click += new System.EventHandler(this.btnAddSchema_Click);
            // 
            // btnDeleteSchema
            // 
            this.btnDeleteSchema.Image = global::FdoToolbox.Core.Properties.Resources.chart_organisation_delete;
            this.btnDeleteSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteSchema.Name = "btnDeleteSchema";
            this.btnDeleteSchema.Size = new System.Drawing.Size(58, 22);
            this.btnDeleteSchema.Text = "Delete";
            this.btnDeleteSchema.Click += new System.EventHandler(this.btnDeleteSchema_Click);
            // 
            // btnSaveSchema
            // 
            this.btnSaveSchema.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asXMLToolStripMenuItem,
            this.asNewSDFToolStripMenuItem});
            this.btnSaveSchema.Image = global::FdoToolbox.Core.Properties.Resources.disk;
            this.btnSaveSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveSchema.Name = "btnSaveSchema";
            this.btnSaveSchema.Size = new System.Drawing.Size(60, 22);
            this.btnSaveSchema.Text = "Save";
            this.btnSaveSchema.Visible = false;
            // 
            // asXMLToolStripMenuItem
            // 
            this.asXMLToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.page_white_code;
            this.asXMLToolStripMenuItem.Name = "asXMLToolStripMenuItem";
            this.asXMLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.asXMLToolStripMenuItem.Text = "As XML";
            this.asXMLToolStripMenuItem.ToolTipText = "Save the selected schema definition to an XML file";
            this.asXMLToolStripMenuItem.Click += new System.EventHandler(this.SaveSchemaAsXML_Click);
            // 
            // asNewSDFToolStripMenuItem
            // 
            this.asNewSDFToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.database;
            this.asNewSDFToolStripMenuItem.Name = "asNewSDFToolStripMenuItem";
            this.asNewSDFToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.asNewSDFToolStripMenuItem.Text = "As New SDF";
            this.asNewSDFToolStripMenuItem.ToolTipText = "Apply this selected schema to a new SDF file";
            this.asNewSDFToolStripMenuItem.Click += new System.EventHandler(this.SaveSchemaAsSDF_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstClasses);
            this.groupBox2.Controls.Add(this.toolStrip2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(261, 248);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Classes";
            // 
            // lstClasses
            // 
            this.lstClasses.DisplayMember = "Name";
            this.lstClasses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClasses.FormattingEnabled = true;
            this.lstClasses.Location = new System.Drawing.Point(3, 41);
            this.lstClasses.Name = "lstClasses";
            this.lstClasses.Size = new System.Drawing.Size(255, 199);
            this.lstClasses.TabIndex = 1;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddClass,
            this.btnEditClass,
            this.btnDeleteClass});
            this.toolStrip2.Location = new System.Drawing.Point(3, 16);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(255, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnAddClass
            // 
            this.btnAddClass.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featureClassToolStripMenuItem,
            this.classNonFeatureToolStripMenuItem,
            this.networkClassToolStripMenuItem,
            this.networkLayerClassToolStripMenuItem,
            this.networkLinkClassToolStripMenuItem});
            this.btnAddClass.Enabled = false;
            this.btnAddClass.Image = global::FdoToolbox.Core.Properties.Resources.table_add;
            this.btnAddClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddClass.Name = "btnAddClass";
            this.btnAddClass.Size = new System.Drawing.Size(55, 22);
            this.btnAddClass.Text = "Add";
            // 
            // featureClassToolStripMenuItem
            // 
            this.featureClassToolStripMenuItem.Name = "featureClassToolStripMenuItem";
            this.featureClassToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.featureClassToolStripMenuItem.Text = "Feature Class";
            this.featureClassToolStripMenuItem.Click += new System.EventHandler(this.featureClassToolStripMenuItem_Click);
            // 
            // classNonFeatureToolStripMenuItem
            // 
            this.classNonFeatureToolStripMenuItem.Name = "classNonFeatureToolStripMenuItem";
            this.classNonFeatureToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.classNonFeatureToolStripMenuItem.Text = "Class (Non-Feature)";
            this.classNonFeatureToolStripMenuItem.Click += new System.EventHandler(this.classNonFeatureToolStripMenuItem_Click);
            // 
            // networkClassToolStripMenuItem
            // 
            this.networkClassToolStripMenuItem.Enabled = false;
            this.networkClassToolStripMenuItem.Name = "networkClassToolStripMenuItem";
            this.networkClassToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.networkClassToolStripMenuItem.Text = "Network Class";
            // 
            // networkLayerClassToolStripMenuItem
            // 
            this.networkLayerClassToolStripMenuItem.Enabled = false;
            this.networkLayerClassToolStripMenuItem.Name = "networkLayerClassToolStripMenuItem";
            this.networkLayerClassToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.networkLayerClassToolStripMenuItem.Text = "Network Layer Class";
            // 
            // networkLinkClassToolStripMenuItem
            // 
            this.networkLinkClassToolStripMenuItem.Enabled = false;
            this.networkLinkClassToolStripMenuItem.Name = "networkLinkClassToolStripMenuItem";
            this.networkLinkClassToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.networkLinkClassToolStripMenuItem.Text = "Network Link Class";
            // 
            // btnEditClass
            // 
            this.btnEditClass.Enabled = false;
            this.btnEditClass.Image = global::FdoToolbox.Core.Properties.Resources.table_edit;
            this.btnEditClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditClass.Name = "btnEditClass";
            this.btnEditClass.Size = new System.Drawing.Size(45, 22);
            this.btnEditClass.Text = "Edit";
            this.btnEditClass.Click += new System.EventHandler(this.btnEditClass_Click);
            // 
            // btnDeleteClass
            // 
            this.btnDeleteClass.Enabled = false;
            this.btnDeleteClass.Image = global::FdoToolbox.Core.Properties.Resources.table_delete;
            this.btnDeleteClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteClass.Name = "btnDeleteClass";
            this.btnDeleteClass.Size = new System.Drawing.Size(58, 22);
            this.btnDeleteClass.Text = "Delete";
            this.btnDeleteClass.Click += new System.EventHandler(this.btnDeleteClass_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(326, 255);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(407, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SchemaMgrCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SchemaMgrCtl";
            this.Size = new System.Drawing.Size(485, 281);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstClasses;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstSchemas;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnEditClass;
        private System.Windows.Forms.ToolStripButton btnAddSchema;
        private System.Windows.Forms.ToolStripButton btnDeleteSchema;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripDropDownButton btnAddClass;
        private System.Windows.Forms.ToolStripMenuItem featureClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem classNonFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkLayerClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkLinkClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnDeleteClass;
        private System.Windows.Forms.ToolStripDropDownButton btnSaveSchema;
        private System.Windows.Forms.ToolStripMenuItem asXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asNewSDFToolStripMenuItem;
    }
}
