#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoJoinDialog : Form
    {
        private FdoConnection _conn;

        public FdoJoinDialog(FdoConnection conn)
        {
            InitializeComponent();
            _conn = conn;
        }

        public FdoJoinCriteriaInfo Criteria
        {
            get;
            private set;
        }

        protected override void OnLoad(EventArgs e)
        {
            cmbJoinType.DataSource = _conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_JoinTypes);
            using (var svc = _conn.CreateFeatureService())
            {
                cmbSchema.DataSource = svc.DescribeSchema();
                cmbSchema.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Criteria = new FdoJoinCriteriaInfo()
            {
                JoinClass = ((ClassDefinition)cmbClass.SelectedItem).Name,
                JoinClassAlias = txtJoinClassAlias.Text,
                JoinFilter = txtJoinFilter.Text,
                JoinType = ((OSGeo.FDO.Expression.JoinType)cmbJoinType.SelectedItem)
            };
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            var cls = cmbClass.SelectedItem as ClassDefinition;
            if (cls != null)
            {
                string expr = ExpressionEditor.EditExpression(_conn, cls, txtJoinFilter.Text, ExpressionMode.Filter);
                if (expr != null)
                    txtJoinFilter.Text = expr;
            }
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            var schema = cmbSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbClass.DataSource = schema.Classes;
                cmbClass.SelectedIndex = 0;
            }
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = cmbClass.SelectedIndex >= 0;
        }
    }
}
