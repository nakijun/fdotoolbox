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
using System.Text;
using OSGeo.FDO.Connections;
using FdoToolbox.Core;
using System.Windows.Forms;
using FdoToolbox.Core.Controls;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Forms;
using OSGeo.FDO.Commands.Schema;
using System.IO;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Extension module for express/specialized FDO functionality 
    /// that bypasses the normal generic approach, saving a few time-consuming 
    /// steps in the process.
    /// 
    /// Most of this functionality concerns the flat-file providers, mainly
    /// SDF and SHP.
    /// </summary>
    public class ExpressModule : ModuleBase
    {
        #region Command Names

        public const string CMD_SDFCONNECT = "sdfconnect";
        public const string CMD_SHPCONNECT = "shpconnect";
        public const string CMD_SHPDIRCONNECT = "shpdirconnect";
        public const string CMD_SDFCREATE = "sdfcreate";
        public const string CMD_SHPCREATE = "shpcreate";
        public const string CMD_SDF2SDF = "sdf2sdf";
        public const string CMD_SDF2SHP = "sdf2shp";
        public const string CMD_SHP2SDF = "shp2sdf";
        public const string CMD_SHP2SHP = "shp2shp";

        #endregion

        public override string Name
        {
            get { return "express"; }
        }

        public override string Description
        {
            get { return "FDO Express Utility Module"; }
        }

        public override void Initialize() { }

        public override void Cleanup() { }

        [Command(ExpressModule.CMD_SDFCONNECT, "Connect to SDF", Description = "Connect to a SDF data source")]
        public void SdfConnect()
        {
            IConnection conn = null;
            string sdfFile = HostApplication.Instance.OpenFile("Create SDF connection", "SDF Files (*.sdf)|*.sdf");
            if(File.Exists(sdfFile))
            {
                conn = ExpressUtility.CreateSDFConnection(sdfFile, false);
                ISpatialConnectionMgr mgr = HostApplication.Instance.SpatialConnectionManager;
                string name = mgr.CreateUniqueName();
                name = StringInputDlg.GetInput("Connection name", "Enter a name for this connection", name);
                CoreModule.AddConnection(conn, name);
            }
        }

        [Command(ExpressModule.CMD_SHPCONNECT, "Connect to SHP", Description = "Connect to a SHP data source")]
        public void ShpConnect()
        {
            IConnection conn = null;            
            string shpFile = HostApplication.Instance.OpenFile("Create SHP connection", "SHP Files (*.shp)|*.shp");
            if(File.Exists(shpFile))
            {
                conn = ExpressUtility.CreateSHPConnection(shpFile);
                ISpatialConnectionMgr mgr = HostApplication.Instance.SpatialConnectionManager;
                string name = mgr.CreateUniqueName();
                name = StringInputDlg.GetInput("Connection name", "Enter a name for this connection", name);
                CoreModule.AddConnection(conn, name);
            }
        }

        [Command(ExpressModule.CMD_SDFCREATE, "Create SDF", Description = "Creates a new SDF file")]
        public void CreateSDF()
        {
            string sdfFile = HostApplication.Instance.SaveFile("Create SDF", "SDF File (*.sdf)|*.sdf");
            if (sdfFile != null)
            {
                if (ExpressUtility.CreateSDF(sdfFile))
                {
                    if (AppConsole.Confirm("Create SDF", "SDF File created at " + sdfFile + ". Create a connection to it?"))
                    {
                        string name = HostApplication.Instance.SpatialConnectionManager.CreateUniqueName();
                        name = StringInputDlg.GetInput("Connection name", "Enter the name for this connection", name);
                        IConnection conn = ExpressUtility.CreateSDFConnection(sdfFile, false);
                        CoreModule.AddConnection(conn, name);
                    }
                }
                else
                {
                    AppConsole.Alert("Create SDF", "Failed to create SDF file", true);
                }
            }
        }

        [Command(ExpressModule.CMD_SHPCREATE, "Create SHP", Description = "Creates a new SHP file")]
        public void CreateSHP()
        {
            //There is no actual CreateDataStore support for SHP.
            //Instead we use the technique described here:
            //http://www.nabble.com/Writing-data-to-a-shapefile-example--td16942559.html
            //
            //Because of this, we have to force the user to *explicity*
            //create a Class Definition up-front as part of the process.

            string shpFile = HostApplication.Instance.SaveFile("Create SHP file", "SHP Files (*.shp)|*.shp");
            if(shpFile != null)
            {
                //If file exists, the user would've been asked to
                //overwrite, so we can safely delete it first.
                File.Delete(shpFile);

                string dir = Path.GetDirectoryName(shpFile);
                string className = Path.GetFileNameWithoutExtension(shpFile);
                string connStr = string.Format("DefaultFileLocation={0}", dir);

                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
                
                conn.ConnectionString = connStr;
                if (conn.Open() == ConnectionState.ConnectionState_Open)
                {
                    FeatureSchema schema = new FeatureSchema("Default", "Default SHP Schema");
                    FeatureClass classDef = new FeatureClass();
                    classDef.Name = className;
                    schema.Classes.Add(classDef);
                    //Use a dummy name for the connection
                    SpatialConnectionInfo connInfo = new SpatialConnectionInfo(conn.GetHashCode().ToString(), conn);
                    ClassDefCtl ctl = new ClassDefCtl(classDef, connInfo);
                    Form frm = FormFactory.CreateFormForControl(ctl);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        FeatureService service = new FeatureService(conn);
                        //Because we are connecting to a directory, this should create an empty
                        //SHP file for each feature class of the same name
                        service.ApplySchema(schema);
                        if (AppConsole.Confirm("Create SHP", "New SHP file created at: " + shpFile + ". Create a connection?"))
                        {
                            string name = HostApplication.Instance.SpatialConnectionManager.CreateUniqueName();
                            name = StringInputDlg.GetInput("Connection name", "Enter the name for this connection", name);
                            CoreModule.AddConnection(conn, name);
                            return; //Save this connection, don't dispose
                        }
                    }
                }
                conn.Dispose();
            }
        }

        
        [Command(ExpressModule.CMD_SHPDIRCONNECT, "Connect to SHP (directory)", "Connect to a directory of SHP files")]
        public void ShpDirConnect()
        {
            IConnection conn = null;
            FolderBrowserDialog diag = new FolderBrowserDialog();
            diag.SelectedPath = HostApplication.Instance.AppPath;
            diag.Description = "Select the directory that contains the SHP files";
            diag.ShowNewFolderButton = false;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                conn = ExpressUtility.CreateSHPConnection(diag.SelectedPath);
                ISpatialConnectionMgr mgr = HostApplication.Instance.SpatialConnectionManager;
                string name = mgr.CreateUniqueName();
                name = StringInputDlg.GetInput("Connection name", "Enter a name for this connection", name);
                CoreModule.AddConnection(conn, name);
            }
        }

        [Command(ExpressModule.CMD_SDF2SDF, "SDF to SDF", "Copy feature data from an SDF data source to another SDF data source")]
        public void SdfToSdf()
        {
            ExpressSpatialBulkCopyCtl ctl = new ExpressSpatialBulkCopyCtl(ExpressProvider.SDF, ExpressProvider.SDF);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(ExpressModule.CMD_SHP2SHP, "SHP to SHP", "Copy feature data from an SHP data source to another SHP data source")]
        public void ShpToShp()
        {
            ExpressSpatialBulkCopyCtl ctl = new ExpressSpatialBulkCopyCtl(ExpressProvider.SHP, ExpressProvider.SHP);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(ExpressModule.CMD_SHP2SDF, "SHP to SDF", "Copy feature data from an SHP data source to an SDF data source")]
        public void ShpToSdf()
        {
            ExpressSpatialBulkCopyCtl ctl = new ExpressSpatialBulkCopyCtl(ExpressProvider.SHP, ExpressProvider.SDF);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(ExpressModule.CMD_SDF2SHP, "SDF to SHP", "Copy feature data from an SDF data source to an SHP data source")]
        public void SdfToShp()
        {
            ExpressSpatialBulkCopyCtl ctl = new ExpressSpatialBulkCopyCtl(ExpressProvider.SDF, ExpressProvider.SHP);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }
    }
}
