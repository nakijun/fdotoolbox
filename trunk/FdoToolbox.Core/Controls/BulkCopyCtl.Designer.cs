namespace FdoToolbox.Core.Controls
{
    partial class BulkCopyCtl
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbSrcClass = new System.Windows.Forms.ComboBox();
            this.cmbSrcSchema = new System.Windows.Forms.ComboBox();
            this.cmbSrcConn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbDestClass = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbDestSchema = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbDestConn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.grdMappings = new System.Windows.Forms.DataGridView();
            this.COL_SOURCE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_TARGET = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkCoerce = new System.Windows.Forms.CheckBox();
            this.txtLimit = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkTransform = new System.Windows.Forms.CheckBox();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(334, 308);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(415, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbSrcClass);
            this.groupBox1.Controls.Add(this.cmbSrcSchema);
            this.groupBox1.Controls.Add(this.cmbSrcConn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(241, 114);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // cmbSrcClass
            // 
            this.cmbSrcClass.DisplayMember = "Name";
            this.cmbSrcClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcClass.FormattingEnabled = true;
            this.cmbSrcClass.Location = new System.Drawing.Point(103, 78);
            this.cmbSrcClass.Name = "cmbSrcClass";
            this.cmbSrcClass.Size = new System.Drawing.Size(121, 21);
            this.cmbSrcClass.TabIndex = 5;
            this.cmbSrcClass.SelectedIndexChanged += new System.EventHandler(this.cmbSrcClass_SelectedIndexChanged);
            // 
            // cmbSrcSchema
            // 
            this.cmbSrcSchema.DisplayMember = "Name";
            this.cmbSrcSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcSchema.FormattingEnabled = true;
            this.cmbSrcSchema.Location = new System.Drawing.Point(103, 49);
            this.cmbSrcSchema.Name = "cmbSrcSchema";
            this.cmbSrcSchema.Size = new System.Drawing.Size(121, 21);
            this.cmbSrcSchema.TabIndex = 4;
            this.cmbSrcSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSrcSchema_SelectedIndexChanged);
            // 
            // cmbSrcConn
            // 
            this.cmbSrcConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcConn.FormattingEnabled = true;
            this.cmbSrcConn.Location = new System.Drawing.Point(103, 19);
            this.cmbSrcConn.Name = "cmbSrcConn";
            this.cmbSrcConn.Size = new System.Drawing.Size(121, 21);
            this.cmbSrcConn.TabIndex = 3;
            this.cmbSrcConn.SelectedIndexChanged += new System.EventHandler(this.cmbSrcConn_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Feature Class";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Schema";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbDestClass);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cmbDestSchema);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cmbDestConn);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(251, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 114);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target";
            // 
            // cmbDestClass
            // 
            this.cmbDestClass.DisplayMember = "Name";
            this.cmbDestClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestClass.FormattingEnabled = true;
            this.cmbDestClass.Location = new System.Drawing.Point(102, 78);
            this.cmbDestClass.Name = "cmbDestClass";
            this.cmbDestClass.Size = new System.Drawing.Size(121, 21);
            this.cmbDestClass.TabIndex = 11;
            this.cmbDestClass.SelectedIndexChanged += new System.EventHandler(this.cmbDestClass_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Connection";
            // 
            // cmbDestSchema
            // 
            this.cmbDestSchema.DisplayMember = "Name";
            this.cmbDestSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestSchema.FormattingEnabled = true;
            this.cmbDestSchema.Location = new System.Drawing.Point(102, 49);
            this.cmbDestSchema.Name = "cmbDestSchema";
            this.cmbDestSchema.Size = new System.Drawing.Size(121, 21);
            this.cmbDestSchema.TabIndex = 10;
            this.cmbDestSchema.SelectedIndexChanged += new System.EventHandler(this.cmbDestSchema_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Schema";
            // 
            // cmbDestConn
            // 
            this.cmbDestConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestConn.FormattingEnabled = true;
            this.cmbDestConn.Location = new System.Drawing.Point(102, 19);
            this.cmbDestConn.Name = "cmbDestConn";
            this.cmbDestConn.Size = new System.Drawing.Size(121, 21);
            this.cmbDestConn.TabIndex = 9;
            this.cmbDestConn.SelectedIndexChanged += new System.EventHandler(this.cmbDestConn_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Feature Class";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.grdMappings);
            this.groupBox3.Location = new System.Drawing.Point(4, 150);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(486, 73);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Property Mappings";
            // 
            // grdMappings
            // 
            this.grdMappings.AllowUserToAddRows = false;
            this.grdMappings.AllowUserToDeleteRows = false;
            this.grdMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMappings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_SOURCE,
            this.COL_TARGET});
            this.grdMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMappings.Location = new System.Drawing.Point(3, 16);
            this.grdMappings.Name = "grdMappings";
            this.grdMappings.Size = new System.Drawing.Size(480, 54);
            this.grdMappings.TabIndex = 0;
            // 
            // COL_SOURCE
            // 
            this.COL_SOURCE.HeaderText = "Source Property";
            this.COL_SOURCE.MinimumWidth = 120;
            this.COL_SOURCE.Name = "COL_SOURCE";
            this.COL_SOURCE.ReadOnly = true;
            this.COL_SOURCE.Width = 120;
            // 
            // COL_TARGET
            // 
            this.COL_TARGET.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_TARGET.HeaderText = "Target Property";
            this.COL_TARGET.Name = "COL_TARGET";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chkCoerce);
            this.groupBox4.Controls.Add(this.txtLimit);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.chkTransform);
            this.groupBox4.Controls.Add(this.chkDelete);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(4, 226);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(483, 76);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other Settings";
            // 
            // chkCoerce
            // 
            this.chkCoerce.AutoSize = true;
            this.chkCoerce.Location = new System.Drawing.Point(257, 44);
            this.chkCoerce.Name = "chkCoerce";
            this.chkCoerce.Size = new System.Drawing.Size(118, 17);
            this.chkCoerce.TabIndex = 4;
            this.chkCoerce.Text = "Coerce Data Types";
            this.chkCoerce.UseVisualStyleBackColor = true;
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(330, 17);
            this.txtLimit.Mask = "000000000";
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(140, 20);
            this.txtLimit.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(254, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Feature Limit";
            // 
            // chkTransform
            // 
            this.chkTransform.AutoSize = true;
            this.chkTransform.Location = new System.Drawing.Point(20, 44);
            this.chkTransform.Name = "chkTransform";
            this.chkTransform.Size = new System.Drawing.Size(180, 17);
            this.chkTransform.TabIndex = 1;
            this.chkTransform.Text = "Transform Geometry Coordinates";
            this.chkTransform.UseVisualStyleBackColor = true;
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Location = new System.Drawing.Point(20, 20);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(208, 17);
            this.chkDelete.TabIndex = 0;
            this.chkDelete.Text = "Clear/Delete target first before copying";
            this.chkDelete.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Task Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(104, 7);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(370, 20);
            this.txtName.TabIndex = 7;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // BulkCopyCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "BulkCopyCtl";
            this.Size = new System.Drawing.Size(493, 336);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbSrcClass;
        private System.Windows.Forms.ComboBox cmbSrcSchema;
        private System.Windows.Forms.ComboBox cmbSrcConn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDestClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbDestSchema;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbDestConn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView grdMappings;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.MaskedTextBox txtLimit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkTransform;
        private System.Windows.Forms.CheckBox chkDelete;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_SOURCE;
        private System.Windows.Forms.DataGridViewComboBoxColumn COL_TARGET;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkCoerce;
    }
}
