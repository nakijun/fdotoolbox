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
using System.ComponentModel;
using FdoToolbox.Core;

namespace FdoToolbox.Lib
{
    /// <summary>
    /// Coordinate system catalog interface
    /// </summary>
    public interface ICoordinateSystemCatalog
    {
        /// <summary>
        /// Add a new coordinate system
        /// </summary>
        /// <param name="cs">The coordinate system to add</param>
        void AddProjection(CoordinateSystem cs);
        /// <summary>
        /// Updates an existing coordinate system (by name)
        /// </summary>
        /// <param name="cs">The updated coordinate system object</param>
        /// <param name="oldName">The name of the coordinate system to update</param>
        /// <returns>true if updated, false otherwise</returns>
        bool UpdateProjection(CoordinateSystem cs, string oldName);
        /// <summary>
        /// Deletes a coordinate system
        /// </summary>
        /// <param name="cs">The coordinate system to delete</param>
        /// <returns>true if deleted, false otherwise</returns>
        bool DeleteProjection(CoordinateSystem cs);
        /// <summary>
        /// Checks if a given coordinate system (by name) exists.
        /// </summary>
        /// <param name="name">The name of the coordinate system</param>
        /// <returns>true if it exists, false otherwise</returns>
        bool ProjectionExists(string name);
        /// <summary>
        /// Gets all the coordinate systems stored in the catalog
        /// </summary>
        /// <returns>A list of coordinate systems</returns>
        BindingList<CoordinateSystem> GetAllProjections();
    }
}
