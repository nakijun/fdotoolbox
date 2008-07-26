using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using System.Collections.Specialized;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;

namespace Mkdstore
{
    public class MkDStoreApp : ConsoleApplication
    {
        private NameValueCollection _DataStoreProperties;

        public NameValueCollection DataStoreProperties
        {
            get { return _DataStoreProperties; }
            set { _DataStoreProperties = value; }
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
	
	
        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string provider = GetArgument("-provider", args);
            string dstorestring = GetArgument("-properties", args);
            string connstr = GetArgument("-connection", args);

            if (string.IsNullOrEmpty(provider))
                throw new ArgumentException("-provider parameter required");

            if (string.IsNullOrEmpty(dstorestring))
                throw new ArgumentException("-properties parameter required");

            string[] properties = dstorestring.Split(';');
            NameValueCollection props = new NameValueCollection();
            if (properties.Length > 0)
            {
                foreach (string prop in properties)
                {
                    string[] tokens = prop.Split('=');
                    props.Add(tokens[0], tokens[1]);
                }
            }
            else
            {
                string[] tokens = dstorestring.Split('=');
                props.Add(tokens[0], tokens[1]);
            }
            this.FdoProvider = provider;
            this.DataStoreProperties = props;
            this.ConnectionString = connstr;
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: Mkdstore.exe -provider:<provider name> -properties:<datastore properties> -connection:<connection string>");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 2, 2);
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
                if (!string.IsNullOrEmpty(this.ConnectionString))
                {
                    conn.ConnectionString = this.ConnectionString;
                    conn.Open();
                    AppConsole.WriteLine("Opening connection...");
                }
                using (conn)
                {
                    using (ICreateDataStore create = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                    {
                        foreach (string key in this.DataStoreProperties.AllKeys)
                        {
                            create.DataStoreProperties.SetProperty(key, this.DataStoreProperties[key]);
                        }
                        create.Execute();
                        AppConsole.WriteLine("Data Store Created!");
                    }
                }
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.WriteLine("Error: {0}", ex.Message);
                AppConsole.WriteException(ex);
            }
        }
    }
}
