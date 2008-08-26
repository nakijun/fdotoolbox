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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;

namespace FdoInfo
{
    public class ListClassesCommand : SpatialConnectionCommand
    {
        private string _schema;

        public ListClassesCommand(string provider, string connStr, string schemaName)
            : base(provider, connStr)
        {
            _schema = schemaName;
        }

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
                FeatureSchema fs = service.GetSchemaByName(_schema);
                if (fs != null)
                {
                    using (fs)
                    {
                        AppConsole.WriteLine("\nClasses in schema {0}: {1}\n", fs.Name, fs.Classes.Count);
                        foreach (ClassDefinition cd in fs.Classes)
                        {
                            AppConsole.WriteLine("Name: {0} ({1})\n\n\tQualified Name: {2}", cd.Name, cd.ClassType, cd.QualifiedName);
                            AppConsole.WriteLine("\tDescription: {0}", cd.Description);
                            AppConsole.WriteLine("\tIs Abstract: {0}\n\tIs Computed: {1}", cd.IsAbstract, cd.IsComputed);
                            if (cd.BaseClass != null)
                                AppConsole.WriteLine("\tBase Class: {0}", cd.BaseClass.Name);
                            AppConsole.WriteLine("\tAttributes:");
                            WriteAttributes(cd.Attributes);
                            AppConsole.WriteLine("");
                        }
                    }
                }
                else
                {
                    AppConsole.Err.WriteLine("Could not find schema: {0}", _schema);
                    return (int)CommandStatus.E_FAIL_SCHEMA_NOT_FOUND;
                }
            }

            conn.Close();
            return (int)CommandStatus.E_OK;
        }

        private void WriteAttributes(SchemaAttributeDictionary schemaAttributeDictionary)
        {
            if (schemaAttributeDictionary.AttributeNames.Length > 0)
            {
                foreach (string name in schemaAttributeDictionary.AttributeNames)
                {
                    AppConsole.WriteLine("\t\t- {0} : {1}", name, schemaAttributeDictionary.GetAttributeValue(name));
                }
            }
            else
            {
                AppConsole.WriteLine("\t\tNone");
            }
        }
    }
}
