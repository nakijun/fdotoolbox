using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.Forms;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using System.IO;

namespace FdoToolbox.Core.Controls
{
    public partial class ExpressBulkCopyCtl : BaseDocumentCtl
    {
        internal ExpressBulkCopyCtl()
        {
            InitializeComponent();
            this.Title = "Express Bulk Copy";
            openFileDialog.InitialDirectory = HostApplication.Instance.AppPath;
            saveFileDialog.InitialDirectory = HostApplication.Instance.AppPath;
        }

        public ExpressBulkCopyCtl(ExpressProvider source, ExpressProvider target)
            : this()
        {
            this.SourceProvider = source;
            this.TargetProvider = target;
        }

        private ExpressProvider _SourceProvider;

        public ExpressProvider SourceProvider
        {
            get { return _SourceProvider; }
            set 
            { 
                _SourceProvider = value;
                grpSource.Text = "Source (" + value + ")";
            }
        }

        private ExpressProvider _TargetProvider;

        public ExpressProvider TargetProvider
        {
            get { return _TargetProvider; }
            set
            {
                _TargetProvider = value;
                grpTarget.Text = "Source (" + value + ")";
            }
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            switch (this.SourceProvider)
            {
                case ExpressProvider.SDF:
                    openFileDialog.Filter = "SDF files (*.sdf)|*.sdf";
                    break;
                case ExpressProvider.SHP:
                    openFileDialog.Filter = "SHP files (*.shp)|*.shp";
                    break;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtSource.Text = openFileDialog.FileName;
            }
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            switch (this.TargetProvider)
            {
                case ExpressProvider.SDF:
                    saveFileDialog.Filter = "SDF files (*.sdf)|*.sdf";
                    break;
                case ExpressProvider.SHP:
                    saveFileDialog.Filter = "SHP files (*.shp)|*.shp";
                    break;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtTarget.Text = saveFileDialog.FileName;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            bool valid = true;
            if (string.IsNullOrEmpty(txtSource.Text))
            {
                errorProvider.SetError(txtSource, "Required");
                valid = false;
            }
            if (string.IsNullOrEmpty(txtTarget.Text))
            {
                errorProvider.SetError(txtTarget, "Required");
                valid = false;
            }
            if (!valid)
                return;

            string source = txtSource.Text;
            string target = txtTarget.Text;

            BulkCopyOptions options = null;
            try
            {
                options = new BulkCopyOptions(this.SourceProvider, this.TargetProvider, source, target);
                options.CopySpatialContexts = chkCopySpatialContexts.Checked;

                BulkCopyTask task = new BulkCopyTask("EXPR_BCP", options);
                TaskProgressDlg diag = new TaskProgressDlg(task);
                diag.Run();
            }
            catch (BulkCopyException ex)
            {
                AppConsole.Alert("Error", ex.Message);
                AppConsole.WriteException(ex);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.Alert("Error", ex.Message);
                AppConsole.WriteException(ex);
            }
            finally
            {
                if (options != null)
                    options.Dispose();
            }
        }
    }

    public enum ExpressProvider
    {
        SDF,
        SHP
    }
}
