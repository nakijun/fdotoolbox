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
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;
using FdoToolbox.Base.Forms;
using ICSharpCode.Core;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    /// <summary>
    /// Helper class to perform manipulation of the tree nodes
    /// </summary>
    internal class OptionsNodeDecorator
    {
        private TreeNode _node;

        internal readonly CopyTaskNodeDecorator Parent;

        const string OPT_DEL_TARGET = "OPT_DEL_TARGET";
        const string OPT_CLS_FILTER = "OPT_CLS_FILTER";
        const string OPT_BATCH_SIZE = "OPT_BATCH_SIZE";
        const string OPT_FLATTEN = "OPT_FLATTEN";

        private ContextMenuStrip ctxDeleteTarget;
        private ContextMenuStrip ctxSourceFilter;
        private ContextMenuStrip ctxBatchSize;
        private ContextMenuStrip ctxFlatten;

        internal OptionsNodeDecorator(CopyTaskNodeDecorator parent, TreeNode optionsNode)
        {
            Parent = parent;
            _node = optionsNode;

            InitContextMenus();

            //Options - Delete Target
            TreeNode delTargetNode = new TreeNode("Delete Target");
            delTargetNode.ToolTipText = "Delete all features on the feature class before copying (true: enabled, false: disabled)";
            delTargetNode.Name = OPT_DEL_TARGET;
            delTargetNode.ContextMenuStrip = ctxDeleteTarget;

            //Options - Source Class Filter
            TreeNode srcFilterNode = new TreeNode("Source Class Filter");
            srcFilterNode.ToolTipText = "The filter to apply to the source query where the features will be copied from";
            srcFilterNode.Name = OPT_CLS_FILTER;
            srcFilterNode.ContextMenuStrip = ctxSourceFilter;

            //Options - Flatten Geometries
            TreeNode flattenNode = new TreeNode("Flatten Geometries");
            flattenNode.ToolTipText = "If true, will strip all Z and M coordinates from geometries being copied";
            flattenNode.Name = OPT_FLATTEN;
            flattenNode.ContextMenuStrip = ctxFlatten;

            _node.Nodes.Add(delTargetNode);
            _node.Nodes.Add(srcFilterNode);
            _node.Nodes.Add(flattenNode);

            //Set default values to avoid any nasty surprises
            this.Delete = false;
            this.SourceFilter = string.Empty;
            this.Flatten = false;

            //Test for batch support
            using (FdoFeatureService svc = Parent.GetTargetConnection().CreateFeatureService())
            {
                if (svc.SupportsBatchInsertion())
                {
                    TreeNode batchNode = new TreeNode("Insert Batch Size");
                    batchNode.ToolTipText = "The batch size to use for batch insert. If set to 0, normal insert will be used";
                    batchNode.ContextMenuStrip = ctxBatchSize;
                    _node.Nodes.Add(batchNode);
                    //Set default values to avoid any nasty surprises
                    this.BatchSize = 0;
                }
            }
        }

        private void InitContextMenus()
        {
            ctxDeleteTarget = new ContextMenuStrip();
            ctxSourceFilter = new ContextMenuStrip();
            ctxBatchSize = new ContextMenuStrip();
            ctxFlatten = new ContextMenuStrip();

            //Delete Target
            ctxDeleteTarget.Items.Add("True", null, delegate { this.Delete = true; });
            ctxDeleteTarget.Items.Add("False", null, delegate { this.Delete = false; });

            //Source Filter
            ctxSourceFilter.Items.Add("Set Filter", null, delegate {
                string filter = this.SourceFilter;
                string newFilter = ExpressionEditor.EditExpression(Parent.GetSourceConnection(), Parent.SourceClass, filter, ExpressionMode.Filter);
                if (filter != newFilter)
                {
                    this.SourceFilter = newFilter;
                }
            });
            ctxSourceFilter.Items.Add("Clear", null, delegate { this.SourceFilter = string.Empty; });
            
            //Flatten Geometries
            ctxFlatten.Items.Add("True", null, delegate { this.Flatten = true; });
            ctxFlatten.Items.Add("False", null, delegate { this.Flatten = false; });

            //Batch Size
            ctxBatchSize.Items.Add("Set Size", null, delegate {
                string result = MessageService.ShowInputBox("Batch Size", "Set batch size", this.BatchSize.ToString());
                int size;
                while (!int.TryParse(result, out size))
                {
                    result = MessageService.ShowInputBox("Batch Size", "Set batch size", result);
                    if (result == null) //cancelled
                        return;
                }
                this.BatchSize = size;
            });
        }

        public bool Delete
        {
            get { return Convert.ToBoolean(_node.Nodes[0].Tag); }
            set 
            { 
                _node.Nodes[0].Tag = value;
                _node.Nodes[0].Text = "Delete: " + value;
            }
        }

        public string SourceFilter
        {
            get { return _node.Nodes[1].Tag.ToString(); }
            set 
            { 
                _node.Nodes[1].Tag = string.IsNullOrEmpty(value) ? string.Empty : value;
                _node.Nodes[1].Text = "Source Class Filter: " + value;
            }
        }

        public bool Flatten
        {
            get { return Convert.ToBoolean(_node.Nodes[2].Tag); }
            set
            {
                _node.Nodes[2].Tag = value;
                _node.Nodes[2].Text = "Flatten Geometries: " + value;
            }
        }

        public int BatchSize
        {
            get
            {
                if (_node.Nodes.Count == 4)
                {
                    return Convert.ToInt32(_node.Nodes[3].Tag);
                }
                return 0;
            }
            set 
            {
                if (_node.Nodes.Count == 4)
                {
                    _node.Nodes[3].Tag = value;
                    _node.Nodes[3].Text = "Insert Batch Size: " + value;
                }
            }
        }
    }
}
