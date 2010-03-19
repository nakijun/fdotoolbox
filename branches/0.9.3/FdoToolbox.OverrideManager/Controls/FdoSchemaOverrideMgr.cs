#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Schema;
using FdoToolbox.OverrideManager.Controls.SchemaOverrideMgr;

namespace FdoToolbox.OverrideManager.Controls
{
    public partial class FdoSchemaOverrideMgr : ViewContent
    {
        private FdoSchemaOverrideMgr()
        {
            InitializeComponent();
            this.Title = ResourceService.GetString("TITLE_SCHEMA_OVERRIDES");
        }

        private FdoConnection _conn;

        public FdoSchemaOverrideMgr(FdoConnection conn) : this()
        {
            _conn = conn;
            var mapping = _conn.CreateSchemaMapping();
            var node = NodeFactory.CreateNode(mapping);
            (node.Tag as IPhysicalSchemaMapping).Name = "Fdo";
            treeMappings.Nodes.Add(node);
        }

        public override bool CanClose
        {
            get
            {
                return true;
            }
        }

        private void btnLoadMappings_Click(object sender, EventArgs e)
        {
            LoadMappings();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyMappingsToConnection();
        }

        private void LoadMappings()
        {
            using (var svc = _conn.CreateFeatureService())
            {
                var mappings = svc.DescribeSchemaMapping(true);
                LoadMappingTree(mappings);
            }
        }

        private void LoadMappingTree(PhysicalSchemaMappingCollection mappings)
        {
            treeMappings.Nodes.Clear();
            foreach (PhysicalSchemaMapping schemaMap in mappings)
            {
                TreeNode schemaNode = NodeFactory.CreateNode(schemaMap);
                treeMappings.Nodes.Add(schemaNode);
            }
            treeMappings.ExpandAll();
        }

        private void ApplyMappingsToConnection()
        {
            
        }

        private void treeMappings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                propGrid.SelectedObject = e.Node.Tag;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var diag = new SaveFileDialog();
            diag.Filter = "Schema Mapping Configuration (*.xml)|*.xml";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                PhysicalSchemaMappingCollection schemas = new PhysicalSchemaMappingCollection();
                foreach (TreeNode node in treeMappings.Nodes)
                {
                    var mapping = node.Tag as IPhysicalSchemaMapping;
                    if (mapping != null)
                    {
                        schemas.Add(mapping.Mapping);
                    }
                }
                schemas.WriteXml(diag.FileName);
                MessageService.ShowMessage("Schema mapping saved to: " + diag.FileName + ". You can specify this mapping file the next time you create this connection");
            }
        }
    }
}
