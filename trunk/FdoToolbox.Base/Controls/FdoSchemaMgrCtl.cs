using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using OSGeo.FDO.Schema;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoSchemaMgrCtl : UserControl, IViewContent, IFdoSchemaMgrView
    {
        private FdoSchemaMgrPresenter _presenter;
        private FdoStandaloneSchemaEditorPresenter _standalonePresenter;

        internal FdoSchemaMgrCtl()
        {
            InitializeComponent();
        }

        internal FdoSchemaMgrCtl(bool standalone) : this()
        {
            this.Standalone = standalone;
            if(standalone)
                _standalonePresenter = new FdoStandaloneSchemaEditorPresenter(this); 
        }

        public FdoSchemaMgrCtl(FdoConnection conn) : this(false)
        {
            _presenter = new FdoSchemaMgrPresenter(this, conn);
        }

        private BindingSource _bsSchemas = new BindingSource();
        private BindingSource _bsClasses = new BindingSource();

        protected override void OnLoad(EventArgs e)
        {
            if (!Standalone)
                _presenter.GetSchemas();
            else
                _standalonePresenter.Init();
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            { 
                _Title = value; 
                TitleChanged(this, EventArgs.Empty); 
            }
        }

        public event EventHandler TitleChanged = delegate {};

        public bool CanClose
        {
            get { return true; }
        }

        public bool Close()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }

        public bool SaveAs()
        {
            return true;
        }

        public event EventHandler ViewContentClosing = delegate { };


        public bool ApplyEnabled
        {
            set { btnApply.Enabled = value; }
        }

        private bool _Standalone;

        public bool Standalone
        {
            get { return _Standalone; }
            set { _Standalone = value; }
        }
	

        public FeatureSchemaCollection SchemaList
        {
            get
            {
                return (_bsSchemas.DataSource as FeatureSchemaCollection);
            }
            set
            {
                lstSchemas.DisplayMember = "Name";
                _bsSchemas.DataSource = value;
                lstSchemas.DataSource = _bsSchemas;
            }
        }

        public object ClassList
        {
            set 
            {
                lstClasses.DisplayMember = "Name";
                _bsClasses.DataSource = value;
                lstClasses.DataSource = _bsClasses;
            }
        }

        public OSGeo.FDO.Schema.FeatureSchema SelectedSchema
        {
            get { return lstSchemas.SelectedItem as OSGeo.FDO.Schema.FeatureSchema; }
        }

        public OSGeo.FDO.Schema.ClassDefinition SelectedClass
        {
            get { return lstClasses.SelectedItem as OSGeo.FDO.Schema.ClassDefinition; }
        }

        public int SchemaCount
        {
            get { return lstSchemas.Items.Count; }
        }

        public int ClassCount
        {
            get { return lstClasses.Items.Count; }
        }

        public bool AddSchemaEnabled
        {
            set { btnAddSchema.Enabled = value; }
        }

        public bool DeleteSchemaEnabled
        {
            set { btnDeleteSchema.Enabled = value; }
        }

        public bool EditClassEnabled
        {
            set { btnEditClass.Enabled = value; }
        }

        public bool DeleteClassEnabled
        {
            set { btnDeleteClass.Enabled = value; }
        }

        public bool AddClassEnabled
        {
            set { classNonFeatureToolStripMenuItem.Enabled = value; }
        }

        public bool AddFeatureClassEnabled
        {
            set { featureClassToolStripMenuItem.Enabled = value; }
        }

        private void btnAddSchema_Click(object sender, EventArgs e)
        {
            FeatureSchema schema = SchemaInfoDialog.NewSchema();
            if (schema != null)
            {
                if (this.Standalone)
                    _standalonePresenter.AddSchema(schema);
                else
                    _presenter.AddSchema(schema);
            }
        }

        private void btnDeleteSchema_Click(object sender, EventArgs e)
        {
            if (this.Standalone)
                _standalonePresenter.DeleteSchema();
            else
                _presenter.DeleteSchema();
        }

        private void featureClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //_presenter.AddClass(OSGeo.FDO.Schema.ClassType.ClassType_FeatureClass);
        }

        private void classNonFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //_presenter.AddClass(OSGeo.FDO.Schema.ClassType.ClassType_Class);
        }

        private void btnEditClass_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDeleteClass_Click(object sender, EventArgs e)
        {
            if (this.Standalone)
                _standalonePresenter.DeleteClass();
            else
                _presenter.DeleteClass();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (this.Standalone)
            {
            }
            else
            {
                //Check dirty state
                if (_presenter.CheckDirty())
                {
                    if (MessageService.AskQuestion(ResourceService.GetString("QUESTION_UNSAVED_CHANGES"), ResourceService.GetString("TITLE_SCHEMA_MANAGEMENT")))
                    {
                        if (_presenter.ApplyChanges())
                        {
                            MessageService.ShowMessage(ResourceService.GetString("MSG_CHANGES_APPLIED"), ResourceService.GetString("TITLE_SCHEMA_MANAGEMENT"));
                        }
                        else
                        {
                            MessageService.ShowError(ResourceService.GetString("ERR_UNABLE_TO_SAVE_SCHEMAS"));
                        }
                    }
                }
                ViewContentClosing(this, EventArgs.Empty);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (this.Standalone)
            {

            }
            else
            {
                if (_presenter.ApplyChanges())
                {
                    MessageService.ShowMessage(ResourceService.GetString("MSG_CHANGES_APPLIED"), ResourceService.GetString("TITLE_SCHEMA_MANAGEMENT"));
                    this.ApplyEnabled = false;
                }
            }
        }


        public void AddSchema(FeatureSchema schema)
        {
            _bsSchemas.Add(schema);   
        }


        public void RemoveSchema(FeatureSchema schema)
        {
            _bsSchemas.Remove(schema);
        }

        public void AddClass(ClassDefinition classDef)
        {
            
        }
    }
}
