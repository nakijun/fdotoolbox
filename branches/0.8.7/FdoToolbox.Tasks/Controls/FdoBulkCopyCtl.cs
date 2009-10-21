using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Tasks.Controls.BulkCopy;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base;
using OSGeo.FDO.Schema;
using System.Diagnostics;
using FdoToolbox.Core.ETL.Specialized;
using ICSharpCode.Core;
using FdoToolbox.Base.Forms;
using FdoToolbox.Core.Configuration;
using System.Collections.Specialized;
using FdoToolbox.Tasks.Services;

namespace FdoToolbox.Tasks.Controls
{
    public partial class FdoBulkCopyCtl : ViewContent
    {
        private FdoConnectionManager _connMgr;

        public FdoBulkCopyCtl()
        {
            InitializeComponent();
            _connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
        }

        public FdoBulkCopyCtl(string name, FdoBulkCopy task)
            : this()
        {
            Load(task.Options, name);
            txtName.ReadOnly = true;
            btnSave.Enabled = true;
        }

        public override string Title
        {
            get
            {
                return ResourceService.GetString("TITLE_BULK_COPY_SETTINGS") + ": " + txtName.Text; 
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            PopulateConnections();
            base.OnLoad(e);
        }

        private void PopulateConnections()
        {
            foreach (string name in _connMgr.GetConnectionNames())
            {
                string connName = name; //Can't bind to iter variable
                btnAddConnection.DropDown.Items.Add(connName, null, delegate(object sender, EventArgs e)
                {
                    AddParticipatingConnection(connName);
                });
            }
        }

        private void AddParticipatingConnection(string name)
        {
            FdoConnection conn = _connMgr.GetConnection(name);
            grdConnections.Rows.Add(name, conn.Provider, conn.SafeConnectionString);
        }

        private CopyTaskNodeDecorator AddNewTask(TreeNode root, string srcConnName, string srcSchema, string srcClass, string dstConnName, string dstSchema, string dstClass, string taskName)
        {
            return new CopyTaskNodeDecorator(root, srcConnName, srcSchema, srcClass, dstConnName, dstSchema, dstClass, taskName);
        }

        private string[] GetAvailableConnectionNames()
        {
            List<string> values = new List<string>();
            foreach (DataGridViewRow row in grdConnections.Rows)
            {
                values.Add(row.Cells[0].Value.ToString());
            }
            return values.ToArray();
        }

        private SortedList<int, CopyTaskNodeDecorator> _tasks = new SortedList<int, CopyTaskNodeDecorator>();

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            if (GetAvailableConnectionNames().Length == 0)
            {
                MessageService.ShowMessage("Add some participating connections first", "No connections");
                return;
            }
            NewTaskDialog dlg = new NewTaskDialog(GetAvailableConnectionNames());
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TreeNode root = mTreeView.Nodes[0];
                CopyTaskNodeDecorator task = AddNewTask(
                                               root,
                                               dlg.SourceConnectionName,
                                               dlg.SourceSchema,
                                               dlg.SourceClass,
                                               dlg.TargetConnectionName,
                                               dlg.TargetSchema,
                                               dlg.TargetClass,
                                               dlg.TaskName);

                _tasks[task.DecoratedNode.Index] = task;
                root.Expand();

                btnSave.Enabled = (root.Nodes.Count > 0);
            }
        }

        const int NODE_LEVEL_TASK = 1;

        private void mTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Level)
            {
                case NODE_LEVEL_TASK:
                    btnRemoveTask.Enabled = true;
                    break;
                default:
                    btnRemoveTask.Enabled = false;
                    break;
            }
        }

        private void btnRemoveTask_Click(object sender, EventArgs e)
        {
            TreeNode node = mTreeView.SelectedNode;
            if (node.Level == NODE_LEVEL_TASK)
                mTreeView.Nodes.Remove(node);
        }

        //Right-click TreeView hack
        private void mTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mTreeView.SelectedNode = mTreeView.GetNodeAt(e.X, e.Y);
            }
        }

        public bool IsNew
        {
            get { return !txtName.ReadOnly; }
            set { txtName.ReadOnly = !value; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mTreeView.Nodes[0].Nodes.Count == 0)
            {
                MessageService.ShowMessage("Please define at least one copy task");
                return;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageService.ShowMessage("Please specify a name for this task");
                return;
            }

            TaskManager tmgr = ServiceManager.Instance.GetService<TaskManager>();

            if (IsNew && tmgr.GetTask(txtName.Text) != null)
            {
                MessageService.ShowMessage("A task named " + txtName.Text + " already exists. Please specify a name for this task");
                return;
            }

            TaskLoader loader = new TaskLoader();
            if (IsNew)
            {   
                string name = string.Empty;
                FdoBulkCopyOptions opts = loader.BulkCopyFromXml(Save(), ref name, false);
                FdoBulkCopy bcp = new FdoBulkCopy(opts);
                tmgr.AddTask(name, bcp);
                this.Close();
            }
            else
            {
                FdoBulkCopy bcp = tmgr.GetTask(txtName.Text) as FdoBulkCopy;
                if (bcp == null)
                {
                    MessageService.ShowMessage("This named task is not a bulk copy task or could not find the named task to update");
                    return;
                }
                string name = string.Empty;
                FdoBulkCopyOptions opts = loader.BulkCopyFromXml(Save(), ref name, false);
                Debug.Assert(name == txtName.Text); //unchanged

                //Update options
                bcp.Options = opts;
                MessageService.ShowMessage("Saved");
                this.Close();
            }
        }

        private FdoBulkCopyTaskDefinition Save()
        {
            FdoBulkCopyTaskDefinition def = new FdoBulkCopyTaskDefinition();
            def.name = txtName.Text;
            List<FdoConnectionEntryElement> conns = new List<FdoConnectionEntryElement>();
            foreach (DataGridViewRow row in grdConnections.Rows)
            {
                FdoConnectionEntryElement entry = new FdoConnectionEntryElement();
                entry.name = row.Cells[0].Value.ToString();
                entry.provider = row.Cells[1].Value.ToString();
                entry.ConnectionString = row.Cells[2].Value.ToString();
                conns.Add(entry);
            }
            List<FdoCopyTaskElement> tasks = new List<FdoCopyTaskElement>();
            foreach (CopyTaskNodeDecorator dec in _tasks.Values)
            {
                FdoCopyTaskElement task = new FdoCopyTaskElement();
                task.name = dec.Name;
                task.Source = new FdoCopySourceElement();
                task.Target = new FdoCopyTargetElement();
                task.Options = new FdoCopyOptionsElement();
                List<FdoPropertyMappingElement> pmaps = new List<FdoPropertyMappingElement>();
                List<FdoExpressionMappingElement> emaps = new List<FdoExpressionMappingElement>();
                
                //Source
                task.Source.@class = dec.SourceClassName;
                task.Source.connection = dec.SourceConnectionName;
                task.Source.schema = dec.SourceSchemaName;

                //Target
                task.Target.@class = dec.TargetClassName;
                task.Target.connection = dec.TargetConnectionName;
                task.Target.schema = dec.TargetSchemaName;

                //Options
                task.Options.BatchSize = dec.Options.BatchSize.ToString();
                task.Options.DeleteTarget = dec.Options.Delete;
                task.Options.Filter = dec.Options.SourceFilter;

                //Property Mappings
                NameValueCollection mappings = dec.PropertyMappings.GetPropertyMappings();
                foreach (string srcProp in mappings.Keys)
                {
                    string dstProp = mappings[srcProp];
                    FdoPropertyMappingElement p = new FdoPropertyMappingElement();
                    p.source = srcProp;
                    p.target = dstProp;

                    PropertyConversionNodeDecorator conv = dec.PropertyMappings.GetConversionRule(p.source);
                    p.nullOnFailedConversion = conv.NullOnFailedConversion;
                    p.truncate = conv.Truncate;

                    pmaps.Add(p);
                }

                foreach (string alias in dec.ExpressionMappings.GetAliases())
                {
                    FdoExpressionMappingElement e = new FdoExpressionMappingElement();
                    e.alias = alias;
                    ExpressionMappingInfo exMap = dec.ExpressionMappings.GetMapping(alias);
                    e.Expression = exMap.Expression;
                    e.target = exMap.TargetProperty;

                    PropertyConversionNodeDecorator conv = dec.ExpressionMappings.GetConversionRule(e.alias);
                    e.nullOnFailedConversion = conv.NullOnFailedConversion;
                    e.truncate = conv.Truncate;

                    emaps.Add(e);
                }

                task.PropertyMappings = pmaps.ToArray();
                task.ExpressionMappings = emaps.ToArray();

                tasks.Add(task);
            }
            def.Connections = conns.ToArray();
            def.CopyTasks = tasks.ToArray();
            return def;
        }

        private void Load(FdoBulkCopyOptions def, string name)
        {
            txtName.Text = name;

            grdConnections.Rows.Clear();

            foreach (string connName in def.ConnectionNames)
            {
                FdoConnection conn = _connMgr.GetConnection(connName);
                grdConnections.Rows.Add(connName, conn.Provider, conn.ConnectionString);
            }

            TreeNode root = mTreeView.Nodes[0];
            foreach (FdoClassCopyOptions task in def.ClassCopyOptions)
            {
                //Init w/ defaults
                CopyTaskNodeDecorator dec = AddNewTask(
                                               root,
                                               task.SourceConnectionName,
                                               task.SourceSchema,
                                               task.SourceClassName,
                                               task.TargetConnectionName,
                                               task.TargetSchema,
                                               task.TargetClassName,
                                               task.Name);

                _tasks[dec.DecoratedNode.Index] = dec;
                root.Expand();

                btnSave.Enabled = (root.Nodes.Count > 0);

                //Options
                dec.Options.BatchSize = task.BatchSize;
                dec.Options.Delete = task.DeleteTarget;
                dec.Options.SourceFilter = task.SourceFilter;

                //Property Mappings
                foreach (string srcProp in task.SourcePropertyNames)
                {
                    string dstProp = task.GetTargetProperty(srcProp);

                    dec.PropertyMappings.MapProperty(srcProp, dstProp);

                    FdoDataPropertyConversionRule rule = task.GetDataConversionRule(srcProp);
                    PropertyConversionNodeDecorator cd = dec.PropertyMappings.GetConversionRule(srcProp);
                    if (rule != null)
                    {
                        cd.NullOnFailedConversion = rule.NullOnFailure;
                        cd.Truncate = rule.Truncate;
                    }
                }

                //Expression Mappings
                foreach (string alias in task.SourceAliases)
                {
                    string expr = task.GetExpression(alias);
                    string dstProp = task.GetTargetPropertyForAlias(alias);

                    dec.ExpressionMappings.AddExpression(alias, expr);
                    dec.ExpressionMappings.MapExpression(alias, dstProp);

                    FdoDataPropertyConversionRule rule = task.GetDataConversionRule(alias);
                    PropertyConversionNodeDecorator cd = dec.ExpressionMappings.GetConversionRule(alias);

                    if (rule != null)
                    {
                        cd.NullOnFailedConversion = rule.NullOnFailure;
                        cd.Truncate = rule.Truncate;
                    }
                }
            }
        }
    }
    
    internal class ExpressionMappingInfo
    {
    	public string Expression;
    	public string TargetProperty;
    }
    
    [global::System.Serializable]
    public class MappingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public MappingException() { }
        public MappingException(string message) : base(message) { }
        public MappingException(string message, Exception inner) : base(message, inner) { }
        protected MappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
