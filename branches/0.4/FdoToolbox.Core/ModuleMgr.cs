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
using System.IO;
using OSGeo.FDO.Connections;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Manages extension modules
    /// </summary>
    public class ModuleMgr : IDisposable, IModuleMgr
    {
        private List<IModule> _Modules;

        //Use SortedDictionary so that the Keys collection (command names)
        //is in alphabetical order
        private SortedDictionary<string, Command> _GlobalNamespace;

        private Dictionary<string, ICommandVerifier> _CommandVerifiers;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModuleMgr() 
        { 
            _Modules = new List<IModule>();
            _GlobalNamespace = new SortedDictionary<string, Command>();
            _CommandVerifiers = new Dictionary<string, ICommandVerifier>();
        }

        /// <summary>
        /// The list of currently loaded modules
        /// </summary>
        public IModule[] LoadedModules
        {
            get { return _Modules.ToArray(); }
        }
        
        /// <summary>
        /// Load an extension module
        /// </summary>
        /// <param name="module"></param>
        public void LoadModule(IModule module)
        {
            _Modules.Add(module);
            module.Initialize();

            ICommandVerifier verifier = module as ICommandVerifier;
            if (verifier != null)
                _CommandVerifiers.Add(module.Name, verifier);

            //Add module commands to global namespace
            ICollection<string> cmdNames = module.CommandNames;
            foreach (string name in cmdNames)
            {
                if (_GlobalNamespace.ContainsKey(name))
                    throw new ModuleLoadException("A command named " + name + " is already defined in another loaded module");
                else
                    _GlobalNamespace.Add(name, module.GetCommand(name));
            }
            if (this.ModuleLoaded != null)
                this.ModuleLoaded(module);
        }

        /// <summary>
        /// Unloads an extension module
        /// </summary>
        /// <param name="module"></param>
        public void UnloadModule(IModule module)
        {
            _Modules.Remove(module);
            module.Cleanup();

            //Remove module commands from global namespace
            ICollection<string> cmdNames = module.CommandNames;
            foreach (string name in cmdNames)
            {
                _GlobalNamespace.Remove(name);
            }

            if (this.ModuleUnloaded != null)
                this.ModuleUnloaded(module);
        }

        public void Dispose()
        {
            foreach (IModule mod in _Modules)
            {
                mod.Cleanup();
            }
            _Modules.Clear();
        }

        /// <summary>
        /// Gets a command from the global namespace
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Command GetCommand(string name)
        {
            if (_GlobalNamespace.ContainsKey(name))
                return _GlobalNamespace[name];
            return null;
        }

        /// <summary>
        /// Get a list of commands in the global namespace
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetCommandNames()
        {
            return _GlobalNamespace.Keys;
        }

        public event ModuleEventHandler ModuleLoaded;

        public event ModuleEventHandler ModuleUnloaded;

        private void LoadExtension(Assembly asm)
        {
            //Probe assembly for all types that extend from
            //ModuleBase or implement IModule
            Type[] types = asm.GetTypes();
            List<IModule> modules = new List<IModule>();
            foreach (Type type in types)
            {
                //type implements IModule
                bool exists = Array.Exists<Type>(type.GetInterfaces(), delegate(Type t) { return t == typeof(IModule); });
                if (exists)
                {
                    IModule mod = (IModule)Activator.CreateInstance(type);
                    modules.Add(mod);
                }
            }
            if (modules.Count == 0)
                throw new ModuleLoadException("No types that implement IModule were found in this assembly");
            
            //Load each individual module
            modules.ForEach(delegate(IModule module)
            {
                this.LoadModule(module);
            });
        }

        public void LoadExtension(string assemblyFile)
        {
            string asmPath = assemblyFile;
            //Is this thing rooted? :-)
            if (!System.IO.Path.IsPathRooted(assemblyFile))
                asmPath = System.IO.Path.Combine(HostApplication.Instance.AppPath, asmPath);
            Assembly asm = Assembly.LoadFile(asmPath);
            LoadExtension(asm);

            string uiExtensionFile = assemblyFile.Replace(".dll", ".UIExtension");
            if (File.Exists(uiExtensionFile))
                HostApplication.Instance.ExtendUI(uiExtensionFile);
        }

        public bool IsCommandExecutable(string cmdName, IConnection conn)
        {
            //Get the parent module of the command, and if it is also a verifier,
            //Check if it is executable. Otherwise it is assumed to be executable (true)
            Command cmd = this.GetCommand(cmdName);
            if (_CommandVerifiers.ContainsKey(cmd.ModuleName))
                return _CommandVerifiers[cmd.ModuleName].IsCommandExecutable(cmdName, conn);

            return true;
        }

        public IModule GetLoadedModule(string name)
        {
            return _Modules.Find(delegate(IModule mod) { return mod.Name == name; });
        }
    }
}
