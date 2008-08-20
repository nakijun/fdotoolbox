#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Geometry;
using OSGeo.FDO.Filter;
using OSGeo.FDO.Connections.Capabilities;
using FdoToolbox.Core.Forms;
using System.Collections.Specialized;
using OSGeo.FDO.Expression;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Common;
using System.Threading;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Controls
{
    public partial class SpatialDataPreviewCtl : SpatialConnectionBoundControl
    {
        const int TAB_STANDARD = 0;
        const int TAB_AGGREGATE = 1;
        const int TAB_SQL = 2;

        private FgfGeometryFactory _GeomFactory;

        internal SpatialDataPreviewCtl()
        {
            InitializeComponent();
            _GeomFactory = new FgfGeometryFactory();
            this.Disposed += delegate { _GeomFactory.Dispose(); };
        }

        public SpatialDataPreviewCtl(SpatialConnectionInfo conn, string key)
            : base(conn, key)
        {
            InitializeComponent();
            _GeomFactory = new FgfGeometryFactory();
            this.Disposed += delegate { _GeomFactory.Dispose(); };
            _BoundConnection = conn;
            ToggleUI();
        }

        private void ToggleUI()
        {
            if (!this.BoundConnection.Connection.ConnectionCapabilities.SupportsSQL())
                tabQueryMode.TabPages.RemoveAt(TAB_SQL);
            if (!Array.Exists<int>(this.BoundConnection.Connection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates; }))
                tabQueryMode.TabPages.RemoveAt(TAB_AGGREGATE);
        }

        protected override void OnLoad(EventArgs e)
        {
            FeatureService service = AppGateway.RunningApplication.SpatialConnectionManager.CreateService(this.BoundConnection.Name);
            cmbSchema.DataSource = service.DescribeSchema();
            cmbAggSchema.DataSource = service.DescribeSchema();
            SetSelectedClass();
            base.OnLoad(e);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            switch (tabQueryMode.SelectedIndex)
            {
                case TAB_STANDARD:
                    QueryStandard();
                    break;
                case TAB_AGGREGATE:
                    QueryAggregate();
                    break;
                case TAB_SQL:
                    QuerySQL();
                    break;
            }
        }

        private void QuerySQL()
        {
            string sql = txtSQL.Text;
            if (string.IsNullOrEmpty(sql))
            {
                AppConsole.Alert("Error", "Please enter the SQL query text");
                return;
            }
            if (!sql.TrimStart().StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
            {
                AppConsole.Alert("Error", "Only SQL SELECT statements are allowed for data previewing");
                return;
            }
            SQLFeatureQuery qry = new SQLFeatureQuery();
            qry.SQLText = sql;
            ClearGrid();
            btnQuery.Enabled = false;
            btnClear.Enabled = false;
            bgSql.RunWorkerAsync(qry);
        }

        private void QueryAggregate()
        {
            ClassDefinition classDef = cmbAggClass.SelectedItem as ClassDefinition;
            if (CheckValidAggregates() && classDef != null)
            {
                using (ISelectAggregates select = this.BoundConnection.Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates) as ISelectAggregates)
                {
                    try
                    {
                        select.Distinct = chkDistinct.Checked;
                        select.SetFeatureClassName(classDef.Name);
                        if (!string.IsNullOrEmpty(txtAggFilter.Text))
                            select.SetFilter(txtAggFilter.Text);

                        NameValueCollection aggParams = GetAggregateParameters();
                        foreach (string identifier in aggParams.AllKeys)
                        {
                            select.PropertyNames.Add(new ComputedIdentifier(identifier, Expression.Parse(aggParams[identifier])));
                        }

                        using (OSGeo.FDO.Commands.Feature.IDataReader reader = select.Execute())
                        {
                            DataTable table = new DataTable();
                            PrepareGrid(table, reader);
                            try
                            {
                                while (reader.ReadNext())
                                {
                                    ProcessDataReader(table, aggParams, reader);
                                }
                            }
                            catch (OSGeo.FDO.Common.Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                reader.Close();
                                grdPreview.DataSource = table;
                            }
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                        AppConsole.WriteException(ex);
                    }
                }
            }
        }

        private void ProcessSQLReader(DataTable table, ISQLDataReader reader)
        {
            DataRow row = table.NewRow();
            foreach (DataColumn col in table.Columns)
            {
                string identifier = col.ColumnName;
                if (!reader.IsNull(identifier))
                {
                    PropertyType ptype = reader.GetPropertyType(identifier);
                    switch (ptype)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            {
                                DataType dtype = reader.GetColumnType(identifier);
                                switch (dtype)
                                {
                                    case DataType.DataType_BLOB:
                                        row[identifier] = reader.GetLOB(identifier).Data;
                                        break;
                                    case DataType.DataType_Boolean:
                                        row[identifier] = reader.GetBoolean(identifier);
                                        break;
                                    case DataType.DataType_Byte:
                                        row[identifier] = reader.GetByte(identifier);
                                        break;
                                    case DataType.DataType_CLOB:
                                        row[identifier] = reader.GetLOB(identifier).Data;
                                        break;
                                    case DataType.DataType_DateTime:
                                        row[identifier] = reader.GetDateTime(identifier);
                                        break;
                                    case DataType.DataType_Decimal:
                                        row[identifier] = reader.GetDouble(identifier);
                                        break;
                                    case DataType.DataType_Double:
                                        row[identifier] = reader.GetDouble(identifier);
                                        break;
                                    case DataType.DataType_Int16:
                                        row[identifier] = reader.GetInt16(identifier);
                                        break;
                                    case DataType.DataType_Int32:
                                        row[identifier] = reader.GetInt32(identifier);
                                        break;
                                    case DataType.DataType_Int64:
                                        row[identifier] = reader.GetInt64(identifier);
                                        break;
                                    case DataType.DataType_Single:
                                        row[identifier] = reader.GetSingle(identifier);
                                        break;
                                    case DataType.DataType_String:
                                        row[identifier] = reader.GetString(identifier);
                                        break;
                                }
                            }
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            {
                                byte[] bGeom = reader.GetGeometry(identifier);
                                using (IGeometry geom = _GeomFactory.CreateGeometryFromFgf(bGeom))
                                {
                                    row[identifier] = geom.Text;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    row[identifier] = null;
                }
            }
            bgSql.ReportProgress(0, row);
        }

        private void ProcessDataReader(DataTable table, NameValueCollection aggParams, OSGeo.FDO.Commands.Feature.IDataReader reader)
        {
            DataRow row = table.NewRow();
            foreach (string identifier in aggParams.AllKeys)
            {
                if (!reader.IsNull(identifier))
                {
                    PropertyType ptype = reader.GetPropertyType(identifier);
                    switch (ptype)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            {
                                DataType dtype = reader.GetDataType(identifier);
                                switch (dtype)
                                {
                                    case DataType.DataType_BLOB:
                                        row[identifier] = reader.GetLOB(identifier).Data;
                                        break;
                                    case DataType.DataType_Boolean:
                                        row[identifier] = reader.GetBoolean(identifier);
                                        break;
                                    case DataType.DataType_Byte:
                                        row[identifier] = reader.GetByte(identifier);
                                        break;
                                    case DataType.DataType_CLOB:
                                        row[identifier] = reader.GetLOB(identifier).Data;
                                        break;
                                    case DataType.DataType_DateTime:
                                        row[identifier] = reader.GetDateTime(identifier);
                                        break;
                                    case DataType.DataType_Decimal:
                                        row[identifier] = reader.GetDouble(identifier);
                                        break;
                                    case DataType.DataType_Double:
                                        row[identifier] = reader.GetDouble(identifier);
                                        break;
                                    case DataType.DataType_Int16:
                                        row[identifier] = reader.GetInt16(identifier);
                                        break;
                                    case DataType.DataType_Int32:
                                        row[identifier] = reader.GetInt32(identifier);
                                        break;
                                    case DataType.DataType_Int64:
                                        row[identifier] = reader.GetInt64(identifier);
                                        break;
                                    case DataType.DataType_Single:
                                        row[identifier] = reader.GetSingle(identifier);
                                        break;
                                    case DataType.DataType_String:
                                        row[identifier] = reader.GetString(identifier);
                                        break;
                                }
                            }
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            {
                                byte[] bGeom = reader.GetGeometry(identifier);
                                using (IGeometry geom = _GeomFactory.CreateGeometryFromFgf(bGeom))
                                {
                                    row[identifier] = geom.Text;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    row[identifier] = null;
                }
            }
            table.Rows.Add(row);
        }

        /// <summary>
        /// Prepares the data grid for select aggregate query results
        /// </summary>
        /// <param name="table"></param>
        /// <param name="reader"></param>
        private void PrepareGrid(DataTable table, OSGeo.FDO.Commands.Feature.IDataReader reader)
        {
            int propCount = reader.GetPropertyCount();
            for (int i = 0; i < propCount; i++)
            {
                string propName = reader.GetPropertyName(i);
                table.Columns.Add(propName);
            }
        }

        /// <summary>
        /// Prepares the data grid for SQL query results
        /// </summary>
        /// <param name="table"></param>
        /// <param name="reader"></param>
        private void PrepareGrid(DataTable table, ISQLDataReader reader)
        {
            int propCount = reader.GetColumnCount();
            for (int i = 0; i < propCount; i++)
            {
                string propName = reader.GetColumnName(i);
                table.Columns.Add(propName);
            }
        }

        private bool CheckValidAggregates()
        {
            // Return true only if the following is true
            // - All cells are not empty
            // - All Expressions are valid
            bool valid = true;
            foreach (DataGridViewRow row in grdExpressions.Rows)
            {
                object expr = row.Cells[0].Value;
                object alias = row.Cells[1].Value;

                if (expr == null)
                {
                    row.ErrorText = "Expression cannot be empty";
                    valid = false;
                }
                else
                {
                    try
                    {
                        using (Expression exp = Expression.Parse(expr.ToString())) { }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        row.ErrorText = ex.Message;
                        valid = false;
                    }
                }

                if (alias == null)
                {
                    row.ErrorText = "Alias cannot be empty";
                    valid = false;
                }
            }

            return valid;
        }

        private NameValueCollection GetAggregateParameters()
        {
            NameValueCollection dict = new NameValueCollection();
            foreach (DataGridViewRow row in grdExpressions.Rows)
            {
                dict.Add(row.Cells[1].Value.ToString(), row.Cells[0].Value.ToString());
            }
            return dict;
        }

        private NameValueCollection GetComputedFields()
        {
            NameValueCollection dict = new NameValueCollection();
            foreach (DataGridViewRow row in grdComputedFields.Rows)
            {
                dict.Add(row.Cells[1].Value.ToString(), row.Cells[0].Value.ToString());
            }
            return dict;
        }

        private void QueryStandard()
        {
            if (!CheckValidFilter())
            {
                AppConsole.Alert("Error", "Invalid filter. Please correct");
                return;
            }
            int limit = Convert.ToInt32(numLimit.Value);
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                StandardFeatureQuery qry = new StandardFeatureQuery();
                qry.Limit = limit;
                qry.ClassName = classDef.Name;
                qry.Filter = txtFilter.Text;
                qry.PropertyNames = GetCheckedProperties();

                grpQuery.Enabled = false;
                btnQuery.Enabled = false;
                btnClear.Enabled = false;
                ClearGrid();
                bgStandard.RunWorkerAsync(qry);
            }
        }

        class StandardFeatureQuery
        {
            public string ClassName;
            public string Filter;
            public List<string> PropertyNames;
            public int Limit;
        }

        class SQLFeatureQuery
        {
            public string SQLText;
        }

        private List<string> GetCheckedProperties()
        {
            List<string> propNames = new List<string>();
            foreach (object obj in chkPropertyNames.CheckedItems)
            {
                propNames.Add(obj.ToString());
            }
            return propNames;
        }

        /// <summary>
        /// Prepares the data grid for standard query results
        /// </summary>
        /// <param name="table"></param>
        /// <param name="classDefinition"></param>
        private void PrepareGrid(DataTable table, ClassDefinition classDefinition)
        {
            foreach (PropertyDefinition def in classDefinition.Properties)
            {
                table.Columns.Add(def.Name);
            }
        }

        private void ProcessFeatureReader(DataTable table, PropertyDefinitionCollection propDefs, Dictionary<int, string> cachedPropertyNames, IFeatureReader reader)
        {
            ClassDefinition classDef = reader.GetClassDefinition();
            DataRow row = table.NewRow();
            foreach (int key in cachedPropertyNames.Keys)
            {
                string name = cachedPropertyNames[key];
                PropertyDefinition def = propDefs[key];
                if (!reader.IsNull(name))
                {
                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null)
                    {
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_String:
                                row[name] = reader.GetString(name);
                                break;
                            case DataType.DataType_Int16:
                                row[name] = reader.GetInt16(name);
                                break;
                            case DataType.DataType_Int32:
                                row[name] = reader.GetInt32(name);
                                break;
                            case DataType.DataType_Int64:
                                row[name] = reader.GetInt64(name);
                                break;
                            case DataType.DataType_Double:
                                row[name] = reader.GetDouble(name);
                                break;
                            case DataType.DataType_Boolean:
                                row[name] = reader.GetBoolean(name);
                                break;
                            case DataType.DataType_Byte:
                                row[name] = reader.GetByte(name);
                                break;
                            case DataType.DataType_BLOB:
                                row[name] = reader.GetLOB(name).Data;
                                break;
                            case DataType.DataType_CLOB:
                                row[name] = reader.GetLOB(name).Data;
                                break;
                            case DataType.DataType_DateTime:
                                row[name] = reader.GetDateTime(name);
                                break;
                            case DataType.DataType_Decimal:
                                row[name] = reader.GetDouble(name);
                                break;
                            case DataType.DataType_Single:
                                row[name] = reader.GetSingle(name);
                                break;
                        }
                    }
                    else if (geomDef != null)
                    {
                        byte[] fgf = reader.GetGeometry(name);
                        row[name] = FdoGeometryUtil.GetFgfText(fgf);
                    }
                }
                else
                {
                    row[name] = null;
                }
            }
            bgStandard.ReportProgress(0, row);
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbClass.DataSource = schema.Classes;
            }
        }

        private bool CheckValidFilter()
        {
            bool valid = false;
            try
            {
                if (!string.IsNullOrEmpty(txtFilter.Text))
                {
                    using (Filter filter = Filter.Parse(txtFilter.Text)) { }
                }
                valid = true;
            }
            catch (OSGeo.FDO.Common.Exception)
            {
                valid = false;
            }
            return valid;
        }

        private void txtFilter_Leave(object sender, EventArgs e)
        {
            if (!CheckValidFilter())
            {
                AppConsole.Alert("Error", "Invalid filter. Please correct");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearGrid();
        }

        private void ClearGrid()
        {
            grdPreview.DataSource = null;
            lblCount.Text = "";
        }

        private void cmbAggSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbAggSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbAggClass.DataSource = schema.Classes;
            }
        }

        private void grdExpressions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ClassDefinition classDef = cmbAggClass.SelectedItem as ClassDefinition;
            btnDeleteExpr.Enabled = btnClearComputedFields.Enabled = (grdExpressions.Rows.Count > 0);
            if (classDef != null && grdExpressions.Rows.Count > 0)
            {
                if (e.ColumnIndex == 0)
                {
                    string expr = string.Empty;
                    object obj = grdExpressions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (obj != null)
                        expr = obj.ToString();

                    expr = ExpressionDlg.EditExpression(this.BoundConnection, classDef, expr, ExpressionMode.SelectAggregates);
                    if (!string.IsNullOrEmpty(expr))
                        grdExpressions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = expr;
                }
            }
        }

        private void grdComputedFields_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            btnDeleteComputedField.Enabled = btnClearComputedFields.Enabled = (grdComputedFields.Rows.Count > 0);
            if (classDef != null && grdComputedFields.Rows.Count > 0)
            {
                if (e.ColumnIndex == 0)
                {
                    string expr = string.Empty;
                    object obj = grdComputedFields.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (obj != null)
                        expr = obj.ToString();

                    expr = ExpressionDlg.EditExpression(this.BoundConnection, classDef, expr, ExpressionMode.Normal);
                    if (!string.IsNullOrEmpty(expr))
                        grdComputedFields.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = expr;
                }
            }
        }

        private int counter = 0;

        private void btnAddExpr_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbAggClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                string expr = ExpressionDlg.NewExpression(this.BoundConnection, classDef, ExpressionMode.SelectAggregates);
                if (!string.IsNullOrEmpty(expr))
                {
                    string identifier = "Expr" + (counter++);
                    grdExpressions.Rows.Add(expr, identifier);
                }
            }
        }

        private void btnDeleteExpr_Click(object sender, EventArgs e)
        {
            if (grdExpressions.SelectedRows.Count == 1)
                grdExpressions.Rows.Remove(grdExpressions.SelectedRows[0]);
            else if (grdExpressions.SelectedCells.Count == 1)
                grdExpressions.Rows.RemoveAt(grdExpressions.SelectedCells[0].RowIndex);
        }

        private void btnEditFilter_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if(classDef != null)
            {
                string filterText = txtFilter.Text;
                string newFilter = ExpressionDlg.EditExpression(this.BoundConnection, classDef, filterText, ExpressionMode.Filter);
                if (!string.IsNullOrEmpty(newFilter))
                {
                    txtFilter.Text = newFilter;
                }
            }
        }


        public override void SetName(string name)
        {
            this.BoundConnection.Name = name;
            this.Title = "Data Preview - " + this.BoundConnection.Name;
        }

        private string _schemaName;
        private string _className;

        public void SetInitialClass(string schemaName, string className)
        {
            _schemaName = schemaName;
            _className = className;

            SetSelectedClass();
        }

        private void SetSelectedClass()
        {
            if (!string.IsNullOrEmpty(_schemaName) && !string.IsNullOrEmpty(_className))
            {
                switch (tabQueryMode.SelectedIndex)
                {
                    case TAB_STANDARD:
                        foreach (object obj in cmbSchema.Items)
                        {
                            if ((obj as FeatureSchema).Name == _schemaName)
                                cmbSchema.SelectedItem = obj;
                        }
                        foreach (object obj in cmbClass.Items)
                        {
                            if ((obj as ClassDefinition).Name == _className)
                                cmbClass.SelectedItem = obj;
                        }
                        break;
                    case TAB_AGGREGATE:
                        foreach (object obj in cmbAggSchema.Items)
                        {
                            if ((obj as FeatureSchema).Name == _schemaName)
                                cmbAggSchema.SelectedItem = obj;
                        }
                        foreach (object obj in cmbAggClass.Items)
                        {
                            if ((obj as ClassDefinition).Name == _className)
                                cmbAggClass.SelectedItem = obj;
                        }
                        break;
                }
            }
        }

        public override string GetTabType()
        {
            return CoreModule.TAB_DATA_PREVIEW;
        }

        private void btnClearComputedFields_Click(object sender, EventArgs e)
        {
            grdComputedFields.Rows.Clear();
        }

        private void btnDeleteComputedField_Click(object sender, EventArgs e)
        {
            if (grdComputedFields.SelectedRows.Count == 1)
                grdComputedFields.Rows.Remove(grdComputedFields.SelectedRows[0]);
            else if (grdComputedFields.SelectedCells.Count == 1)
                grdComputedFields.Rows.RemoveAt(grdComputedFields.SelectedCells[0].RowIndex);
        }

        private void btnAddComputedField_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                string expr = ExpressionDlg.NewExpression(this.BoundConnection, classDef, ExpressionMode.Normal);
                if (!string.IsNullOrEmpty(expr))
                {
                    string identifier = "Expr" + (counter++);
                    grdComputedFields.Rows.Add(expr, identifier);
                }
            }
        }

        private void btnClearAggregates_Click(object sender, EventArgs e)
        {
            grdExpressions.Rows.Clear();
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                chkPropertyNames.Items.Clear();
                foreach (PropertyDefinition propDef in classDef.Properties)
                {
                    chkPropertyNames.Items.Add(propDef.Name, true);
                }
            }
        }

        private void btnCheckAllProperties_Click(object sender, EventArgs e)
        {
            CheckAllProperties(true);
        }

        private void CheckAllProperties(bool isChecked)
        {
            for (int i = 0; i < chkPropertyNames.Items.Count; i++)
            {
                chkPropertyNames.SetItemChecked(i, isChecked);
            }
        }

        private void btnUncheckAllProperties_Click(object sender, EventArgs e)
        {
            CheckAllProperties(false);
        }

        private void btnAggFilter_Click(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbAggClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                string filterText = txtAggFilter.Text;
                string newFilter = ExpressionDlg.EditExpression(this.BoundConnection, classDef, filterText, ExpressionMode.Filter);
                if (!string.IsNullOrEmpty(newFilter))
                {
                    txtAggFilter.Text = newFilter;
                }
            }
        }

        private void bgStandard_DoWork(object sender, DoWorkEventArgs e)
        {
            StandardFeatureQuery qry = e.Argument as StandardFeatureQuery;
            int limit = qry.Limit;
            using (ISelect select = this.BoundConnection.Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
            {
                select.SetFeatureClassName(qry.ClassName);
                if (!string.IsNullOrEmpty(qry.Filter))
                    select.Filter = Filter.Parse(qry.Filter);

                select.PropertyNames.Clear();
                foreach (string propName in qry.PropertyNames)
                {
                    select.PropertyNames.Add((Identifier)Identifier.Parse(propName));
                }

                using (IFeatureReader reader = select.Execute())
                {
                    DataTable table = new DataTable(qry.ClassName);
                    int count = 0;
                    ClassDefinition cd = reader.GetClassDefinition();
                    PrepareGrid(table, cd);
                    Dictionary<int, string> cachedPropertyNames = new Dictionary<int, string>();
                    for (int i = 0; i < cd.Properties.Count; i++)
                    {
                        cachedPropertyNames.Add(i, cd.Properties[i].Name);
                    }
                    try
                    {
                        if (limit > 0)
                        {
                            while (reader.ReadNext() && count < limit)
                            {
                                if (bgStandard.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                ProcessFeatureReader(table, cd.Properties, cachedPropertyNames, reader);
                                count++;
                                if (count % 50 == 0)
                                    Thread.Sleep(90);
                            }
                        }
                        else
                        {
                            while (reader.ReadNext())
                            {
                                if (bgStandard.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                ProcessFeatureReader(table, cd.Properties, cachedPropertyNames, reader);
                                count++;
                                if (count % 50 == 0)
                                    Thread.Sleep(75);
                            }
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
        }

        private void bgStandard_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DataRow row = e.UserState as DataRow;
            if (grdPreview.Rows.Count == 0)
            {
                DataTable table = new DataTable();
                table.Merge(row.Table);

                BindingSource bs = new BindingSource();
                bs.DataSource = table;
                grdPreview.DataSource = bs;

                table.Rows.Add(row.ItemArray);
            }
            else
            {
                DataTable table = (grdPreview.DataSource as BindingSource).DataSource as DataTable;
                table.Rows.Add(row.ItemArray);
                lblCount.Text = table.Rows.Count + " results found";
            }
        }

        private void bgStandard_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grpQuery.Enabled = true;
            btnQuery.Enabled = true;
            btnClear.Enabled = true;

            if (e.Error != null)
            {
                AppConsole.Alert("Error", e.Error.ToString());
                AppConsole.WriteException(e.Error);
            }
            else if (e.Cancelled)
            {
                AppConsole.WriteLine("Standard data query cancelled");
            }
        }

        private void bgSql_DoWork(object sender, DoWorkEventArgs e)
        {
            SQLFeatureQuery qry = e.Argument as SQLFeatureQuery;

            using (ISQLCommand sqlCmd = this.BoundConnection.Connection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand) as ISQLCommand)
            {
                sqlCmd.SQLStatement = qry.SQLText;
                int count = 0;
                using (ISQLDataReader reader = sqlCmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    PrepareGrid(table, reader);
                    try
                    {
                        while (reader.ReadNext())
                        {
                            if (bgSql.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            ProcessSQLReader(table, reader);
                            count++;

                            if (count % 50 == 0)
                                Thread.Sleep(75);
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
        }

        private void bgSql_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DataRow row = e.UserState as DataRow;
            if (grdPreview.Rows.Count == 0)
            {
                DataTable table = new DataTable();
                table.Merge(row.Table);

                BindingSource bs = new BindingSource();
                bs.DataSource = table;
                grdPreview.DataSource = bs;

                table.Rows.Add(row.ItemArray);
            }
            else
            {
                DataTable table = (grdPreview.DataSource as BindingSource).DataSource as DataTable;
                table.Rows.Add(row.ItemArray);
                lblCount.Text = table.Rows.Count + " results found";
            }
        }

        private void bgSql_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grpQuery.Enabled = true;
            btnQuery.Enabled = true;
            btnClear.Enabled = true;

            if (e.Error != null)
            {
                AppConsole.Alert("Error", e.Error.ToString());
                AppConsole.WriteException(e.Error);
            }
            else if (e.Cancelled)
            {
                AppConsole.WriteLine("SQL data query cancelled");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            switch (tabQueryMode.SelectedIndex)
            {
                case TAB_SQL:
                    {
                        if (bgSql.IsBusy)
                            bgSql.CancelAsync();
                    }
                    break;
                case TAB_STANDARD:
                    {
                        if (bgStandard.IsBusy)
                            bgStandard.CancelAsync();
                    }
                    break;
            }
        }
    }
}
