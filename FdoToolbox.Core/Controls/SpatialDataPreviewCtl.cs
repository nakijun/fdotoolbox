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
using FdoToolbox.Core.ETL;
using System.IO;

namespace FdoToolbox.Core.Controls
{
    public partial class SpatialDataPreviewCtl : SpatialConnectionBoundControl
    {
        const int TAB_STANDARD = 0;
        const int TAB_AGGREGATE = 1;
        const int TAB_SQL = 2;

        private FeatureService _Service;

        internal SpatialDataPreviewCtl()
        {
            InitializeComponent();
        }

        public SpatialDataPreviewCtl(FdoConnectionInfo conn, string key)
            : base(conn, key)
        {
            InitializeComponent();
            _BoundConnection = conn;
            _Service = conn.CreateFeatureService();
            ToggleUI();
        }

        private void ToggleUI()
        {
            if (!this.BoundConnection.InternalConnection.ConnectionCapabilities.SupportsSQL())
                tabQueryMode.TabPages.RemoveAt(TAB_SQL);
            if (!Array.Exists<int>(this.BoundConnection.InternalConnection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates; }))
                tabQueryMode.TabPages.RemoveAt(TAB_AGGREGATE);
        }

        protected override void OnLoad(EventArgs e)
        {
            cmbSchema.DataSource = _Service.DescribeSchema();
            cmbAggSchema.DataSource = _Service.DescribeSchema();
            SetSelectedClass();
            base.OnLoad(e);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            ClearGrid();
            
            switch (tabQueryMode.SelectedIndex)
            {
                case TAB_STANDARD:
                    {
                        if (ProceedWithQuery())
                        {
                            QueryStandard();
                            if (chkMap.Checked)
                                QueryMap();
                        }
                    }
                    break;
                case TAB_AGGREGATE:
                    {
                        QueryAggregate();
                    }
                    break;
                case TAB_SQL:
                    {
                        if (ProceedWithQuery())
                            QuerySQL();
                    }
                    break;
            }
        }

        private bool ProceedWithQuery()
        {
            long count = GetFeatureCount();
            int limit = AppGateway.RunningApplication.Preferences.GetIntegerPref(PreferenceNames.PREF_INT_WARN_DATASET);
            if (count > limit)
                return AppConsole.Confirm("Warning", "The query you have defined will return a potentially large result set. Do you want to continue?");
            return true;
        }

        private long GetFeatureCount()
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            string filter = txtFilter.Text;
            string property = "FEATURECOUNT";
            long count = 0;
            using (FeatureService service = this.BoundConnection.CreateFeatureService())
            {
                if (service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
                {
                    using (ISQLCommand cmd = service.CreateCommand<ISQLCommand>(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
                    {
                        cmd.SQLStatement = string.Format("SELECT COUNT(*) AS {0} FROM {1}", property, classDef.Name);
                        if (!string.IsNullOrEmpty(filter))
                            cmd.SQLStatement += " WHERE " + filter;

                        using (ISQLDataReader reader = cmd.ExecuteReader())
                        {
                            if(reader.ReadNext())
                            {
                                count = reader.GetInt64(property);
                            }
                        }
                    }
                }
                else if (service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                {
                    using (ISelectAggregates select = service.CreateCommand<ISelectAggregates>(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                    {
                        select.SetFeatureClassName(classDef.Name);
                        if(!string.IsNullOrEmpty(filter))
                            select.Filter = Filter.Parse(filter);
                        select.PropertyNames.Add(new ComputedIdentifier(property, Expression.Parse("COUNT(" + classDef.IdentityProperties[0].Name + ")")));

                        using (OSGeo.FDO.Commands.Feature.IDataReader reader = select.Execute())
                        {
                            if (reader.ReadNext() && !reader.IsNull(property))
                            {
                                count = reader.GetInt64(property);
                            }
                        }
                    }
                }
            }
            return count;
        }

        private void QueryMap()
        {
            mapCtl.Initialize(this.BoundConnection.InternalConnection);
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if (classDef != null)
            {
                FeatureQueryOptions qry = new FeatureQueryOptions(classDef.Name);
                qry.Filter = txtFilter.Text;
                //qry.AddFeatureProperty(GetCheckedProperties());
                //qry.AddComputedProperty(GetComputedFields());
                mapCtl.LoadQuery(qry);
                //mapCtl.ZoomExtents();
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
                try
                {
                    FeatureAggregateOptions options = new FeatureAggregateOptions(classDef.Name);
                    options.Distinct = chkDistinct.Checked;
                    if(!string.IsNullOrEmpty(txtAggFilter.Text))
                        options.Filter = txtAggFilter.Text;

                    NameValueCollection aggParams = GetAggregateParameters();
                    options.AddComputedProperty(aggParams);

                    using (OSGeo.FDO.Commands.Feature.IDataReader reader = _Service.SelectAggregates(options))
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
                        catch (OSGeo.FDO.Common.Exception)
                        {
                            throw;
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
                                row[identifier] = FdoGeometryUtil.GetFgfText(bGeom);
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

        private static void ProcessDataReader(DataTable table, NameValueCollection aggParams, OSGeo.FDO.Commands.Feature.IDataReader reader)
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
                                row[identifier] = FdoGeometryUtil.GetFgfText(bGeom);
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
        private static void PrepareGrid(DataTable table, OSGeo.FDO.Commands.Feature.IDataReader reader)
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
        private static void PrepareGrid(DataTable table, ISQLDataReader reader)
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
                StandardFeatureQuery qry = new StandardFeatureQuery(classDef.Name);
                qry.Limit = limit;
                qry.Filter = txtFilter.Text;
                qry.AddFeatureProperty(GetCheckedProperties());
                qry.AddComputedProperty(GetComputedFields());

                grpQuery.Enabled = false;
                btnQuery.Enabled = false;
                btnClear.Enabled = false;
                ClearGrid();
                bgStandard.RunWorkerAsync(qry);
            }
        }

        public class StandardFeatureQuery : FeatureQueryOptions
        {
            private int _Limit;

            public int Limit
            {
                get { return _Limit; }
                set { _Limit = value; }
            }

            public StandardFeatureQuery(string className) : base(className) { }
        }

        internal class SQLFeatureQuery
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
        
        private void ProcessFeatureReader(DataTable table, PropertyDefinitionCollection propDefs, Dictionary<int, string> cachedPropertyNames, IFeatureReader reader)
        {
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
                        /*
                        using (IGeometry geom = _GeomFactory.CreateGeometryFromFgf(fgf))
                        {
                            string text = geom.Text;
                            row[name] = text;
                        }*/
                    }
                }
                else
                {
                    row[name] = DBNull.Value;
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
            mapCtl.Reset();
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

        private int counter;

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
            
            using (IFeatureReader reader = _Service.SelectFeatures(qry))
            {
                ClassDefinition cd = reader.GetClassDefinition();
                FdoDataTable table = TableFactory.CreateTable(cd);
                int count = 0;
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
                catch (OSGeo.FDO.Common.Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        private void bgStandard_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DataRow row = e.UserState as DataRow;
            if (grdPreview.Rows.Count == 0)
            {
                /*
                DataTable table = new DataTable();
                table.TableName = row.Table.TableName;
                table.Merge(row.Table);
                */
                DataTable table = row.Table;
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
            int count = 0;
            using (ISQLDataReader reader = _Service.ExecuteSQLQuery(qry.SQLText))
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
                catch (OSGeo.FDO.Common.Exception)
                {
                    throw;
                }
                finally
                {
                    reader.Close();
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

        private void saveToSDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (grdPreview.DataSource == null || (grdPreview.DataSource as BindingSource).DataSource == null)
            {
                AppConsole.Alert("Unable to save", "Nothing to save");
                return;
            }
            DataTable table = (grdPreview.DataSource as BindingSource).DataSource as DataTable;
            if (table.Rows.Count == 0)
            {
                AppConsole.Alert("Unable to save", "Nothing to save");
                return;
            }
            if (saveQueryDlg.ShowDialog() == DialogResult.OK)
            {
                string file = saveQueryDlg.FileName;
                string ext = Path.GetExtension(file);

                FdoDataTable fdoTable = table as FdoDataTable;
                DataTableConversionOptions options = null;
                if (ext.ToLower() == ".sdf")
                    options = new DataTableConversionOptions(fdoTable, "OSGeo.SDF", file);
                else if (ext.ToLower() == ".shp")
                    options = new DataTableConversionOptions(fdoTable, "OSGeo.SHP", file);
                else
                {
                    AppConsole.Alert("Error", "Unsupported file extension");
                    return;
                }
                options.SchemaName = "Default";
                options.ClassName = table.TableName;
                options.UseFdoMetaData = true;
                ITask task = new DataTableToFlatFileTask(options);
                new TaskProgressDlg(task).Run();
            }
        }

        private void tabQueryMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitSave.Visible = (tabQueryMode.SelectedIndex == TAB_STANDARD);
        }

        private void chkMap_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMap.Checked)
            {
                if (!tabResults.TabPages.Contains(TAB_RESULTS_MAP))
                    tabResults.TabPages.Add(TAB_RESULTS_MAP);
            }
            else
            {
                tabResults.TabPages.Remove(TAB_RESULTS_MAP);
            }
        }
    }
}
