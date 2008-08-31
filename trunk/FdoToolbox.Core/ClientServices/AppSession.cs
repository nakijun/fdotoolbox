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
using FdoToolbox.Core.IO;
using FdoToolbox.Core.Common;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Configuration;
using System.Xml.Serialization;
using OSGeo.FDO.ClientServices;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Helper class to persist session settings (spatial/non-spatial connections and
    /// tasks)
    /// </summary>
    public sealed class AppSession
    {
        private AppSession() { }

        /// <summary>
        /// Load whatever saved settings from the session directory
        /// </summary>
        public static void Restore()
        {
            try
            {
                string path = AppGateway.RunningApplication.Preferences.GetStringPref(PreferenceNames.PREF_STR_SESSION_DIRECTORY);
                if (!Directory.Exists(path))
                    return;
                if (Directory.GetFiles(path).Length == 0)
                    return;

                string[] taskfiles = Directory.GetFiles(path, "*.task");
                string[] connfiles = Directory.GetFiles(path, "*.conn");
                string[] dbconnfiles = Directory.GetFiles(path, "*.dbconn");
                //Load task first as it will load any dependent connections
                foreach (string file in taskfiles)
                {
                    ITask task = TaskLoader.LoadTask(file, false);
                    AppGateway.RunningApplication.TaskManager.AddTask(task);
                }
                //Now process each connection file, but don't use the loader
                //classes as we don't want to load a connection twice
                XmlSerializer connSerializer = new XmlSerializer(typeof(Connection));
                foreach (string file in connfiles)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        Connection c = (Connection)connSerializer.Deserialize(reader);
                        //See if it exist already by name
                        IConnection conn = AppGateway.RunningApplication.SpatialConnectionManager.GetConnection(c.Name);
                        if (conn != null)
                        {
                            //Connection we are about to load exists
                            //skip it
                            if (conn.ConnectionString == c.ConnectionString &&
                                conn.ConnectionInfo.ProviderName == c.Provider)
                            {
                                continue;
                            }
                        }
                        //Connection doesn't exist, so add it
                        conn = FeatureAccessManager.GetConnectionManager().CreateConnection(c.Provider);
                        conn.ConnectionString = c.ConnectionString;
                        conn.Open();
                        AppGateway.RunningApplication.SpatialConnectionManager.AddConnection(c.Name, conn);
                    }
                }
                //Repeat for database connections
                XmlSerializer dbConnSerializer = new XmlSerializer(typeof(DbConnection));
                foreach (string file in dbconnfiles)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        DbConnection c = (DbConnection)dbConnSerializer.Deserialize(reader);
                        //See if it exist already by name
                        DbConnectionInfo connInfo = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(c.Name);
                        if (connInfo != null)
                        {
                            //Connection we are about to load exists
                            //skip it
                            if (connInfo.Connection.ConnectionString == c.ConnectionString)
                            {
                                continue;
                            }
                        }
                        //Connection doesn't exist, so add it
                        connInfo = new DbConnectionInfo(c.Name, new OleDbConnection(c.ConnectionString));
                        AppGateway.RunningApplication.DatabaseConnectionManager.AddConnection(connInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                AppConsole.WriteLine("Session restore error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Save all connections and tasks to the session directory
        /// </summary>
        public static void Persist()
        {
            string path = AppGateway.RunningApplication.Preferences.GetStringPref(PreferenceNames.PREF_STR_SESSION_DIRECTORY);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //Clear out existing files
            string [] files = Directory.GetFiles(path, "*.conn");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            files = Directory.GetFiles(path, "*.dbconn");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            files = Directory.GetFiles(path, "*.task");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            ICollection<string> fdoConnNames = AppGateway.RunningApplication.SpatialConnectionManager.GetConnectionNames();
            ICollection<string> dbConnNames = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnectionNames();
            ICollection<string> taskNames = AppGateway.RunningApplication.TaskManager.TaskNames;
            foreach (string connName in fdoConnNames)
            {
                IConnection conn = AppGateway.RunningApplication.SpatialConnectionManager.GetConnection(connName);
                SpatialConnectionInfo connInfo = new SpatialConnectionInfo(connName, conn);
                SpatialConnLoader.SaveConnection(connInfo, Path.Combine(path, connName + ".conn"));
            }
            foreach (string connName in dbConnNames)
            {
                DbConnectionInfo connInfo = AppGateway.RunningApplication.DatabaseConnectionManager.GetConnection(connName);
                DbConnLoader.SaveConnection(connInfo, Path.Combine(path, connName + ".dbconn"));
            }
            foreach (string taskName in taskNames)
            {
                ITask task = AppGateway.RunningApplication.TaskManager.GetTask(taskName);
                TaskLoader.SaveTask(task, Path.Combine(path, taskName + ".task"));
            }
        }
    }
}
