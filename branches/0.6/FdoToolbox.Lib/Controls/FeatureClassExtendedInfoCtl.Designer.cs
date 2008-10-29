namespace FdoToolbox.Lib.Controls
{
    partial class FeatureClassExtendedInfoCtl
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbGeometryProperty = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Geometry Property";
            // 
            // cmbGeometryProperty
            // 
            this.cmbGeometryProperty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGeometryProperty.DisplayMember = "Name";
            this.cmbGeometryProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGeometryProperty.FormattingEnabled = true;
            this.cmbGeometryProperty.Location = new System.Drawing.Point(133, 11);
            this.cmbGeometryProperty.Name = "cmbGeometryProperty";
            this.cmbGeometryProperty.Size = new System.Drawing.Size(260, 21);
            this.cmbGeometryProperty.TabIndex = 1;
            // 
            // FeatureClassExtendedInfoCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbGeometryProperty);
            this.Controls.Add(this.label1);
            this.Name = "FeatureClassExtendedInfoCtl";
            this.Size = new System.Drawing.Size(418, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbGeometryProperty;
    }
}
