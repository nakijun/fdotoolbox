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

namespace FdoToolbox.Core.Controls
{
    public partial class DataPreviewCtl : BaseDocumentCtl
    {
        const int TAB_STANDARD = 0;
        const int TAB_AGGREGATE = 1;
        const int TAB_SQL = 2;

        private FgfGeometryFactory _GeomFactory;

        internal DataPreviewCtl()
        {
            InitializeComponent();
            cmbLimit.SelectedIndex = 0;
            _GeomFactory = new FgfGeometryFactory();
            this.Disposed += delegate { _GeomFactory.Dispose(); };
        }

        public DataPreviewCtl(IConnection conn)
            : this()
        {
            _BoundConnection = conn;
            ToggleUI();
        }

        private void ToggleUI()
        {
            if (!_BoundConnection.ConnectionCapabilities.SupportsSQL())
                tabQueryMode.TabPages.RemoveAt(TAB_SQL);
            if (!Array.Exists<int>(_BoundConnection.CommandCapabilities.Commands, delegate(int cmd) { return cmd == (int)OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates; }))
                tabQueryMode.TabPages.RemoveAt(TAB_AGGREGATE);
        }

        private IConnection _BoundConnection;

        protected override void OnLoad(EventArgs e)
        {
            using (IDescribeSchema desc = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = desc.Execute();
                cmbSchema.DataSource = schemas;
            }
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
            throw new NotImplementedException();
        }

        private void QueryAggregate()
        {
            throw new NotImplementedException();
        }

        private void QueryStandard()
        {
            if (!CheckValidFilter())
            {
                AppConsole.Alert("Error", "Invalid filter. Please correct");
                return;
            }
            int limit = Convert.ToInt32(cmbLimit.SelectedItem.ToString());
            using (ISelect select = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
            {
                try
                {
                    ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
                    if (classDef != null)
                    {
                        select.SetFeatureClassName(classDef.Name);
                        if (!string.IsNullOrEmpty(txtFilter.Text))
                            select.SetFilter(txtFilter.Text);
                        using (IFeatureReader reader = select.Execute())
                        {
                            DataTable table = new DataTable(cmbClass.SelectedItem.ToString());
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
                                while (reader.ReadNext() && count < limit)
                                {
                                    ProcessReader(table, cd.Properties, cachedPropertyNames, reader);
                                    count++;
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
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    AppConsole.Alert("Error", ex.Message);
                    AppConsole.WriteException(ex);
                }
            }
        }

        private void PrepareGrid(DataTable table, ClassDefinition classDefinition)
        {
            foreach (PropertyDefinition def in classDefinition.Properties)
            {
                table.Columns.Add(def.Name);
            }
        }

        private void ProcessReader(DataTable table, PropertyDefinitionCollection propDefs, Dictionary<int, string> cachedPropertyNames, IFeatureReader reader)
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
                        byte[] binGeom = reader.GetGeometry(name);
                        using (IGeometry geom = _GeomFactory.CreateGeometryFromFgf(binGeom))
                        {
                            //FIXME: 
                            //The line below when called many times over will eventually
                            //cause "Memory allocation failed"
                            //
                            //API docs state that calling get_Text() on a 
                            //large number of IGeometry objects that are retained 
                            //in memory may cause a noticable increase in memory 
                            //consumption. 
                            //
                            //Does not calling Dispose() or wrapping the
                            //IGeometry in a using block solve this problem?
                            string text = geom.Text;
                            row[name] = text;
                        }
                    }
                }
                else
                {
                    row[name] = null;
                }
            }
            table.Rows.Add(row);
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbClass.DataSource = schema.Classes;
            }
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if(classDef != null)
                this.Title = "Data Preview - " + classDef.Name;
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
            grdPreview.DataSource = null;
        }
    }
}
