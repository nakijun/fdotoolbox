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
using OSGeo.FDO.Schema;
using System.Diagnostics;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    /// <summary>
    /// Helper class to perform manipulation of the tree nodes
    /// </summary>
    internal class CopyTaskNodeDecorator
    {
        private FdoConnectionManager _connMgr;

        private TreeNode _node;

        public TreeNode DecoratedNode
        {
            get { return _node; }
        }

        public string Name
        {
            get { return _node.Text; }
            set { _node.Text = value; }
        }

        public string Description
        {
            get { return _node.ToolTipText; }
            set { _node.ToolTipText = value; }
        }

        internal FdoConnection GetSourceConnection()
        {
            return _connMgr.GetConnection(_srcConnName);
        }

        internal FdoConnection GetTargetConnection()
        {
            return _connMgr.GetConnection(_dstConnName);
        }

        internal CopyTaskNodeDecorator(TreeNode root, string srcConnName, string srcSchema, string srcClass, string dstConnName, string dstSchema, string dstClass, string taskName, bool createIfNotExists)
        {
            _node = new TreeNode();
            root.Nodes.Add(_node);

            _node.Nodes.Add(new TreeNode("Description"));
            _node.Nodes.Add(new TreeNode("Options"));
            _node.Nodes.Add(new TreeNode("Property Mappings"));
            _node.Nodes.Add(new TreeNode("Expression Mappings (Right click to add)"));

            this.Name = taskName;
            this.Description = "Copies features from " + srcClass + " to " + dstClass;

            if (createIfNotExists)
                dstClass = srcClass + " (created if it doesn't exist)";

            InitDescription(srcConnName, srcSchema, srcClass, dstConnName, dstSchema, dstClass);

            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();

            _srcConnName = srcConnName;
            _dstConnName = dstConnName;
            this.CreateIfNotExists = createIfNotExists;

            using (FdoFeatureService srcSvc = GetSourceConnection().CreateFeatureService())
            using (FdoFeatureService dstSvc = GetTargetConnection().CreateFeatureService())
            {
                ClassDefinition sourceClass = srcSvc.GetClassByName(srcSchema, srcClass);
                Debug.Assert(sourceClass != null);

                ClassDefinition targetClass = dstSvc.GetClassByName(dstSchema, dstClass);
                if (targetClass == null && !this.CreateIfNotExists)
                    throw new InvalidOperationException("Target class " + dstClass + " does not exist. If you want this class created make sure you checked \"Create class of the name name\" when creating a new copy task");

                _srcClass = sourceClass;
                _dstClass = targetClass;
            }

            _options = new OptionsNodeDecorator(this, _node.Nodes[1]);
            _propMappings = new PropertyMappingsNodeDecorator(this, _node.Nodes[2]);
            _exprMappings = new ExpressionMappingsNodeDecorator(this, _node.Nodes[3]);

            _node.ExpandAll();
        }

        private string _srcSchemaName;
        private string _srcClassName;
        private string _dstSchemaName;
        private string _dstClassName;

        public string SourceSchemaName { get { return _srcSchemaName; } }

        public string SourceClassName { get { return _srcClassName; } }

        public string TargetSchemaName { get { return _dstSchemaName; } }

        public string TargetClassName { get { return _dstClassName; } }

        public bool CreateIfNotExists { get; private set; }

        private void InitDescription(string srcConnName, string srcSchema, string srcClass, string dstConnName, string dstSchema, string dstClass)
        {
            TreeNode srcNode = new TreeNode("Source");
            TreeNode dstNode = new TreeNode("Target");
            srcNode.Nodes.Add("Connection: " + srcConnName);
            srcNode.Nodes.Add("Schema: " + srcSchema);
            srcNode.Nodes.Add("Feature Class: " + srcClass);

            dstNode.Nodes.Add("Connection: " + dstConnName);
            dstNode.Nodes.Add("Schema: " + dstSchema);
            dstNode.Nodes.Add("Feature Class: " + dstClass);

            _node.Nodes[0].Nodes.Add(srcNode);
            _node.Nodes[0].Nodes.Add(dstNode);

            _srcSchemaName = srcSchema;
            _srcClassName = srcClass;

            _dstSchemaName = dstSchema;
            _dstClassName = dstClass;
        }

        private string _srcConnName;

        public string SourceConnectionName
        {
            get
            {
                return _srcConnName;
            }
        }

        private string _dstConnName;

        public string TargetConnectionName
        {
            get
            {
                return _dstConnName;
            }
        }

        private ClassDefinition _srcClass;

        internal ClassDefinition SourceClass
        {
            get
            {
                return _srcClass;
            }
        }

        private ClassDefinition _dstClass;

        internal ClassDefinition TargetClass
        {
            get
            {
                return _dstClass;
            }
        }

        private OptionsNodeDecorator _options;

        public OptionsNodeDecorator Options
        {
            get
            {
                return _options;
            }
        }

        private PropertyMappingsNodeDecorator _propMappings;

        public PropertyMappingsNodeDecorator PropertyMappings
        {
            get 
            {
                return _propMappings;
            }
        }

        private ExpressionMappingsNodeDecorator _exprMappings;

        public ExpressionMappingsNodeDecorator ExpressionMappings
        {
            get
            {
                return _exprMappings;
            }
        }
    }
}
