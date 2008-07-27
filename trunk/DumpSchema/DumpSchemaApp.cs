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
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;

namespace DumpSchema
{
    public class DumpSchemaApp : ConsoleApplication
    {
        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private string _FdoProvider;

        public string FdoProvider
        {
            get { return _FdoProvider; }
            set { _FdoProvider = value; }
        }

        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private string _SelectedSchema;

        public string SelectedSchema
        {
            get { return _SelectedSchema; }
            set { _SelectedSchema = value; }
        }
	
        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string fileName = GetArgument("-file", args);
            string provider = GetArgument("-provider", args);
            string connStr = GetArgument("-connection", args);
            string schema = GetArgument("-schema", args);

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("-file parameter required");
            else
                this.FileName = fileName;

            if (string.IsNullOrEmpty(provider))
                throw new ArgumentException("-provider parameter required");
            else
                this.FdoProvider = provider;

            if (string.IsNullOrEmpty(connStr))
                throw new ArgumentException("-connection parameter required");
            else
                this.ConnectionString = connStr;

            if (string.IsNullOrEmpty(schema))
                throw new ArgumentException("-schema parameter required");
            else
                this.SelectedSchema = schema;
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: DumpSchema.exe -file:<schema file> -provider:<provider> -connection:<connection string> -schema:<selected schema>");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 4, 4);
            }
            catch (ArgumentException ex)
            {
                AppConsole.Err.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

            try
            {
                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.FdoProvider);
                conn.ConnectionString = this.ConnectionString;
                conn.Open();
                using (conn)
                {
                    using (IDescribeSchema describe = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                    {
                        using (FeatureSchemaCollection schemas = describe.Execute())
                        {
                            FeatureSchema theSchema = null;
                            foreach (FeatureSchema fs in schemas)
                            {
                                if (fs.Name == this.SelectedSchema)
                                {
                                    theSchema = fs;
                                }
                            }
                            if (theSchema == null)
                            {
                                throw new ArgumentException("Selected schema not found: " + this.SelectedSchema);
                            }
                            else
                            {
                                theSchema.WriteXml(this.FileName);
                                AppConsole.WriteLine("Schema saved to {0}", this.FileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppConsole.WriteLine("Error: {0}", ex.Message);
                AppConsole.WriteException(ex);
            }
        }
    }
}
