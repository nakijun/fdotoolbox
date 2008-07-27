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

namespace MakeSdf
{
    public class MakeSdfApp : ConsoleApplication
    {
        private string _SdfFile;

        public string SdfFile
        {
            get { return _SdfFile; }
            set { _SdfFile = value; }
        }

        private string _SchemaFile;

        public string SchemaFile
        {
            get { return _SchemaFile; }
            set { _SchemaFile = value; }
        }

        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string sdf = GetArgument("-path", args);
            string schema = GetArgument("-schema", args);

            if (!string.IsNullOrEmpty(sdf))
            {
                if (!sdf.EndsWith(".sdf", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("-path argument must be a file with a .sdf extension");
                else
                    this.SdfFile = sdf;
            }
            else
            {
                throw new ArgumentException("-path parameter required");
            }

            if (!string.IsNullOrEmpty(schema))
            {
                this.SchemaFile = CheckFile(schema);
            }
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: MakeSdf.exe -path:<path to sdf file> [-schema:<path to feature schema>]");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 1, 2);
            }
            catch (ArgumentException ex)
            {
                AppConsole.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

            if (File.Exists(this.SdfFile))
                File.Delete(this.SdfFile);

            if (ExpressUtility.CreateSDF(this.SdfFile))
            {
                AppConsole.WriteLine("New SDF file created at {0}", this.SdfFile);
                if (!string.IsNullOrEmpty(this.SchemaFile))
                {
                    ExpressUtility.ApplySchemaToSDF(this.SchemaFile, this.SdfFile);
                    AppConsole.WriteLine("Schema applied to {0} from schema file {1}", this.SdfFile, this.SchemaFile);
                }
            }
        }
    }
}
