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
        public void CopySpatialContexts(OSGeo.FDO.Connections.IConnection srcConn, OSGeo.FDO.Connections.IConnection destConn)
        {
            Debug.Assert(destConn.ConnectionInfo.ProviderName.Contains("OSGeo.MySQL"));
            //SendMessage("Copying spatial contexts to destination");
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
                            
                            create.CoordinateSystem = reader.GetCoordinateSystem();
                            create.CoordinateSystemWkt = reader.GetCoordinateSystemWkt();
                            create.Description = reader.GetDescription();
                            create.Extent = reader.GetExtent();
                            create.ExtentType = reader.GetExtentType();
                            create.Name = name;
                            create.XYTolerance = reader.GetXYTolerance();
                            create.ZTolerance = reader.GetZTolerance();
                            create.UpdateExisting = false;

                            try
                            {
                                //Destory first then create
                                using (IDestroySpatialContext destroy = destConn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
                                {
                                    destroy.Name = name;
                                    destroy.Execute();
                                }
                                create.Execute();
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
    }
}
