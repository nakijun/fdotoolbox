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
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Commands;

namespace FdoToolbox.Core.Modules
{
    public delegate void ModuleEventHandler(IModule module);

    /// <summary>
    /// Module manager interface
    /// </summary>
    public interface IModuleMgr
    {
        /// <summary>
        /// Gets the list of loaded modules
        /// </summary>
        IModule[] LoadedModules { get; }

        /// <summary>
        /// Loads the module
        /// </summary>
        /// <param name="module"></param>
        void LoadModule(IModule module);

        /// <summary>
        /// Unloads the module
        /// </summary>
        /// <param name="module"></param>
        void UnloadModule(IModule module);

        /// <summary>
        /// Gets a command (by name) from the global namespace
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Command GetCommand(string name);

        /// <summary>
        /// Gets all the registered command names
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetCommandNames();

        /// <summary>
        /// Loads a .net assembly and loads all IModule instances within
        /// </summary>
        /// <param name="assemblyFile"></param>
        void LoadExtension(string assemblyFile);

        /// <summary>
        /// Fired when a module has been loaded
        /// </summary>
        event ModuleEventHandler ModuleLoaded;

        /// <summary>
        /// Fired when a module has been unloaded
        /// </summary>
        event ModuleEventHandler ModuleUnloaded;

        /// <summary>
        /// Gets a loaded module by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IModule GetLoadedModule(string name);

        /// <summary>
        /// Checks whether a given command is executable under the given
        /// connection context.
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        bool IsCommandExecutable(string cmdName, IConnection conn);
    }
}
