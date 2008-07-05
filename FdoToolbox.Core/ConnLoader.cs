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
