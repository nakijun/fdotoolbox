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

namespace FdoToolbox.Core
{
    /// <summary>
    /// Copy spatial context override for SHP target. SHP requires
    /// that the Spatial Context Name is also the Coordinate System Name
    /// </summary>
    public class ShpCopySpatialContextOverride : ICopySpatialContextOverride
    {
        public void CopySpatialContexts(IConnection srcConn, IConnection destConn, List<string> spatialContextNames)
        {
            if (spatialContextNames.Count == 0)
                return;

            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.SHP"));
            string srcName = spatialContextNames[0];
            using (IGetSpatialContexts cmd = srcConn.CreateCommand(CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                using (ISpatialContextReader reader = cmd.Execute())
                {
                    while (reader.ReadNext())
                    {
                        //Only copy the matching context (by name)
                        if (reader.GetName() == srcName)
                        {
                            using (ICreateSpatialContext create = destConn.CreateCommand(CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
                            {
                                string name = reader.GetName();
                                //Make sure that CSName != Spatial Context Name
                                string wkt = reader.GetCoordinateSystemWkt();
                                WKTParser parser = new WKTParser(wkt);
                                //No wkt. Don't bother creating the context
                                if (!string.IsNullOrEmpty(parser.CSName))
                                {
                                    create.CoordinateSystem = parser.CSName;
                                    create.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
                                    create.Description = reader.GetDescription();
                                    create.Extent = reader.GetExtent();
                                    create.ExtentType = reader.GetExtentType();
                                    create.Name = parser.CSName;
                                    create.XYTolerance = reader.GetXYTolerance();
                                    create.ZTolerance = reader.GetZTolerance();
                                    create.Execute();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
