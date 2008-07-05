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
            this.newDataConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDFToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.sHPToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTasks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulkCopyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.joinOperationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxModules = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sHPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulkCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDFToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sHPToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.ctxSelectedModule = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moduleInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSelectedTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSelectedConnection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageSchemasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSchemasToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSchemasFromXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxConnections.SuspendLayout();
            this.ctxTasks.SuspendLayout();
            this.ctxModules.SuspendLayout();
            this.mToolStrip.SuspendLayout();
            this.ctxSelectedModule.SuspendLayout();
            this.ctxSelectedTask.SuspendLayout();
            this.ctxSelectedConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxConnections
            // 
            this.ctxConnections.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDataConnectionToolStripMenuItem});
            this.ctxConnections.Name = "ctxConnections";
            this.ctxConnections.Size = new System.Drawing.Size(190, 26);
            // 
            // newDataConnectionToolStripMenuItem
            // 
            this.newDataConnectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sDFToolStripMenuItem2,
            this.sHPToolStripMenuItem2,
            this.customToolStripMenuItem2});
            this.newDataConnectionToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.database_connect;
            this.newDataConnectionToolStripMenuItem.Name = "newDataConnectionToolStripMenuItem";
            this.newDataConnectionToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.newDataConnectionToolStripMenuItem.Text = "New Data Connection";
            // 
            // sDFToolStripMenuItem2
            // 
            this.sDFToolStripMenuItem2.Name = "sDFToolStripMenuItem2";
            this.sDFToolStripMenuItem2.Size = new System.Drawing.Size(121, 22);
            this.sDFToolStripMenuItem2.Text = "SDF";
            this.sDFToolStripMenuItem2.Click += new System.EventHandler(this.ConnectSDF_Click);
            // 
            // sHPToolStripMenuItem2
            // 
            this.sHPToolStripMenuItem2.Name = "sHPToolStripMenuItem2";
            this.sHPToolStripMenuItem2.Size = new System.Drawing.Size(121, 22);
            this.sHPToolStripMenuItem2.Text = "SHP";
            this.sHPToolStripMenuItem2.Click += new System.EventHandler(this.ConnectSHP_Click);
            // 
            // customToolStripMenuItem2
            // 
            this.customToolStripMenuItem2.Name = "customToolStripMenuItem2";
            this.customToolStripMenuItem2.Size = new System.Drawing.Size(121, 22);
            this.customToolStripMenuItem2.Text = "Custom";
            this.customToolStripMenuItem2.Click += new System.EventHandler(this.ConnectGeneric_Click);
            // 
            // ctxTasks
            // 
            this.ctxTasks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTaskToolStripMenuItem,
            this.loadTaskToolStripMenuItem});
            this.ctxTasks.Name = "ctxTasks";
            this.ctxTasks.Size = new System.Drawing.Size(134, 48);
            // 
            // newTaskToolStripMenuItem
            // 
            this.newTaskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulkCopyToolStripMenuItem1,
            this.joinOperationToolStripMenuItem1});
            this.newTaskToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.application_go;
            this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
            this.newTaskToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.newTaskToolStripMenuItem.Text = "New Task";
            // 
            // bulkCopyToolStripMenuItem1
            // 
            this.bulkCopyToolStripMenuItem1.Image = global::FdoToolbox.Properties.Resources.table_go;
            this.bulkCopyToolStripMenuItem1.Name = "bulkCopyToolStripMenuItem1";
            this.bulkCopyToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.bulkCopyToolStripMenuItem1.Text = "Bulk Copy";
            this.bulkCopyToolStripMenuItem1.Click += new System.EventHandler(this.CreateBulkCopy_Click);
            // 
            // joinOperationToolStripMenuItem1
            // 
            this.joinOperationToolStripMenuItem1.Image = global::FdoToolbox.Properties.Resources.table_relationship;
            this.joinOperationToolStripMenuItem1.Name = "joinOperationToolStripMenuItem1";
            this.joinOperationToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.joinOperationToolStripMenuItem1.Text = "Join Operation";
            this.joinOperationToolStripMenuItem1.Click += new System.EventHandler(this.CreateJoin_Click);
            // 
            // loadTaskToolStripMenuItem
            // 
            this.loadTaskToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.folder;
            this.loadTaskToolStripMenuItem.Name = "loadTaskToolStripMenuItem";
            this.loadTaskToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.loadTaskToolStripMenuItem.Text = "Load Task";
            this.loadTaskToolStripMenuItem.Click += new System.EventHandler(this.LoadTask_Click);
            // 
            // ctxModules
            // 
            this.ctxModules.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadModuleToolStripMenuItem});
            this.ctxModules.Name = "ctxModules";
            this.ctxModules.Size = new System.Drawing.Size(146, 26);
            // 
            // loadModuleToolStripMenuItem
            // 
            this.loadModuleToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.plugin;
            this.loadModuleToolStripMenuItem.Name = "loadModuleToolStripMenuItem";
            this.loadModuleToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.loadModuleToolStripMenuItem.Text = "Load Module";
            this.loadModuleToolStripMenuItem.Click += new System.EventHandler(this.LoadModule_Click);
            // 
            // mToolStrip
            // 
            this.mToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton3});
            this.mToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mToolStrip.Name = "mToolStrip";
            this.mToolStrip.Size = new System.Drawing.Size(292, 25);
            this.mToolStrip.TabIndex = 0;
            this.mToolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem,
            this.taskToolStripMenuItem,
            this.dataSourceToolStripMenuItem});
            this.toolStripButton1.Image = global::FdoToolbox.Properties.Resources.page_white;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(57, 22);
            this.toolStripButton1.Text = "New";
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sDFToolStripMenuItem,
            this.sHPToolStripMenuItem,
            this.customToolStripMenuItem});
            this.connectionToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.database_connect;
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.connectionToolStripMenuItem.Text = "Connection";
            // 
            // sDFToolStripMenuItem
            // 
            this.sDFToolStripMenuItem.Name = "sDFToolStripMenuItem";
            this.sDFToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.sDFToolStripMenuItem.Text = "SDF";
            this.sDFToolStripMenuItem.Click += new System.EventHandler(this.ConnectSDF_Click);
            // 
            // sHPToolStripMenuItem
            // 
            this.sHPToolStripMenuItem.Name = "sHPToolStripMenuItem";
            this.sHPToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.sHPToolStripMenuItem.Text = "SHP";
            this.sHPToolStripMenuItem.Click += new System.EventHandler(this.ConnectSHP_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.ConnectGeneric_Click);
            // 
            // taskToolStripMenuItem
            // 
            this.taskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulkCopyToolStripMenuItem,
            this.joinOperationToolStripMenuItem});
            this.taskToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.application_go;
            this.taskToolStripMenuItem.Name = "taskToolStripMenuItem";
            this.taskToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.taskToolStripMenuItem.Text = "Task";
            // 
            // bulkCopyToolStripMenuItem
            // 
            this.bulkCopyToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.table_go;
            this.bulkCopyToolStripMenuItem.Name = "bulkCopyToolStripMenuItem";
            this.bulkCopyToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.bulkCopyToolStripMenuItem.Text = "Bulk Copy";
            this.bulkCopyToolStripMenuItem.Click += new System.EventHandler(this.CreateBulkCopy_Click);
            // 
            // joinOperationToolStripMenuItem
            // 
            this.joinOperationToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.table_relationship;
            this.joinOperationToolStripMenuItem.Name = "joinOperationToolStripMenuItem";
            this.joinOperationToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.joinOperationToolStripMenuItem.Text = "Join Operation";
            this.joinOperationToolStripMenuItem.Click += new System.EventHandler(this.CreateJoin_Click);
            // 
            // dataSourceToolStripMenuItem
            // 
            this.dataSourceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sDFToolStripMenuItem1,
            this.sHPToolStripMenuItem1,
            this.customToolStripMenuItem1});
            this.dataSourceToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.database;
            this.dataSourceToolStripMenuItem.Name = "dataSourceToolStripMenuItem";
            this.dataSourceToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.dataSourceToolStripMenuItem.Text = "Data Source";
            // 
            // sDFToolStripMenuItem1
            // 
            this.sDFToolStripMenuItem1.Name = "sDFToolStripMenuItem1";
            this.sDFToolStripMenuItem1.Size = new System.Drawing.Size(121, 22);
            this.sDFToolStripMenuItem1.Text = "SDF";
            this.sDFToolStripMenuItem1.Click += new System.EventHandler(this.CreateSDF_Click);
            // 
            // sHPToolStripMenuItem1
            // 
            this.sHPToolStripMenuItem1.Name = "sHPToolStripMenuItem1";
            this.sHPToolStripMenuItem1.Size = new System.Drawing.Size(121, 22);
            this.sHPToolStripMenuItem1.Text = "SHP";
            this.sHPToolStripMenuItem1.Click += new System.EventHandler(this.CreateSHP_Click);
            // 
            // customToolStripMenuItem1
            // 
            this.customToolStripMenuItem1.Name = "customToolStripMenuItem1";
            this.customToolStripMenuItem1.Size = new System.Drawing.Size(121, 22);
            this.customToolStripMenuItem1.Text = "Custom";
            this.customToolStripMenuItem1.Click += new System.EventHandler(this.CreateCustom_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = global::FdoToolbox.Properties.Resources.plugin;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(87, 22);
            this.toolStripButton3.Text = "Load Module";
            this.toolStripButton3.Click += new System.EventHandler(this.LoadModule_Click);
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
            this.ctxSelectedModule.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moduleInformationToolStripMenuItem});
            this.ctxSelectedModule.Name = "ctxSelectedModule";
            this.ctxSelectedModule.Size = new System.Drawing.Size(179, 26);
            // 
            // moduleInformationToolStripMenuItem
            // 
            this.moduleInformationToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.information;
            this.moduleInformationToolStripMenuItem.Name = "moduleInformationToolStripMenuItem";
            this.moduleInformationToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.moduleInformationToolStripMenuItem.Text = "Module Information";
            this.moduleInformationToolStripMenuItem.Click += new System.EventHandler(this.ModuleInfo_Click);
            // 
            // ctxSelectedTask
            // 
            this.ctxSelectedTask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.ctxSelectedTask.Name = "ctxSelectedTask";
            this.ctxSelectedTask.Size = new System.Drawing.Size(153, 114);
            // 
            // executeToolStripMenuItem
            // 
            this.executeToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.application_go;
            this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            this.executeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.executeToolStripMenuItem.Text = "Execute";
            this.executeToolStripMenuItem.Click += new System.EventHandler(this.ExecuteTask_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.EditTask_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.cross;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteTask_Click);
            // 
            // ctxSelectedConnection
            // 
            this.ctxSelectedConnection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataPreviewToolStripMenuItem,
            this.manageSchemasToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveSchemasToXMLToolStripMenuItem,
            this.loadSchemasFromXMLToolStripMenuItem,
            this.toolStripSeparator4,
            this.removeToolStripMenuItem});
            this.ctxSelectedConnection.Name = "ctxSelectedConnection";
            this.ctxSelectedConnection.Size = new System.Drawing.Size(209, 126);
            // 
            // dataPreviewToolStripMenuItem
            // 
            this.dataPreviewToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.zoom;
            this.dataPreviewToolStripMenuItem.Name = "dataPreviewToolStripMenuItem";
            this.dataPreviewToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.dataPreviewToolStripMenuItem.Text = "Data Preview";
            this.dataPreviewToolStripMenuItem.Click += new System.EventHandler(this.DataPreview_Click);
            // 
            // manageSchemasToolStripMenuItem
            // 
            this.manageSchemasToolStripMenuItem.Name = "manageSchemasToolStripMenuItem";
            this.manageSchemasToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.manageSchemasToolStripMenuItem.Text = "Manage Schemas";
            this.manageSchemasToolStripMenuItem.Click += new System.EventHandler(this.ManageSchemas_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // saveSchemasToXMLToolStripMenuItem
            // 
            this.saveSchemasToXMLToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.disk;
            this.saveSchemasToXMLToolStripMenuItem.Name = "saveSchemasToXMLToolStripMenuItem";
            this.saveSchemasToXMLToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.saveSchemasToXMLToolStripMenuItem.Text = "Save Schema(s) to XML";
            this.saveSchemasToXMLToolStripMenuItem.Click += new System.EventHandler(this.SaveSchemas_Click);
            // 
            // loadSchemasFromXMLToolStripMenuItem
            // 
            this.loadSchemasFromXMLToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.folder;
            this.loadSchemasFromXMLToolStripMenuItem.Name = "loadSchemasFromXMLToolStripMenuItem";
            this.loadSchemasFromXMLToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.loadSchemasFromXMLToolStripMenuItem.Text = "Load Schema(s) from XML";
            this.loadSchemasFromXMLToolStripMenuItem.Click += new System.EventHandler(this.LoadSchemas_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(205, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.cross;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveSelectedConnection_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::FdoToolbox.Properties.Resources.disk;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveTask_Click);
            // 
            // ObjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.mTreeView);
            this.Controls.Add(this.mToolStrip);
            this.Name = "ObjectExplorer";
            this.TabText = "Object Explorer";
            this.Text = "Object Explorer";
            this.ctxConnections.ResumeLayout(false);
            this.ctxTasks.ResumeLayout(false);
            this.ctxModules.ResumeLayout(false);
            this.mToolStrip.ResumeLayout(false);
            this.mToolStrip.PerformLayout();
            this.ctxSelectedModule.ResumeLayout(false);
            this.ctxSelectedTask.ResumeLayout(false);
            this.ctxSelectedConnection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mToolStrip;
        private System.Windows.Forms.TreeView mTreeView;
        private System.Windows.Forms.ImageList mImageList;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedModule;
        private System.Windows.Forms.ToolStripMenuItem moduleInformationToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedTask;
        private System.Windows.Forms.ContextMenuStrip ctxSelectedConnection;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sHPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bulkCopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sDFToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sHPToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSchemasFromXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip ctxConnections;
        private System.Windows.Forms.ToolStripMenuItem newDataConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sDFToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sHPToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip ctxTasks;
        private System.Windows.Forms.ToolStripMenuItem newTaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bulkCopyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem joinOperationToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip ctxModules;
        private System.Windows.Forms.ToolStripMenuItem loadModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageSchemasToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveSchemasToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}