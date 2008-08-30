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
using System.Diagnostics;
using FdoToolbox.Core.ClientServices;
using OSGeo.FDO.Connections;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL
{
    public class MsSqlCopySpatialContextOverride : ICopySpatialContextOverride
    {
        /// <summary>
        /// Copy spatial context override for SQL Server 2008 target. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="spatialContextNames"></param>
        public void CopySpatialContexts(IConnection srcConn, IConnection destConn, List<string> spatialContextNames)
        {
            if (spatialContextNames.Count == 0)
                return;

            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.SQLServerSpatial"));
            FeatureService srcService = new FeatureService(srcConn);
            FeatureService destService = new FeatureService(destConn);
            ReadOnlyCollection<SpatialContextInfo> srcContexts = srcService.GetSpatialContexts();
            ReadOnlyCollection<SpatialContextInfo> destContexts = destService.GetSpatialContexts();
            foreach (SpatialContextInfo ctx in srcContexts)
            {
                if (spatialContextNames.Contains(ctx.Name))
                {
                    try
                    {
                        //Find target spatial context of the same name
                        SpatialContextInfo sci = destService.GetSpatialContext(ctx.Name);
                        if (sci != null)
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
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.WriteException(ex);
                        AppConsole.WriteLine("Ignoring that context");
                    }
                }
            }
        }
    }
}
