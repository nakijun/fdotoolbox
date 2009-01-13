using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Default copy spatial contexts command
    /// </summary>
    public class CopySpatialContext : ICopySpatialContext
    {
        /// <summary>
        /// Determines if the given spatial context in the list of spatial context names
        /// </summary>
        /// <param name="ctx">The spatial context</param>
        /// <param name="names">The spatial context name list</param>
        /// <returns></returns>
        protected bool SpatialContextInSpecifiedList(SpatialContextInfo ctx, string[] names)
        {
            return Array.Exists<string>(names, delegate(string s) { return s == ctx.Name; });
        }

        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite)
        {
            List<string> names = new List<string>();
            using (FdoFeatureService service = source.CreateFeatureService())
            {   
                ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                if (contexts.Count == 0)
                    return;

                foreach (SpatialContextInfo ctx in contexts)
                {
                    names.Add(ctx.Name);
                }
            }
            Execute(source, target, overwrite, names.ToArray());
        }

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite, string[] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            using (FdoFeatureService sService = source.CreateFeatureService())
            using (FdoFeatureService tService = target.CreateFeatureService())
            {
                bool supportsMultipleScs = target.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts).Value;
                bool supportsDestroySc = tService.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext);
                if (supportsMultipleScs)
                {
                    ReadOnlyCollection<SpatialContextInfo> contexts = tService.GetSpatialContexts();
                    if (overwrite && supportsDestroySc)
                    {
                        foreach (SpatialContextInfo c in contexts)
                        {
                            if (SpatialContextInSpecifiedList(c, spatialContextNames))
                                tService.DestroySpatialContext(c);
                        }
                    }
                    foreach (SpatialContextInfo c in contexts)
                    {
                        if (SpatialContextInSpecifiedList(c, spatialContextNames))
                            tService.CreateSpatialContext(c, overwrite);
                    }
                }
                else
                {
                    tService.CreateSpatialContext(sService.GetActiveSpatialContext(), true);
                }
            }
        }

        /// <summary>
        /// Copies the named spatial context
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will ovewrite the spatial context of the same name on the target connection</param>
        /// <param name="spatialContextName"></param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite, string spatialContextName)
        {
            Execute(source, target, overwrite, new string[] { spatialContextName });
        }
    }
}
