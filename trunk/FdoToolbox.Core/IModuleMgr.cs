using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace FdoToolbox.Core
{
    public delegate void ModuleEventHandler(IModule module);

    public interface IModuleMgr
    {
        IModule[] LoadedModules { get; }
        void LoadModule(IModule module);
        void UnloadModule(IModule module);
        Command GetCommand(string name);
        ICollection<string> GetCommandNames();
        void LoadExtension(string assemblyFile);

        event ModuleEventHandler ModuleLoaded;
        event ModuleEventHandler ModuleUnloaded;

        IModule GetLoadedModule(string name);
    }
}
