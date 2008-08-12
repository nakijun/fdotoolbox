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
using FdoToolbox.Core;

namespace FdoToolbox.Tests
{
    public class MockApplication : IHostApplication
    {
        public void Initialize(IShell shell)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExtendUI(string uiExtensionFile)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler OnAppInitialized;

        public void ExecuteCommand(string cmdName, bool fromConsole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IMenuStateMgr MenuStateManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IShell Shell
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public FdoToolbox.Core.Modules.IModuleMgr ModuleManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ITaskManager TaskManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ISpatialConnectionMgr SpatialConnectionManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IDbConnectionManager DatabaseConnectionManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public FdoToolbox.Core.Controls.ISpatialConnectionBoundTabManager TabManager
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ICoordinateSystemCatalog CoordinateSystemCatalog
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public FdoToolbox.Core.ClientServices.IPreferenceDictionary Preferences
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Run()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Quit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void About()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string AppPath
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Version
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ProjectUrl
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string LanguageMappingFile
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string DbTargetsFile
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string OpenFile(string title, string filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string SaveFile(string title, string filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string OpenDirectory(string prompt)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string SaveDirectory(string prompt)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
