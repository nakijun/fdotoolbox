using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Forms
{
    public partial class FdoSpatialContextBrowserDlg : Form
    {
        private FdoSpatialContextBrowserDlg()
        {
            InitializeComponent();
            this.Text = ResourceService.GetString("TITLE_SPATIAL_CONTEXT_BROWSER");
            lblMessage.Text = ResourceService.GetString("MSG_SELECT_SPATIAL_CONTEXT");
        }

        public FdoSpatialContextBrowserDlg(FdoConnection conn)
            : this()
        {
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                grdSpatialContexts.DataSource = service.GetSpatialContexts();
            }
        }

        private void grdSpatialContexts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnOK.Enabled = grdSpatialContexts.Rows.Count > 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public SpatialContextInfo SelectedSpatialContext
        {
            get
            {
                if (grdSpatialContexts.SelectedRows.Count == 1)
                    return grdSpatialContexts.SelectedRows[0].DataBoundItem as SpatialContextInfo;
                else if (grdSpatialContexts.SelectedCells.Count == 1)
                    return grdSpatialContexts.Rows[grdSpatialContexts.SelectedCells[0].RowIndex].DataBoundItem as SpatialContextInfo;
                else
                    return null;
            }
        }

        public static SpatialContextInfo GetSpatialContext(FdoConnection conn)
        {
            FdoSpatialContextBrowserDlg diag = new FdoSpatialContextBrowserDlg(conn);
            if (diag.ShowDialog() == DialogResult.OK)
            {
                return diag.SelectedSpatialContext;
            }
            return null;
        }
    }
}