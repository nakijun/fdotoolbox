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

namespace FdoToolbox.Core
{
    /// <summary>
    /// Host Application class/controller. The gateway object to all the
    /// services provided by the application.
    /// </summary>
    public class HostApplication
    {
        private IShell _shell;
        private IModuleMgr _moduleMgr;
        private ITaskManager _taskMgr;
        private IConnectionMgr _connMgr;
        private static HostApplication _Instance;

        private HostApplication()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            _moduleMgr = new ModuleMgr();
            _connMgr = new ConnectionMgr();
            _taskMgr = new TaskManager();
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString()); 
        }

        private bool _init = false;

        private ToolStripMenuItem CreateSubMenu(string name, XmlNodeList nodes)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Name = menu.Text = name;
            foreach (XmlNode node in nodes)
            {
                ToolStripItem item = null;
                if (node.Name == "SubMenu")
                {
                    item = CreateSubMenu(node.Attributes["name"].Value, node.ChildNodes);
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
                        tsi.Click += delegate(object sender, EventArgs e)
                        {
                            cmd.Execute();
                        };
                        if (node.Attributes["displayName"] != null)
                            tsi.Text = node.Attributes["displayName"].Value;    
                        item = tsi;
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
                ToolStripMenuItem menu = CreateSubMenu(node.Attributes["name"].Value, node.ChildNodes);
                Shell.MainMenu.Items.Add(menu);
            }

            AppConsole.WriteLine("Main Menu initialized");
        }

        private void Cleanup()
        {
            if (_moduleMgr is IDisposable)
                (_moduleMgr as IDisposable).Dispose();
            if (_connMgr is IDisposable)
                (_connMgr as IDisposable).Dispose();
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

                    //Set streams for Application Console
                    AppConsole.In = new ConsoleInputStream(_shell.ConsoleWindow.InputTextBox);
                    AppConsole.Out = new ConsoleOutputStream(_shell.ConsoleWindow.TextWindow);
                    AppConsole.Err = new ConsoleOutputStream(_shell.ConsoleWindow.TextWindow);
                    AppConsole.Err.TextColor = System.Drawing.Color.Red;

                    AppConsole.WriteLine("FDO Toolbox. Version {0}", this.Version);
                    AppConsole.WriteLine("Loading modules");

                    InitMessageHandlers();

                    ModuleManager.LoadModule(new CoreModule());
                    ModuleManager.LoadModule(new ExpressModule());

                    InitMenus();
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
            ConnectionManager.ConnectionRemoved += delegate(string name)
            {
                AppConsole.WriteLine("Connection removed: {0}", name);
            };
            ConnectionManager.ConnectionAdded += delegate(string name)
            {
                AppConsole.WriteLine("New connection added: {0}", name);
            };
            ConnectionManager.ConnectionRenamed += delegate(string oldName, string newName)
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
        /// The connection manager
        /// </summary>
        public IConnectionMgr ConnectionManager
        {
            get { return _connMgr; }
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
            Cleanup();
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
        /// Current working directory path of the application
        /// </summary>
        public string AppPath { get { return System.IO.Path.GetDirectoryName(Application.ExecutablePath); } }

        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(); } }

        /// <summary>
        /// Project home page
        /// </summary>
        public string ProjectUrl { get { return "http://code.google.com/p/fdotoolbox"; } }

        /// <summary>
        /// Displays the about box for this application
        /// </summary>
        public void About()
        {
            new AboutDialog().ShowDialog();
        }
    }
}
