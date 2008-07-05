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

namespace FdoToolbox.Core
{
    /// <summary>
    /// FDO Toolbox Core Extension Module.
    /// 
    /// Core Functionality is implemented here.
    /// </summary>
    public class CoreModule : ModuleBase
    {
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
        }

        public override void Cleanup() { }

        [Command(CoreModule.CMD_ABOUT, "About", Description = "About this application")]
        public void About()
        {
            HostApplication.Instance.About();
        }

        [Command(CoreModule.CMD_QUIT, "Quit", Description = "Quit this application", ShortcutKeys = Keys.Alt | Keys.F4)]
        public void Quit()
        {
            HostApplication.Instance.Quit();
        }

        [Command(CoreModule.CMD_CONNECT, "Connect to Data", Description = "Creates a connection to an FDO-supported data source using the generic connection dialog")]
        public void DataConnect()
        {
            HostApplication.Instance.Shell.ShowDocumentWindow(new GenericConnectCtl());
        }

        [Command(CoreModule.CMD_EXTLOAD, "Load Extension", Description = "Loads a custom extension", ShortcutKeys = Keys.F2)]
        public void LoadExtension()
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Load Extension Module";
            diag.Filter = ".net Assembly (*.dll)|*.dll";
            diag.Multiselect = false;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    HostApplication.Instance.ModuleManager.LoadExtension(diag.FileName);
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

        [Command(CoreModule.CMD_CREATEDATASTORE, "Create Data Store", Description = "Creates a new FDO data store")]
        public void CreateDataStore()
        {
            HostApplication.Instance.Shell.ShowDocumentWindow(new GenericCreateDataStoreCtl());
        }

        [Command(CoreModule.CMD_CMDLIST, "List Commands", "List all available commands")]
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

        [Command(CoreModule.CMD_LISTPROVIDERS, "List Providers In Console", "Display all installed providers in the console")]
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
                    foreach (Provider prov in providers)
                    {
                        AppConsole.WriteLine("-> {0}", prov.Name);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_LOADSCHEMA, "Load Schema")]
        public void LoadSchema()
        {
            string name = string.Empty;
            IConnection conn = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedConnection(ref name);
            if (conn == null)
                AppConsole.WriteLine("Please select the active connection from the Object Explorer before invoking this command");

            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Load schemas from XML";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                FeatureSchemaCollection schemas = null;
                try
                {
                    using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                    {
                        schemas = cmd.Execute();
                        schemas.ReadXml(diag.FileName);
                        foreach (FeatureSchema schema in schemas)
                        {
                            using (IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                            {
                                apply.FeatureSchema = schema;
                                apply.Execute();
                            }
                        }
                        AppConsole.Alert("Load schemas", "Schemas loaded into connection " + name);
                        HostApplication.Instance.Shell.ObjectExplorer.RefreshConnection(name);
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    AppConsole.Alert("Error", ex.Message);
                }
            }
        }

        [Command(CoreModule.CMD_SAVESCHEMA, "Save Schema")]
        public void SaveSchema()
        {
            string name = string.Empty;
            IConnection conn = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedConnection(ref name);
            if (conn == null)
                AppConsole.WriteLine("Please select the active connection from the Object Explorer before invoking this command");

            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save schemas to XML";
            diag.Filter = "XML files (*.xml)|*.xml";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                using (IDescribeSchema cmd = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    using (FeatureSchemaCollection schemas = cmd.Execute())
                    {
                        schemas.WriteXml(diag.FileName);
                        AppConsole.Alert("Save schemas", "Schemas Saved to " + diag.FileName);
                    }
                }
            }
        }

        [Command(CoreModule.CMD_MANSCHEMA, "Manage Schemas")]
        public void ManageSchema()
        {
            string name = string.Empty;
            IConnection conn = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedConnection(ref name);
            
            //Must've been invoked from console
            if (conn == null)
                AppConsole.WriteLine("Please select the connection to manage from the Object Explorer before invoking this command");

            SchemaMgrCtl ctl = new SchemaMgrCtl(conn);
            ctl.OnSchemasApplied += delegate
            {
                HostApplication.Instance.Shell.ObjectExplorer.RefreshConnection(name);
            };
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
            
        }

        [Command(CoreModule.CMD_HELP, "Help", "Show the help documentation")]
        public void ShowHelp()
        {
            AppConsole.Alert("Help", "Help documentation is currently unavailable");
        }

        [Command(CoreModule.CMD_MODINFO, "Module Information", "Display information about a loaded module")]
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

        [Command(CoreModule.CMD_CREATEBCP, "Create Bulk Copy")]
        public void CreateBulkCopy()
        {
            if (HostApplication.Instance.ConnectionManager.GetConnectionNames().Count < 2)
            {
                AppConsole.Alert("Error", "Cannot create a bulk copy task. At least two open connections are required");
                return;
            }
            BulkCopyCtl ctl = new BulkCopyCtl();
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_EDITTASK, "Edit Task")]
        public void EditTask()
        {
            throw new NotImplementedException();
        }

        [Command(CoreModule.CMD_EXECUTETASK, "Execute Task")]
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

        [Command(CoreModule.CMD_DELETETASK, "Delete Task")]
        public void DeleteTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            HostApplication.Instance.TaskManager.RemoveTask(task.Name);
            AppConsole.WriteLine("Task Deleted: {0}", task.Name);
        }

        [Command(CoreModule.CMD_DATAPREVIEW, "Data Preview")]
        public void DataPreview()
        {
            string name = string.Empty;
            IConnection conn = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedConnection(ref name);
            if (conn == null)
            {
                AppConsole.WriteLine("Please select the connection from the Object Explorer before invoking this command");
                return;
            }
            BaseDocumentCtl ctl = new DataPreviewCtl(conn);
            HostApplication.Instance.Shell.ShowDocumentWindow(ctl);
        }

        [Command(CoreModule.CMD_LOADTASK, "Load Task")]
        public void LoadTask()
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Load Task";
            diag.Filter = "Task Definition (*.xml)|*.xml";
            diag.Multiselect = false;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                ITask task = TaskLoader.LoadTask(diag.FileName);
                if (task != null)
                {
                    HostApplication.Instance.TaskManager.AddTask(task);
                    AppConsole.WriteLine("Task loaded from {0}", diag.FileName);
                }
            }
        }

        [Command(CoreModule.CMD_SAVETASK, "Save Task")]
        public void SaveTask()
        {
            ITask task = HostApplication.Instance.Shell.ObjectExplorer.GetSelectedTask();
            if (task == null)
            {
                AppConsole.WriteLine("Please select the task from the Object Explorer before invoking this command");
                return;
            }
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save Task";
            diag.Filter = "Task Definition (*.xml)|*.xml";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                TaskLoader.SaveTask(task, diag.FileName);
                AppConsole.WriteLine("Task saved to {0}", diag.FileName);
            }
        }
    }
}
