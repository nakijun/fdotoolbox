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
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Controls;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Host application interface
    /// </summary>
    public interface IHostApplication
    {
        void Initialize(IShell shell);
        void ExtendUI(string uiExtensionFile);

        event EventHandler OnAppInitialized;
        void ExecuteCommand(string cmdName, bool fromConsole);

        IMenuStateMgr MenuStateManager { get; }
        IShell Shell { get; }
        IModuleMgr ModuleManager { get; }
        ITaskManager TaskManager { get; }
        ISpatialConnectionMgr SpatialConnectionManager { get; }
        IDbConnectionManager DatabaseConnectionManager { get; }
        ISpatialConnectionBoundTabManager TabManager { get; }
        ICoordinateSystemCatalog CoordinateSystemCatalog { get; }
        IPreferenceDictionary Preferences { get; }

        void Run();
        void Quit();
        void About();

        string AppPath { get; }
        string Name { get; }
        string Version { get; }
        string ProjectUrl { get; }
        string LanguageMappingFile { get; }
        string DbTargetsFile { get; }

        string OpenFile(string title, string filter);
        string SaveFile(string title, string filter);
        string OpenDirectory(string prompt);
        string SaveDirectory(string prompt);
    }
}
