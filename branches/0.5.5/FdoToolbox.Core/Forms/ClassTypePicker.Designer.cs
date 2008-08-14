namespace FdoToolbox.Core.Forms
{
    partial class ClassTypePicker
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdNetworkLink = new System.Windows.Forms.RadioButton();
            this.rdNetworkNode = new System.Windows.Forms.RadioButton();
            this.rdNetworkLayer = new System.Windows.Forms.RadioButton();
            this.rdNetwork = new System.Windows.Forms.RadioButton();
            this.rdFeature = new System.Windows.Forms.RadioButton();
            this.rdClass = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(124, 215);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(205, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rdNetworkLink);
            this.groupBox1.Controls.Add(this.rdNetworkNode);
            this.groupBox1.Controls.Add(this.rdNetworkLayer);
            this.groupBox1.Controls.Add(this.rdNetwork);
            this.groupBox1.Controls.Add(this.rdFeature);
            this.groupBox1.Controls.Add(this.rdClass);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 197);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select the class type to create";
            // 
            // rdNetworkLink
            // 
            this.rdNetworkLink.AutoSize = true;
            this.rdNetworkLink.Enabled = false;
            this.rdNetworkLink.Location = new System.Drawing.Point(32, 148);
            this.rdNetworkLink.Name = "rdNetworkLink";
            this.rdNetworkLink.Size = new System.Drawing.Size(88, 17);
            this.rdNetworkLink.TabIndex = 5;
            this.rdNetworkLink.TabStop = true;
            this.rdNetworkLink.Text = "Network Link";
            this.rdNetworkLink.UseVisualStyleBackColor = true;
            this.rdNetworkLink.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // rdNetworkNode
            // 
            this.rdNetworkNode.AutoSize = true;
            this.rdNetworkNode.Enabled = false;
            this.rdNetworkNode.Location = new System.Drawing.Point(32, 125);
            this.rdNetworkNode.Name = "rdNetworkNode";
            this.rdNetworkNode.Size = new System.Drawing.Size(94, 17);
            this.rdNetworkNode.TabIndex = 4;
            this.rdNetworkNode.TabStop = true;
            this.rdNetworkNode.Text = "Network Node";
            this.rdNetworkNode.UseVisualStyleBackColor = true;
            this.rdNetworkNode.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // rdNetworkLayer
            // 
            this.rdNetworkLayer.AutoSize = true;
            this.rdNetworkLayer.Enabled = false;
            this.rdNetworkLayer.Location = new System.Drawing.Point(32, 101);
            this.rdNetworkLayer.Name = "rdNetworkLayer";
            this.rdNetworkLayer.Size = new System.Drawing.Size(94, 17);
            this.rdNetworkLayer.TabIndex = 3;
            this.rdNetworkLayer.TabStop = true;
            this.rdNetworkLayer.Text = "Network Layer";
            this.rdNetworkLayer.UseVisualStyleBackColor = true;
            this.rdNetworkLayer.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // rdNetwork
            // 
            this.rdNetwork.AutoSize = true;
            this.rdNetwork.Enabled = false;
            this.rdNetwork.Location = new System.Drawing.Point(32, 78);
            this.rdNetwork.Name = "rdNetwork";
            this.rdNetwork.Size = new System.Drawing.Size(93, 17);
            this.rdNetwork.TabIndex = 2;
            this.rdNetwork.TabStop = true;
            this.rdNetwork.Text = "Network Class";
            this.rdNetwork.UseVisualStyleBackColor = true;
            this.rdNetwork.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // rdFeature
            // 
            this.rdFeature.AutoSize = true;
            this.rdFeature.Enabled = false;
            this.rdFeature.Location = new System.Drawing.Point(32, 54);
            this.rdFeature.Name = "rdFeature";
            this.rdFeature.Size = new System.Drawing.Size(89, 17);
            this.rdFeature.TabIndex = 1;
            this.rdFeature.TabStop = true;
            this.rdFeature.Text = "Feature Class";
            this.rdFeature.UseVisualStyleBackColor = true;
            this.rdFeature.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // rdClass
            // 
            this.rdClass.AutoSize = true;
            this.rdClass.Enabled = false;
            this.rdClass.Location = new System.Drawing.Point(32, 30);
            this.rdClass.Name = "rdClass";
            this.rdClass.Size = new System.Drawing.Size(113, 17);
            this.rdClass.TabIndex = 0;
            this.rdClass.TabStop = true;
            this.rdClass.Text = "Class (non-feature)";
            this.rdClass.UseVisualStyleBackColor = true;
            this.rdClass.CheckedChanged += new System.EventHandler(this.Class_CheckChanged);
            // 
            // ClassTypePicker
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 250);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "ClassTypePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Class";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdNetworkNode;
        private System.Windows.Forms.RadioButton rdNetworkLayer;
        private System.Windows.Forms.RadioButton rdNetwork;
        private System.Windows.Forms.RadioButton rdFeature;
        private System.Windows.Forms.RadioButton rdClass;
        private System.Windows.Forms.RadioButton rdNetworkLink;

    }
}