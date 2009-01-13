using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Defines a method to override the default behaviour of copying spatial contexts
    /// across.
    /// </summary>
    public interface ICopySpatialContext
    {
        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite);

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite, string [] spatialContextNames);

        /// <summary>
        /// Copies the named spatial context
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will ovewrite the spatial context of the same name on the target connection</param>
        /// <param name="spatialContextName"></param>
        void Execute(FdoConnection source, FdoConnection target, bool overwrite, string spatialContextName);
    }
}
