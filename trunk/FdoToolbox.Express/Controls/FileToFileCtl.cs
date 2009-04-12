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
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Utility;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Express.Controls
{
    public partial class FileToFileCtl : ViewContent
    {
        public FileToFileCtl()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return ResourceService.GetString("TITLE_EXPRESS_BULK_COPY"); }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_EXPRESS_BCP"));
            if (file != null)
                txtSource.Text = file;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_FILE"), ResourceService.GetString("FILTER_EXPRESS_BCP"));
            if (file != null)
                txtTarget.Text = file;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text;
            string target = txtTarget.Text;

            if (FileService.FileExists(source) && !string.IsNullOrEmpty(target))
            {
                using (FdoBulkCopy bcp = ExpressUtility.CreateBulkCopy(source, target, chkCopySpatialContexts.Checked, true))
                {
                    EtlProcessCtl ctl = new EtlProcessCtl(bcp);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
                    base.Close();
                }
            }
            else
            {
                this.ShowError("Source and Target fields are required");
            }
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            string dir = FileService.GetDirectory(ResourceService.GetString("TITLE_CHOOSE_DIRECTORY"));
            if (dir != null)
                txtTarget.Text = dir;
        }
    }
}
