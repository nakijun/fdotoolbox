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
using OSGeo.FDO.Connections;

namespace FdoToolbox.Lib
{
    /// <summary>
    /// Provides an interface to determine whether a given command
    /// is executable under the context of a given connection. 
    /// 
    /// (eg. A "Delete Class" command is not executable, if the connection
    /// doesn't support schema modification)
    /// 
    /// Only apply this interfaces to subclasses of ModuleBase or implementers of
    /// IModule, otherwise it will not be picked up by the Module Manager.
    /// </summary>
    public interface ICommandVerifier
    {
        /// <summary>
        /// Returns true if the given command (name) can be executed
        /// under the context of this connection
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        bool IsCommandExecutable(string cmdName, IConnection conn);
    }
}
