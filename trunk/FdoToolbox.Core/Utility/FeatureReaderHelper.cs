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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Utility
{
    public sealed class FeatureReaderHelper
    {
        private FeatureReaderHelper() { }

        public static void DumpReaderClass(IFeatureReader reader)
        {
            AppConsole.WriteLine("Feature Reader class dump:");
            ClassDefinition classDef = reader.GetClassDefinition();
            AppConsole.WriteLine("Class Name: {0}", classDef.Name);
            foreach (PropertyDefinition def in classDef.Properties)
            {
                string id = string.Empty;
                if (def.PropertyType == PropertyType.PropertyType_DataProperty &&
                    classDef.IdentityProperties.Contains((DataPropertyDefinition)def))
                {
                    id = " (IDENTITY)";
                }
                AppConsole.WriteLine(" -> {0} ({1}){2}", def.Name, def.PropertyType, id);
            }
        }
    }
}
