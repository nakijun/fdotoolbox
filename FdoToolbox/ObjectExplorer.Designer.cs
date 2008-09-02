namespace FdoToolbox
{
    partial class ObjectExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectExplorer));
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.ctxSelectedModule = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedSpatialConnection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedSchema = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedClass = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedDatabaseConnection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedTable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedDatabase = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // mToolStrip
            // 
            this.mToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mToolStrip.Name = "mToolStrip";
            this.mToolStrip.Size = new System.Drawing.Size(292, 25);
            this.mToolStrip.TabIndex = 0;
            this.mToolStrip.Text = "toolStrip1";
            // 
            // mTreeView
            // 
            this.mTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTreeView.ImageIndex = 0;
            this.mTreeView.ImageList = this.mImageList;
            this.mTreeView.Location = new System.Drawing.Point(0, 25);
            this.mTreeView.Name = "mTreeView";
            this.mTreeView.SelectedImageIndex = 0;
            this.mTreeView.ShowNodeToolTips = true;
            this.mTreeView.Size = new System.Drawing.Size(292, 241);
            this.mTreeView.TabIndex = 1;
            this.mTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            this.mTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
            // 
            // mImageList
            // 
            this.mImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList.ImageStream")));
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList.Images.SetKeyName(0, "database_connect.png");
            this.mImageList.Images.SetKeyName(1, "application_go.png");
            this.mImageList.Images.SetKeyName(2, "cog_go.png");
            this.mImageList.Images.SetKeyName(3, "chart_organisation.png");
            this.mImageList.Images.SetKeyName(4, "database_table.png");
            this.mImageList.Images.SetKeyName(5, "table.png");
            this.mImageList.Images.SetKeyName(6, "key.png");
            this.mImageList.Images.SetKeyName(7, "image.png");
            this.mImageList.Images.SetKeyName(8, "shape_handles.png");
            this.mImageList.Images.SetKeyName(9, "table_relationship.png");
            this.mImageList.Images.SetKeyName(10, "package.png");
            this.mImageList.Images.SetKeyName(11, "database.png");
            // 
            // ctxSelectedModule
            // 
            this.ctxSelectedModule.Name = "ctxSelectedModule";
            this.ctxSelectedModule.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedTask
            // 
            this.ctxSelectedTask.Name = "ctxSelectedTask";
            this.ctxSelectedTask.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedSpatialConnection
            // 
            this.ctxSelectedSpatialConnection.Name = "ctxSelectedConnection";
            this.ctxSelectedSpatialConnection.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedSchema
            // 
            this.ctxSelectedSchema.Name = "ctxSelectedSchema";
            this.ctxSelectedSchema.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedClass
            // 
            this.ctxSelectedClass.Name = "ctxSelectedClass";
            this.ctxSelectedClass.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedDatabaseConnection
            // 
            this.ctxSelectedDatabaseConnection.Name = "ctxSelectedDatabaseConnection";
            this.ctxSelectedDatabaseConnection.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedTable
            // 
            this.ctxSelectedTable.Name = "ctxSelectedTable";
            this.ctxSelectedTable.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxSelectedDatabase
            // 
            this.ctxSelectedDatabase.Name = "ctxSelectedDatabase";
            this.ctxSelectedDatabase.Size = new System.Drawing.Size(61, 4);
            // 
            // ObjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.mTreeView);
            this.Controls.Add(this.mToolStrip);
            this.HideOnClose = true;
            this.Name = "ObjectExplorer";
            this.TabText = "Object Explorer";
            this.Text = "Object Explorer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mToolStrip;
        private System.Windows.Forms.TreeView mTreeView;
        private System.Windows.Forms.ImageList mImageList;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedModule;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedTask;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedSpatialConnection;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedSchema;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedClass;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedDatabaseConnection;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedTable;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedDatabase;
    }
}