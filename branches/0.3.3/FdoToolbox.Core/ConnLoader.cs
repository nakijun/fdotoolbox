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
using OSGeo.FDO.Connections;
using System.Xml;
using OSGeo.FDO.ClientServices;

namespace FdoToolbox.Core
{
    public sealed class ConnLoader
    {
        public static ConnectionInfo LoadConnection(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            string name = doc.SelectSingleNode("//Connection/Name").InnerText;
            string provider = doc.SelectSingleNode("//Connection/Provider").InnerText;
            string connStr = doc.SelectSingleNode("//Connection/ConnectionString").InnerText;
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            conn.ConnectionString = connStr;
            return new ConnectionInfo(name, conn);
        }

        public static void SaveConnection(ConnectionInfo cinfo, string file)
        {
            string xmlTemplate =
@"<?xml version=""1.0""?>
<Connection>
    <Name>{0}</Name>
    <Provider>{1}</Provider>
    <ConnectionString>{2}</ConnectionString>
</Connection>
";
            string xml = string.Format(xmlTemplate, cinfo.Name, cinfo.Connection.ConnectionInfo.ProviderName, cinfo.Connection.ConnectionString);
            System.IO.File.WriteAllText(file, xml, Encoding.UTF8);
        }
    }
}
