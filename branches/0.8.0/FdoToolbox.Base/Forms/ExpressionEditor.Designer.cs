namespace FdoToolbox.Base.Forms
{
    partial class ExpressionEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnFunctions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnConditions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnDistance = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSpatial = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnValidate = new System.Windows.Forms.ToolStripButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ctxInsert = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertGeometryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.curveStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.curvePolygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.exprPanel = new System.Windows.Forms.Panel();
            this.txtExpression = new System.Windows.Forms.RichTextBox();
            this._autoCompleteTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1.SuspendLayout();
            this.ctxInsert.SuspendLayout();
            this.panel1.SuspendLayout();
            this.exprPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFunctions,
            this.btnConditions,
            this.btnDistance,
            this.btnSpatial,
            this.toolStripSeparator1,
            this.btnValidate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(637, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnFunctions
            // 
            this.btnFunctions.Image = global::FdoToolbox.Base.Images.bricks;
            this.btnFunctions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFunctions.Name = "btnFunctions";
            this.btnFunctions.Size = new System.Drawing.Size(82, 22);
            this.btnFunctions.Text = "Functions";
            // 
            // btnConditions
            // 
            this.btnConditions.Image = global::FdoToolbox.Base.Images.add;
            this.btnConditions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConditions.Name = "btnConditions";
            this.btnConditions.Size = new System.Drawing.Size(86, 22);
            this.btnConditions.Text = "Conditions";
            // 
            // btnDistance
            // 
            this.btnDistance.Image = global::FdoToolbox.Base.Images.arrow_right;
            this.btnDistance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDistance.Name = "btnDistance";
            this.btnDistance.Size = new System.Drawing.Size(77, 22);
            this.btnDistance.Text = "Distance";
            // 
            // btnSpatial
            // 
            this.btnSpatial.Image = global::FdoToolbox.Base.Images.map;
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
            this.btnValidate.Image = global::FdoToolbox.Base.Images.accept;
            this.btnValidate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(65, 22);
            this.btnValidate.Text = "Validate";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(468, 12);
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
            this.btnCancel.Location = new System.Drawing.Point(549, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ctxInsert
            // 
            this.ctxInsert.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertPropertyToolStripMenuItem,
            this.insertGeometryToolStripMenuItem});
            this.ctxInsert.Name = "ctxInsert";
            this.ctxInsert.Size = new System.Drawing.Size(154, 48);
            // 
            // insertPropertyToolStripMenuItem
            // 
            this.insertPropertyToolStripMenuItem.Image = global::FdoToolbox.Base.Images.table;
            this.insertPropertyToolStripMenuItem.Name = "insertPropertyToolStripMenuItem";
            this.insertPropertyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.insertPropertyToolStripMenuItem.Text = "Insert Property";
            // 
            // insertGeometryToolStripMenuItem
            // 
            this.insertGeometryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pointToolStripMenuItem,
            this.lineStringToolStripMenuItem,
            this.polygonToolStripMenuItem,
            this.curveStringToolStripMenuItem,
            this.curvePolygonToolStripMenuItem});
            this.insertGeometryToolStripMenuItem.Image = global::FdoToolbox.Base.Images.shape_handles;
            this.insertGeometryToolStripMenuItem.Name = "insertGeometryToolStripMenuItem";
            this.insertGeometryToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.insertGeometryToolStripMenuItem.Text = "Insert Geometry";
            // 
            // pointToolStripMenuItem
            // 
            this.pointToolStripMenuItem.Name = "pointToolStripMenuItem";
            this.pointToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.pointToolStripMenuItem.Text = "Point";
            this.pointToolStripMenuItem.Click += new System.EventHandler(this.pointToolStripMenuItem_Click);
            // 
            // lineStringToolStripMenuItem
            // 
            this.lineStringToolStripMenuItem.Name = "lineStringToolStripMenuItem";
            this.lineStringToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.lineStringToolStripMenuItem.Text = "LineString";
            this.lineStringToolStripMenuItem.Click += new System.EventHandler(this.lineStringToolStripMenuItem_Click);
            // 
            // polygonToolStripMenuItem
            // 
            this.polygonToolStripMenuItem.Name = "polygonToolStripMenuItem";
            this.polygonToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.polygonToolStripMenuItem.Text = "Polygon";
            this.polygonToolStripMenuItem.Click += new System.EventHandler(this.polygonToolStripMenuItem_Click);
            // 
            // curveStringToolStripMenuItem
            // 
            this.curveStringToolStripMenuItem.Name = "curveStringToolStripMenuItem";
            this.curveStringToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.curveStringToolStripMenuItem.Text = "CurveString";
            this.curveStringToolStripMenuItem.Click += new System.EventHandler(this.curveStringToolStripMenuItem_Click);
            // 
            // curvePolygonToolStripMenuItem
            // 
            this.curvePolygonToolStripMenuItem.Name = "curvePolygonToolStripMenuItem";
            this.curvePolygonToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.curvePolygonToolStripMenuItem.Text = "CurvePolygon";
            this.curvePolygonToolStripMenuItem.Click += new System.EventHandler(this.curvePolygonToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(637, 49);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press Alt + Right to invoke auto-complete at any time";
            // 
            // exprPanel
            // 
            this.exprPanel.Controls.Add(this.txtExpression);
            this.exprPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exprPanel.Location = new System.Drawing.Point(0, 25);
            this.exprPanel.Name = "exprPanel";
            this.exprPanel.Size = new System.Drawing.Size(637, 246);
            this.exprPanel.TabIndex = 4;
            // 
            // txtExpression
            // 
            this.txtExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpression.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(0, 0);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(637, 246);
            this.txtExpression.TabIndex = 5;
            this.txtExpression.Text = "";
            this.txtExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyDown);
            this.txtExpression.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyUp);
            // 
            // ExpressionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 320);
            this.Controls.Add(this.exprPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ExpressionEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ctxInsert.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.exprPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnFunctions;
        private System.Windows.Forms.ToolStripDropDownButton btnConditions;
        private System.Windows.Forms.ToolStripDropDownButton btnSpatial;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnValidate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxInsert;
        private System.Windows.Forms.ToolStripDropDownButton btnDistance;
        private System.Windows.Forms.ToolStripMenuItem insertPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertGeometryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polygonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem curveStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem curvePolygonToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel exprPanel;
        private System.Windows.Forms.RichTextBox txtExpression;
        private System.Windows.Forms.ToolTip _autoCompleteTooltip;
    }
}