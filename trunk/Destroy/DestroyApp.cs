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
using System.Collections.Specialized;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands.DataStore;

namespace Destroy
{
    public class DestroyApp : ConsoleApplication
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

        private NameValueCollection _DataStoreProperties;

        public NameValueCollection DataStoreProperties
        {
            get { return _DataStoreProperties; }
            set { _DataStoreProperties = value; }
        }
	
        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            string provider = GetArgument("-provider", args);
            string connStr = GetArgument("-connection", args);
            string dsStr = GetArgument("-properties", args);

            if (string.IsNullOrEmpty(provider))
                throw new ArgumentException("-provider parameter required");

            if (string.IsNullOrEmpty(dsStr))
                throw new ArgumentException("-properties parameter required");

            this.FdoProvider = provider;
            this.ConnectionString = connStr;

            string[] propTokens = dsStr.Split(';');
            this.DataStoreProperties = new NameValueCollection();
            if (propTokens.Length > 0)
            {
                foreach (string pToken in propTokens)
                {
                    string[] tokens = pToken.Split('=');
                    this.DataStoreProperties.Add(tokens[0], tokens[1]);
                }
            }
            else
            {
                string[] tokens = dsStr.Split('=');
                this.DataStoreProperties.Add(tokens[0], tokens[1]);
            }
        }

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: Destroy.exe -provider:<provider> [-connection:<connection string>] -properties:<data store properties>");
        }

        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 2, 3);
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
                    AppConsole.WriteLine("Opening connection");
                }
                using (conn)
                {
                    using (IDestroyDataStore destroy = conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
                    {
                        foreach (string key in this.DataStoreProperties.AllKeys)
                        {
                            destroy.DataStoreProperties.SetProperty(key, this.DataStoreProperties[key]);
                        }
                        destroy.Execute();
                        AppConsole.WriteLine("Data Store Destroyed!");
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
