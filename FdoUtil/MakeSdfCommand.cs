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
using System.IO;

namespace FdoUtil
{
    public class MakeSdfCommand : ConsoleCommand
    {
        private string _sdfFile;
        private string _schemaFile;

        public MakeSdfCommand(string sdfFile)
        {
            _sdfFile = sdfFile;
        }

        public MakeSdfCommand(string sdfFile, string schemaFile)
            : this(sdfFile)
        {
            _schemaFile = schemaFile;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            if (ExpressUtility.CreateSDF(_sdfFile))
            {
                WriteLine("SDF file created");
                if (!string.IsNullOrEmpty(_schemaFile) && File.Exists(_schemaFile))
                {
                    try
                    {
                        ExpressUtility.ApplySchemaToSDF(_schemaFile, _sdfFile);
                        WriteLine("Schema Applied to {0}", _sdfFile);
                        retCode = CommandStatus.E_OK;
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        WriteException(ex);
                        retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                    }
                }
                else
                {   
                    retCode = CommandStatus.E_OK;
                }
            }
            else
            {
                WriteError("Failed to create SDF file");
                retCode = CommandStatus.E_FAIL_SDF_CREATE;
            }

            return (int)retCode;
        }
    }
}
