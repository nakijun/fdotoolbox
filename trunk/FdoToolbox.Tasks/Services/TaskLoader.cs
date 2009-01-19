#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL;
using FdoToolbox.Base;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Tasks.Services
{
    public class TaskLoader : BaseDefinitionLoader
    {
        protected override FdoConnection CreateConnection(string provider, string connStr)
        {
            //Try to find matching open connection first
            IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();

            FdoConnection conn = connMgr.GetConnection(provider, connStr);
            if (conn == null)
            {
                conn = new FdoConnection(provider, connStr);
                string name = ServiceManager.Instance.GetService<NamingService>().GetDefaultConnectionName(provider);
                connMgr.AddConnection(name, conn);
            }
            return conn;
        }
    }
}
