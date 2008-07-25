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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Classes");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.cmbSrcSchema = new System.Windows.Forms.ComboBox();
            this.cmbSrcConn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.cmbDestSchema = new System.Windows.Forms.ComboBox();
            this.cmbDestConn = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkCoerceDataTypes = new System.Windows.Forms.CheckBox();
            this.chkCopySpatialContexts = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkSourceContextList = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(523, 49);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(90, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(400, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // grpSource
            // 
            this.grpSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSource.Controls.Add(this.cmbSrcSchema);
            this.grpSource.Controls.Add(this.cmbSrcConn);
            this.grpSource.Controls.Add(this.label3);
            this.grpSource.Controls.Add(this.label2);
            this.grpSource.Location = new System.Drawing.Point(281, 59);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(246, 71);
            this.grpSource.TabIndex = 1;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // cmbSrcSchema
            // 
            this.cmbSrcSchema.DisplayMember = "Name";
            this.cmbSrcSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcSchema.FormattingEnabled = true;
            this.cmbSrcSchema.Location = new System.Drawing.Point(74, 43);
            this.cmbSrcSchema.Name = "cmbSrcSchema";
            this.cmbSrcSchema.Size = new System.Drawing.Size(160, 21);
            this.cmbSrcSchema.TabIndex = 3;
            this.cmbSrcSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSrcSchema_SelectedIndexChanged);
            // 
            // cmbSrcConn
            // 
            this.cmbSrcConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSrcConn.FormattingEnabled = true;
            this.cmbSrcConn.Location = new System.Drawing.Point(74, 16);
            this.cmbSrcConn.Name = "cmbSrcConn";
            this.cmbSrcConn.Size = new System.Drawing.Size(160, 21);
            this.cmbSrcConn.TabIndex = 2;
            this.cmbSrcConn.SelectedIndexChanged += new System.EventHandler(this.cmbSrcConn_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Schema";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Connection";
            // 
            // grpTarget
            // 
            this.grpTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTarget.Controls.Add(this.cmbDestSchema);
            this.grpTarget.Controls.Add(this.cmbDestConn);
            this.grpTarget.Controls.Add(this.label5);
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Location = new System.Drawing.Point(281, 136);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Size = new System.Drawing.Size(249, 75);
            this.grpTarget.TabIndex = 2;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "Target";
            // 
            // cmbDestSchema
            // 
            this.cmbDestSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDestSchema.DisplayMember = "Name";
            this.cmbDestSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestSchema.FormattingEnabled = true;
            this.cmbDestSchema.Location = new System.Drawing.Point(74, 46);
            this.cmbDestSchema.Name = "cmbDestSchema";
            this.cmbDestSchema.Size = new System.Drawing.Size(160, 21);
            this.cmbDestSchema.TabIndex = 7;
            this.cmbDestSchema.SelectedIndexChanged += new System.EventHandler(this.cmbDestSchema_SelectedIndexChanged);
            // 
            // cmbDestConn
            // 
            this.cmbDestConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDestConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestConn.FormattingEnabled = true;
            this.cmbDestConn.Location = new System.Drawing.Point(74, 19);
            this.cmbDestConn.Name = "cmbDestConn";
            this.cmbDestConn.Size = new System.Drawing.Size(160, 21);
            this.cmbDestConn.TabIndex = 6;
            this.cmbDestConn.SelectedIndexChanged += new System.EventHandler(this.cmbDestConn_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Connection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Schema";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.mTreeView);
            this.groupBox2.Location = new System.Drawing.Point(4, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(271, 283);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mappings";
            // 
            // mTreeView
            // 
            this.mTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTreeView.Location = new System.Drawing.Point(3, 16);
            this.mTreeView.Name = "mTreeView";
            treeNode1.Name = "NODE_CLASSES";
            treeNode1.Text = "Classes";
            this.mTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.mTreeView.Size = new System.Drawing.Size(265, 264);
            this.mTreeView.TabIndex = 0;
            this.mTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mTreeView_MouseDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Location = new System.Drawing.Point(281, 217);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(246, 125);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Bulk Copy Options";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.chkSourceContextList);
            this.panel1.Controls.Add(this.chkCoerceDataTypes);
            this.panel1.Controls.Add(this.chkCopySpatialContexts);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 106);
            this.panel1.TabIndex = 0;
            // 
            // chkCoerceDataTypes
            // 
            this.chkCoerceDataTypes.AutoSize = true;
            this.chkCoerceDataTypes.Enabled = false;
            this.chkCoerceDataTypes.Location = new System.Drawing.Point(8, 86);
            this.chkCoerceDataTypes.Name = "chkCoerceDataTypes";
            this.chkCoerceDataTypes.Size = new System.Drawing.Size(155, 17);
            this.chkCoerceDataTypes.TabIndex = 5;
            this.chkCoerceDataTypes.Text = "Coerce Source Data Types";
            this.toolTip.SetToolTip(this.chkCoerceDataTypes, "If source/target properties are of different types, the conver the source data ty" +
                    "pe to the target\'s data type");
            this.chkCoerceDataTypes.UseVisualStyleBackColor = true;
            // 
            // chkCopySpatialContexts
            // 
            this.chkCopySpatialContexts.AutoSize = true;
            this.chkCopySpatialContexts.Location = new System.Drawing.Point(8, 3);
            this.chkCopySpatialContexts.Name = "chkCopySpatialContexts";
            this.chkCopySpatialContexts.Size = new System.Drawing.Size(129, 17);
            this.chkCopySpatialContexts.TabIndex = 4;
            this.chkCopySpatialContexts.Text = "Copy Spatial Contexts";
            this.toolTip.SetToolTip(this.chkCopySpatialContexts, "Copy spatial contexts defined in the source connection to the target connection");
            this.chkCopySpatialContexts.UseVisualStyleBackColor = true;
            this.chkCopySpatialContexts.CheckedChanged += new System.EventHandler(this.chkCopySpatialContexts_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(371, 348);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(452, 348);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // chkSourceContextList
            // 
            this.chkSourceContextList.FormattingEnabled = true;
            this.chkSourceContextList.Location = new System.Drawing.Point(7, 27);
            this.chkSourceContextList.Name = "chkSourceContextList";
            this.chkSourceContextList.Size = new System.Drawing.Size(224, 49);
            this.chkSourceContextList.TabIndex = 6;
            // 
            // BulkCopyCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.groupBox1);
            this.Name = "BulkCopyCtl";
            this.Size = new System.Drawing.Size(530, 374);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.ComboBox cmbSrcSchema;
        private System.Windows.Forms.ComboBox cmbSrcConn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.ComboBox cmbDestSchema;
        private System.Windows.Forms.ComboBox cmbDestConn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TreeView mTreeView;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkCoerceDataTypes;
        private System.Windows.Forms.CheckBox chkCopySpatialContexts;
        private System.Windows.Forms.CheckedListBox chkSourceContextList;
    }
}
