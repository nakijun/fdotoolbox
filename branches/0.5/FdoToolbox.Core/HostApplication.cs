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
using System.Xml;
using FdoToolbox.Core.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.IO;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Host Application class/controller. The gateway object to all the
    /// services provided by the application.
    /// </summary>
    public class HostApplication : BaseApplication
    {
        private IShell _shell;
        private IModuleMgr _moduleMgr;
        private ITaskManager _taskMgr;
        private ISpatialConnectionMgr _connMgr;
        private IDbConnectionManager _dbConnMgr;
        private ISpatialConnectionBoundTabManager _TabManager;
        private ICoordinateSystemCatalog _CsCatalog;
        private static HostApplication _Instance;

        private HostApplication() : base()
        {
            InitializeDialogs();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            _dbConnMgr = new DbConnectionManager();
            _connMgr = new SpatialConnectionMgr();
            _moduleMgr = new ModuleMgr();
            _taskMgr = new TaskManager();
            _MenuStateMgr = new MenuStateMgr();
        }

        /// <summary>
        /// Initialize the application
        /// </summary>
        /// <param name="shell">The top-level window</param>
        public void Initialize(IShell shell)
        {
            if (!_init)
            {
                try
                {
                    _shell = shell;
                    _shell.Title = this.Name;
                    _shell.ConsoleWindow.ConsoleInput += new ConsoleInputHandler(delegate(string input) { ExecuteCommand(input, true); });

                    _CsCatalog = new CoordSysCatalog();
                    _TabManager = new SpatialConnectionBoundTabManager();

                    //Set streams for Application Console
                    AppConsole.In = new TextConsoleInputStream(_shell.ConsoleWindow.InputTextBox);
                    AppConsole.Out = new TextConsoleOutputStream(_shell.ConsoleWindow.TextWindow);
                    AppConsole.Err = new TextConsoleOutputStream(_shell.ConsoleWindow.TextWindow);
                    AppConsole.Err.TextColor = System.Drawing.Color.Red;

                    AppConsole.DoConfirm += delegate(string title, string text)
                    {
                        return MessageBox.Show(text, title, MessageBoxButtons.YesNo) == DialogResult.Yes;
                    };

                    AppConsole.DoAlert += delegate(string title, string text)
                    {
                        MessageBox.Show(text, title);
                    };

                    InitMessageHandlers();

                    bool? timestamp = this.Preferences.GetBooleanPref(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE);
                    AppConsole.Out.TimestampEntries = timestamp.HasValue ? timestamp.Value : false;
                    AppConsole.Err.TimestampEntries = timestamp.HasValue ? timestamp.Value : false;
                    AppConsole.WriteLine("FDO Toolbox. Version {0}", this.Version);
                    AppConsole.WriteLine("Loading modules");

                    ModuleManager.LoadModule(new CoreModule());
                    ModuleManager.LoadModule(new ExpressModule());
                    ModuleManager.LoadModule(new AdoNetModule());
#if DEBUG
                    ModuleManager.LoadModule(new TestModule());
#endif
                    InitMenus();
                    LoadDefinedModules();
                    _init = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Initialization Error");
                    Application.Exit();
                }

                if (this.OnAppInitialized != null)
                    this.OnAppInitialized(this, EventArgs.Empty);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Cleanup();
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            OnApplicationException(e.Exception);
        }

        protected override void CheckFdoPath()
        {
            string fdoPath = this.Preferences.GetStringPref(PreferenceNames.PREF_STR_FDO_HOME);
            //Don't use OpenDirectory as dialogs haven't been initialized
            //at this point
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (!Directory.Exists(fdoPath))
            {
                while (!Directory.Exists(fdoPath))
                {
                    diag.ShowNewFolderButton = true;
                    diag.Description = "Select the path where the FDO libraries are located";
                    if (diag.ShowDialog() == DialogResult.OK)
                    {
                        fdoPath = diag.SelectedPath;
                        this.Preferences.SetStringPref(PreferenceNames.PREF_STR_FDO_HOME, fdoPath);
                    }
                }
            }
        }

        protected void OnApplicationException(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }

        private bool _init = false;

        private ToolStripMenuItem CreateSubMenu(ToolStripMenuItem menu, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                ToolStripItem item = null;
                if (node.Name == "SubMenu")
                {
                    item = new ToolStripMenuItem();
                    item.Name = item.Text = node.Attributes["name"].Value;
                    item = CreateSubMenu((ToolStripMenuItem)item, node.ChildNodes);
                    if (node.Attributes["resource"] != null)
                    {
                        object resource = Properties.Resources.ResourceManager.GetObject(node.Attributes["resource"].Value);
                        if (resource != null)
                            item.Image = (Image)resource;
                    }
                }
                else if(node.Name == "Command")
                {
                    string cmdName = node.Attributes["name"].Value;
                    Command cmd = ModuleManager.GetCommand(cmdName);
                   
                    if (cmd != null && (cmd.InvocationType != CommandInvocationType.Console))
                    {
                        ToolStripMenuItem tsi = new ToolStripMenuItem();
                        tsi.Name = cmd.Name;
                        tsi.Text = cmd.DisplayName;
                        tsi.ToolTipText = cmd.Description;
                        tsi.Image = cmd.CommandImage;
                        tsi.ShortcutKeys = cmd.ShortcutKeys;
                        tsi.Tag = cmd;
                        tsi.Click += delegate(object sender, EventArgs e)
                        {
                            cmd.Execute();
                        };
                        if (node.Attributes["displayName"] != null)
                            tsi.Text = node.Attributes["displayName"].Value;    
                        item = tsi;
                        MenuStateManager.RegisterMenuItem(cmd.Name, tsi);
                    }
                    else
                    {
                        AppConsole.Err.WriteLine("Unable to add menu entry for command: {0}", cmdName);
                    }
                }
                else if (node.Name == "Separator")
                {
                    item = new ToolStripSeparator();
                }
                if(item != null)
                    menu.DropDown.Items.Add(item);
            }
            return menu;
        }

        private void InitMenus()
        {
            AppConsole.WriteLine("Initializing Object Explorer menus");
            string oefile = "OEMenuMap.xml";
            if (!File.Exists(Path.Combine(this.AppPath, oefile)))
            {
                AppConsole.WriteLine("{0} not found. Restoring backup copy", oefile);
                File.WriteAllText(Path.Combine(this.AppPath, oefile), Properties.Resources.OEMenuMap);
                AppConsole.WriteLine("{0} restored", oefile);
            }
            Shell.ObjectExplorer.InitializeMenus(oefile);
            AppConsole.WriteLine("Object Explorer menus initialized");
            
            AppConsole.WriteLine("Initializing main menu");
            string file = "MenuMap.xml";
            if (!File.Exists(Path.Combine(this.AppPath, file)))
            {
                AppConsole.WriteLine("{0} not found. Restoring backup copy", file);
                File.WriteAllText(Path.Combine(this.AppPath, file), Properties.Resources.MenuMap);
                AppConsole.WriteLine("{0} restored", file);
            }
            //Parse Menu Map
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNode root = doc.SelectNodes("//MenuMap")[0];
            foreach (XmlNode node in root.ChildNodes)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem();
                menu.Name = menu.Text = node.Attributes["name"].Value;
                menu = CreateSubMenu(menu, node.ChildNodes);
                Shell.MainMenu.Items.Add(menu);
            }

            AppConsole.WriteLine("Main Menu initialized");
        }

        public void ExtendUI(string uiExtensionFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(uiExtensionFile);

            XmlNode shellExt = doc.SelectSingleNode("//UIExtension/Shell");
            if (shellExt != null)
            {
                foreach (XmlNode menuNode in shellExt.ChildNodes)
                {
                    if (menuNode.Name != "Menu")
                        AppConsole.Err.WriteLine("Unknown element {0} in {1}. Skipping", menuNode.Name, uiExtensionFile);

                    string name = menuNode.Attributes["name"].Value;
                    if (Shell.MainMenu.Items.ContainsKey(name))
                    {
                        ToolStripMenuItem menu = Shell.MainMenu.Items[name] as ToolStripMenuItem;
                        menu = CreateSubMenu(menu, menuNode.ChildNodes);
                    }
                    else
                    {
                        ToolStripMenuItem menu = new ToolStripMenuItem();
                        menu.Text = menu.Name = menuNode.Attributes["name"].Value;
                        menu = CreateSubMenu(menu, menuNode.ChildNodes);
                        Shell.MainMenu.Items.Add(menu);
                    }
                }
            }
            Shell.ObjectExplorer.ExtendUI(uiExtensionFile);
        }

        protected override void Cleanup()
        {
            _OpenFileDialog.Dispose();
            _OpenFolderDialog.Dispose();
            _SaveFileDialog.Dispose();
            _SaveFolderDialog.Dispose();
            if (_moduleMgr is IDisposable)
                (_moduleMgr as IDisposable).Dispose();
            if (_connMgr is IDisposable)
                (_connMgr as IDisposable).Dispose();

            base.Cleanup();
        }

        public static HostApplication Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new HostApplication();
                return _Instance;
            }
        }

        /// <summary>
        /// Post-initialization event
        /// </summary>
        public event EventHandler OnAppInitialized;

        private void InitializeDialogs()
        {
            _OpenFileDialog = new OpenFileDialog();
            _OpenFolderDialog = new FolderBrowserDialog();
            _SaveFolderDialog = new FolderBrowserDialog();
            _SaveFileDialog = new SaveFileDialog();

            string initialPath = Preferences.GetStringPref(PreferenceNames.PREF_STR_WORKING_DIRECTORY);
            if (string.IsNullOrEmpty(initialPath))
                initialPath = HostApplication.Instance.AppPath;

            _OpenFolderDialog.SelectedPath = initialPath;
            _OpenFileDialog.InitialDirectory = initialPath;
            _SaveFileDialog.InitialDirectory = initialPath;
            _SaveFolderDialog.SelectedPath = initialPath;

            _OpenFileDialog.Multiselect = false;
        }

        private void LoadDefinedModules()
        {
            string fileName = "Modules.xml";
            if (File.Exists(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                XmlNodeList modList = doc.SelectNodes("//ModuleList/Module");
                if (modList.Count > 0)
                {
                    foreach (XmlNode modNode in modList)
                    {
                        string assembly = modNode.InnerText;
                        if (File.Exists(assembly))
                        {
                            AppConsole.WriteLine("Loading assembly: {0}", assembly);
                            ModuleManager.LoadExtension(assembly);
                        }
                        else
                        {
                            AppConsole.Err.WriteLine("Assembly not found: {0}. Skipping", assembly);
                        }
                    }
                }
                else
                {
                    AppConsole.Err.WriteLine("No assemblies defined in {0}. Skipping", fileName);
                }
            }
            else
            {
                AppConsole.Err.WriteLine("Configuration file {0} not found. Skipping auto-loading", fileName);
            }
        }

        private void InitMessageHandlers()
        {
            ModuleManager.ModuleLoaded += delegate(IModule module)
            {
                AppConsole.WriteLine("Module loaded: {0}", module.Name);
            };
            TaskManager.TaskRemoved += delegate(string name)
            {
                AppConsole.WriteLine("Task Deleted: {0}", name);
            };
            SpatialConnectionManager.ConnectionRemoved += delegate(string name)
            {
                AppConsole.WriteLine("Connection removed: {0}", name);
            };
            SpatialConnectionManager.ConnectionAdded += delegate(string name)
            {
                AppConsole.WriteLine("New connection added: {0}", name);
            };
            SpatialConnectionManager.ConnectionRenamed += delegate(string oldName, string newName)
            {
                AppConsole.WriteLine("Connection {0} renamed to {1}", oldName, newName);
            };
        }

        /// <summary>
        /// Execute a command in the global namespace
        /// </summary>
        /// <param name="cmdName"></param>
        public void ExecuteCommand(string cmdName, bool fromConsole)
        {
            Command cmd = this.ModuleManager.GetCommand(cmdName);
            if (cmd == null)
            {
                AppConsole.Err.WriteLine("Command not found: {0}", cmdName);
            }
            else
            {
                try
                {
                    if (fromConsole)
                    {
                        //Must not be CommandInvocationType.UI
                        if (cmd.InvocationType != CommandInvocationType.UI)
                        {
                            cmd.Execute();
                        }
                        else
                        {
                            AppConsole.Err.WriteLine("Command cannot be invoked in this mode");
                        }
                    }
                    else
                    {
                        //Must not be CommandInvocationType.Console
                        if (cmd.InvocationType != CommandInvocationType.Console)
                        {
                            cmd.Execute();
                        }
                        else
                        {
                            AppConsole.Err.WriteLine("Command cannot be invoked in this mode");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppConsole.WriteException(ex);
                }
            }
        }

        private MenuStateMgr _MenuStateMgr;

        public IMenuStateMgr MenuStateManager
        {
            get { return _MenuStateMgr; }
        }
        
        /// <summary>
        /// The top-level application window (shell)
        /// </summary>
        public IShell Shell
        {
            get { return _shell; }
        }

        /// <summary>
        /// The module manager
        /// </summary>
        public IModuleMgr ModuleManager
        {
            get { return _moduleMgr; }
        }

        /// <summary>
        /// The task manager
        /// </summary>
        public ITaskManager TaskManager
        {
            get { return _taskMgr; }
        }

        /// <summary>
        /// The spatial connection manager
        /// </summary>
        public ISpatialConnectionMgr SpatialConnectionManager
        {
            get { return _connMgr; }
        }

        /// <summary>
        /// The database connection manager
        /// </summary>
        public IDbConnectionManager DatabaseConnectionManager
        {
            get { return _dbConnMgr; }
        }

        /// <summary>
        /// The connection-bound tab manager
        /// </summary>
        public ISpatialConnectionBoundTabManager TabManager
        {
            get { return _TabManager; }
        }

        /// <summary>
        /// Start the application. Must call Initialize() first!
        /// </summary>
        public void Run()
        {
            Application.Run(_shell.FormObj);
        }

        /// <summary>
        /// Quit the application
        /// </summary>
        public void Quit()
        {
            //Cleanup();
            Application.Exit();
        }

        /// <summary>
        /// Application name
        /// </summary>
        public string Name 
        {
            get { return "FDO Toolbox"; } 
        }

        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(); } }

        /// <summary>
        /// Project home page
        /// </summary>
        public string ProjectUrl { get { return "http://code.google.com/p/fdotoolbox"; } }

        /// <summary>
        /// The coordinate system catalog
        /// </summary>
        public ICoordinateSystemCatalog CoordinateSystemCatalog
        {
            get { return _CsCatalog; }
        }
        
        /// <summary>
        /// Displays the about box for this application
        /// </summary>
        public void About()
        {
            new AboutDialog().ShowDialog();
        }

        private OpenFileDialog _OpenFileDialog;
        private SaveFileDialog _SaveFileDialog;
        private FolderBrowserDialog _OpenFolderDialog;
        private FolderBrowserDialog _SaveFolderDialog;

        public string OpenFile(string title, string filter)
        {
            _OpenFileDialog.Title = title;
            _OpenFileDialog.Filter = filter;
            if (_OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                return _OpenFileDialog.FileName;
            }
            return null;
        }

        public string SaveFile(string title, string filter)
        {
            _SaveFileDialog.Title = title;
            _SaveFileDialog.Filter = filter;
            if (_SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return _SaveFileDialog.FileName;
            }
            return null;
        }

        public string OpenDirectory(string prompt)
        {
            _OpenFolderDialog.Description = prompt;
            if (_OpenFolderDialog.ShowDialog() == DialogResult.OK)
            {
                return _OpenFolderDialog.SelectedPath;
            }
            return null;
        }

        public string SaveDirectory(string prompt)
        {
            _SaveFolderDialog.Description = prompt;
            if (_SaveFolderDialog.ShowDialog() == DialogResult.OK)
            {
                return _SaveFolderDialog.SelectedPath;
            }
            return null;
        }
    }
}
