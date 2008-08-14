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
using OSGeo.FDO.Connections;
using FdoToolbox.Core;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.ClientServices;

namespace FdoUtil
{
    public class ApplySchemaCommand : SpatialConnectionCommand
    {
        private string _schemaFile;

        public ApplySchemaCommand(string provider, string connStr, string schemaFile)
            : base(provider, connStr)
        {
            _schemaFile = schemaFile;
        }

        public override int Execute()
        {
            CommandStatus retCode;

            IConnection conn = null;
            try
            {
                conn = CreateConnection();
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_CONNECT;
                return (int)retCode;
            }

            using (conn)
            {
                FeatureService service = new FeatureService(conn);
                using (service)
                {
                    try
                    {
                        service.LoadSchemasFromXml(_schemaFile);
                        WriteLine("Schema(s) applied");
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                        return (int)retCode;
                    }
                }
            }
            return (int)retCode;
        }
    }
}
