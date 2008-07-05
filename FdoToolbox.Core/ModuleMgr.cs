using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public ModuleMgr() 
        { 
            _Modules = new List<IModule>();
            _GlobalNamespace = new SortedDictionary<string, Command>();
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

            //Add module commands to global namespace
            ICollection<string> cmdNames = module.CommandNames;
            foreach (string name in cmdNames)
            {
                if (_GlobalNamespace.ContainsKey(name))
                    throw new ModuleLoadException("A command named " + name + " is already defined in another loaded module");
                else
                    _GlobalNamespace.Add(name, module.GetCommand(name));
            }
            AppConsole.WriteLine("Module loaded: {0}", module.Name);
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

        public void LoadExtension(Assembly asm)
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
        }
        
        public IModule GetLoadedModule(string name)
        {
            return _Modules.Find(delegate(IModule mod) { return mod.Name == name; });
        }
    }
}
