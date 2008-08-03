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
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Commands;

namespace FdoUtil
{
    public class FdoUtilApp : ConsoleApplication
    {
        private IConsoleCommand _Command;

        private void ThrowIfEmpty(string value, string parameter)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentException("Missing required parameter: " + parameter);
        }

        public override void ParseArguments(string[] args)
        {
            string cmdName = GetArgument("-cmd", args);
            ThrowIfEmpty(cmdName, "-cmd");

            if (IsSwitchDefined("-test", args))
                this.IsTestOnly = true;

            if (IsSwitchDefined("-quiet", args))
                this.IsSilent = true;

            switch (cmdName)
            {
                case "MakeSdf":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Create a new SDF with the option of applying a schema to it",
                                "FdoUtil.exe -cmd:MakeSdf -path:<sdf file> [-schema:<schema file>] [-test] [-quiet]");
                            return;
                        }

                        string sdfFile = GetArgument("-path", args);
                        string schemaFile = GetArgument("-schema", args);
                        ThrowIfEmpty(sdfFile, "-path");

                        if (string.IsNullOrEmpty(schemaFile))
                            _Command = new MakeSdfCommand(sdfFile);
                        else
                            _Command = new MakeSdfCommand(sdfFile, schemaFile);
                    }
                    break;
                case "ApplySchema":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Applies a schema definition xml file to a FDO data source",
                                "FdoUtil.exe -cmd:ApplySchema -schema:<schema file> -provider:<provider name> -connection:<connection string> [-quiet]");
                            return;
                        }

                        string schemaFile = GetArgument("-file", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(schemaFile, "-file");
                        ThrowIfEmpty(provider, "-provider");
                        ThrowIfEmpty(connStr, "-connection");

                        _Command = new ApplySchemaCommand(provider, connStr, schemaFile);
                    }
                    break;
                case "Destroy":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Destroys a datastore in an FDO data source",
                                "FdoUtil.exe -cmd:Destroy -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-test] [-quiet]");
                            return;
                        }

                        string dstoreStr = GetArgument("-properties", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(dstoreStr, "-properties");
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new DestroyCommand(provider, connStr, dstoreStr);
                    }
                    break;
                case "DumpSchema":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Writes a schema(s) in a FDO data store to an XML file",
                                "FdoUtil.exe -cmd:DumpSchema -file:<schema file> -provider:<provider> -connection:<connection string> [-schema:<selected schema>] [-test] [-quiet]");
                            return;
                        }

                        string file = GetArgument("-file", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);
                        string schema = GetArgument("-schema", args);

                        ThrowIfEmpty(file, "-file");
                        ThrowIfEmpty(provider, "-provider");
                        ThrowIfEmpty(connStr, "-connection");

                        if (string.IsNullOrEmpty(schema))
                            _Command = new DumpSchemaCommand(provider, connStr, file);
                        else
                            _Command = new DumpSchemaCommand(provider, connStr, file, schema);
                    }
                    break;
                case "ExpressBCP":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Loads and executes a Task Definition",
                                "FdoUtil.exe -cmd:ExpressBCP -src_provider:<provider name> -src_conn:<connection string> -dest_provider:<provider name> -dest_file:<file> -schema:<source schema name> [-classes:<comma-separated list of class names>] [-copy_srs:<source spatial context name>] [-quiet]");
                            return;
                        }

                        string srcProvider = GetArgument("-src_provider", args);
                        string srcConnStr = GetArgument("-src_conn", args);
                        string destProvider = GetArgument("-dest_provider", args);
                        string destFile = GetArgument("-dest_file", args);
                        string srcSchema = GetArgument("-schema", args);
                        string classes = GetArgument("-classes", args);
                        string srcSpatialContext = GetArgument("-copy_srs", args);

                        ThrowIfEmpty(srcProvider, "-src_provider");
                        ThrowIfEmpty(srcConnStr, "-src_conn");
                        ThrowIfEmpty(destProvider, "-dest_provider");
                        ThrowIfEmpty(destFile, "-dest_file");
                        ThrowIfEmpty(srcSchema, "-schema");

                        List<string> srcClasses = new List<string>();
                        if(!string.IsNullOrEmpty(classes))
                        {
                            string [] tokens = classes.Split(',');
                            if(tokens.Length > 0)
                            {
                                foreach(string className in tokens)
                                {
                                    srcClasses.Add(className);
                                }
                            }
                            else
                            {
                                srcClasses.Add(classes);
                            }
                        }
                        _Command = new ExpressBcpCommand(srcProvider, srcConnStr, srcSchema, destProvider, destFile, srcClasses, srcSpatialContext);
                    }
                    break;
                case "CreateDataStore":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Create a new FDO data store",
                                "FdoUtil.exe -cmd:CreateDataStore -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-test] [-quiet]");
                            return;
                        }

                        string dstoreStr = GetArgument("-properties", args);
                        string provider = GetArgument("-provider", args);
                        string connStr = GetArgument("-connection", args);

                        ThrowIfEmpty(dstoreStr, "-properties");
                        ThrowIfEmpty(provider, "-provider");

                        _Command = new CreateDataStoreCommand(provider, connStr, dstoreStr);
                    }
                    break;
                case "RunTask":
                    {
                        if (IsSwitchDefined("-help", args))
                        {
                            AppConsole.WriteLine("Description: {0}\nUsage: {1}",
                                "Loads and executes a Task Definition",
                                "FdoUtil.exe -cmd:RunTask -file:<task definition file> [-log:<log file>] [-quiet]");
                            return;
                        }

                        string taskFile = GetArgument("-file", args);
                        string logFile = GetArgument("-log", args);

                        ThrowIfEmpty(taskFile, "-file");

                        _Command = new RunTaskCommand(taskFile, logFile);
                    }
                    break;
                default:
                    throw new ArgumentException("Unknown command name: " + cmdName);
            }

            _Command.IsSilent = this.IsSilent;
            _Command.IsTestOnly = this.IsTestOnly;
        }

        public override void ShowUsage()
        {
            string usage =
@"FdoUtil.exe -cmd:<command name> [-quiet] [-test] <command parameters>
-quiet: Run in quiet mode (no console output)
-test: Simulate the command execution (note that some commands will do nothing)
<command name> can be any of the following:
 - ApplySchema
 - CreateDataStore
 - Destroy
 - DumpSchema
 - ExpressBCP
 - MakeSdf
 - RunTask
For more information about a command type: FdoUtil.exe -cmd:<command name> -help
For more help. Consult the help file cmd_readme.txt";
            AppConsole.WriteLine(usage);
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args);
            }
            catch (ArgumentException ex)
            {
                AppConsole.Err.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

#if DEBUG
            if(_Command != null)
                AppConsole.WriteLine("Silent: {0}\nTest: {1}", _Command.IsSilent, _Command.IsTestOnly);
#endif

            int retCode = (int)CommandStatus.E_OK;
            if (_Command != null)
            {
                retCode = _Command.Execute();
            }
#if DEBUG
            AppConsole.WriteLine("Status: {0}", retCode);
#endif
            System.Environment.ExitCode = retCode;
        }
    }
}
