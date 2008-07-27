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
using OSGeo.FDO.Schema;
using System.IO;

namespace ExpressBCP
{
    public class ExpressBcpApp : ConsoleApplication
    {
        private string _SrcFdoProvider;

        public string SrcFdoProvider
        {
            get { return _SrcFdoProvider; }
            set { _SrcFdoProvider = value; }
        }

        private string _SrcConnectionString;

        public string SrcConnectionString
        {
            get { return _SrcConnectionString; }
            set { _SrcConnectionString = value; }
        }

        private string _SrcSchema;

        public string SrcSchema
        {
            get { return _SrcSchema; }
            set { _SrcSchema = value; }
        }

        private string _DestFdoProvider;

        public string DestFdoProvider
        {
            get { return _DestFdoProvider; }
            set { _DestFdoProvider = value; }
        }

        private string _DestFile;

        public string DestPath
        {
            get { return _DestFile; }
            set { _DestFile = value; }
        }

        private List<string> _SrcClassList = new List<string>();

        private string _SrcSpatialContext;

        public string SrcSpatialContext
        {
            get { return _SrcSpatialContext; }
            set { _SrcSpatialContext = value; }
        }
	

        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string src_provider = GetArgument("-src_provider", args);
            string src_conn = GetArgument("-src_conn", args);
            string schema = GetArgument("-schema", args);
            string dest_provider = GetArgument("-dest_provider", args);
            string dest_file = GetArgument("-dest_path", args);
            string classes = GetArgument("-classes", args);
            string copy_srs = GetArgument("-copy_srs", args);

            if (string.IsNullOrEmpty(src_provider))
                throw new ArgumentException("-src_provider parameter required");

            if (string.IsNullOrEmpty(src_conn))
                throw new ArgumentException("-src_conn parameter required");

            if (string.IsNullOrEmpty(schema))
                throw new ArgumentException("-schema parameter required");

            if (string.IsNullOrEmpty(dest_provider))
                throw new ArgumentException("-dest_provider parameter required");

            if (string.IsNullOrEmpty(dest_file))
                throw new ArgumentException("-dest_path parameter required");

            this.SrcFdoProvider = src_provider;
            this.SrcConnectionString = src_conn;
            this.SrcSchema = schema;

            if (dest_provider.StartsWith("OSGeo.SDF"))// || dest_provider.StartsWith("OSGeo.SHP"))
                this.DestFdoProvider = dest_provider;
            else
                throw new ArgumentException("-dest_provider must be OSGeo.SDF");
                //throw new ArgumentException("-dest_provider must be either OSGeo.SDF or OSGeo.SHP");

            this.DestPath = dest_file;
            this._SrcSpatialContext = copy_srs;

            if (!string.IsNullOrEmpty(classes))
            {
                string[] tokens = classes.Split(',');
                if (tokens.Length > 0)
                {
                    foreach (string tok in tokens)
                    {
                        _SrcClassList.Add(tok);
                    }
                }
                else
                {
                    _SrcClassList.Add(classes);
                }
            }
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: ExpressBCP.exe -src_provider:<provider name> -src_conn:<connection string> -dest_provider:<provider name> -dest_path:<path to sdf or shp dir> -schema:<source schema name> [-classes:<comma-separated list of class names>] [-copy_srs:<source spatial context name>]");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 5, 7);
            }
            catch (ArgumentException ex)
            {
                AppConsole.Err.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

            IConnection srcConn = null;
            IConnection destConn = null;
            try
            {
                srcConn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.SrcFdoProvider);
                destConn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.DestFdoProvider);

                srcConn.ConnectionString = this.SrcConnectionString;
                if (this.DestFdoProvider.StartsWith("OSGeo.SDF"))
                {
                    if(ExpressUtility.CreateSDF(this.DestPath))
                        destConn.ConnectionString = string.Format("File={0}", this.DestPath);
                }
                //else if (this.DestFdoProvider.StartsWith("OSGeo.SHP"))
                //{
                //    destConn.ConnectionString = string.Format("DefaultFileLocation={0}", Path.GetDirectoryName(this.DestPath));
                //}

                srcConn.Open();
                destConn.Open();

                ConnectionInfo srcConnInfo = new ConnectionInfo("SOURCE", srcConn);
                ConnectionInfo destConnInfo = new ConnectionInfo("TARGET", destConn);

                BulkCopyOptions options = new BulkCopyOptions(srcConnInfo, destConnInfo);
                options.CopySpatialContexts = !string.IsNullOrEmpty(this.SrcSpatialContext);

                if (options.CopySpatialContexts)
                    options.SourceSpatialContexts.Add(this.SrcSpatialContext);

                options.SourceSchemaName = this.SrcSchema;
                if (_SrcClassList.Count > 0)
                {
                    options.ClearClassCopyOptions();
                    ClassCollection srcClasses = BulkCopyTask.GetSourceClasses(options);
                    foreach (ClassDefinition classDef in srcClasses)
                    {
                        if (_SrcClassList.Contains(classDef.Name))
                        {
                            AppConsole.WriteLine("Adding class to copy: {0}", classDef.Name);
                            options.AddClassCopyOption(new ClassCopyOptions(classDef));
                        }
                    }
                }
                else
                {
                    ClassCollection srcClasses = BulkCopyTask.GetSourceClasses(options);
                    foreach (ClassDefinition classDef in srcClasses)
                    {
                        AppConsole.WriteLine("Adding class to copy: {0}", classDef.Name);
                        options.AddClassCopyOption(new ClassCopyOptions(classDef));
                    }
                }

                BulkCopyTask task = new BulkCopyTask("BCP", options);
                task.OnItemProcessed += new TaskPercentageEventHandler(task_OnItemProcessed);
                task.OnTaskMessage += new TaskProgressMessageEventHandler(task_OnTaskMessage);
                task.ValidateTaskParameters();
                task.Execute();
            }
            catch (Exception ex)
            {
                AppConsole.WriteException(ex);
            }
            finally
            {
                if (srcConn != null)
                {
                    if (srcConn.ConnectionState != ConnectionState.ConnectionState_Closed)
                        srcConn.Close();

                    srcConn.Dispose();
                }

                if (destConn != null)
                {
                    if (destConn.ConnectionState != ConnectionState.ConnectionState_Closed)
                        destConn.Close();

                    destConn.Dispose();
                }
            }
        }

        void task_OnTaskMessage(string msg)
        {
            AppConsole.WriteLine(msg);
        }

        void task_OnItemProcessed(int pc)
        {
            AppConsole.WriteLine("{0}% done", pc);
        }
    }
}
