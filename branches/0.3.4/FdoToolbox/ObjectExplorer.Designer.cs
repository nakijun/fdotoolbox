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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Connections", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Tasks", 1, 1);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Modules", 2, 2);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectExplorer));
            this.ctxConnections = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxTasks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxModules = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.ctxSelectedModule = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSelectedConnection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // ctxConnections
            // 
            this.ctxConnections.Name = "ctxConnections";
            this.ctxConnections.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxTasks
            // 
            this.ctxTasks.Name = "ctxTasks";
            this.ctxTasks.Size = new System.Drawing.Size(61, 4);
            // 
            // ctxModules
            // 
            this.ctxModules.Name = "ctxModules";
            this.ctxModules.Size = new System.Drawing.Size(61, 4);
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
            treeNode1.ContextMenuStrip = this.ctxConnections;
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "NODE_CONNECTIONS";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Connections";
            treeNode2.ContextMenuStrip = this.ctxTasks;
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "NODE_TASKS";
            treeNode2.SelectedImageIndex = 1;
            treeNode2.Text = "Tasks";
            treeNode3.ContextMenuStrip = this.ctxModules;
            treeNode3.ImageIndex = 2;
            treeNode3.Name = "NODE_MODULES";
            treeNode3.SelectedImageIndex = 2;
            treeNode3.Text = "Modules";
            this.mTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.mTreeView.SelectedImageIndex = 0;
            this.mTreeView.ShowNodeToolTips = true;
            this.mTreeView.Size = new System.Drawing.Size(292, 241);
            this.mTreeView.TabIndex = 1;
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
            // ctxSelectedConnection
            // 
            this.ctxSelectedConnection.Name = "ctxSelectedConnection";
            this.ctxSelectedConnection.Size = new System.Drawing.Size(61, 4);
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
        private System.Windows.Forms.ContextMenuStrip ctxSelectedConnection;
        private System.Windows.Forms.ContextMenuStrip ctxConnections;
        private System.Windows.Forms.ContextMenuStrip ctxTasks;
        private System.Windows.Forms.ContextMenuStrip ctxModules;
    }
}