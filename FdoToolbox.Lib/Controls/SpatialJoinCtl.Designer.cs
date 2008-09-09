namespace FdoToolbox.Lib.Controls
{
    partial class SpatialJoinCtl
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkPrimaryProperties = new System.Windows.Forms.CheckedListBox();
            this.cmbPrimaryClass = new System.Windows.Forms.ComboBox();
            this.cmbPrimarySchema = new System.Windows.Forms.ComboBox();
            this.cmbPrimaryConnection = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtColumnPrefix = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.chkSecondaryColumns = new System.Windows.Forms.CheckedListBox();
            this.cmbSecondaryTable = new System.Windows.Forms.ComboBox();
            this.cmbSecondaryConnection = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtTargetClassName = new System.Windows.Forms.TextBox();
            this.cmbTargetSchema = new System.Windows.Forms.ComboBox();
            this.cmbTargetConnection = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.grdJoins = new System.Windows.Forms.DataGridView();
            this.COL_PROPERTY = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.COL_COLUMN = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbCardinality = new System.Windows.Forms.ComboBox();
            this.cmbJoinType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnAddJoin = new System.Windows.Forms.Button();
            this.btnDeleteJoin = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtTaskName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdJoins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkPrimaryProperties);
            this.groupBox1.Controls.Add(this.cmbPrimaryClass);
            this.groupBox1.Controls.Add(this.cmbPrimarySchema);
            this.groupBox1.Controls.Add(this.cmbPrimaryConnection);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 166);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Source";
            // 
            // chkPrimaryProperties
            // 
            this.chkPrimaryProperties.FormattingEnabled = true;
            this.chkPrimaryProperties.Location = new System.Drawing.Point(81, 98);
            this.chkPrimaryProperties.Name = "chkPrimaryProperties";
            this.chkPrimaryProperties.Size = new System.Drawing.Size(176, 49);
            this.chkPrimaryProperties.TabIndex = 7;
            this.toolTip1.SetToolTip(this.chkPrimaryProperties, "Check the properties you want to include in the join");
            // 
            // cmbPrimaryClass
            // 
            this.cmbPrimaryClass.DisplayMember = "Name";
            this.cmbPrimaryClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrimaryClass.FormattingEnabled = true;
            this.cmbPrimaryClass.Location = new System.Drawing.Point(81, 71);
            this.cmbPrimaryClass.Name = "cmbPrimaryClass";
            this.cmbPrimaryClass.Size = new System.Drawing.Size(176, 21);
            this.cmbPrimaryClass.TabIndex = 6;
            this.cmbPrimaryClass.ValueMember = "Name";
            this.cmbPrimaryClass.SelectedIndexChanged += new System.EventHandler(this.cmbPrimaryClass_SelectedIndexChanged);
            // 
            // cmbPrimarySchema
            // 
            this.cmbPrimarySchema.DisplayMember = "Name";
            this.cmbPrimarySchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrimarySchema.FormattingEnabled = true;
            this.cmbPrimarySchema.Location = new System.Drawing.Point(81, 44);
            this.cmbPrimarySchema.Name = "cmbPrimarySchema";
            this.cmbPrimarySchema.Size = new System.Drawing.Size(176, 21);
            this.cmbPrimarySchema.TabIndex = 5;
            this.cmbPrimarySchema.ValueMember = "Name";
            this.cmbPrimarySchema.SelectedIndexChanged += new System.EventHandler(this.cmbPrimarySchema_SelectedIndexChanged);
            // 
            // cmbPrimaryConnection
            // 
            this.cmbPrimaryConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrimaryConnection.FormattingEnabled = true;
            this.cmbPrimaryConnection.Location = new System.Drawing.Point(81, 19);
            this.cmbPrimaryConnection.Name = "cmbPrimaryConnection";
            this.cmbPrimaryConnection.Size = new System.Drawing.Size(176, 21);
            this.cmbPrimaryConnection.TabIndex = 4;
            this.cmbPrimaryConnection.SelectedIndexChanged += new System.EventHandler(this.cmbPrimaryConnection_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Properties";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Schema";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtColumnPrefix);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.chkSecondaryColumns);
            this.groupBox2.Controls.Add(this.cmbSecondaryTable);
            this.groupBox2.Controls.Add(this.cmbSecondaryConnection);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(285, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(290, 166);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Source";
            // 
            // txtColumnPrefix
            // 
            this.txtColumnPrefix.Location = new System.Drawing.Point(89, 140);
            this.txtColumnPrefix.Name = "txtColumnPrefix";
            this.txtColumnPrefix.Size = new System.Drawing.Size(179, 20);
            this.txtColumnPrefix.TabIndex = 7;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 143);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Column Prefix";
            // 
            // chkSecondaryColumns
            // 
            this.chkSecondaryColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSecondaryColumns.FormattingEnabled = true;
            this.chkSecondaryColumns.Location = new System.Drawing.Point(89, 71);
            this.chkSecondaryColumns.Name = "chkSecondaryColumns";
            this.chkSecondaryColumns.Size = new System.Drawing.Size(179, 64);
            this.chkSecondaryColumns.TabIndex = 5;
            this.toolTip1.SetToolTip(this.chkSecondaryColumns, "Check the columns you want to include in the join");
            // 
            // cmbSecondaryTable
            // 
            this.cmbSecondaryTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSecondaryTable.DisplayMember = "Name";
            this.cmbSecondaryTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecondaryTable.FormattingEnabled = true;
            this.cmbSecondaryTable.Location = new System.Drawing.Point(89, 44);
            this.cmbSecondaryTable.Name = "cmbSecondaryTable";
            this.cmbSecondaryTable.Size = new System.Drawing.Size(180, 21);
            this.cmbSecondaryTable.TabIndex = 4;
            this.cmbSecondaryTable.ValueMember = "Name";
            this.cmbSecondaryTable.SelectedIndexChanged += new System.EventHandler(this.cmbSecondaryTable_SelectedIndexChanged);
            // 
            // cmbSecondaryConnection
            // 
            this.cmbSecondaryConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSecondaryConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecondaryConnection.FormattingEnabled = true;
            this.cmbSecondaryConnection.Location = new System.Drawing.Point(89, 19);
            this.cmbSecondaryConnection.Name = "cmbSecondaryConnection";
            this.cmbSecondaryConnection.Size = new System.Drawing.Size(180, 21);
            this.cmbSecondaryConnection.TabIndex = 3;
            this.cmbSecondaryConnection.SelectedIndexChanged += new System.EventHandler(this.cmbSecondaryConnection_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Columns";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(50, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Table";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Connection";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtTargetClassName);
            this.groupBox3.Controls.Add(this.cmbTargetSchema);
            this.groupBox3.Controls.Add(this.cmbTargetConnection);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(4, 226);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(571, 76);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target";
            // 
            // txtTargetClassName
            // 
            this.txtTargetClassName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetClassName.Location = new System.Drawing.Point(82, 44);
            this.txtTargetClassName.Name = "txtTargetClassName";
            this.txtTargetClassName.Size = new System.Drawing.Size(468, 20);
            this.txtTargetClassName.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtTargetClassName, "Enter the name of the joined class to be created");
            // 
            // cmbTargetSchema
            // 
            this.cmbTargetSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetSchema.DisplayMember = "Name";
            this.cmbTargetSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetSchema.FormattingEnabled = true;
            this.cmbTargetSchema.Location = new System.Drawing.Point(371, 17);
            this.cmbTargetSchema.Name = "cmbTargetSchema";
            this.cmbTargetSchema.Size = new System.Drawing.Size(179, 21);
            this.cmbTargetSchema.TabIndex = 4;
            this.cmbTargetSchema.ValueMember = "Name";
            // 
            // cmbTargetConnection
            // 
            this.cmbTargetConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetConnection.FormattingEnabled = true;
            this.cmbTargetConnection.Location = new System.Drawing.Point(82, 17);
            this.cmbTargetConnection.Name = "cmbTargetConnection";
            this.cmbTargetConnection.Size = new System.Drawing.Size(175, 21);
            this.cmbTargetConnection.TabIndex = 3;
            this.cmbTargetConnection.SelectedIndexChanged += new System.EventHandler(this.cmbTargetConnection_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(40, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Class";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(304, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Schema";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Connection";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.grdJoins);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.cmbCardinality);
            this.groupBox4.Controls.Add(this.cmbJoinType);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Location = new System.Drawing.Point(4, 308);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(571, 130);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Join Options";
            // 
            // grdJoins
            // 
            this.grdJoins.AllowUserToAddRows = false;
            this.grdJoins.AllowUserToDeleteRows = false;
            this.grdJoins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdJoins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdJoins.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.COL_PROPERTY,
            this.COL_COLUMN});
            this.grdJoins.Location = new System.Drawing.Point(17, 66);
            this.grdJoins.Name = "grdJoins";
            this.grdJoins.Size = new System.Drawing.Size(533, 49);
            this.grdJoins.TabIndex = 5;
            this.toolTip1.SetToolTip(this.grdJoins, "Select the properties and columns to be joined");
            this.grdJoins.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdJoins_CellContentClick);
            this.grdJoins.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdJoins_CellContentClick);
            // 
            // COL_PROPERTY
            // 
            this.COL_PROPERTY.HeaderText = "Property";
            this.COL_PROPERTY.Name = "COL_PROPERTY";
            this.COL_PROPERTY.Width = 200;
            // 
            // COL_COLUMN
            // 
            this.COL_COLUMN.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.COL_COLUMN.HeaderText = "Column";
            this.COL_COLUMN.Name = "COL_COLUMN";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(133, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Joined Properties/Columns";
            // 
            // cmbCardinality
            // 
            this.cmbCardinality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCardinality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardinality.FormattingEnabled = true;
            this.cmbCardinality.Location = new System.Drawing.Point(371, 17);
            this.cmbCardinality.Name = "cmbCardinality";
            this.cmbCardinality.Size = new System.Drawing.Size(179, 21);
            this.cmbCardinality.TabIndex = 3;
            // 
            // cmbJoinType
            // 
            this.cmbJoinType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJoinType.FormattingEnabled = true;
            this.cmbJoinType.Location = new System.Drawing.Point(82, 16);
            this.cmbJoinType.Name = "cmbJoinType";
            this.cmbJoinType.Size = new System.Drawing.Size(175, 21);
            this.cmbJoinType.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(295, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Cardinality";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Join Type";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(419, 445);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(500, 445);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // btnAddJoin
            // 
            this.btnAddJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddJoin.Location = new System.Drawing.Point(4, 445);
            this.btnAddJoin.Name = "btnAddJoin";
            this.btnAddJoin.Size = new System.Drawing.Size(75, 23);
            this.btnAddJoin.TabIndex = 6;
            this.btnAddJoin.Text = "Add Join";
            this.btnAddJoin.UseVisualStyleBackColor = true;
            this.btnAddJoin.Click += new System.EventHandler(this.btnAddJoin_Click);
            // 
            // btnDeleteJoin
            // 
            this.btnDeleteJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteJoin.Location = new System.Drawing.Point(86, 445);
            this.btnDeleteJoin.Name = "btnDeleteJoin";
            this.btnDeleteJoin.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteJoin.TabIndex = 7;
            this.btnDeleteJoin.Text = "Delete Join";
            this.btnDeleteJoin.UseVisualStyleBackColor = true;
            this.btnDeleteJoin.Click += new System.EventHandler(this.btnDeleteJoin_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.txtTaskName);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Location = new System.Drawing.Point(4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(571, 44);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "General";
            // 
            // txtTaskName
            // 
            this.txtTaskName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaskName.Location = new System.Drawing.Point(82, 13);
            this.txtTaskName.Name = "txtTaskName";
            this.txtTaskName.Size = new System.Drawing.Size(468, 20);
            this.txtTaskName.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Task Name";
            // 
            // SpatialJoinCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnDeleteJoin);
            this.Controls.Add(this.btnAddJoin);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SpatialJoinCtl";
            this.Size = new System.Drawing.Size(578, 479);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdJoins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckedListBox chkPrimaryProperties;
        private System.Windows.Forms.ComboBox cmbPrimaryClass;
        private System.Windows.Forms.ComboBox cmbPrimarySchema;
        private System.Windows.Forms.ComboBox cmbPrimaryConnection;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox chkSecondaryColumns;
        private System.Windows.Forms.ComboBox cmbSecondaryTable;
        private System.Windows.Forms.ComboBox cmbSecondaryConnection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTargetClassName;
        private System.Windows.Forms.ComboBox cmbTargetSchema;
        private System.Windows.Forms.ComboBox cmbTargetConnection;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView grdJoins;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbCardinality;
        private System.Windows.Forms.ComboBox cmbJoinType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button btnDeleteJoin;
        private System.Windows.Forms.Button btnAddJoin;
        private System.Windows.Forms.DataGridViewComboBoxColumn COL_PROPERTY;
        private System.Windows.Forms.DataGridViewComboBoxColumn COL_COLUMN;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtTaskName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtColumnPrefix;
        private System.Windows.Forms.Label label15;
    }
}
