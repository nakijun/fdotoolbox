using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Copy spatial context override for SHP target. SHP requires
    /// that the Spatial Context Name is also the Coordinate System Name
    /// </summary>
    public class ShpCopySpatialContextOverride : CopySpatialContext
    {
        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public override void Execute(FdoConnection source, FdoConnection target, bool overwrite, string [] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            string srcName = spatialContextNames[0];
            FdoFeatureService srcService = source.CreateFeatureService();
            FdoFeatureService destService = target.CreateFeatureService();
            SpatialContextInfo context = srcService.GetSpatialContext(srcName);
            if (context != null)
            {
                //Make sure that CSName != Spatial Context Name
                WKTParser parser = new WKTParser(context.CoordinateSystemWkt);
                if (!string.IsNullOrEmpty(parser.CSName))
                {
                    context.CoordinateSystem = parser.CSName;
                    context.Name = parser.CSName;
                    destService.CreateSpatialContext(context, overwrite);
                }
            }
        }
    }
}
