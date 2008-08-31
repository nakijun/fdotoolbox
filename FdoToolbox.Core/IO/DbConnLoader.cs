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
using System.Xml.Serialization;
using FdoToolbox.Core.Configuration;
using System.IO;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Common;
using System.Data;
using System.Data.OleDb;
using System.Xml;

namespace FdoToolbox.Core.IO
{
    /// <summary>
    /// Helper class to load and save database connection settings
    /// </summary>
    public sealed class DbConnLoader
    {
        private DbConnLoader() { }

        /// <summary>
        /// Loads a connection from file
        /// </summary>
        /// <param name="file">The connection settings file</param>
        /// <returns>The spatial connection</returns>
        public static DbConnectionInfo LoadConnection(string file)
        {
            DbConnection c = null;
            XmlSerializer serializer = new XmlSerializer(typeof(DbConnection));
            using (StreamReader reader = new StreamReader(file))
            {
                c = (DbConnection)serializer.Deserialize(reader);
            }
            return new DbConnectionInfo(c.Name, new OleDbConnection(c.ConnectionString));
        }

        /// <summary>
        /// Saves a connection to file
        /// </summary>
        /// <param name="cinfo">The spatial connection</param>
        /// <param name="file">The file to save it to</param>
        public static void SaveConnection(DbConnectionInfo cinfo, string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DbConnection));
            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;

                DbConnection c = new DbConnection();
                c.Name = cinfo.Name;
                c.ConnectionString = cinfo.Connection.ConnectionString;

                serializer.Serialize(writer, c);
            }
        }
    }
}
