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
using System.Xml;
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO;
using OSGeo.FDO.Runtime;

namespace FdoToolbox.Core
{
    public sealed class TaskLoader
    {
        public static ITask LoadTask(string configFile, bool consoleMode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);

            XmlNode bcpNode = doc.SelectSingleNode("//BulkCopyTask");
            if(bcpNode != null)
                return LoadBulkCopy(doc, consoleMode);
            else
                return null;
        }

        public static void SaveTask(ITask task, string file)
        {
            switch (task.TaskType)
            {
                case TaskType.BulkCopy:
                    SaveBulkCopy((SpatialBulkCopyTask)task, file);
                    break;
                default:
                    AppConsole.WriteLine("Unknown or unsupported task type: {0}", task.TaskType);
                    break;
            }
        }

        private static SpatialBulkCopyTask LoadBulkCopy(XmlDocument doc, bool consoleMode)
        {
            string srcName = HostApplication.Instance.ConnectionManager.CreateUniqueName();
            string destName = HostApplication.Instance.ConnectionManager.CreateUniqueName();
            
            try
            {
                XmlNode sourceNode = doc.SelectSingleNode("//BulkCopyTask/Source");
                XmlNode targetNode = doc.SelectSingleNode("//BulkCopyTask/Target");
                XmlNode optionNode = doc.SelectSingleNode("//BulkCopyTask/BulkCopyOptions");
                
                string srcProvider = sourceNode.SelectSingleNode("Provider").InnerText;
                string srcConnStr = sourceNode.SelectSingleNode("ConnectionString").InnerText;
                string srcSchema = sourceNode.SelectSingleNode("Schema").InnerText;
                
                string destProvider = targetNode.SelectSingleNode("Provider").InnerText;
                string destConnStr = targetNode.SelectSingleNode("ConnectionString").InnerText;
                string destSchema = targetNode.SelectSingleNode("Schema").InnerText;
                
                bool copySpatialContexts = Convert.ToBoolean(optionNode.SelectSingleNode("CopySpatialContexts").InnerText);
                bool coerceDataTypes = Convert.ToBoolean(optionNode.SelectSingleNode("CoerceDataTypes").InnerText);
                string spatialFilter = optionNode.SelectSingleNode("GlobalSpatialFilter").InnerText;

                IConnection srcConn = FeatureAccessManager.GetConnectionManager().CreateConnection(srcProvider);
                IConnection destConn = FeatureAccessManager.GetConnectionManager().CreateConnection(destProvider);

                srcConn.ConnectionString = srcConnStr;
                destConn.ConnectionString = destConnStr;

                if (consoleMode)
                {
                    srcConn.Open();
                    destConn.Open();
                }
                else
                {
                    IConnectionMgr mgr = HostApplication.Instance.ConnectionManager;
                    mgr.AddConnection(srcName, srcConn);
                    mgr.AddConnection(destName, destConn);
                }

                string name = doc.DocumentElement.Attributes["name"].Value;

                ConnectionInfo srcConnInfo = new ConnectionInfo(srcName, srcConn);
                ConnectionInfo destConnInfo = new ConnectionInfo(destName, destConn);

                SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(srcConnInfo, destConnInfo);
                options.SourceSchemaName = srcSchema;
                options.TargetSchemaName = destSchema;
                options.CoerceDataTypes = coerceDataTypes;
                options.CopySpatialContexts = copySpatialContexts;
                options.GlobalSpatialFilter = spatialFilter;
                if (options.CopySpatialContexts)
                {
                    XmlNodeList contextList = sourceNode.SelectNodes("SpatialContextList/Name");
                    if (contextList != null)
                    {
                        foreach (XmlNode contextNode in contextList)
                        {
                            options.SourceSpatialContexts.Add(contextNode.InnerText);
                        }
                    }
                }

                FeatureService srcService = new FeatureService(srcConn);

                FeatureSchemaCollection schemas = srcService.DescribeSchema();

                XmlNodeList classMappingNodeList = doc.SelectNodes("//BulkCopyTask/ClassMappings/Mapping");
                foreach (XmlNode classMappingNode in classMappingNodeList)
                {
                    bool deleteTarget = Convert.ToBoolean(classMappingNode.SelectSingleNode("DeleteTarget").InnerText);
                    string src = classMappingNode.SelectSingleNode("SourceClass").InnerText;
                    string target = classMappingNode.SelectSingleNode("TargetClass").InnerText;
                    string filter = classMappingNode.SelectSingleNode("SourceFilter").InnerText;

                    ClassDefinition classDef = FindClass(schemas, src);
                    if (classDef == null)
                        throw new TaskLoaderException("Unable to find SourceClass " + src);
                    ClassCopyOptions copt = new ClassCopyOptions(classDef);
                    copt.TargetClassName = target;
                    copt.DeleteClassData = deleteTarget;

                    if (!string.IsNullOrEmpty(filter))
                        copt.AttributeFilter = filter;

                    XmlNodeList propertyMappingList = classMappingNode.SelectNodes("Properties/PropertyMapping");
                    foreach (XmlNode propertyNode in propertyMappingList)
                    {
                        string sourceProperty = propertyNode.SelectSingleNode("SourceProperty").InnerText;
                        string targetProperty = propertyNode.SelectSingleNode("TargetProperty").InnerText;
                        copt.AddProperty(GetPropertyDefinition(classDef, sourceProperty), targetProperty);
                    }
                    options.AddClassCopyOption(copt);
                }
                return new SpatialBulkCopyTask(name, options);
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                AppConsole.WriteException(ex);
                if (!consoleMode)
                {
                    HostApplication.Instance.ConnectionManager.RemoveConnection(srcName);
                    HostApplication.Instance.ConnectionManager.RemoveConnection(destName);
                }
                return null;
            }
        }

        private static PropertyDefinition GetPropertyDefinition(ClassDefinition classDef, string sourceProperty)
        {
            int idx = classDef.Properties.IndexOf(sourceProperty);
            if (idx < 0)
                return null;
            return classDef.Properties[idx];
        }

        private static ClassDefinition FindClass(FeatureSchemaCollection schemas, string className)
        {
            foreach (FeatureSchema schema in schemas)
            {
                int idx = schema.Classes.IndexOf(className);
                if (idx >= 0)
                    return schema.Classes[idx];
            }
            return null;
        }

        private static void SaveBulkCopy(SpatialBulkCopyTask task, string configFile)
        {
            ClassCopyOptions[] cOptions = task.Options.GetClassCopyOptions();
            string classMappingXml = string.Empty;
            foreach (ClassCopyOptions copt in cOptions)
            {
                bool delete = copt.DeleteClassData;
                string srcClass = copt.ClassName;
                string destClass = copt.TargetClassName;
                string mappingsXml = string.Empty;
                foreach (string propertyName in copt.PropertyNames)
                {
                    string targetPropertyName = copt.GetTargetPropertyName(propertyName);
                    mappingsXml += string.Format(Properties.Resources.PropertyMapping, propertyName, targetPropertyName) + "\n";
                }
                classMappingXml += string.Format(Properties.Resources.ClassMapping, delete, srcClass, destClass, mappingsXml, copt.AttributeFilter) + "\n";
            }
            StringBuilder sb = new StringBuilder();
            if (task.Options.CopySpatialContexts)
            {
                foreach (string name in task.Options.SourceSpatialContexts)
                {
                    sb.Append("<Name>" + name + "</Name>\n");
                }
            }
            string contextXml = sb.ToString();
            string configXml = string.Format(
                Properties.Resources.BulkCopyTask,
                task.Name,
                task.Options.Source.Connection.ConnectionInfo.ProviderName,
                task.Options.Source.Connection.ConnectionString,
                task.Options.SourceSchemaName,
                contextXml,
                task.Options.Target.Connection.ConnectionInfo.ProviderName,
                task.Options.Target.Connection.ConnectionString,
                task.Options.TargetSchemaName,
                classMappingXml,
                task.Options.CopySpatialContexts,
                task.Options.CoerceDataTypes,
                task.Options.GlobalSpatialFilter
            );
            System.IO.File.Delete(configFile);
            using (XmlTextWriter writer = new XmlTextWriter(configFile, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteRaw(configXml);
                writer.Close();
            }
        }
    }
}
