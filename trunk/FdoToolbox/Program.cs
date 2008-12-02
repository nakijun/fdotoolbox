using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using System.Reflection;
using FdoToolbox.Base;
using System.IO;
using System.Resources;
using FdoToolbox.Base.Services;
using FdoToolbox.Core;

namespace FdoToolbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            // The LoggingService is a small wrapper around log4net.
            // Our application contains a .config file telling log4net to write
            // to System.Diagnostics.Trace.
            LoggingService.Info("Application start");

            // Get a reference to the entry assembly (Startup.exe)
            Assembly exe = typeof(Program).Assembly;

            // Set the root path of our application. ICSharpCode.Core looks for some other
            // paths relative to the application root:
            // "data/resources" for language resources, "data/options" for default options
            FileUtility.ApplicationRootPath = Path.GetDirectoryName(exe.Location);

            LoggingService.Info("Starting core services...");

            // CoreStartup is a helper class making starting the Core easier.
            // The parameter is used as the application name, e.g. for the default title of
            // MessageService.ShowMessage() calls.
            CoreStartup coreStartup = new CoreStartup("FDO Toolbox");
            // It is also used as default storage location for the application settings:
            // "%Application Data%\%Application Name%", but you can override that by setting c.ConfigDirectory

            // Specify the name of the application settings file (.xml is automatically appended)
            coreStartup.PropertiesName = "AppProperties";

            // Initializes the Core services (ResourceService, PropertyService, etc.)
            coreStartup.StartCoreServices();

            LoggingService.Info("Looking for AddIns...");
            // Searches for ".addin" files in the application directory.
            coreStartup.AddAddInsFromDirectory(Path.Combine(FileUtility.ApplicationRootPath, "AddIns"));

            // Searches for a "AddIns.xml" in the user profile that specifies the names of the
            // add-ins that were deactivated by the user, and adds "external" AddIns.
            coreStartup.ConfigureExternalAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddIns.xml"));

            // Searches for add-ins installed by the user into his profile directory. This also
            // performs the job of installing, uninstalling or upgrading add-ins if the user
            // requested it the last time this application was running.
            coreStartup.ConfigureUserAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddInInstallTemp"),
                                            Path.Combine(PropertyService.ConfigDirectory, "AddIns"));

            LoggingService.Info("Checking FDO");
            // Set FDO path
            string fdoPath = Preferences.FdoPath;
            if (!FdoAssemblyResolver.IsValidFdoPath(fdoPath))
            {
                fdoPath = Path.Combine(FileUtility.ApplicationRootPath, "FDO");
                Preferences.FdoPath = fdoPath;
            }
            //FdoAssemblyResolver.SetFdoPath(fdoPath);
            FdoAssemblyResolver.InitializeFdo(fdoPath);
            LoggingService.Info("FDO path set to: " + fdoPath);

            LoggingService.Info("Loading AddInTree...");
            // Now finally initialize the application. This parses the ".addin" files and
            // creates the AddIn tree. It also automatically runs the commands in
            // "/Workspace/Autostart"
            coreStartup.RunInitialization();

            LoggingService.Info("Initializing Workbench...");
            // Workbench is our class from the base project, this method creates an instance
            // of the main form.
            log4net.Config.XmlConfigurator.Configure();
            Workbench.InitializeWorkbench();

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            
            try
            {
                LoggingService.Info("Running application...");
                // Workbench.Instance is the instance of the main form, run the message loop.
                Application.Run(Workbench.Instance);
            }
            finally
            {
                try
                {
                    // Save changed properties
                    PropertyService.Save();
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex, "Error storing properties");
                }
            }

            LoggingService.Info("Application shutdown");
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageService.ShowError(e.Exception);
        }
    }
}