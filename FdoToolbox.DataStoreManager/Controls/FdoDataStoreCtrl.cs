#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using FdoToolbox.DataStoreManager.Controls.SchemaDesigner;
using FdoToolbox.Base.Services;

namespace FdoToolbox.DataStoreManager.Controls
{
    public partial class FdoDataStoreCtrl : ViewContent
    {
        public FdoDataStoreCtrl()
        {
            InitializeComponent();
            this.Title = ResourceService.GetString("TITLE_DATA_STORE");
        }

        private SchemaDesignContext _context;

        private FdoConnection _conn;

        public FdoDataStoreCtrl(FdoConnection conn) : this()
        {
            _conn = conn;
        }

        protected override void OnLoad(EventArgs e)
        {
            _context = new SchemaDesignContext(_conn);

            if (!_context.IsConnected)
                this.Title = ResourceService.GetString("TITLE_DATA_STORE_STANDALONE");

            btnApply.Enabled = btnReload.Enabled = _context.IsConnected;

            schemaView.Context = _context;
            spatialContextView.Context = _context;
            base.OnLoad(e);
        }

        private void btnSaveXmlConfig_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile("Save XML Configuration", "XML Files (*.xml)|*.xml");
            if (!string.IsNullOrEmpty(file))
            {
                var conf = _context.GetConfiguration();
                conf.Save(file);
                MessageService.ShowMessageFormatted("Configuration saved to {0}", file);
                LoggingService.Info("Configuration Saved");
            }
        }

        private void btnSaveSdf_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveSqlite_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveEverything_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveSpatialContexts_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveAllSchemas_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveSelectedSchema_Click(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile("Open XML Configuration", "XML files (*.xml)|*.xml");
            if (!string.IsNullOrEmpty(file))
            {
                var conf = FdoDataStoreConfiguration.FromFile(file);
                _context.SetConfiguration(conf);
                schemaView.Reset();
            }
        }
    }
}
