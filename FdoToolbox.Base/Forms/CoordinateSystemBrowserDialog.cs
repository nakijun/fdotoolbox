using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.CoordinateSystems;

namespace FdoToolbox.Base.Forms
{
    public partial class CoordinateSystemBrowserDialog : Form, ICoordinateSystemBrowserView
    {
        private CoordinateSystemBrowserDialogPresenter _presenter;

        internal CoordinateSystemBrowserDialog()
        {
            InitializeComponent();
            _presenter = new CoordinateSystemBrowserDialogPresenter(this, ServiceManager.Instance.GetService<ICoordinateSystemCatalog>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            base.OnLoad(e);
        }

        private void grdContexts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _presenter.CoordinateSystemSelected();
        }

        public BindingList<CoordinateSystemDefinition> CoordinateSystems
        {
            set { grdContexts.DataSource = value; }
        }

        public CoordinateSystemDefinition SelectedCS
        {
            get 
            {
                CoordinateSystemDefinition cs = null;
                if (grdContexts.SelectedRows.Count == 1)
                    cs = grdContexts.SelectedRows[0].DataBoundItem as CoordinateSystemDefinition;
                else if (grdContexts.SelectedCells.Count == 1)
                    cs = grdContexts.Rows[grdContexts.SelectedCells[0].RowIndex].DataBoundItem as CoordinateSystemDefinition;
                return cs;
            }
        }

        public bool OkEnabled
        {
            set { btnOK.Enabled = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public static CoordinateSystemDefinition GetCoordinateSystem()
        {
            CoordinateSystemBrowserDialog diag = new CoordinateSystemBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.SelectedCS;
            }
            return null;
        }
    }
}