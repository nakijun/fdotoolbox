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
using System.Windows.Forms;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Common.Io;
using System.IO;
using OSGeo.FDO.Commands;
using System.Collections.Specialized;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.IO;

namespace FdoToolbox.Core.Modules
{
    /// <summary>
    /// FDO Toolbox Core Extension Module.
    /// 
    /// Core Functionality is implemented here.
    /// </summary>
    public class CoreModule : ModuleBase, ICommandVerifier
    {
        #region Connection-bound tab types
        
        public const string TAB_DATA_PREVIEW = "TAB_DATA_PREVIEW";
        public const string TAB_SPATIAL_CONTEXT_MGMT = "TAB_SPATIAL_CONTEXT_MGMT";
        public const string TAB_SCHEMA_MGMT = "TAB_SCHEMA_MGMT";
        public const string TAB_DATASTORE_MGMT = "TAB_DATASTORE_MGMT";
        
        #endregion

        #region Command Names

        public const string CMD_QUIT = "quit";
        public const string CMD_GC = "gc";
        public const string CMD_ABOUT = "about";
        public const string CMD_EXTLOAD = "extload";
        public const string CMD_CONNECT = "connect";
        public const string CMD_CREATEDATASTORE = "mkdstore";
        public const string CMD_CREATEBCP = "createbcp";
        public const string CMD_CMDLIST = "cmdlist";
        public const string CMD_REGPROVIDER = "regprovider";
        public const string CMD_UNREGPROVIDER = "unregprovider";
        public const string CMD_CLEAR = "clear";
        public const string CMD_MANSCHEMA = "manschema";
        public const string CMD_SAVESCHEMA = "saveschema";
        public const string CMD_LOADSCHEMA = "loadschema";
        public const string CMD_LISTPROVIDERS = "listproviders";
        public const string CMD_MODINFO = "modinfo";
        public const string CMD_DELETETASK = "deletetask";
        public const string CMD_EXECUTETASK = "exectask";
        public const string CMD_EDITTASK = "edittask";
        public const string CMD_DATAPREVIEW = "datapreview";
        public const string CMD_HELP = "help";
        public const string CMD_LOADTASK = "loadtask";
        public const string CMD_SAVETASK = "savetask";
        public const string CMD_SAVECONN = "saveconn";
        public const string CMD_LOADCONN = "loadconn";
        public const string CMD_REMOVECONN = "removeconn";
        public const string CMD_MANAGESPATIALCONTEXTS = "mansc";
        public const string CMD_RENAMECONNECTION = "renameconn";
        public const string CMD_REFRESHCONN = "refreshconn";
        public const string CMD_SHOWOBJECTEXPLORER = "showobjectexplorer";
        public const string CMD_SHOWCONSOLE = "showconsole";
        public const string CMD_MANAGEDATASTORES = "mandstore";
        public const string CMD_CSMANAGER = "csmanager";
        public const string CMD_EDITSCHEMA_ATTRIBUTES = "editschemaattributes";
        public const string CMD_EDITCLASS_ATTRIBUTES = "editclassattributes";
        public const string CMD_SAVE_SCHEMA_XML = "saveschemaxml";
        public const string CMD_SAVE_SCHEMA_SDF = "saveschemasdf";
        public const string CMD_DELETE_SCHEMA = "delschema";
        public const string CMD_PREVIEWCLASS = "previewclass";
        public const string CMD_CREATEJOIN = "createdbjoin";
        public const string CMD_PREFERENCES = "preferences";
        #endregion

        public override string Name
        {
            get { return "core"; }
        }

        public override string Description
        {
            get { return "FDO Toolbox Core Module"; }
        }

        public override void Initialize() 
        {
            HostApplication.Instance.OnAppInitialized += delegate
            {
                AppConsole.WriteLine("Type \"{0}\" for a list of all available commands", CMD_CMDLIST);
            };

            HostApplication.Instance.TabManager.RegisterTabType(typeof(SpatialDataPreviewCtl));
            HostApplication.Instance.TabManager.RegisterTabType(typeof(DataStoreMgrCtl));
            HostApplication.Instance.TabManager.RegisterTabType(typeof(SchemaMgrCtl));
            HostApplication.Instance.TabManager.RegisterTabType(typeof(SpatialContextCtl));
        }

        public override void Cleanup() { }

        /// <summary>
        /// Helper method to add a connection by the given name, if that name
        /// already exists, the user will be asked to enter a new name, and
        /// will be repeatedly asked to do this until they enter a unique name 
        /// (connection added)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="name"></param>
        /// <returns>The name of the added connection</returns>
        public static void AddConnection(IConnection conn, string name)
        {
            IConnection conn2 = HostApplication.Instance.SpatialConnectionManager.GetConnection(name);
            while (conn2 != null)
            {
                AppConsole.Alert("Error", "This connection name already exists. Please pick another");
                name = StringInputDlg.GetInput("Connection name", "Enter the name for this connection", name);
                conn2 = HostApplication.Instance.SpatialConnectionManager.GetConnection(name);
            }
            HostApplication.Instance.SpatialConnectionManager.AddConnection(name, conn);
        }

        [Command(CoreModule.CMD_ABOUT, "About", Description = "About this application", ImageResourceName = "information")]
        public void About()
        {
            HostApplication.Instance.About();
        }

        [Command(CoreModule.CMD_QUIT, "Quit", Description = "Quit this application", ShortcutKeys = Keys.Alt | Keys.F4)]
        public void Quit()
        {
            HostApplication.Instance.Quit();
        }

        [Command(CoreModule.CMD_CONNECT, "Connect to Data", Description = "Creates a connection to an FDO-supported data source using the generic connection dialog", ImageResourceName = "database_connect")]
        public void DataConnect()
        {
            HostApplication.Instance.Shell.ShowDocumentWindow(new GenericSpatialConnectCtl());
        }

        [Command(CoreModule.CMD_EXTLOAD, "Load Extension", Description = "Loads a custom extension", ImageResourceName = "plugin", ShortcutKeys = Keys.F2)]
        public void LoadExtension()
        {
            string assemblyFile = HostApplication.Instance.OpenFile("Load Extension Module", ".net Assembly (*.dll)|*.dll");
            if(File.Exists(assemblyFile))
            {
                try
                {
                    HostApplication.Instance.ModuleManager.LoadExtension(assemblyFile);
                }
                catch (ModuleLoadException ex)
                {
                    AppConsole.Alert("Error", ex.Message);
                }
            }
        }

        [Command(CoreModule.CMD_GC, "Invoke Garbage Collector", Description = "Invoke the Garbage Collector to free up unused memory")]
        public void InvokeGC()
        {
            System.GC.Collect();
        }

        [Command(CoreModule.CMD_CREATEDATASTORE, "Create Data Store", Description = "Creates a new FDO data store", ImageResourceName = "database")]
        public void CreateDataStore()
        {
            HostApplication.Instance.Shell.ShowDocumentWindow(new GenericCreateDataStoreCtl());
        }

        [Command(CoreModule.CMD_CMDLIST, "List Commands", "List all available commands", InvocationType = CommandInvocationType.Console)]
        public void CommandList()
        {
            ICollection<string> cmdNames = HostApplication.Instance.ModuleManager.GetCommandNames();
            AppConsole.WriteLine("Registered Commands:\n");
            foreach (string name in cmdNames)
            {
                AppConsole.Write("{0}  ", name);
            }
            AppConsole.WriteLine("");
        }

        [Command(CoreModule.CMD_CLEAR, "Clear Console", "Clears the Application Console")]
        public void ClearConsole()
        {
            HostApplication.Instance.Shell.ConsoleWindow.TextWindow.Clear();
        }

        [Command(CoreModule.CMD_REGPROVIDER, "Register Provider", "Add a new FDO Provider into the registry")]
        public void RegisterProvider()
        {
            HostApplication.Instance.Shell.ShowDocumentWindow(new RegProviderCtl());
        }

        [Command(CoreModule.CMD_UNREGPROVIDER, "Unregister Provider", "Removes an installed FDO Provider from the registry")]
        public void UnregisterProvider()
        {
            new UnregProviderDlg().ShowDialog();
        }

        [Command(CoreModule.CMD_LISTPROVIDERS, "List Providers In Console", "Display all installed providers in the console", InvocationType = CommandInvocationType.Console)]
        public void ListProviders()
        {
            using (ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders())
            {
                if (providers.Count == 0)
                {
                    AppConsole.WriteLine("There are no registered FDO providers");
                }
                else
                {
                    AppConsole.WriteLine("Registered FDO Providers:");
                    foreach (OSGeo.FDO.ClientServices.Provider prov in providers)
                    {
                        AppConsole.WriteLine("-> {0}", prov.Name);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_LOADSCHEMA, "Load Schema", ImageResourceName = "folder")]
        public void LoadSchema()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                string schemaFile = HostApplication.Instance.OpenFile("Load schemas from XML", "Feature Schema Definition (*.schema)|*.schema");
                if (File.Exists(schemaFile))
                {
                    try
                    {
                        service.LoadSchemasFromXml(schemaFile);
                        AppConsole.Alert("Load schemas", "Schemas loaded into connection " + connInfo.Name);
                        HostApplication.Instance.Shell.ObjectExplorer.RefreshSpatialConnection(connInfo.Name);
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                        AppConsole.WriteException(ex);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_SAVESCHEMA, "Save Schema", ImageResourceName = "disk", InvocationType = CommandInvocationType.UI)]
        public void SaveSchema()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                string schemaFile = HostApplication.Instance.SaveFile("Save schemas to XML", "Feature Schema Definition (*.schema)|*.schema");
                if (schemaFile != null)
                {
                    if (File.Exists(schemaFile))
                        File.Delete(schemaFile);

                    try
                    {
                        service.WriteSchemaToXml(schemaFile);
                        AppConsole.Alert("Save schemas", "Schemas Saved to " + schemaFile);
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                        AppConsole.WriteException(ex);
                    }       
                }
            }
        }

        [Command(CoreModule.CMD_MANSCHEMA, "Manage Schemas", ImageResourceName = "chart_organisation", InvocationType = CommandInvocationType.UI)]
        public void ManageSchema()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            if (connInfo != null)
            {
                ISpatialConnectionBoundCtl ctl = HostApplication.Instance.TabManager.CreateTab(typeof(SchemaMgrCtl), connInfo);

                SchemaMgrCtl schemaCtl = (SchemaMgrCtl)ctl.WrappedControl;
                schemaCtl.SetInitialSchema(schemaName);
                schemaCtl.OnSchemasApplied += delegate
                {
                    HostApplication.Instance.Shell.ObjectExplorer.RefreshSpatialConnection(connInfo.Name);
                };
                HostApplication.Instance.Shell.ShowDocumentWindow(schemaCtl);
            }
        }

        [Command(CoreModule.CMD_HELP, "Help", "Show the help documentation", ImageResourceName = "help")]
        public void ShowHelp()
        {
            AppConsole.Alert("Help", "Help documentation is currently unavailable");
        }

        [Command(CoreModule.CMD_MODINFO, "Module Information", "Display information about a loaded module", ImageResourceName = "information", InvocationType = CommandInvocationType.UI)]
        public void ModuleInfo()
        {
            IModule mod = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedModule();
            //invoked from console
            if (mod == null)
            {
                AppConsole.WriteLine("Please select the module from the Object Explorer before invoking this command");
                return;
            }

            BaseDocumentCtl ctl = new ModuleInfoCtl(mod);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_CREATEBCP, "Create Bulk Copy", Description = "Create a new Bulk Copy Task", ImageResourceName = "table_go")]
        public void CreateBulkCopy()
        {
            if (HostApplication.Instance.SpatialConnectionManager.GetConnectionNames().Count < 2)
            {
                AppConsole.Alert("Error", "Cannot create a bulk copy task. At least two open connections are required");
                return;
            }
            SpatialBulkCopyCtl ctl = new SpatialBulkCopyCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_EDITTASK, "Edit Task", InvocationType = CommandInvocationType.UI)]
        public void EditTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            switch (task.TaskType)
            {
                case TaskType.BulkCopy:
                    {
                        BaseDocumentCtl ctl = new SpatialBulkCopyCtl((SpatialBulkCopyTask)task);
                        HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
                    }
                    break;
                case TaskType.DbJoin:
                    {
                        SpatialJoinCtl ctl = new SpatialJoinCtl();
                        ctl.LoadSettings((SpatialJoinTask)task);
                        HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
                    }
                    break;
                default:
                    {
                        AppConsole.Alert("Error", "This task type is currently not editable");
                    }
                    break;
            }
        }

        [Command(CoreModule.CMD_EXECUTETASK, "Execute Task", ImageResourceName = "application_go", InvocationType = CommandInvocationType.UI)]
        public void ExecuteTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            AppConsole.WriteLine("Executing Task: {0}", task.Name);
            new TaskProgressDlg(task).Run();
        }

        [Command(CoreModule.CMD_DELETETASK, "Delete Task", ImageResourceName = "cross", InvocationType = CommandInvocationType.UI)]
        public void DeleteTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            HostApplication.Instance.TaskManager.RemoveTask(task.Name);
            
        }

        [Command(CoreModule.CMD_DATAPREVIEW, "Data Preview", ImageResourceName = "zoom", InvocationType = CommandInvocationType.UI)]
        public void DataPreview()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            ISpatialConnectionBoundCtl ctl = HostApplication.Instance.TabManager.CreateTab(typeof(SpatialDataPreviewCtl), connInfo);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl.WrappedControl);
        }

        [Command(CoreModule.CMD_LOADTASK, "Load Task", ImageResourceName = "folder")]
        public void LoadTask()
        {
            string taskDef = HostApplication.Instance.OpenFile("Load Task", "Task Definition (*.task)|*.task");
            if(File.Exists(taskDef))
            {
                ITask task = TaskLoader.LoadTask(taskDef, false);
                if (task != null)
                {
                    HostApplication.Instance.TaskManager.AddTask(task);
                    AppConsole.WriteLine("Task loaded from {0}", taskDef);
                }
            }
        }

        [Command(CoreModule.CMD_SAVETASK, "Save Task", ImageResourceName = "disk", InvocationType = CommandInvocationType.UI)]
        public void SaveTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            string taskDef = HostApplication.Instance.SaveFile("Save Task", "Task Definition (*.task)|*.task");
            if (taskDef != null)
            {
                if (File.Exists(taskDef))
                    File.Delete(taskDef);
                TaskLoader.SaveTask(task, taskDef);
                AppConsole.WriteLine("Task saved to {0}", taskDef);
            }
        }

        [Command(CoreModule.CMD_LOADCONN, "Load Connection", ImageResourceName = "folder")]
        public void LoadConnection()
        {
            string connDef = HostApplication.Instance.OpenFile("Load connection information", "Connection information (*.conn)|*.conn");
            if(File.Exists(connDef))
            {
                SpatialConnectionInfo connInfo = SpatialConnLoader.LoadConnection(connDef);
                IConnection conn = HostApplication.Instance.SpatialConnectionManager.GetConnection(connInfo.Name);
                if (conn != null)
                {
                    AppConsole.Write("A connection named {0} already exists. ", connInfo.Name);
                    connInfo.Name = HostApplication.Instance.SpatialConnectionManager.CreateUniqueName();
                    AppConsole.WriteLine("Attempting to load as {0} instead", connInfo.Name);
                }
                HostApplication.Instance.SpatialConnectionManager.AddConnection(connInfo.Name, connInfo.Connection);
                AppConsole.WriteLine("Connection loaded from {0}", connDef);
            }
        }

        [Command(CoreModule.CMD_SAVECONN, "Save Connection", ImageResourceName = "disk", InvocationType = CommandInvocationType.UI)]
        public void SaveConnection()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            string connDef = HostApplication.Instance.SaveFile("Save connection information", "Connection information (*.conn)|*.conn");
            if (connDef != null)
            {
                if (File.Exists(connDef))
                    File.Delete(connDef);
                SpatialConnLoader.SaveConnection(connInfo, connDef);
                AppConsole.WriteLine("Connection saved to {0}", connDef);
            }
        }

        [Command(CoreModule.CMD_REMOVECONN, "Remove Connection", ImageResourceName = "cross", InvocationType = CommandInvocationType.UI)]
        public void RemoveConnection()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            HostApplication.Instance.SpatialConnectionManager.RemoveConnection(connInfo.Name);
        }

        [Command(CoreModule.CMD_MANAGESPATIALCONTEXTS, "Manage Spatial Contexts", InvocationType = CommandInvocationType.UI)]
        public void SpatialContextInfo()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            ISpatialConnectionBoundCtl ctl = HostApplication.Instance.TabManager.CreateTab(typeof(SpatialContextCtl), connInfo);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl.WrappedControl);
        }

        [Command(CoreModule.CMD_RENAMECONNECTION, "Rename Connection", InvocationType = CommandInvocationType.UI)]
        public void RenameConnection()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            string oldName = connInfo.Name;
            string newName = StringInputDlg.GetInput("Rename Connection", "Enter the new name for the connection", oldName);
            if (!string.IsNullOrEmpty(newName) && oldName != newName)
            {
                string reason = string.Empty;
                if (HostApplication.Instance.SpatialConnectionManager.CanRenameConnection(oldName, newName, ref reason))
                    HostApplication.Instance.SpatialConnectionManager.RenameConnection(oldName, newName);
                else
                    AppConsole.Alert("Error", reason);
            }
        }

        [Command(CoreModule.CMD_REFRESHCONN, "Refresh Connection", Description = "Refresh the selected connection", InvocationType = CommandInvocationType.UI, ImageResourceName = "page_refresh")]
        public void RefreshConnection()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            HostApplication.Instance.Shell.ObjectExplorer.RefreshSpatialConnection(connInfo.Name);
        }

        [Command(CoreModule.CMD_SHOWOBJECTEXPLORER, "Show Object Explorer", ShortcutKeys = Keys.Control | Keys.D1, ImageResourceName = "application_side_boxes")]
        public void ShowObjectExplorer()
        {
            HostApplication.Instance.Shell.ObjectExplorer.UnHide();
        }

        [Command(CoreModule.CMD_SHOWCONSOLE, "Show Application Console", ShortcutKeys = Keys.Control | Keys.D2, ImageResourceName = "application_xp_terminal")]
        public void ShowConsole()
        {
            HostApplication.Instance.Shell.ConsoleWindow.UnHide();
        }

        [Command(CoreModule.CMD_MANAGEDATASTORES, "Manage Data Stores", InvocationType = CommandInvocationType.UI, ImageResourceName = "database_edit")]
        public void ManageDataStores()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            if (connInfo != null)
            {
                ISpatialConnectionBoundCtl ctl = HostApplication.Instance.TabManager.CreateTab(typeof(DataStoreMgrCtl), connInfo);
                HostApplication.Instance.Shell.ShowDocumentWindow(ctl.WrappedControl);
            }
        }

        [Command(CoreModule.CMD_CSMANAGER, "Coordinate System Catalog", Description = "Manage Coordinate Systems", ImageResourceName = "world")]
        public void ManageCoordinateSystems()
        {
            CoordSysManagerCtl ctl = new CoordSysManagerCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_EDITSCHEMA_ATTRIBUTES, "Edit Schema Attributes", Description = "Edit the attributes of this schema", InvocationType = CommandInvocationType.UI, ImageResourceName = "application_edit")]
        public void EditSchemaAttributes()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                FeatureSchema theSchema = service.GetSchemaByName(schemaName);
                if (theSchema != null)
                {
                    NameValueCollection nvc = DictionaryDialog.GetParameters("Edit Schema Attributes", theSchema.Attributes);
                    if (nvc != null)
                    {
                        foreach (string key in nvc.AllKeys)
                        {
                            theSchema.Attributes.SetAttributeValue(key, nvc[key]);
                        }
                        service.ApplySchema(theSchema);
                        AppConsole.WriteLine("Schema attributes saved");
                    }
                }
            }
        }

        [Command(CoreModule.CMD_EDITCLASS_ATTRIBUTES, "Edit Class Attributes", Description = "Edit the attributes of this class", InvocationType = CommandInvocationType.UI, ImageResourceName = "application_edit")]
        public void EditClassAttributes()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            string className = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedClass();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                ClassDefinition theClass = service.GetClassByName(schemaName, className);
                if (theClass != null)
                {
                    NameValueCollection nvc = DictionaryDialog.GetParameters("Edit Class Attributes", theClass.Attributes);
                    if (nvc != null)
                    {
                        foreach (string key in nvc.AllKeys)
                        {
                            theClass.Attributes.SetAttributeValue(key, nvc[key]);
                        }
                        service.ApplySchema(theClass.FeatureSchema);
                        AppConsole.WriteLine("Class attributes saved");
                    }
                }
            }
        }

        [Command(CoreModule.CMD_SAVE_SCHEMA_XML, "Save schema as XML", InvocationType = CommandInvocationType.UI, ImageResourceName = "page_white_code")]
        public void SaveSchemaAsXml()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                FeatureSchema schema = service.GetSchemaByName(schemaName);
                if (schema != null)
                {
                    string file = HostApplication.Instance.SaveFile("Save schema", "Feature Schema Definition (*.schema)|*.schema");
                    if (file != null)
                    {
                        if (File.Exists(file))
                            File.Delete(file);

                        service.WriteSchemaToXml(schemaName, file);
                        AppConsole.Alert("Save schema", "Schema saved to: " + file);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_SAVE_SCHEMA_SDF, "Save schema as SDF", InvocationType = CommandInvocationType.UI, ImageResourceName = "database")]
        public void SaveSchemaAsSdf()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                FeatureSchema schema = service.GetSchemaByName(schemaName);
                if (schema != null)
                {
                    string sdfFile = HostApplication.Instance.SaveFile("Save schema to SDF", "SDF files (*.sdf)|*.sdf");
                    if (sdfFile != null)
                    {
                        if (File.Exists(sdfFile))
                            File.Delete(sdfFile);

                        ExpressUtility.ApplySchemaToNewSDF(schema, sdfFile);
                        AppConsole.Alert("Save to SDF", "Schema applied to new SDF file: " + sdfFile);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_DELETE_SCHEMA, "Delete Schema", InvocationType = CommandInvocationType.UI, ImageResourceName = "cross")]
        public void DeleteSchema()
        { 
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            if (connInfo != null && !string.IsNullOrEmpty(schemaName))
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                service.DestroySchema(schemaName);
                AppConsole.Alert("Delete Schema", "Schema Deleted");
            }
        }

        [Command(CoreModule.CMD_PREVIEWCLASS, "Class Data Preview", InvocationType = CommandInvocationType.UI, ImageResourceName = "zoom")]
        public void PreviewClass()
        {
            SpatialConnectionInfo connInfo = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSpatialConnection();
            string schemaName = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedSchema();
            string className = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedClass();
            if (connInfo != null)
            {
                FeatureService service = HostApplication.Instance.SpatialConnectionManager.CreateService(connInfo.Name);
                ClassDefinition theClass = service.GetClassByName(schemaName, className);
                if (theClass != null)
                {
                    ISpatialConnectionBoundCtl ctl = HostApplication.Instance.TabManager.CreateTab(typeof(SpatialDataPreviewCtl), connInfo);
                    SpatialDataPreviewCtl dpreview = (SpatialDataPreviewCtl)ctl.WrappedControl;
                    dpreview.SetInitialClass(schemaName, className);
                    HostApplication.Instance.Shell.ShowDocumentWindow(dpreview);
                }
            }
        }

        [Command(CoreModule.CMD_CREATEJOIN, "Create Database Join", ImageResourceName = "table_relationship")]
        public void CreateDbJoin()
        {
            BaseDocumentCtl ctl = new SpatialJoinCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_PREFERENCES, "Preferences", ImageResourceName = "application_form_edit")]
        public void ShowPreferences()
        {
            //TODO: make single instance
            PreferencesCtl ctl = new PreferencesCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        public bool IsCommandExecutable(string cmdName, IConnection conn)
        {
            bool executable = true;
            switch (cmdName)
            {
                case CMD_LOADSCHEMA:
                    {
                        executable = (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_ApplySchema) >= 0);
                    }
                    break;
                case CMD_MANAGEDATASTORES:
                    {
                        executable = (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_ListDataStores) >= 0);
                    }
                    break;
                case CMD_DELETE_SCHEMA:
                    {
                        executable = (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_DestroySchema) >= 0);
                    }
                    break;
            }
            return executable;
        }
    }
}
