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
using System.Collections.ObjectModel;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Copy spatial context override for SHP target. SHP requires
    /// that the Spatial Context Name is also the Coordinate System Name
    /// </summary>
    public class ShpCopySpatialContextOverride : ICopySpatialContextOverride
    {
        /// <summary>
        /// Copies the spatial contexts
        /// </summary>
        /// <param name="srcConn"></param>
        /// <param name="destConn"></param>
        /// <param name="spatialContextNames"></param>
        public void CopySpatialContexts(IConnection srcConn, IConnection destConn, ReadOnlyCollection<string> spatialContextNames)
        {
            if (spatialContextNames.Count == 0)
                return;

            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.SHP"));
            string srcName = spatialContextNames[0];
            FeatureService srcService = new FeatureService(srcConn);
            FeatureService destService = new FeatureService(destConn);
            SpatialContextInfo context = srcService.GetSpatialContext(srcName);
            if (context != null)
            {
                //Make sure that CSName != Spatial Context Name
                WKTParser parser = new WKTParser(context.CoordinateSystemWkt);
                if (!string.IsNullOrEmpty(parser.CSName))
                {
                    context.CoordinateSystem = parser.CSName;
                    context.Name = parser.CSName;
                    destService.CreateSpatialContext(context, true);
                }
            }
        }
    }
}
