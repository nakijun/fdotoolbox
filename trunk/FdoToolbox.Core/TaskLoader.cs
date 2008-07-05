using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using System.Windows.Forms;

namespace FdoToolbox.Core
{
    public sealed class TaskLoader
    {
        public static ITask LoadTask(string configFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);

            XmlNode bcpNode = doc.SelectSingleNode("//BulkCopyTask");
            if(bcpNode != null)
                return LoadBulkCopy(doc);
            else
                return null;
        }

        public static void SaveTask(ITask task, string file)
        {
            switch (task.TaskType)
            {
                case TaskType.BulkCopy:
                    SaveBulkCopy((BulkCopyTask)task, file);
                    break;
                default:
                    AppConsole.WriteLine("Unknown or unsupported task type: {0}", task.TaskType);
                    break;
            }
        }

        private static BulkCopyTask LoadBulkCopy(XmlDocument doc)
        {
            string srcName = HostApplication.Instance.ConnectionManager.CreateUniqueName();
            string destName = HostApplication.Instance.ConnectionManager.CreateUniqueName();

            try
            {
                XmlNode sourceNode = doc.SelectSingleNode("//BulkCopyTask/BulkCopySource");
                XmlNode targetNode = doc.SelectSingleNode("//BulkCopyTask/BulkCopyTarget");
                XmlNode optionNode = doc.SelectSingleNode("//BulkCopyTask/BulkCopyOptions");
                XmlNode mappingsNode = doc.SelectSingleNode("//BulkCopyTask/PropertyMappings");

                string srcProvider = sourceNode.SelectSingleNode("Provider").InnerText;
                string srcConnStr = sourceNode.SelectSingleNode("ConnectionString").InnerText;
                string srcSchema = sourceNode.SelectSingleNode("FeatureSchema").InnerText;
                string srcClass = sourceNode.SelectSingleNode("Class").InnerText;

                string destProvider = targetNode.SelectSingleNode("Provider").InnerText;
                string destConnStr = targetNode.SelectSingleNode("ConnectionString").InnerText;
                string destSchema = targetNode.SelectSingleNode("FeatureSchema").InnerText;
                string destClass = targetNode.SelectSingleNode("Class").InnerText;

                IConnection srcConn = FeatureAccessManager.GetConnectionManager().CreateConnection(srcProvider);
                IConnection destConn = FeatureAccessManager.GetConnectionManager().CreateConnection(destProvider);

                srcConn.ConnectionString = srcConnStr;
                destConn.ConnectionString = destConnStr;

                HostApplication.Instance.ConnectionManager.AddConnection(srcName, srcConn);
                HostApplication.Instance.ConnectionManager.AddConnection(destName, destConn);

                string name = "BCP" + DateTime.Now.ToShortDateString();
                BulkCopySource bcpSource = new BulkCopySource(new ConnectionInfo(srcName, srcConn), srcSchema, srcClass);
                BulkCopyTarget bcpTarget = new BulkCopyTarget(new ConnectionInfo(destName, destConn), destSchema, destClass);
                BulkCopyTask bcp = new BulkCopyTask(name, bcpSource, bcpTarget);

                XmlNodeList mappingNodes = doc.SelectNodes("//BulkCopyTask/PropertyMappings/Mapping");
                if (mappingNodes != null)
                {
                    foreach (XmlNode mappingNode in mappingNodes)
                    {
                        bcp.Source.AddMapping(mappingNode.Attributes["source"].Value, mappingNode.Attributes["target"].Value);
                    }
                }

                bcpTarget.DeleteBeforeCopy = Convert.ToBoolean(optionNode.SelectSingleNode("DeleteSourceBeforeCopy").InnerText);
                bcpSource.FeatureLimit = Convert.ToInt32(optionNode.SelectSingleNode("FeatureLimit").InnerText);
                bcp.TransformCoordinates = Convert.ToBoolean(optionNode.SelectSingleNode("TransformCoordinates").InnerText);
                bcp.CoerceDataTypes = Convert.ToBoolean(optionNode.SelectSingleNode("CoerceDataTypes").InnerText);
                return bcp;
            }
            catch (Exception ex)
            {
                AppConsole.WriteException(ex);
                HostApplication.Instance.ConnectionManager.RemoveConnection(srcName);
                HostApplication.Instance.ConnectionManager.RemoveConnection(destName);
                return null;
            }
        }

        private static void SaveBulkCopy(BulkCopyTask task, string configFile)
        {
            //TODO: Do something more elegant.
            string configTemplate = @"
            <?xml version=""1.0""?>
            <BulkCopyTask name=""{0}"">
                <BulkCopySource>
                    <Provider>{1}</Provider>
                    <ConnectionString>{2}</ConnectionString>
                    <FeatureSchema>{3}</FeatureSchema>
                    <Class>{4}</Class>
                </BulkCopySource>
                <BulkCopyTarget>
                    <Provider>{5}</Provider>
                    <ConnectionString>{6}</ConnectionString>
                    <FeatureSchema>{7}</FeatureSchema>
                    <Class>{8}</Class>
                </BulkCopyTarget>
                <PropertyMappings>
                    {9}
                </PropertyMappings>
                <BulkCopyOptions>
                    <CoerceDataTypes>{10}</CoerceDataTypes>
                    <FeatureLimit>{11}</FeatureLimit>
                    <DeleteSourceBeforeCopy>{12}</DeleteSourceBeforeCopy>
                    <TransformCoordinates>{13}</TransformCoordinates>
                </BulkCopyOptions>
            </BulkCopyTask>
            ";
            string mappingsXml = string.Empty;
            foreach (string srcPropName in task.Source.SourcePropertyNames)
            {
                mappingsXml += string.Format("<Mapping source=\"{0}\" target=\"{1}\" />", srcPropName, task.Source.GetTargetPropertyName(srcPropName));
            }
            string xml = string.Format(
                configTemplate,
                task.Name,
                task.Source.ConnInfo.Connection.ConnectionInfo.ProviderName,
                task.Source.ConnInfo.Connection.ConnectionString,
                task.Source.SchemaName,
                task.Source.ClassName,
                task.Target.ConnInfo.Connection.ConnectionInfo.ProviderName,
                task.Target.ConnInfo.Connection.ConnectionString,
                task.Target.SchemaName,
                task.Target.ClassName,
                mappingsXml,
                task.CoerceDataTypes,
                task.Source.FeatureLimit,
                task.Target.DeleteBeforeCopy,
                task.TransformCoordinates);

            System.IO.File.Delete(configFile);
            using (XmlTextWriter writer = new XmlTextWriter(configFile, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteRaw(xml);
                writer.Close();
            }
        }
    }
}
