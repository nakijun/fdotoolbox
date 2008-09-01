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
using FdoToolbox.Core.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.IO
{
    /// <summary>
    /// Utility class to parse and load task definitions
    /// </summary>
    public sealed class TaskLoader
    {
        private TaskLoader() { }

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
                return LoadBulkCopy(configFile, consoleMode);
            else if (joinNode != null)
                return LoadJoinTask(configFile, consoleMode);
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
                case TaskType.SpatialBulkCopy:
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

        private static ITask LoadJoinTask(string configFile, bool consoleMode)
        {
            ITask task = null;
            string priName = null;
            string secName = null;
            string targetName = null;
            try
            {
                DatabaseJoinTask djt = null;
                XmlSerializer serializer = new XmlSerializer(typeof(DatabaseJoinTask));
                using (StreamReader reader = new StreamReader(configFile))
                {
                    djt = (DatabaseJoinTask)serializer.Deserialize(reader);
                }

                priName = djt.PrimarySource.name;
                secName = djt.SecondarySource.name;
                targetName = djt.Target.name;

                IConnection priConn = FeatureAccessManager.GetConnectionManager().CreateConnection(djt.PrimarySource.Provider);
                priConn.ConnectionString = djt.PrimarySource.ConnectionString;

                IConnection targetConn = FeatureAccessManager.GetConnectionManager().CreateConnection(djt.Target.Provider);
                targetConn.ConnectionString = djt.Target.ConnectionString;

                FdoConnectionInfo priConnInfo = new FdoConnectionInfo(priName, priConn);
                FdoConnectionInfo targetConnInfo = new FdoConnectionInfo(targetName, targetConn);
                DbConnectionInfo secConnInfo = new DbConnectionInfo(secName, new OleDbConnection(djt.SecondarySource.ConnectionString));

                if (consoleMode)
                {
                    priConnInfo.InternalConnection.Open();
                    secConnInfo.Connection.Open();
                    targetConnInfo.InternalConnection.Open();
                }
                else
                {
                    AppGateway.RunningApplication.SpatialConnectionManager.AddConnection(priConnInfo.Name, priConnInfo.InternalConnection);
                    AppGateway.RunningApplication.SpatialConnectionManager.AddConnection(targetConnInfo.Name, targetConnInfo.InternalConnection);
                    AppGateway.RunningApplication.DatabaseConnectionManager.AddConnection(secConnInfo);
                }

                SpatialJoinOptions options = new SpatialJoinOptions();
                options.SetPrimary(priConnInfo, djt.PrimarySource.FeatureSchema, djt.PrimarySource.Class);
                options.SetSecondary(secConnInfo, djt.SecondarySource.Table);
                options.SetTarget(targetConnInfo, djt.Target.FeatureSchema, djt.Target.Class);
                options.PrimaryPrefix = djt.PrimarySource.Prefix;
                options.SecondaryPrefix = djt.SecondarySource.Prefix;

                foreach (string prop in djt.PrimarySource.PropertyList)
                {
                    options.AddProperty(prop);
                }

                foreach (string column in djt.SecondarySource.ColumnList)
                {
                    options.AddColumn(column);
                }

                foreach (Join j in djt.Joins)
                {
                    options.AddJoinPair(j.primary, j.secondary);
                }
                options.Cardinality = djt.JoinOptions.JoinCardinality;
                options.JoinType = djt.JoinOptions.JoinType;

                task = new SpatialJoinTask(options);
                task.Name = djt.name;
            }
            catch (Exception ex)
            {
                AppConsole.WriteException(ex);
                if (!consoleMode)
                {
                    AppGateway.RunningApplication.SpatialConnectionManager.RemoveConnection(priName);
                    AppGateway.RunningApplication.SpatialConnectionManager.RemoveConnection(targetName);
                    AppGateway.RunningApplication.DatabaseConnectionManager.RemoveConnection(secName);
                }
                return null;
            }

            return task;
        }

        private static void SaveJoinTask(SpatialJoinTask task, string configFile)
        {
            DatabaseJoinTask djt = new DatabaseJoinTask();
            djt.name = task.Name;
            djt.PrimarySource = new PrimarySource();
            djt.SecondarySource = new SecondarySource();
            djt.Target = new JoinTarget();
            djt.JoinOptions = new JoinOptions();
            List<Join> joins = new List<Join>();

            djt.PrimarySource.name = task.Options.PrimarySource.Name;
            djt.PrimarySource.Provider = task.Options.PrimarySource.InternalConnection.ConnectionInfo.ProviderName;
            djt.PrimarySource.ConnectionString = task.Options.PrimarySource.InternalConnection.ConnectionString;
            djt.PrimarySource.FeatureSchema = task.Options.SchemaName;
            djt.PrimarySource.Class = task.Options.ClassName;
            djt.PrimarySource.Prefix = task.Options.PrimaryPrefix;
            djt.PrimarySource.PropertyList = task.Options.GetPropertyNames();
            
            djt.SecondarySource.ColumnList = task.Options.GetColumnNames();
            djt.SecondarySource.ConnectionString = task.Options.SecondarySource.Connection.ConnectionString;
            
            djt.SecondarySource.name = task.Options.SecondarySource.Name;
            djt.SecondarySource.Prefix = task.Options.SecondaryPrefix;
            djt.SecondarySource.Table = task.Options.TableName;

            djt.Target.Class = task.Options.TargetClassName;
            djt.Target.ConnectionString = task.Options.Target.InternalConnection.ConnectionString;
            djt.Target.FeatureSchema = task.Options.TargetSchema;
            djt.Target.name = task.Options.Target.Name;
            djt.Target.Provider = task.Options.Target.InternalConnection.ConnectionInfo.ProviderName;
           
            foreach (string prop in task.Options.GetJoinedProperties())
            {
                Join j = new Join();
                j.primary = prop;
                j.secondary = task.Options.GetMatchingColumn(prop);
                joins.Add(j);
            }
            djt.Joins = joins.ToArray();

            djt.JoinOptions.JoinCardinality = task.Options.Cardinality;
            djt.JoinOptions.JoinType = task.Options.JoinType;

            XmlSerializer serialzier = new XmlSerializer(typeof(DatabaseJoinTask));
            using (XmlTextWriter writer = new XmlTextWriter(configFile, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;
                serialzier.Serialize(writer, djt);
            }
        }

        /// <summary>
        /// Loads a bulk copy task
        /// </summary>
        /// <param name="doc">The loaded xml document</param>
        /// <param name="consoleMode">Is this application a console application</param>
        /// <returns>The Bulk Copy task, null if loading failed</returns>
        private static SpatialBulkCopyTask LoadBulkCopy(string configFile, bool consoleMode)
        {
            string srcName = AppGateway.RunningApplication.SpatialConnectionManager.CreateUniqueName();
            string destName = AppGateway.RunningApplication.SpatialConnectionManager.CreateUniqueName();
            
            try
            {
                BulkCopyTask bcp = null;
                XmlSerializer serializer = new XmlSerializer(typeof(BulkCopyTask));
                using (StreamReader reader = new StreamReader(configFile))
                {
                    bcp = (BulkCopyTask)serializer.Deserialize(reader);
                }

                IConnection srcConn = FeatureAccessManager.GetConnectionManager().CreateConnection(bcp.Source.Provider);
                IConnection destConn = FeatureAccessManager.GetConnectionManager().CreateConnection(bcp.Target.Provider);

                srcConn.ConnectionString = bcp.Source.ConnectionString;
                destConn.ConnectionString = bcp.Target.ConnectionString;

                srcName = bcp.Source.name;
                destName = bcp.Target.name;

                if (consoleMode)
                {
                    srcConn.Open();
                    destConn.Open();
                }
                else
                {
                    ISpatialConnectionMgr mgr = AppGateway.RunningApplication.SpatialConnectionManager;
                    mgr.AddConnection(srcName, srcConn);
                    mgr.AddConnection(destName, destConn);
                }

                string name = bcp.name;

                FdoConnectionInfo srcConnInfo = new FdoConnectionInfo(srcName, srcConn);
                FdoConnectionInfo destConnInfo = new FdoConnectionInfo(destName, destConn);

                SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(srcConnInfo, destConnInfo);
                options.SourceSchemaName = bcp.Source.Schema;
                options.TargetSchemaName = bcp.Target.Schema;
                options.CoerceDataTypes = bcp.BulkCopyOptions.CoerceDataTypes;
                options.CopySpatialContexts = bcp.BulkCopyOptions.CopySpatialContexts;
                options.GlobalSpatialFilter = bcp.BulkCopyOptions.GlobalSpatialFilter;
                int size = 0;
                if(int.TryParse(bcp.BulkCopyOptions.BatchInsertSize, out size))
                    options.BatchInsertSize = size;
                else
                    options.BatchInsertSize = 0;

                if (options.CopySpatialContexts)
                {
                    foreach (string context in bcp.Source.SpatialContextList)
                    {
                        options.AddSourceSpatialContext(context);
                    }
                }

                FeatureService srcService = new FeatureService(srcConn);
                FeatureSchemaCollection schemas = srcService.DescribeSchema();
                foreach (Mapping m in bcp.ClassMappings)
                {
                    ClassDefinition classDef = FindClass(schemas, m.SourceClass);
                    if (classDef == null)
                        throw new TaskLoaderException("Unable to find SourceClass " + m.SourceClass);
                    ClassCopyOptions copt = new ClassCopyOptions(classDef);
                    copt.TargetClassName = m.TargetClass;
                    copt.DeleteClassData = m.DeleteTarget;

                    if (!string.IsNullOrEmpty(m.SourceFilter))
                        copt.AttributeFilter = m.SourceFilter;

                    foreach (PropertyMapping pm in m.Properties)
                    {
                        copt.AddProperty(GetPropertyDefinition(classDef, pm.SourceProperty), pm.TargetProperty);
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
                    AppGateway.RunningApplication.SpatialConnectionManager.RemoveConnection(srcName);
                    AppGateway.RunningApplication.SpatialConnectionManager.RemoveConnection(destName);
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
            BulkCopyTask bcp = new BulkCopyTask();
            bcp.BulkCopyOptions = new BulkCopyOptions();
            bcp.name = task.Name;
            bcp.Source = new CopySource();
            bcp.Target = new CopyTarget();
            List<Mapping> mappings = new List<Mapping>();

            bcp.Source.ConnectionString = task.Options.Source.InternalConnection.ConnectionString;
            bcp.Source.name = task.Options.Source.Name;
            bcp.Source.Provider = task.Options.Source.InternalConnection.ConnectionInfo.ProviderName;
            bcp.Source.Schema = task.Options.SourceSchemaName;

            bcp.Target.ConnectionString = task.Options.Target.InternalConnection.ConnectionString;
            bcp.Target.name = task.Options.Target.Name;
            bcp.Target.Provider = task.Options.Target.InternalConnection.ConnectionInfo.ProviderName;
            bcp.Target.Schema = task.Options.TargetSchemaName;
            
            ReadOnlyCollection<ClassCopyOptions> cOptions = task.Options.ClassCopyOptions;
            foreach (ClassCopyOptions copt in cOptions)
            {
                Mapping m = new Mapping();
                m.DeleteTarget = copt.DeleteClassData;
                m.SourceClass = copt.ClassName;
                m.SourceFilter = copt.AttributeFilter;
                m.TargetClass = copt.TargetClassName;
                List<PropertyMapping> pms = new List<PropertyMapping>();
                foreach (string propertyName in copt.PropertyNames)
                {
                    string tp = copt.GetTargetPropertyName(propertyName);
                    PropertyMapping pm = new PropertyMapping();
                    pm.SourceProperty = propertyName;
                    pm.TargetProperty = tp;
                    pms.Add(pm);
                }
                m.Properties = pms.ToArray();

                mappings.Add(m);
            }

            bcp.ClassMappings = mappings.ToArray();
            bcp.BulkCopyOptions.CopySpatialContexts = task.Options.CopySpatialContexts;
            bcp.BulkCopyOptions.CoerceDataTypes = task.Options.CoerceDataTypes;
            bcp.BulkCopyOptions.GlobalSpatialFilter = task.Options.GlobalSpatialFilter;
            bcp.BulkCopyOptions.BatchInsertSize = task.Options.BatchInsertSize.ToString();
            if (task.Options.CopySpatialContexts)
            {
                List<string> contexts = new List<string>();
                foreach (string name in task.Options.SourceSpatialContexts)
                {
                    contexts.Add(name);
                }
                bcp.Source.SpatialContextList = contexts.ToArray();
            }

            XmlSerializer serialzier = new XmlSerializer(typeof(BulkCopyTask));
            using (XmlTextWriter writer = new XmlTextWriter(configFile, Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;
                serialzier.Serialize(writer, bcp);
            }
        }
    }
}
