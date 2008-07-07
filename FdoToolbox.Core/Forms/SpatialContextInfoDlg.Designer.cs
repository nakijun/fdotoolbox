namespace FdoToolbox.Core.Forms
{
    partial class SpatialContextInfoDlg
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
            this.txtZTolerance = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtXYTolerance = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCoordSysWkt = new System.Windows.Forms.TextBox();
            this.txtCoordSys = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbExtentType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpExtents = new System.Windows.Forms.GroupBox();
            this.txtUpperRightY = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtUpperRightX = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtLowerLeftY = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtLowerLeftX = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.grpExtents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(247, 342);
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
            this.btnCancel.Location = new System.Drawing.Point(328, 342);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtZTolerance);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtXYTolerance);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtCoordSysWkt);
            this.groupBox1.Controls.Add(this.txtCoordSys);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbExtentType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 231);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // txtZTolerance
            // 
            this.txtZTolerance.Location = new System.Drawing.Point(160, 176);
            this.txtZTolerance.Name = "txtZTolerance";
            this.txtZTolerance.Size = new System.Drawing.Size(214, 20);
            this.txtZTolerance.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(89, 179);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Z Tolerance";
            // 
            // txtXYTolerance
            // 
            this.txtXYTolerance.Location = new System.Drawing.Point(160, 149);
            this.txtXYTolerance.Name = "txtXYTolerance";
            this.txtXYTolerance.Size = new System.Drawing.Size(214, 20);
            this.txtXYTolerance.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(77, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "X/Y Tolerance";
            // 
            // txtCoordSysWkt
            // 
            this.txtCoordSysWkt.Location = new System.Drawing.Point(160, 100);
            this.txtCoordSysWkt.Multiline = true;
            this.txtCoordSysWkt.Name = "txtCoordSysWkt";
            this.txtCoordSysWkt.Size = new System.Drawing.Size(214, 42);
            this.txtCoordSysWkt.TabIndex = 7;
            // 
            // txtCoordSys
            // 
            this.txtCoordSys.Location = new System.Drawing.Point(160, 73);
            this.txtCoordSys.Name = "txtCoordSys";
            this.txtCoordSys.Size = new System.Drawing.Size(214, 20);
            this.txtCoordSys.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(160, 47);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(214, 20);
            this.txtDescription.TabIndex = 5;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(160, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(214, 20);
            this.txtName.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Coordinate System WKT";
            // 
            // cmbExtentType
            // 
            this.cmbExtentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtentType.FormattingEnabled = true;
            this.cmbExtentType.Location = new System.Drawing.Point(160, 202);
            this.cmbExtentType.Name = "cmbExtentType";
            this.cmbExtentType.Size = new System.Drawing.Size(214, 21);
            this.cmbExtentType.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Coordinate System";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(89, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Extent Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // grpExtents
            // 
            this.grpExtents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpExtents.Controls.Add(this.txtUpperRightY);
            this.grpExtents.Controls.Add(this.label11);
            this.grpExtents.Controls.Add(this.txtUpperRightX);
            this.grpExtents.Controls.Add(this.label12);
            this.grpExtents.Controls.Add(this.txtLowerLeftY);
            this.grpExtents.Controls.Add(this.label10);
            this.grpExtents.Controls.Add(this.txtLowerLeftX);
            this.grpExtents.Controls.Add(this.label9);
            this.grpExtents.Controls.Add(this.label8);
            this.grpExtents.Location = new System.Drawing.Point(13, 250);
            this.grpExtents.Name = "grpExtents";
            this.grpExtents.Size = new System.Drawing.Size(390, 77);
            this.grpExtents.TabIndex = 3;
            this.grpExtents.TabStop = false;
            this.grpExtents.Text = "Extents";
            // 
            // txtUpperRightY
            // 
            this.txtUpperRightY.Location = new System.Drawing.Point(274, 45);
            this.txtUpperRightY.Name = "txtUpperRightY";
            this.txtUpperRightY.Size = new System.Drawing.Size(100, 20);
            this.txtUpperRightY.TabIndex = 10;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(195, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Upper Right (Y)";
            // 
            // txtUpperRightX
            // 
            this.txtUpperRightX.Location = new System.Drawing.Point(86, 45);
            this.txtUpperRightX.Name = "txtUpperRightX";
            this.txtUpperRightX.Size = new System.Drawing.Size(100, 20);
            this.txtUpperRightX.TabIndex = 8;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(5, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Upper Right (X)";
            // 
            // txtLowerLeftY
            // 
            this.txtLowerLeftY.Location = new System.Drawing.Point(274, 19);
            this.txtLowerLeftY.Name = "txtLowerLeftY";
            this.txtLowerLeftY.Size = new System.Drawing.Size(100, 20);
            this.txtLowerLeftY.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(202, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Lower Left (Y)";
            // 
            // txtLowerLeftX
            // 
            this.txtLowerLeftX.Location = new System.Drawing.Point(86, 19);
            this.txtLowerLeftX.Name = "txtLowerLeftX";
            this.txtLowerLeftX.Size = new System.Drawing.Size(100, 20);
            this.txtLowerLeftX.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Lower Left (X)";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // SpatialContextInfoDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 377);
            this.Controls.Add(this.grpExtents);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpatialContextInfoDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spatial Context Information";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpExtents.ResumeLayout(false);
            this.grpExtents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpExtents;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtZTolerance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtXYTolerance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCoordSysWkt;
        private System.Windows.Forms.TextBox txtCoordSys;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbExtentType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUpperRightY;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtUpperRightX;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtLowerLeftY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtLowerLeftX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}