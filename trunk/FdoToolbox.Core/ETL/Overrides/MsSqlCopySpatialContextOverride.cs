using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FdoToolbox.Core.Feature;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Copy spatial context override for SQL Server 2008 target
    /// </summary>
    public class MsSqlCopySpatialContextOverride : CopySpatialContext
    {

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public override void Execute(FdoConnection source, FdoConnection target, bool overwrite, string[] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            FdoFeatureService srcService = source.CreateFeatureService();
            FdoFeatureService destService = target.CreateFeatureService();
            ReadOnlyCollection<SpatialContextInfo> srcContexts = srcService.GetSpatialContexts();
            ReadOnlyCollection<SpatialContextInfo> destContexts = destService.GetSpatialContexts();
            foreach (SpatialContextInfo ctx in srcContexts)
            {
                if (SpatialContextInSpecifiedList(ctx, spatialContextNames))
                {
                    try
                    {
                        //Find target spatial context of the same name
                        SpatialContextInfo sci = destService.GetSpatialContext(ctx.Name);
                        if (sci != null && overwrite)
                        {
                            //If found, destroy then create
                            destService.DestroySpatialContext(ctx.Name);
                            destService.CreateSpatialContext(ctx, false);
                        }
                        else
                        {
                            destService.CreateSpatialContext(ctx, false);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
