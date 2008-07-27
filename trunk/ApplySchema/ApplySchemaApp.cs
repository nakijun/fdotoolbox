using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using System.IO;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Commands;

namespace ApplySchema
{
    public class ApplySchemaApp : ConsoleApplication
    {
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

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string file = GetArgument("-file", args);
            string provider = GetArgument("-provider", args);
            string connStr = GetArgument("-connection", args);

            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("-file parameter required");
            else
                this.FileName = CheckFile(file);

            if (string.IsNullOrEmpty(provider))
                throw new ArgumentException("-provider parameter required");
            else
                this.FdoProvider = provider;

            if (string.IsNullOrEmpty(connStr))
                throw new ArgumentException("-connection parameter required");
            else
                this.ConnectionString = connStr;
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: ApplySchema.exe -file:<schema definition file> -provider:<provider> -connection:<connection string>");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 3, 3);
            }
            catch (ArgumentException ex)
            {
                AppConsole.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

            try
            {
                IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(this.FdoProvider);
                if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_ApplySchema) < 0)
                    throw new ArgumentException("This provider does not support applying schemas");
                
                conn.ConnectionString = this.ConnectionString;
                conn.Open();
                using (conn)
                {
                    using (FeatureSchemaCollection schemas = new FeatureSchemaCollection(null))
                    {
                        schemas.ReadXml(this.FileName);
                        using (IApplySchema apply = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
                        {
                            apply.FeatureSchema = schemas[0];
                            apply.Execute();
                            AppConsole.WriteLine("Schema applied!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppConsole.WriteException(ex);
            }
        }
    }
}
