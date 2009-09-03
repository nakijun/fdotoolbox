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
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base
{
    public class FdoConnectionUtil
    {
        /// <summary>
        /// If true, the connection will use partial schema discovery
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        static bool ShouldForceFullSchemaDiscovery(FdoConnection conn)
        {
            string[] providers = Preferences.ExcludePartialSchemaProviders;

            if (providers.Length == 0) //Nothing to ignore
                return false;

            string prvName = conn.Provider;
            foreach (string prv in providers)
            {
                if (prvName == prv) //Is in ignore list, 
                    return true;
            }

            return false; //Not in ignore list
        }

        public static FdoFeatureService CreateFeatureService(FdoConnection conn)
        {
            bool force = ShouldForceFullSchemaDiscovery(conn);
            return conn.CreateFeatureService(force);
        }
    }
}
