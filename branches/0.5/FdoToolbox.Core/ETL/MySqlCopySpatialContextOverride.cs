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
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Copy spatial context override for MySQL target. 
    /// 
    /// MySQL requires that if an existing spatial context already exists 
    /// (by name) that it be destoryed, as ICreateSpatialContext::updateExisting 
    /// does not work
    /// 
    /// Also, an existing spatial context cannot be destroyed if there are
    /// Geometric Properties using that spatial context. In this case, we don't
    /// copy that context across
    /// </summary>
    public class MySqlCopySpatialContextOverride : ICopySpatialContextOverride
    {
        public void CopySpatialContexts(IConnection srcConn, IConnection destConn, List<string> spatialContextNames)
        {
            if (spatialContextNames.Count == 0)
                return;

            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.MySQL"));
            FeatureService srcService = new FeatureService(srcConn);
            FeatureService destService = new FeatureService(destConn);
            List<SpatialContextInfo> srcContexts = srcService.GetSpatialContexts();
            srcContexts.ForEach(delegate(SpatialContextInfo ctx)
            {
                if (spatialContextNames.Contains(ctx.Name))
                {
                    try
                    {
                        //Destroy then create
                        destService.DestroySpatialContext(ctx.Name);
                        destService.CreateSpatialContext(ctx, false);
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.WriteException(ex);
                        AppConsole.WriteLine("Ignoring that context");
                    }
                }
            });
        }
    }
}
