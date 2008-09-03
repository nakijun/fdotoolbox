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
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Commands;
using System.Resources;
using System.IO;

namespace FdoToolbox.Core.Modules
{
    /// <summary>
    /// Base Module class. A module is a collection of commands.
    /// Each command is a discrete unit of functionality
    /// 
    /// Commands are associated to module methods by applying
    /// the [Command] attribute to the desired methods.
    /// 
    /// Care must be taken with command names. A ModuleLoadException
    /// will be thrown when attempting to load a module with a command
    /// whose name already exists in the global namespace.
    /// 
    /// Subclasses of ModuleBase must implement ICommandVerifier if they
    /// contain commands that cannot execute if certain capabilities are 
    /// not met.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Initialize();
        public abstract void Cleanup();
        protected Dictionary<string, Command> _CommandDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ModuleBase() 
        { 
            _CommandDictionary = new Dictionary<string, Command>(); 
            
            //Find all methods with [Command] attribute applied
            Type t = this.GetType();
            ResourceManager resMgr = GetResourceManager();
            MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                CommandAttribute [] cmdAttrs = (CommandAttribute[])method.GetCustomAttributes(typeof(CommandAttribute), true);
                if (cmdAttrs != null && cmdAttrs.Length == 1)
                {
                    CommandAttribute attr = cmdAttrs[0];
                    CommandExecuteHandler handler = null;
                    
                    if (method.IsStatic)
                    {
                        handler = (CommandExecuteHandler)Delegate.CreateDelegate(typeof(CommandExecuteHandler), method);
                    }
                    else
                    {
                        handler = (CommandExecuteHandler)Delegate.CreateDelegate(typeof(CommandExecuteHandler), this, method);
                    }

                    Command cmd = new Command(
                        attr.Name,
                        attr.DisplayName,
                        attr.Description,
                        handler,
                        this.Name
                    );
                    cmd.InvocationType = attr.InvocationType;
                    if (!string.IsNullOrEmpty(attr.ImageResourceName))
                    {
                        object img = resMgr.GetObject(attr.ImageResourceName);
                        if (img != null)
                            cmd.CommandImage = (Image)img;
                    }
                    cmd.ShortcutKeys = attr.ShortcutKeys;
                    AddCommand(cmd);
                }
            }
        }

        /// <summary>
        /// Returns the resource manager that all resource queries will be
        /// made to. By default this will return the ResourceManager of this assembly
        /// (the one containing this class). Subclasses should override this method
        /// to return their assembly-specific ResourceManager
        /// </summary>
        /// <returns></returns>
        protected virtual ResourceManager GetResourceManager()
        {
            return Properties.Resources.ResourceManager;
        }

        public ICollection<string> CommandNames
        {
            get { return _CommandDictionary.Keys; }
        }

        public Command GetCommand(string name)
        {
            if (_CommandDictionary.ContainsKey(name))
                return _CommandDictionary[name];
            return null;
        }

        /// <summary>
        /// Register a module command
        /// </summary>
        /// <param name="cmd">The command object to register</param>
        /// <exception cref="ModuleLoadException">
        /// thrown if this command name already exists
        /// </exception>
        public void AddCommand(Command cmd)
        {
            if (_CommandDictionary.ContainsKey(cmd.Name))
                throw new ModuleLoadException("A command named " + cmd.Name + " already exists in this module");
            _CommandDictionary.Add(cmd.Name, cmd);
        }

        public IList<Command> Commands
        {
            get { return new List<Command>(_CommandDictionary.Values); }
        }
    }
}
