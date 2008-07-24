namespace FdoToolbox.Core.Forms
{
    partial class ExpressionDlg
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
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnFunctions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnConditions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnDistance = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSpatial = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnValidate = new System.Windows.Forms.ToolStripButton();
            this.txtExpression = new System.Windows.Forms.RichTextBox();
            this.ctxExpression = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.mToolStrip.SuspendLayout();
            this.ctxExpression.SuspendLayout();
            this.SuspendLayout();
            // 
            // mToolStrip
            // 
            this.mToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFunctions,
            this.btnConditions,
            this.btnDistance,
            this.btnSpatial,
            this.toolStripSeparator1,
            this.btnValidate});
            this.mToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mToolStrip.Name = "mToolStrip";
            this.mToolStrip.Size = new System.Drawing.Size(634, 25);
            this.mToolStrip.TabIndex = 0;
            this.mToolStrip.Text = "toolStrip1";
            // 
            // btnFunctions
            // 
            this.btnFunctions.Image = global::FdoToolbox.Core.Properties.Resources.bricks;
            this.btnFunctions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFunctions.Name = "btnFunctions";
            this.btnFunctions.Size = new System.Drawing.Size(82, 22);
            this.btnFunctions.Text = "Functions";
            // 
            // btnConditions
            // 
            this.btnConditions.Image = global::FdoToolbox.Core.Properties.Resources.add;
            this.btnConditions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConditions.Name = "btnConditions";
            this.btnConditions.Size = new System.Drawing.Size(86, 22);
            this.btnConditions.Text = "Conditions";
            // 
            // btnDistance
            // 
            this.btnDistance.Image = global::FdoToolbox.Core.Properties.Resources.arrow_right;
            this.btnDistance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDistance.Name = "btnDistance";
            this.btnDistance.Size = new System.Drawing.Size(77, 22);
            this.btnDistance.Text = "Distance";
            // 
            // btnSpatial
            // 
            this.btnSpatial.Image = global::FdoToolbox.Core.Properties.Resources.map;
            this.btnSpatial.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpatial.Name = "btnSpatial";
            this.btnSpatial.Size = new System.Drawing.Size(68, 22);
            this.btnSpatial.Text = "Spatial";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnValidate
            // 
            this.btnValidate.Image = global::FdoToolbox.Core.Properties.Resources.accept;
            this.btnValidate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(120, 22);
            this.btnValidate.Text = "Validate Expression";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // txtExpression
            // 
            this.txtExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExpression.ContextMenuStrip = this.ctxExpression;
            this.txtExpression.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(0, 25);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(634, 195);
            this.txtExpression.TabIndex = 1;
            this.txtExpression.Text = "";
            // 
            // ctxExpression
            // 
            this.ctxExpression.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertPropertyToolStripMenuItem});
            this.ctxExpression.Name = "ctxExpression";
            this.ctxExpression.Size = new System.Drawing.Size(149, 26);
            // 
            // insertPropertyToolStripMenuItem
            // 
            this.insertPropertyToolStripMenuItem.Image = global::FdoToolbox.Core.Properties.Resources.table;
            this.insertPropertyToolStripMenuItem.Name = "insertPropertyToolStripMenuItem";
            this.insertPropertyToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.insertPropertyToolStripMenuItem.Text = "Insert Property";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(466, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(547, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExpressionDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(634, 261);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtExpression);
            this.Controls.Add(this.mToolStrip);
            this.Name = "ExpressionDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expression Editor";
            this.mToolStrip.ResumeLayout(false);
            this.mToolStrip.PerformLayout();
            this.ctxExpression.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton btnFunctions;
        private System.Windows.Forms.RichTextBox txtExpression;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnValidate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripDropDownButton btnConditions;
        private System.Windows.Forms.ToolStripDropDownButton btnDistance;
        private System.Windows.Forms.ToolStripDropDownButton btnSpatial;
        private System.Windows.Forms.ContextMenuStrip ctxExpression;
        private System.Windows.Forms.ToolStripMenuItem insertPropertyToolStripMenuItem;
    }
}