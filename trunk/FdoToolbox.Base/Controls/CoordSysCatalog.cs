using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Forms;

namespace FdoToolbox.Base.Controls
{
    public partial class CoordSysCatalog : UserControl, IViewContent, ICoordSysCatalogView
    {
        private CoordSysCatalogPresenter _presenter;

        public CoordSysCatalog()
        {
            InitializeComponent();
            _presenter = new CoordSysCatalogPresenter(this, ServiceManager.Instance.GetService<FdoToolbox.Base.Services.CoordSysCatalog>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_COORDSYS_CATALOG"); }
        }

        public event EventHandler TitleChanged;

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

        public CoordinateSystemDefinition SelectedCS
        {
            get
            {
                if (grdCs.SelectedRows.Count == 1)
                    return grdCs.SelectedRows[0].DataBoundItem as CoordinateSystemDefinition;
                else if (grdCs.SelectedCells.Count == 1)
                    return grdCs.Rows[grdCs.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystemDefinition;
                else
                    return null;
            }
        }

        public event EventHandler ViewContentClosing = delegate { };

        public void OnClose()
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = CoordinateSystemDialog.NewCoordinateSystem();
            if (cs != null)
            {
                _presenter.AddNew(cs);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                string oldName = cs.Name;
                if (CoordinateSystemDialog.EditCooridinateSystem(cs))
                {
                    _presenter.Update(oldName, cs);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CoordinateSystemDefinition cs = this.SelectedCS;
            if (cs != null)
            {
                _presenter.Delete(cs);
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            //_presenter.Filter(txtFilter.Text);
        }

        public BindingList<CoordinateSystemDefinition> CoordSysDefinitions
        {
            set 
            {
                grdCs.DataSource = value;
            }
        }

        public bool EditEnabled
        {
            set { btnEdit.Enabled = value; }
        }

        public bool DeleteEnabled
        {
            set { btnDelete.Enabled = value; }
        }

        private void grdCs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _presenter.CheckStatus();
        }
    }
}
