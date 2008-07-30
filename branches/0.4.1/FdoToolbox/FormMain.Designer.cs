namespace FdoToolbox
{
    partial class FormMain
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
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.SuspendLayout();
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMenuStrip.Name = "mMenuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size(632, 24);
            this.mMenuStrip.TabIndex = 0;
            this.mMenuStrip.Text = "menuStrip1";
            // 
            // mStatusStrip
            // 
            this.mStatusStrip.Location = new System.Drawing.Point(0, 424);
            this.mStatusStrip.Name = "mStatusStrip";
            this.mStatusStrip.Size = new System.Drawing.Size(632, 22);
            this.mStatusStrip.TabIndex = 1;
            // 
            // mDockPanel
            // 
            this.mDockPanel.ActiveAutoHideContent = null;
            this.mDockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mDockPanel.DockLeftPortion = 200;
            this.mDockPanel.Location = new System.Drawing.Point(0, 24);
            this.mDockPanel.Name = "mDockPanel";
            this.mDockPanel.Size = new System.Drawing.Size(632, 400);
            this.mDockPanel.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.mDockPanel);
            this.Controls.Add(this.mStatusStrip);
            this.Controls.Add(this.mMenuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mMenuStrip;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mMenuStrip;
        private System.Windows.Forms.StatusStrip mStatusStrip;
        private WeifenLuo.WinFormsUI.Docking.DockPanel mDockPanel;
    }
}

