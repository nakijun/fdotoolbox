namespace FdoToolbox.Core.Controls
{
    partial class SpatialDataPreviewCtl
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
            this.btnClear = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabQueryMode = new System.Windows.Forms.TabControl();
            this.pageStandard = new System.Windows.Forms.TabPage();
            this.btnEditFilter = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbLimit = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSchema = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pageAggregates = new System.Windows.Forms.TabPage();
            this.btnDeleteExpr = new System.Windows.Forms.Button();
            this.btnAddExpr = new System.Windows.Forms.Button();
            this.grdExpressions = new System.Windows.Forms.DataGridView();
            this.COL_EXPR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COL_ALIAS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkDistinct = new System.Windows.Forms.CheckBox();
            this.cmbAggSchema = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbAggClass = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pageSQL = new System.Windows.Forms.TabPage();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.grdPreview = new System.Windows.Forms.DataGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabQueryMode.SuspendLayout();
            this.pageStandard.SuspendLayout();
            this.pageAggregates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdExpressions)).BeginInit();
            this.pageSQL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnQuery);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grdPreview);
            this.splitContainer1.Size = new System.Drawing.Size(494, 345);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(413, 212);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear Grid";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Image = global::FdoToolbox.Core.Properties.Resources.application_go;
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new System.Drawing.Point(360, 212);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(47, 23);
            this.btnQuery.TabIndex = 1;
            this.btnQuery.Text = "Go";
            this.btnQuery.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tabQueryMode);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 206);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Query Parameters";
            // 
            // tabQueryMode
            // 
            this.tabQueryMode.Controls.Add(this.pageStandard);
            this.tabQueryMode.Controls.Add(this.pageAggregates);
            this.tabQueryMode.Controls.Add(this.pageSQL);
            this.tabQueryMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabQueryMode.Location = new System.Drawing.Point(3, 16);
            this.tabQueryMode.Name = "tabQueryMode";
            this.tabQueryMode.SelectedIndex = 0;
            this.tabQueryMode.Size = new System.Drawing.Size(482, 187);
            this.tabQueryMode.TabIndex = 0;
            // 
            // pageStandard
            // 
            this.pageStandard.Controls.Add(this.btnEditFilter);
            this.pageStandard.Controls.Add(this.txtFilter);
            this.pageStandard.Controls.Add(this.label4);
            this.pageStandard.Controls.Add(this.cmbLimit);
            this.pageStandard.Controls.Add(this.label3);
            this.pageStandard.Controls.Add(this.cmbClass);
            this.pageStandard.Controls.Add(this.label2);
            this.pageStandard.Controls.Add(this.cmbSchema);
            this.pageStandard.Controls.Add(this.label1);
            this.pageStandard.Location = new System.Drawing.Point(4, 22);
            this.pageStandard.Name = "pageStandard";
            this.pageStandard.Padding = new System.Windows.Forms.Padding(3);
            this.pageStandard.Size = new System.Drawing.Size(474, 161);
            this.pageStandard.TabIndex = 0;
            this.pageStandard.Text = "Standard";
            this.pageStandard.UseVisualStyleBackColor = true;
            // 
            // btnEditFilter
            // 
            this.btnEditFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditFilter.Location = new System.Drawing.Point(440, 61);
            this.btnEditFilter.Name = "btnEditFilter";
            this.btnEditFilter.Size = new System.Drawing.Size(28, 23);
            this.btnEditFilter.TabIndex = 8;
            this.btnEditFilter.Text = "...";
            this.btnEditFilter.UseVisualStyleBackColor = true;
            this.btnEditFilter.Click += new System.EventHandler(this.btnEditFilter_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(91, 61);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(343, 94);
            this.txtFilter.TabIndex = 7;
            this.txtFilter.Leave += new System.EventHandler(this.txtFilter_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Filter";
            // 
            // cmbLimit
            // 
            this.cmbLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLimit.FormattingEnabled = true;
            this.cmbLimit.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100"});
            this.cmbLimit.Location = new System.Drawing.Point(91, 33);
            this.cmbLimit.Name = "cmbLimit";
            this.cmbLimit.Size = new System.Drawing.Size(139, 21);
            this.cmbLimit.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Limit";
            // 
            // cmbClass
            // 
            this.cmbClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbClass.DisplayMember = "Name";
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(303, 6);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(131, 21);
            this.cmbClass.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Class";
            // 
            // cmbSchema
            // 
            this.cmbSchema.DisplayMember = "Name";
            this.cmbSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchema.FormattingEnabled = true;
            this.cmbSchema.Location = new System.Drawing.Point(91, 6);
            this.cmbSchema.Name = "cmbSchema";
            this.cmbSchema.Size = new System.Drawing.Size(139, 21);
            this.cmbSchema.TabIndex = 1;
            this.cmbSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSchema_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Schema";
            // 
            // pageAggregates
            // 
            this.pageAggregates.Controls.Add(this.btnDeleteExpr);
            this.pageAggregates.Controls.Add(this.btnAddExpr);
            this.pageAggregates.Controls.Add(this.grdExpressions);
            this.pageAggregates.Controls.Add(this.chkDistinct);
            this.pageAggregates.Controls.Add(this.cmbAggSchema);
            this.pageAggregates.Controls.Add(this.label6);
            this.pageAggregates.Controls.Add(this.cmbAggClass);
            this.pageAggregates.Controls.Add(this.label5);
            this.pageAggregates.Location = new System.Drawing.Point(4, 22);
            this.pageAggregates.Name = "pageAggregates";
            this.pageAggregates.Padding = new System.Windows.Forms.Padding(3);
            this.pageAggregates.Size = new System.Drawing.Size(474, 161);
            this.pageAggregates.TabIndex = 1;
            this.pageAggregates.Text = "Aggregates";
            this.pageAggregates.UseVisualStyleBackColor = true;
            // 
            // btnDeleteExpr
            // 
            this.btnDeleteExpr.Enabled = false;
            this.btnDeleteExpr.Location = new System.Drawing.Point(59, 132);
            this.btnDeleteExpr.Name = "btnDeleteExpr";
            this.btnDeleteExpr.Size = new System.Drawing.Size(52, 23);
            this.btnDeleteExpr.TabIndex = 9;
            this.btnDeleteExpr.Text = "Delete";
            this.btnDeleteExpr.UseVisualStyleBackColor = true;
            this.btnDeleteExpr.Click += new System.EventHandler(this.btnDeleteExpr_Click);
            // 
            // btnAddExpr
            // 
            this.btnAddExpr.Location = new System.Drawing.Point(7, 132);
            this.btnAddExpr.Name = "btnAddExpr";
            this.btnAddExpr.Size = new System.Drawing.Size(46, 23);
            this.btnAddExpr.TabIndex = 7;
            this.btnAddExpr.Text = "Add";
            this.btnAddExpr.UseVisualStyleBackColor = true;
            this.btnAddExpr.Click += new System.EventHandler(this.btnAddExpr_Click);
            // 
            // grdExpressions
            // 
            this.grdExpressions.AllowUserToAddRows = false;
            this.grdExpressions.AllowUserToDeleteRows = false;
            this.grdExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdExpressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdExpressions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_EXPR,
            this.COL_ALIAS});
            this.grdExpressions.Location = new System.Drawing.Point(9, 34);
            this.grdExpressions.Name = "grdExpressions";
            this.grdExpressions.Size = new System.Drawing.Size(447, 92);
            this.grdExpressions.TabIndex = 6;
            this.grdExpressions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdExpressions_CellContentClick);
            // 
            // COL_EXPR
            // 
            this.COL_EXPR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_EXPR.HeaderText = "Expression";
            this.COL_EXPR.Name = "COL_EXPR";
            this.COL_EXPR.ReadOnly = true;
            // 
            // COL_ALIAS
            // 
            this.COL_ALIAS.HeaderText = "Alias";
            this.COL_ALIAS.Name = "COL_ALIAS";
            // 
            // chkDistinct
            // 
            this.chkDistinct.AutoSize = true;
            this.chkDistinct.Location = new System.Drawing.Point(117, 136);
            this.chkDistinct.Name = "chkDistinct";
            this.chkDistinct.Size = new System.Drawing.Size(61, 17);
            this.chkDistinct.TabIndex = 5;
            this.chkDistinct.Text = "Distinct";
            this.chkDistinct.UseVisualStyleBackColor = true;
            // 
            // cmbAggSchema
            // 
            this.cmbAggSchema.DisplayMember = "Name";
            this.cmbAggSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAggSchema.FormattingEnabled = true;
            this.cmbAggSchema.Location = new System.Drawing.Point(97, 7);
            this.cmbAggSchema.Name = "cmbAggSchema";
            this.cmbAggSchema.Size = new System.Drawing.Size(121, 21);
            this.cmbAggSchema.TabIndex = 4;
            this.cmbAggSchema.SelectedIndexChanged += new System.EventHandler(this.cmbAggSchema_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Feature Schema";
            // 
            // cmbAggClass
            // 
            this.cmbAggClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAggClass.DisplayMember = "Name";
            this.cmbAggClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAggClass.FormattingEnabled = true;
            this.cmbAggClass.Location = new System.Drawing.Point(310, 7);
            this.cmbAggClass.Name = "cmbAggClass";
            this.cmbAggClass.Size = new System.Drawing.Size(146, 21);
            this.cmbAggClass.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Feature Class";
            // 
            // pageSQL
            // 
            this.pageSQL.Controls.Add(this.txtSQL);
            this.pageSQL.Location = new System.Drawing.Point(4, 22);
            this.pageSQL.Name = "pageSQL";
            this.pageSQL.Size = new System.Drawing.Size(474, 161);
            this.pageSQL.TabIndex = 2;
            this.pageSQL.Text = "SQL";
            this.pageSQL.UseVisualStyleBackColor = true;
            // 
            // txtSQL
            // 
            this.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSQL.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQL.Location = new System.Drawing.Point(0, 0);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.Size = new System.Drawing.Size(474, 161);
            this.txtSQL.TabIndex = 0;
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
            this.grdPreview.Size = new System.Drawing.Size(494, 101);
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
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabQueryMode.ResumeLayout(false);
            this.pageStandard.ResumeLayout(false);
            this.pageStandard.PerformLayout();
            this.pageAggregates.ResumeLayout(false);
            this.pageAggregates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdExpressions)).EndInit();
            this.pageSQL.ResumeLayout(false);
            this.pageSQL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grdPreview;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabQueryMode;
        private System.Windows.Forms.TabPage pageStandard;
        private System.Windows.Forms.TabPage pageAggregates;
        private System.Windows.Forms.TabPage pageSQL;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbLimit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSchema;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAggClass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbAggSchema;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkDistinct;
        private System.Windows.Forms.DataGridView grdExpressions;
        private System.Windows.Forms.Button btnDeleteExpr;
        private System.Windows.Forms.Button btnAddExpr;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_EXPR;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_ALIAS;
        private System.Windows.Forms.Button btnEditFilter;
    }
}
