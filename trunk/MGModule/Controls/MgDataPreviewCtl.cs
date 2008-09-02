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
using FdoToolbox.Core.Controls;
using OSGeo.MapGuide.MaestroAPI;
using MGModule.Common;

namespace MGModule.Controls
{
    public partial class MgDataPreviewCtl : BaseDocumentCtl, IMgConnectionBoundCtl
    {
        internal MgDataPreviewCtl()
        {
            InitializeComponent();
            this.Title = "Feature Source Preview";
        }

        public MgDataPreviewCtl(ServerConnectionI conn)
            : this()
        {
            _Conn = conn;
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
        }
    }
}
