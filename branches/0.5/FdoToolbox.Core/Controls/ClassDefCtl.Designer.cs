namespace FdoToolbox.Core.Controls
{
    partial class ClassDefCtl
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkComputed = new System.Windows.Forms.CheckBox();
            this.chkAbstract = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grdProperties = new System.Windows.Forms.DataGridView();
            this.COL_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_TYPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNewProperty = new System.Windows.Forms.ToolStripSplitButton();
            this.dataPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geometryPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.associationPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnMakeIdentity = new System.Windows.Forms.ToolStripButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpExtended = new System.Windows.Forms.GroupBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstIdentityProperties = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdProperties)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(100, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(344, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkComputed);
            this.groupBox1.Controls.Add(this.chkAbstract);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 97);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Class Information";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(100, 46);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(344, 20);
            this.txtDescription.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Description";
            // 
            // chkComputed
            // 
            this.chkComputed.AutoSize = true;
            this.chkComputed.Location = new System.Drawing.Point(182, 72);
            this.chkComputed.Name = "chkComputed";
            this.chkComputed.Size = new System.Drawing.Size(85, 17);
            this.chkComputed.TabIndex = 5;
            this.chkComputed.Text = "Is Computed";
            this.chkComputed.UseVisualStyleBackColor = true;
            // 
            // chkAbstract
            // 
            this.chkAbstract.AutoSize = true;
            this.chkAbstract.Location = new System.Drawing.Point(100, 72);
            this.chkAbstract.Name = "chkAbstract";
            this.chkAbstract.Size = new System.Drawing.Size(76, 17);
            this.chkAbstract.TabIndex = 4;
            this.chkAbstract.Text = "Is Abstract";
            this.chkAbstract.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.grdProperties);
            this.groupBox2.Controls.Add(this.toolStrip1);
            this.groupBox2.Location = new System.Drawing.Point(3, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(298, 136);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // grdProperties
            // 
            this.grdProperties.AllowUserToAddRows = false;
            this.grdProperties.AllowUserToDeleteRows = false;
            this.grdProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_NAME,
            this.COL_TYPE});
            this.grdProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdProperties.Location = new System.Drawing.Point(3, 41);
            this.grdProperties.Name = "grdProperties";
            this.grdProperties.RowHeadersWidth = 20;
            this.grdProperties.Size = new System.Drawing.Size(292, 92);
            this.grdProperties.TabIndex = 1;
            this.grdProperties.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdProperties_CellClick);
            this.grdProperties.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdProperties_CellClick);
            // 
            // COL_NAME
            // 
            this.COL_NAME.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_NAME.DataPropertyName = "Name";
            this.COL_NAME.HeaderText = "Name";
            this.COL_NAME.Name = "COL_NAME";
            this.COL_NAME.ReadOnly = true;
            // 
            // COL_TYPE
            // 
            this.COL_TYPE.DataPropertyName = "PropertyType";
            this.COL_TYPE.HeaderText = "Type";
            this.COL_TYPE.MinimumWidth = 160;
            this.COL_TYPE.Name = "COL_TYPE";
            this.COL_TYPE.ReadOnly = true;
            this.COL_TYPE.Width = 160;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProperty,
            this.btnEdit,
            this.btnDelete,
            this.btnMakeIdentity});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(292, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNewProperty
            // 
            this.btnNewProperty.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataPropertyToolStripMenuItem,
            this.geometryPropertyToolStripMenuItem,
            this.rasterPropertyToolStripMenuItem,
            this.objectPropertyToolStripMenuItem,
            this.associationPropertyToolStripMenuItem});
            this.btnNewProperty.Image = global::FdoToolbox.Core.Properties.Resources.page_white;
            this.btnNewProperty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewProperty.Name = "btnNewProperty";
            this.btnNewProperty.Size = new System.Drawing.Size(60, 22);
            this.btnNewProperty.Text = "New";
            // 
            // dataPropertyToolStripMenuItem
            // 
            this.dataPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.table;
            this.dataPropertyToolStripMenuItem.Name = "dataPropertyToolStripMenuItem";
            this.dataPropertyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dataPropertyToolStripMenuItem.Text = "Data Property";
            this.dataPropertyToolStripMenuItem.Click += new System.EventHandler(this.NewDataProperty_Click);
            // 
            // geometryPropertyToolStripMenuItem
            // 
            this.geometryPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.shape_handles;
            this.geometryPropertyToolStripMenuItem.Name = "geometryPropertyToolStripMenuItem";
            this.geometryPropertyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.geometryPropertyToolStripMenuItem.Text = "Geometry Property";
            this.geometryPropertyToolStripMenuItem.Click += new System.EventHandler(this.NewGeometryProperty_Click);
            // 
            // rasterPropertyToolStripMenuItem
            // 
            this.rasterPropertyToolStripMenuItem.Enabled = false;
            this.rasterPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.image;
            this.rasterPropertyToolStripMenuItem.Name = "rasterPropertyToolStripMenuItem";
            this.rasterPropertyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.rasterPropertyToolStripMenuItem.Text = "Raster Property";
            this.rasterPropertyToolStripMenuItem.Click += new System.EventHandler(this.NewRasterProperty_Click);
            // 
            // objectPropertyToolStripMenuItem
            // 
            this.objectPropertyToolStripMenuItem.Enabled = false;
            this.objectPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.package;
            this.objectPropertyToolStripMenuItem.Name = "objectPropertyToolStripMenuItem";
            this.objectPropertyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.objectPropertyToolStripMenuItem.Text = "Object Property";
            this.objectPropertyToolStripMenuItem.Click += new System.EventHandler(this.NewObjectProperty_Click);
            // 
            // associationPropertyToolStripMenuItem
            // 
            this.associationPropertyToolStripMenuItem.Enabled = false;
            this.associationPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.table_relationship;
            this.associationPropertyToolStripMenuItem.Name = "associationPropertyToolStripMenuItem";
            this.associationPropertyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.associationPropertyToolStripMenuItem.Text = "Association Property";
            this.associationPropertyToolStripMenuItem.Click += new System.EventHandler(this.NewAssociationProperty_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Enabled = false;
            this.btnEdit.Image = global::FdoToolbox.Core.Properties.Resources.application_edit;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(45, 22);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = global::FdoToolbox.Core.Properties.Resources.cross;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(58, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMakeIdentity
            // 
            this.btnMakeIdentity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMakeIdentity.Enabled = false;
            this.btnMakeIdentity.Image = global::FdoToolbox.Core.Properties.Resources.key;
            this.btnMakeIdentity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMakeIdentity.Name = "btnMakeIdentity";
            this.btnMakeIdentity.Size = new System.Drawing.Size(23, 22);
            this.btnMakeIdentity.Text = "Make Identity";
            this.btnMakeIdentity.Click += new System.EventHandler(this.btnMakeIdentity_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(303, 339);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(384, 339);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpExtended
            // 
            this.grpExtended.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpExtended.Location = new System.Drawing.Point(3, 245);
            this.grpExtended.Name = "grpExtended";
            this.grpExtended.Size = new System.Drawing.Size(459, 88);
            this.grpExtended.TabIndex = 8;
            this.grpExtended.TabStop = false;
            this.grpExtended.Text = "Extended Information";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lstIdentityProperties);
            this.groupBox3.Controls.Add(this.toolStrip2);
            this.groupBox3.Location = new System.Drawing.Point(308, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(154, 135);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Identity Properties";
            // 
            // lstIdentityProperties
            // 
            this.lstIdentityProperties.DisplayMember = "Name";
            this.lstIdentityProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstIdentityProperties.FormattingEnabled = true;
            this.lstIdentityProperties.Location = new System.Drawing.Point(3, 41);
            this.lstIdentityProperties.Name = "lstIdentityProperties";
            this.lstIdentityProperties.Size = new System.Drawing.Size(148, 82);
            this.lstIdentityProperties.TabIndex = 1;
            this.lstIdentityProperties.SelectedIndexChanged += new System.EventHandler(this.IdentityProperties_SelectedIndexChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemove});
            this.toolStrip2.Location = new System.Drawing.Point(3, 16);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(148, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Image = global::FdoToolbox.Core.Properties.Resources.cross;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(66, 22);
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // ClassDefCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grpExtended);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ClassDefCtl";
            this.Size = new System.Drawing.Size(466, 369);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdProperties)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkComputed;
        private System.Windows.Forms.CheckBox chkAbstract;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView grdProperties;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton btnNewProperty;
        private System.Windows.Forms.ToolStripMenuItem dataPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geometryPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rasterPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem associationPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpExtended;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_TYPE;
        private System.Windows.Forms.ToolStripButton btnMakeIdentity;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lstIdentityProperties;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnRemove;
    }
}
