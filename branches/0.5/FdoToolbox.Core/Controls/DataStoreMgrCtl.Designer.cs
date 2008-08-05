namespace FdoToolbox.Core.Controls
{
    partial class DataStoreMgrCtl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDestroy = new System.Windows.Forms.ToolStripButton();
            this.grdDataStores = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataStores)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDestroy});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(542, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::FdoToolbox.Core.Properties.Resources.add;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(97, 22);
            this.btnAdd.Text = "Add Datastore";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDestroy
            // 
            this.btnDestroy.Enabled = false;
            this.btnDestroy.Image = global::FdoToolbox.Core.Properties.Resources.cross;
            this.btnDestroy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDestroy.Name = "btnDestroy";
            this.btnDestroy.Size = new System.Drawing.Size(65, 22);
            this.btnDestroy.Text = "Destroy";
            this.btnDestroy.Click += new System.EventHandler(this.btnDestroy_Click);
            // 
            // grdDataStores
            // 
            this.grdDataStores.AllowUserToAddRows = false;
            this.grdDataStores.AllowUserToDeleteRows = false;
            this.grdDataStores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDataStores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDataStores.Location = new System.Drawing.Point(0, 25);
            this.grdDataStores.Name = "grdDataStores";
            this.grdDataStores.ReadOnly = true;
            this.grdDataStores.Size = new System.Drawing.Size(542, 380);
            this.grdDataStores.TabIndex = 1;
            this.grdDataStores.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDataStores_CellContentClick);
            // 
            // DataStoreMgrCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grdDataStores);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DataStoreMgrCtl";
            this.Size = new System.Drawing.Size(542, 405);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataStores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.DataGridView grdDataStores;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDestroy;
    }
}
