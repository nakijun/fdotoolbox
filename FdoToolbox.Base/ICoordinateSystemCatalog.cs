using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FdoToolbox.Core.CoordinateSystems;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Coordinate system catalog interface
    /// </summary>
    public interface ICoordinateSystemCatalog : IService
    {
        /// <summary>
        /// Add a new coordinate system
        /// </summary>
        /// <param name="cs">The coordinate system to add</param>
        void AddProjection(CoordinateSystemDefinition cs);
        /// <summary>
        /// Updates an existing coordinate system (by name)
        /// </summary>
        /// <param name="cs">The updated coordinate system object</param>
        /// <param name="oldName">The name of the coordinate system to update</param>
        /// <returns>true if updated, false otherwise</returns>
        bool UpdateProjection(CoordinateSystemDefinition cs, string oldName);
        /// <summary>
        /// Deletes a coordinate system
        /// </summary>
        /// <param name="cs">The coordinate system to delete</param>
        /// <returns>true if deleted, false otherwise</returns>
        bool DeleteProjection(CoordinateSystemDefinition cs);
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
        BindingList<CoordinateSystemDefinition> GetAllProjections();
    }
}
