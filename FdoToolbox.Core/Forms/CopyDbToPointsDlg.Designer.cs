namespace FdoToolbox.Core.Forms
{
    partial class CopyDbToPointsDlg
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkColumns = new System.Windows.Forms.CheckedListBox();
            this.txtTable = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.cmbTargetSchema = new System.Windows.Forms.ComboBox();
            this.cmbTargetConnection = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chk3d = new System.Windows.Forms.CheckBox();
            this.cmbZ = new System.Windows.Forms.ComboBox();
            this.cmbY = new System.Windows.Forms.ComboBox();
            this.cmbX = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(282, 307);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(364, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkColumns);
            this.groupBox1.Controls.Add(this.txtTable);
            this.groupBox1.Controls.Add(this.txtDatabase);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 288);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // chkColumns
            // 
            this.chkColumns.FormattingEnabled = true;
            this.chkColumns.Location = new System.Drawing.Point(20, 103);
            this.chkColumns.Name = "chkColumns";
            this.chkColumns.Size = new System.Drawing.Size(165, 169);
            this.chkColumns.TabIndex = 5;
            // 
            // txtTable
            // 
            this.txtTable.Location = new System.Drawing.Point(85, 57);
            this.txtTable.Name = "txtTable";
            this.txtTable.ReadOnly = true;
            this.txtTable.Size = new System.Drawing.Size(100, 20);
            this.txtTable.TabIndex = 4;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(85, 30);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(100, 20);
            this.txtDatabase.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Table";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtClass);
            this.groupBox2.Controls.Add(this.cmbTargetSchema);
            this.groupBox2.Controls.Add(this.cmbTargetConnection);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(220, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(219, 116);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target";
            // 
            // txtClass
            // 
            this.txtClass.Location = new System.Drawing.Point(89, 84);
            this.txtClass.Name = "txtClass";
            this.txtClass.Size = new System.Drawing.Size(121, 20);
            this.txtClass.TabIndex = 5;
            // 
            // cmbTargetSchema
            // 
            this.cmbTargetSchema.DisplayMember = "Name";
            this.cmbTargetSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetSchema.FormattingEnabled = true;
            this.cmbTargetSchema.Location = new System.Drawing.Point(89, 57);
            this.cmbTargetSchema.Name = "cmbTargetSchema";
            this.cmbTargetSchema.Size = new System.Drawing.Size(121, 21);
            this.cmbTargetSchema.TabIndex = 4;
            this.cmbTargetSchema.ValueMember = "Name";
            // 
            // cmbTargetConnection
            // 
            this.cmbTargetConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetConnection.FormattingEnabled = true;
            this.cmbTargetConnection.Location = new System.Drawing.Point(89, 29);
            this.cmbTargetConnection.Name = "cmbTargetConnection";
            this.cmbTargetConnection.Size = new System.Drawing.Size(121, 21);
            this.cmbTargetConnection.TabIndex = 3;
            this.cmbTargetConnection.SelectedIndexChanged += new System.EventHandler(this.cmbTargetConnection_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Class";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Schema";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Connection";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chk3d);
            this.groupBox3.Controls.Add(this.cmbZ);
            this.groupBox3.Controls.Add(this.cmbY);
            this.groupBox3.Controls.Add(this.cmbX);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(220, 136);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(219, 165);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Point";
            // 
            // chk3d
            // 
            this.chk3d.AutoSize = true;
            this.chk3d.Location = new System.Drawing.Point(89, 109);
            this.chk3d.Name = "chk3d";
            this.chk3d.Size = new System.Drawing.Size(87, 17);
            this.chk3d.TabIndex = 6;
            this.chk3d.Text = "3 dimensions";
            this.chk3d.UseVisualStyleBackColor = true;
            this.chk3d.CheckedChanged += new System.EventHandler(this.chk3d_CheckedChanged);
            // 
            // cmbZ
            // 
            this.cmbZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZ.FormattingEnabled = true;
            this.cmbZ.Location = new System.Drawing.Point(89, 73);
            this.cmbZ.Name = "cmbZ";
            this.cmbZ.Size = new System.Drawing.Size(121, 21);
            this.cmbZ.TabIndex = 5;
            // 
            // cmbY
            // 
            this.cmbY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbY.FormattingEnabled = true;
            this.cmbY.Location = new System.Drawing.Point(89, 46);
            this.cmbY.Name = "cmbY";
            this.cmbY.Size = new System.Drawing.Size(121, 21);
            this.cmbY.TabIndex = 4;
            // 
            // cmbX
            // 
            this.cmbX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbX.FormattingEnabled = true;
            this.cmbX.Location = new System.Drawing.Point(89, 19);
            this.cmbX.Name = "cmbX";
            this.cmbX.Size = new System.Drawing.Size(121, 21);
            this.cmbX.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Z Column";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Y Column";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "X Column";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // CopyDbToPointsDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(451, 342);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyDbToPointsDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Convert Table";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtTable;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbTargetSchema;
        private System.Windows.Forms.ComboBox cmbTargetConnection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox chkColumns;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chk3d;
        private System.Windows.Forms.ComboBox cmbZ;
        private System.Windows.Forms.ComboBox cmbY;
        private System.Windows.Forms.ComboBox cmbX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}