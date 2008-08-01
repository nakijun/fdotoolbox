#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
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
            lblWarning.Visible = (this.TargetProvider == ExpressProvider.SHP);
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

            SpatialBulkCopyOptions options = null;
            try
            {
                options = new SpatialBulkCopyOptions(this.SourceProvider, this.TargetProvider, source, target);
                options.CopySpatialContexts = chkCopySpatialContexts.Checked;

                SpatialBulkCopyTask task = new SpatialBulkCopyTask("EXPR_BCP", options);
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
