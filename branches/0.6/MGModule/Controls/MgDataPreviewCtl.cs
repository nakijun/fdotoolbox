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
using FdoToolbox.Lib.Controls;
using OSGeo.MapGuide.MaestroAPI;
using MGModule.Common;
using Topology.Geometries;

namespace MGModule.Controls
{
    public partial class MgDataPreviewCtl : BaseDocumentCtl, IMgConnectionBoundCtl
    {
        internal MgDataPreviewCtl()
        {
            InitializeComponent();
            this.Title = "Feature Source Preview";
        }

        private Color _RefreshBtnBackColor;

        public MgDataPreviewCtl(ServerConnectionI conn)
            : this()
        {
            _Conn = conn;
            _RefreshBtnBackColor = btnRefresh.BackColor;
        }

        private string _FeatureSource;

        public string FeatureSourceId
        {
            get { return _FeatureSource; }
            set { _FeatureSource = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            FeatureSourceDescription desc = this.BoundConnection.DescribeFeatureSource(this.FeatureSourceId);
            cmbFeatureClass.DataSource = desc.Schemas;
            cmbFeatureClass.DisplayMember = "Fullname";
            cmbFeatureClass.ValueMember = "Name";

            base.OnLoad(e);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            lblCount.Text = "";
            tabPreview.SelectedIndex = IDX_TAB_GRID;
            string className = cmbFeatureClass.SelectedValue.ToString();
            List<string> columns = new List<string>();
            foreach (object obj in chkColumns.CheckedItems)
            {
                columns.Add(obj.ToString());
            }
            FeatureSetReader reader = this.BoundConnection.QueryFeatureSource(this.FeatureSourceId, className, txtFilter.Text, columns.ToArray());
            MgDataTable table = new MgDataTable();
            table.LoadFromReader(reader);
            grdPreview.DataSource = table;
            lblCount.Text = table.Rows.Count + " features found";
        }

        private ServerConnectionI _Conn;

        public ServerConnectionI BoundConnection
        {
            get { return _Conn; }
        }

        private void cmbFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSourceDescription.FeatureSourceSchema schema = cmbFeatureClass.SelectedItem as FeatureSourceDescription.FeatureSourceSchema;
            if (schema != null)
            {
                chkColumns.Items.Clear();
                foreach (FeatureSetColumn col in schema.Columns)
                {
                    chkColumns.Items.Add(col.Name, true);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            grdPreview.DataSource = null;
            lblCount.Text = "";
        }

        private void RefreshMap()
        {
            string url = GetPreviewUrl();

            webBrowser.Navigate(url);
        }

        private string GetPreviewUrl()
        {
            string root = this.BoundConnection.DisplayName;
            
            FeatureSourceDescription.FeatureSourceSchema schema = cmbFeatureClass.SelectedItem as FeatureSourceDescription.FeatureSourceSchema;
            string resId = this.FeatureSourceId;
            string sessionId = this.BoundConnection.SessionID;
            //string pathTemplate = "/schemareport/describeschema.php?resId={0}&sessionId={1}&schemaName={2}&className={3}";
            //return root + string.Format(pathTemplate, resId, sessionId, schema.Schema, schema.Name);
            string pathTemplate = "/schemareport/describeschema.php?resId={0}&sessionId={1}";

            return root + string.Format(pathTemplate, resId, sessionId);
        }

        const int IDX_TAB_GRID = 0;
        const int IDX_TAB_MAP = 1;

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefresh.BackColor = _RefreshBtnBackColor;
            RefreshMap();
        }
    }
}
