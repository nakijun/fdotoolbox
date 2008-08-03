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
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;

namespace FdoInfo
{
    public class ListSpatialContextsCommand : SpatialConnectionCommand
    {
        public ListSpatialContextsCommand(string provider, string connStr)
            : base(provider, connStr)
        { }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection();
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (FeatureService service = new FeatureService(conn))
            {
                List<SpatialContextInfo> contexts = service.GetSpatialContexts();
                AppConsole.WriteLine("\nSpatial Contexts in connection: {0}", contexts.Count);
                foreach (SpatialContextInfo ctx in contexts)
                {
                    AppConsole.WriteLine("\nName: {0}\n", ctx.Name);
                    AppConsole.WriteLine("\tDescriptionn: {0}", ctx.Description);
                    AppConsole.WriteLine("\tXY Tolerance: {0}\n\tZ Tolerance: {1}", ctx.XYTolerance, ctx.ZTolerance);
                    AppConsole.WriteLine("\tCoordinate System: {0}\n\tCoordinate System WKT:\n\t\t{1}", ctx.CoordinateSystem, ctx.CoordinateSystemWkt);
                    AppConsole.WriteLine("\tExtent Type: {0}", ctx.ExtentType);
                    if (ctx.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
                        AppConsole.WriteLine("\tExtent:\n\t\t{0}", ctx.ExtentGeometryText);
                }
            }
            conn.Close();
            return (int)CommandStatus.E_OK;
        }
    }
}
