namespace FdoToolbox.Core.Controls
{
    partial class DataPreviewCtl
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
            this.btnQuery = new System.Windows.Forms.Button();
            this.grpSQL = new System.Windows.Forms.GroupBox();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.grpStandard = new System.Windows.Forms.GroupBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbLimit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.cmbSchema = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbQueryMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grdPreview = new System.Windows.Forms.DataGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpSQL.SuspendLayout();
            this.grpStandard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnQuery);
            this.splitContainer1.Panel1.Controls.Add(this.grpSQL);
            this.splitContainer1.Panel1.Controls.Add(this.grpStandard);
            this.splitContainer1.Panel1.Controls.Add(this.cmbQueryMethod);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grdPreview);
            this.splitContainer1.Size = new System.Drawing.Size(494, 345);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Image = global::FdoToolbox.Core.Properties.Resources.application_go;
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new System.Drawing.Point(442, 10);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(46, 23);
            this.btnQuery.TabIndex = 4;
            this.btnQuery.Text = "Go";
            this.btnQuery.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // grpSQL
            // 
            this.grpSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSQL.Controls.Add(this.txtSQL);
            this.grpSQL.Location = new System.Drawing.Point(3, 124);
            this.grpSQL.Name = "grpSQL";
            this.grpSQL.Size = new System.Drawing.Size(488, 56);
            this.grpSQL.TabIndex = 3;
            this.grpSQL.TabStop = false;
            this.grpSQL.Text = "SQL";
            // 
            // txtSQL
            // 
            this.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSQL.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQL.Location = new System.Drawing.Point(3, 16);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.Size = new System.Drawing.Size(482, 37);
            this.txtSQL.TabIndex = 0;
            // 
            // grpStandard
            // 
            this.grpStandard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStandard.Controls.Add(this.txtFilter);
            this.grpStandard.Controls.Add(this.label5);
            this.grpStandard.Controls.Add(this.cmbLimit);
            this.grpStandard.Controls.Add(this.label4);
            this.grpStandard.Controls.Add(this.cmbClass);
            this.grpStandard.Controls.Add(this.cmbSchema);
            this.grpStandard.Controls.Add(this.label3);
            this.grpStandard.Controls.Add(this.label2);
            this.grpStandard.Location = new System.Drawing.Point(3, 39);
            this.grpStandard.Name = "grpStandard";
            this.grpStandard.Size = new System.Drawing.Size(488, 79);
            this.grpStandard.TabIndex = 2;
            this.grpStandard.TabStop = false;
            this.grpStandard.Text = "Query Options";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(319, 45);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(162, 20);
            this.txtFilter.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(242, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Attribute Filter";
            // 
            // cmbLimit
            // 
            this.cmbLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLimit.FormattingEnabled = true;
            this.cmbLimit.Items.AddRange(new object[] {
            "10",
            "50",
            "100",
            "250",
            "500",
            "1000"});
            this.cmbLimit.Location = new System.Drawing.Point(86, 45);
            this.cmbLimit.Name = "cmbLimit";
            this.cmbLimit.Size = new System.Drawing.Size(149, 21);
            this.cmbLimit.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Preview Limit";
            // 
            // cmbClass
            // 
            this.cmbClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbClass.DisplayMember = "Name";
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(319, 16);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(162, 21);
            this.cmbClass.TabIndex = 3;
            this.cmbClass.SelectedIndexChanged += new System.EventHandler(this.cmbClass_SelectedIndexChanged);
            // 
            // cmbSchema
            // 
            this.cmbSchema.DisplayMember = "Name";
            this.cmbSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchema.FormattingEnabled = true;
            this.cmbSchema.Location = new System.Drawing.Point(86, 16);
            this.cmbSchema.Name = "cmbSchema";
            this.cmbSchema.Size = new System.Drawing.Size(149, 21);
            this.cmbSchema.TabIndex = 2;
            this.cmbSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSchema_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Class";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Schema";
            // 
            // cmbQueryMethod
            // 
            this.cmbQueryMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryMethod.FormattingEnabled = true;
            this.cmbQueryMethod.Items.AddRange(new object[] {
            "Standard"});
            this.cmbQueryMethod.Location = new System.Drawing.Point(105, 12);
            this.cmbQueryMethod.Name = "cmbQueryMethod";
            this.cmbQueryMethod.Size = new System.Drawing.Size(121, 21);
            this.cmbQueryMethod.TabIndex = 1;
            this.cmbQueryMethod.SelectedIndexChanged += new System.EventHandler(this.cmbQueryMethod_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Query Method";
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
            this.grdPreview.Size = new System.Drawing.Size(494, 149);
            this.grdPreview.TabIndex = 0;
            // 
            // DataPreviewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DataPreviewCtl";
            this.Size = new System.Drawing.Size(494, 345);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.grpSQL.ResumeLayout(false);
            this.grpSQL.PerformLayout();
            this.grpStandard.ResumeLayout(false);
            this.grpStandard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grdPreview;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.GroupBox grpSQL;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.GroupBox grpStandard;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbQueryMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbLimit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbClass;
        private System.Windows.Forms.ComboBox cmbSchema;
    }
}
