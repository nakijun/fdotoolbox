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

namespace FdoToolbox.Core
{
    /// <summary>
    /// Copy spatial context override for SHP target. SHP requires
    /// that the Spatial Context Name is also the Coordinate System Name
    /// </summary>
    public class ShpCopySpatialContextOverride : ICopySpatialContextOverride
    {
        public void CopySpatialContexts(OSGeo.FDO.Connections.IConnection srcConn, OSGeo.FDO.Connections.IConnection destConn)
        {
            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.SHP"));
            using (IGetSpatialContexts cmd = srcConn.CreateCommand(CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                using (ISpatialContextReader reader = cmd.Execute())
                {
                    while (reader.ReadNext())
                    {
                        using (ICreateSpatialContext create = destConn.CreateCommand(CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
                        {
                            string name = reader.GetName();
                            //SendMessage("Copying spatial context: " + name);
                            //SHP-Specific processing (ugh!) It doesn't like it when
                            //CSName != Spatial Context Name
                            
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
