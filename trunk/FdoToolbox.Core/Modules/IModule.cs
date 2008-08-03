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
using FdoToolbox.Core.Commands;

namespace FdoToolbox.Core.Modules
{
    /// <summary>
    /// Extension Module Interface
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Module name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Module description
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Names of all registered commands
        /// </summary>
        ICollection<string> CommandNames { get; }

        /// <summary>
        /// The list of all registered commands
        /// </summary>
        IList<Command> Commands { get; }

        /// <summary>
        /// Gets a command by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Command GetCommand(string name);

        /// <summary>
        /// Initialize the module
        /// </summary>
        void Initialize();

        /// <summary>
        /// Cleanup the module
        /// </summary>
        void Cleanup();
    }
}
