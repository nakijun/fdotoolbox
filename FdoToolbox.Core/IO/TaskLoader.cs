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
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;
using System.Data;
using System.Data.OleDb;

namespace FdoToolbox.Core.IO
{
    /// <summary>
    /// Utility class to parse and load task definitions
    /// </summary>
    public sealed class TaskLoader
    {
        /// <summary>
        /// Load a task definition
        /// </summary>
        /// <param name="configFile">The task definition file</param>
        /// <param name="consoleMode">Is the application a console application?</param>
        /// <returns>The task definition object, null if loading failed</returns>
        public static ITask LoadTask(string configFile, bool consoleMode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);

            XmlNode bcpNode = doc.SelectSingleNode("//BulkCopyTask");
            XmlNode joinNode = doc.SelectSingleNode("//DatabaseJoinTask");
            if (bcpNode != null)
                return LoadBulkCopy(doc, consoleMode);
            else if (joinNode != null)
                return LoadJoinTask(doc, consoleMode);
            else
                return null;
        }

        /// <summary>
        /// Saves a task definition to file
        /// </summary>
        /// <param name="task">The task definition</param>
        /// <param name="file">The file to save it to</param>
        public static void SaveTask(ITask task, string file)
        {
            switch (task.TaskType)
            {
                case TaskType.BulkCopy:
                    SaveBulkCopy((SpatialBulkCopyTask)task, file);
                    break;
                case TaskType.DbJoin:
                    SaveJoinTask((SpatialJoinTask)task, file);
                    break;
                default:
                    AppConsole.WriteLine("Unknown or unsupported task type: {0}", task.TaskType);
                    break;
            }
        }

        private static ITask LoadJoinTask(XmlDocument doc, bool consoleMode)
        {
            ITask task = null;
            string priName = null;
            string secName = null;
            string targetName = null;
            try
            {
                XmlNode primaryNode = doc.SelectSingleNode("//DatabaseJoinTask/PrimarySource");
                XmlNode secondaryNode = doc.SelectSingleNode("//DatabaseJoinTask/SecondarySource");
                XmlNode targetNode = doc.SelectSingleNode("//DatabaseJoinTask/Target");
                XmlNode joinNode = doc.SelectSingleNode("//DatabaseJoinTask/Joins");
                XmlNode optionsNode = doc.SelectSingleNode("//DatabaseJoinTask/JoinOptions");

                priName = primaryNode.SelectSingleNode("Name").InnerText;
                string priProvider = primaryNode.SelectSingleNode("Provider").InnerText;
                string priConnStr = primaryNode.SelectSingleNode("ConnectionString").InnerText;
                string priSchema = primaryNode.SelectSingleNode("FeatureSchema").InnerText;
                string priClass = primaryNode.SelectSingleNode("Class").InnerText;
                string priPrefix = primaryNode.SelectSingleNode("Prefix").InnerText;

                secName = secondaryNode.SelectSingleNode("Name").InnerText;
                string secDriver = secondaryNode.SelectSingleNode("Driver").InnerText;
                string secConnStr = secondaryNode.SelectSingleNode("ConnectionString").InnerText;
                string secTable = secondaryNode.SelectSingleNode("Table").InnerText;
                string secPrefix = secondaryNode.SelectSingleNode("Prefix").InnerText;

                targetName = targetNode.SelectSingleNode("Name").InnerText;
                string targetProvider = targetNode.SelectSingleNode("Provider").InnerText;
                string targetConnStr = targetNode.SelectSingleNode("ConnectionString").InnerText;
                string targetSchema = targetNode.SelectSingleNode("FeatureSchema").InnerText;
                string targetClass = targetNode.SelectSingleNode("Class").InnerText;

                IConnection priConn = FeatureAccessManager.GetConnectionManager().CreateConnection(priProvider);
                priConn.ConnectionString = priConnStr;

                IConnection targetConn = FeatureAccessManager.GetConnectionManager().CreateConnection(targetProvider);
                targetConn.ConnectionString = targetConnStr;

                IDbConnection secConn = new OleDbConnection(secConnStr);

                SpatialConnectionInfo priConnInfo = new SpatialConnectionInfo(priName, priConn);
                SpatialConnectionInfo targetConnInfo = new SpatialConnectionInfo(targetName, targetConn);
                DbConnectionInfo secConnInfo = new DbConnectionInfo(secName, secConn, secDriver);

                if (consoleMode)
                {
                    priConnInfo.Connection.Open();
                    secConnInfo.Connection.Open();
                    targetConnInfo.Connection.Open();
                }
                else
                {
                    HostApplication.Instance.SpatialConnectionManager.AddConnection(priConnInfo.Name, priConnInfo.Connection);
                    HostApplication.Instance.SpatialConnectionManager.AddConnection(targetConnInfo.Name, targetConnInfo.Connection);
                    HostApplication.Instance.DatabaseConnectionManager.AddConnection(secConnInfo);
                }

                SpatialJoinOptions options = new SpatialJoinOptions();
                options.SetPrimary(priConnInfo, priSchema, priClass);
                options.SetSecondary(secConnInfo, secTable);
                options.SetTarget(targetConnInfo, targetSchema, targetClass);
                options.PrimaryPrefix = priPrefix;
                options.SecondaryPrefix = secPrefix;

                foreach (XmlNode node in primaryNode.SelectNodes("PropertyList/Property"))
                {
                    options.AddProperty(node.InnerText);
                }

                foreach (XmlNode node in secondaryNode.SelectNodes("ColumnList/Column"))
                {
                    options.AddColumn(node.InnerText);
                }

                foreach (XmlNode node in joinNode.SelectNodes("Join"))
                {
                    string primary = node.Attributes["primary"].Value;
                    string secondary = node.Attributes["secondary"].Value;

                    options.AddJoinPair(primary, secondary);
                }
                options.Cardinality = (SpatialJoinCardinality)Enum.Parse(typeof(SpatialJoinCardinality), optionsNode.SelectSingleNode("JoinCardinality").InnerText);
                options.JoinType = (SpatialJoinType)Enum.Parse(typeof(SpatialJoinType), optionsNode.SelectSingleNode("JoinType").InnerText);

                task = new SpatialJoinTask(options);
                task.Name = doc.SelectSingleNode("//DatabaseJoinTask").Attributes["name"].Value;
            }
            catch (Exception ex)
            {
                AppConsole.WriteException(ex);
                if (!consoleMode)
                {
                    HostApplication.Instance.SpatialConnectionManager.RemoveConnection(priName);
                    HostApplication.Instance.SpatialConnectionManager.RemoveConnection(targetName);
                    HostApplication.Instance.DatabaseConnectionManager.RemoveConnection(secName);
                }
                return null;
            }

            return task;
        }

        private static void SaveJoinTask(SpatialJoinTask task, string configFile)
        {
            string xmlTemplate = Properties.Resources.DbJoinTask;
            StringBuilder propertyList = new StringBuilder();
            StringBuilder columnList = new StringBuilder();
            StringBuilder joinList = new StringBuilder();
            string[] propNames = task.Options.GetPropertyNames();
            string[] colNames = task.Options.GetColumnNames();
            string[] joinProps = task.Options.GetJoinedProperties();
            foreach (string prop in propNames)
            {
                propertyList.Append("<Property>" + prop + "</Property>\n");
            }
            foreach (string col in colNames)
            {
                columnList.Append("<Column>" + col + "</Column>\n");
            }
            foreach (string prop in joinProps)
            {
                joinList.Append("<Join primary=\""+ prop +"\" secondary=\""+ task.Options.GetMatchingColumn(prop) +"\" />\n");
            }
            string xml = string.Format(xmlTemplate,
                task.Name,
                task.Options.PrimarySource.Name,
                task.Options.PrimarySource.Connection.ConnectionInfo.ProviderName,
                task.Options.PrimarySource.Connection.ConnectionString,
                task.Options.SchemaName,
                task.Options.ClassName,
                propertyList.ToString(),
                task.Options.PrimaryPrefix,
                task.Options.SecondarySource.Name,
                task.Options.SecondarySource.Driver,
                task.Options.SecondarySource.Connection.ConnectionString,
                task.Options.TableName,
                columnList.ToString(),
                task.Options.SecondaryPrefix,
                task.Options.Target.Name,
                task.Options.Target.Connection.ConnectionInfo.ProviderName,
                task.Options.Target.Connection.ConnectionString,
                task.Options.TargetSchema,
                task.Options.TargetClassName,
                joinList.ToString(),
                task.Options.JoinType,
                task.Options.Cardinality);
            System.IO.File.Delete(configFile);
            using (XmlTextWriter writer = new XmlTextWriter(configFile, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteRaw(xml);
                writer.Close();
            }
        }

        /// <summary>
        /// Loads a bulk copy task
        /// </summary>
        /// <param name="doc">The loaded xml document</param>
        /// <param name="consoleMode">Is this application a console application</param>
        /// <returns>The Bulk Copy task, null if loading failed</returns>
        private static SpatialBulkCopyTask LoadBulkCopy(XmlDocument doc, bool consoleMode)
        {
            string srcName = HostApplication.Instance.SpatialConnectionManager.CreateUniqueName();
            string destName = HostApplication.Instance.SpatialConnectionManager.CreateUniqueName();
            
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
                    ISpatialConnectionMgr mgr = HostApplication.Instance.SpatialConnectionManager;
                    mgr.AddConnection(srcName, srcConn);
                    mgr.AddConnection(destName, destConn);
                }

                string name = doc.DocumentElement.Attributes["name"].Value;

                SpatialConnectionInfo srcConnInfo = new SpatialConnectionInfo(srcName, srcConn);
                SpatialConnectionInfo destConnInfo = new SpatialConnectionInfo(destName, destConn);

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
                    HostApplication.Instance.SpatialConnectionManager.RemoveConnection(srcName);
                    HostApplication.Instance.SpatialConnectionManager.RemoveConnection(destName);
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

        /// <summary>
        /// Saves a bulk copy task to file
        /// </summary>
        /// <param name="task">The bulk copy task</param>
        /// <param name="configFile">The file to save it to</param>
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
