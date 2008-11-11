namespace FdoToolbox.Base.Controls
{
    partial class FdoSchemaMgrCtl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstSchemas = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddSchema = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteSchema = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstClasses = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnAddClass = new System.Windows.Forms.ToolStripSplitButton();
            this.featureClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classNonFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditClass = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteClass = new System.Windows.Forms.ToolStripButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lstSchemas);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 319);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Schemas";
            // 
            // lstSchemas
            // 
            this.lstSchemas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSchemas.FormattingEnabled = true;
            this.lstSchemas.Location = new System.Drawing.Point(3, 41);
            this.lstSchemas.Name = "lstSchemas";
            this.lstSchemas.Size = new System.Drawing.Size(187, 264);
            this.lstSchemas.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddSchema,
            this.btnDeleteSchema});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(187, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddSchema
            // 
            this.btnAddSchema.Image = global::FdoToolbox.Base.Images.chart_organisation_add;
            this.btnAddSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddSchema.Name = "btnAddSchema";
            this.btnAddSchema.Size = new System.Drawing.Size(46, 22);
            this.btnAddSchema.Text = "Add";
            this.btnAddSchema.Click += new System.EventHandler(this.btnAddSchema_Click);
            // 
            // btnDeleteSchema
            // 
            this.btnDeleteSchema.Image = global::FdoToolbox.Base.Images.chart_organisation_delete;
            this.btnDeleteSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteSchema.Name = "btnDeleteSchema";
            this.btnDeleteSchema.Size = new System.Drawing.Size(58, 22);
            this.btnDeleteSchema.Text = "Delete";
            this.btnDeleteSchema.Click += new System.EventHandler(this.btnDeleteSchema_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lstClasses);
            this.groupBox2.Controls.Add(this.toolStrip2);
            this.groupBox2.Location = new System.Drawing.Point(199, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(342, 319);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Classes";
            // 
            // lstClasses
            // 
            this.lstClasses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClasses.FormattingEnabled = true;
            this.lstClasses.Location = new System.Drawing.Point(3, 41);
            this.lstClasses.Name = "lstClasses";
            this.lstClasses.Size = new System.Drawing.Size(336, 264);
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
            this.toolStrip2.Size = new System.Drawing.Size(336, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnAddClass
            // 
            this.btnAddClass.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featureClassToolStripMenuItem,
            this.classNonFeatureToolStripMenuItem});
            this.btnAddClass.Image = global::FdoToolbox.Base.Images.table_add;
            this.btnAddClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddClass.Name = "btnAddClass";
            this.btnAddClass.Size = new System.Drawing.Size(58, 22);
            this.btnAddClass.Text = "Add";
            // 
            // featureClassToolStripMenuItem
            // 
            this.featureClassToolStripMenuItem.Name = "featureClassToolStripMenuItem";
            this.featureClassToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.featureClassToolStripMenuItem.Text = "Feature Class";
            this.featureClassToolStripMenuItem.Click += new System.EventHandler(this.featureClassToolStripMenuItem_Click);
            // 
            // classNonFeatureToolStripMenuItem
            // 
            this.classNonFeatureToolStripMenuItem.Name = "classNonFeatureToolStripMenuItem";
            this.classNonFeatureToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.classNonFeatureToolStripMenuItem.Text = "Class (Non-Feature)";
            this.classNonFeatureToolStripMenuItem.Click += new System.EventHandler(this.classNonFeatureToolStripMenuItem_Click);
            // 
            // btnEditClass
            // 
            this.btnEditClass.Image = global::FdoToolbox.Base.Images.table_edit;
            this.btnEditClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditClass.Name = "btnEditClass";
            this.btnEditClass.Size = new System.Drawing.Size(45, 22);
            this.btnEditClass.Text = "Edit";
            this.btnEditClass.Click += new System.EventHandler(this.btnEditClass_Click);
            // 
            // btnDeleteClass
            // 
            this.btnDeleteClass.Image = global::FdoToolbox.Base.Images.table_delete;
            this.btnDeleteClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteClass.Name = "btnDeleteClass";
            this.btnDeleteClass.Size = new System.Drawing.Size(58, 22);
            this.btnDeleteClass.Text = "Delete";
            this.btnDeleteClass.Click += new System.EventHandler(this.btnDeleteClass_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(381, 328);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(463, 328);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FdoSchemaMgrCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FdoSchemaMgrCtl";
            this.Size = new System.Drawing.Size(544, 360);
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

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ListBox lstSchemas;
        private System.Windows.Forms.ListBox lstClasses;
        private System.Windows.Forms.ToolStripButton btnAddSchema;
        private System.Windows.Forms.ToolStripButton btnDeleteSchema;
        private System.Windows.Forms.ToolStripButton btnEditClass;
        private System.Windows.Forms.ToolStripButton btnDeleteClass;
        private System.Windows.Forms.ToolStripSplitButton btnAddClass;
        private System.Windows.Forms.ToolStripMenuItem featureClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem classNonFeatureToolStripMenuItem;
    }
}
