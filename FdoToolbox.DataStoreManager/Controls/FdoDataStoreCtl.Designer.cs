namespace FdoToolbox.DataStoreManager.Controls
{
    partial class FdoDataStoreCtl
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.TAB_SPATIAL_CONTEXT = new System.Windows.Forms.TabPage();
            this.TAB_LOGICAL = new System.Windows.Forms.TabPage();
            this.TAB_PHYSICAL = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.TAB_SPATIAL_CONTEXT);
            this.tabs.Controls.Add(this.TAB_LOGICAL);
            this.tabs.Controls.Add(this.TAB_PHYSICAL);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(594, 424);
            this.tabs.TabIndex = 0;
            // 
            // TAB_SPATIAL_CONTEXT
            // 
            this.TAB_SPATIAL_CONTEXT.Location = new System.Drawing.Point(4, 22);
            this.TAB_SPATIAL_CONTEXT.Name = "TAB_SPATIAL_CONTEXT";
            this.TAB_SPATIAL_CONTEXT.Size = new System.Drawing.Size(586, 398);
            this.TAB_SPATIAL_CONTEXT.TabIndex = 0;
            this.TAB_SPATIAL_CONTEXT.Text = "Spatial Contexts";
            this.TAB_SPATIAL_CONTEXT.UseVisualStyleBackColor = true;
            // 
            // TAB_LOGICAL
            // 
            this.TAB_LOGICAL.Location = new System.Drawing.Point(4, 22);
            this.TAB_LOGICAL.Name = "TAB_LOGICAL";
            this.TAB_LOGICAL.Size = new System.Drawing.Size(586, 398);
            this.TAB_LOGICAL.TabIndex = 1;
            this.TAB_LOGICAL.Text = "Logical Schema";
            this.TAB_LOGICAL.UseVisualStyleBackColor = true;
            // 
            // TAB_PHYSICAL
            // 
            this.TAB_PHYSICAL.Location = new System.Drawing.Point(4, 22);
            this.TAB_PHYSICAL.Name = "TAB_PHYSICAL";
            this.TAB_PHYSICAL.Size = new System.Drawing.Size(586, 398);
            this.TAB_PHYSICAL.TabIndex = 2;
            this.TAB_PHYSICAL.Text = "Physical Mapping";
            this.TAB_PHYSICAL.UseVisualStyleBackColor = true;
            // 
            // FdoDataStoreCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabs);
            this.Name = "FdoDataStoreCtl";
            this.Size = new System.Drawing.Size(594, 424);
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage TAB_SPATIAL_CONTEXT;
        private System.Windows.Forms.TabPage TAB_LOGICAL;
        private System.Windows.Forms.TabPage TAB_PHYSICAL;
    }
}
