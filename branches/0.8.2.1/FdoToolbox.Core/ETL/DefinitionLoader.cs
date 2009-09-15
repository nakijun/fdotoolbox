#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.Feature;
using System.Xml.Serialization;
using System.IO;
using OSGeo.FDO.Filter;
using System.Xml;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Task definition loader base class
    /// </summary>
    public abstract class BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <returns></returns>
        protected abstract FdoConnection CreateConnection(string provider, string connStr);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDefinitionLoader"/> class.
        /// </summary>
        protected BaseDefinitionLoader() { }

        /// <summary>
        /// Loads bulk copy options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(string file, ref string name, bool owner)
        {
            FdoBulkCopyTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            def = (FdoBulkCopyTaskDefinition)ser.Deserialize(new StreamReader(file));

            return BulkCopyFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="def">The deserialized definition.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(FdoBulkCopyTaskDefinition def, ref string name, bool owner)
        {
            FdoBulkCopyOptions opt = new FdoBulkCopyOptions(
                CreateConnection(def.Source.Provider, def.Source.ConnectionString),
                CreateConnection(def.Target.Provider, def.Target.ConnectionString), owner);

            name = def.name;

            opt.SourceSchema = def.Source.Schema;
            opt.TargetSchema = def.Target.Schema;

            foreach (FdoClassMapping mapping in def.ClassMappings)
            {   
                FdoClassCopyOptions copt = new FdoClassCopyOptions(mapping.SourceClass, mapping.TargetClass);
                foreach (FdoPropertyMapping pmap in mapping.Properties)
                {
                    copt.AddPropertyMapping(pmap.SourceProperty, pmap.TargetProperty);
                }
                foreach (FdoExpressionMapping emap in mapping.Expressions)
                {
                    string alias = emap.SourceAlias;
                    string expr = emap.SourceExpression;
                    string targetProp = emap.TargetProperty;
                    copt.AddSourceExpression(alias, expr, targetProp);
                }
                copt.DeleteTarget = mapping.DeleteTarget;
                copt.SourceFilter = mapping.Filter;
                opt.AddClassCopyOption(copt);
            }

            if (def.Source.SpatialContextList.Length > 0)
            {
                using (FdoFeatureService service = opt.SourceConnection.CreateFeatureService())
                {
                    foreach (string str in def.Source.SpatialContextList)
                    {
                        SpatialContextInfo ctx = service.GetSpatialContext(str);
                        if(ctx != null)
                            opt.AddSourceSpatialContext(ctx);
                    }
                }
            }

            if (def.Target.BatchSizeSpecified)
                opt.BatchSize = def.Target.BatchSize;

            return opt;
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoJoinOptions JoinFromXml(string file, ref string name, bool owner)
        {
            FdoJoinTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
            def = (FdoJoinTaskDefinition)ser.Deserialize(new StreamReader(file));

            return JoinFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="def">The deserialized definition</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        private FdoJoinOptions JoinFromXml(FdoJoinTaskDefinition def, ref string name, bool owner)
        {
            FdoJoinOptions opts = new FdoJoinOptions(owner);
            name = def.name;
            if (def.JoinSettings.DesignatedGeometry != null)
            {
                opts.GeometryProperty = def.JoinSettings.DesignatedGeometry.Property;
                opts.Side = def.JoinSettings.DesignatedGeometry.Side;
            }
            foreach (JoinKey key in def.JoinSettings.JoinKeys)
            {
                opts.JoinPairs.Add(key.left, key.right);
            }
            opts.JoinType = (FdoJoinType)Enum.Parse(typeof(FdoJoinType), def.JoinSettings.JoinType.ToString());
            opts.SetLeft(
                CreateConnection(def.Left.Provider, def.Left.ConnectionString),
                def.Left.FeatureSchema,
                def.Left.Class);
            foreach (string p in def.Left.PropertyList)
            {
                opts.AddLeftProperty(p);
            }
            opts.SetRight(
                CreateConnection(def.Right.Provider, def.Right.ConnectionString),
                def.Right.FeatureSchema,
                def.Right.Class);
            foreach (string p in def.Right.PropertyList)
            {
                opts.AddRightProperty(p);
            }

            opts.SetTarget(
                CreateConnection(def.Target.Provider, def.Target.ConnectionString),
                def.Target.FeatureSchema,
                def.Target.Class);

            opts.LeftPrefix = def.Left.Prefix;
            opts.RightPrefix = def.Right.Prefix;
            opts.ForceOneToOne = def.JoinSettings.ForceOneToOne;
            if (def.JoinSettings.SpatialPredicateSpecified)
                opts.SpatialJoinPredicate = (SpatialOperations)Enum.Parse(typeof(SpatialOperations), def.JoinSettings.SpatialPredicate.ToString());

            return opts;
        }

        /// <summary>
        /// Saves the join options to xml
        /// </summary>
        /// <param name="opts">The opts.</param>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        public void ToXml(FdoJoinOptions opts, string name, string file)
        {
            FdoJoinTaskDefinition jdef = new FdoJoinTaskDefinition();
            jdef.name = name;
            jdef.JoinSettings = new FdoJoinSettings();
            if (!string.IsNullOrEmpty(opts.GeometryProperty))
            {
                jdef.JoinSettings.DesignatedGeometry = new FdoDesignatedGeometry();
                jdef.JoinSettings.DesignatedGeometry.Property = opts.GeometryProperty;
                jdef.JoinSettings.DesignatedGeometry.Side = opts.Side;
            }
            List<JoinKey> keys = new List<JoinKey>();
            foreach (string left in opts.JoinPairs.Keys)
            {
                JoinKey key = new JoinKey();
                key.left = left;
                key.right = opts.JoinPairs[left];
                keys.Add(key);
            }
            jdef.JoinSettings.JoinKeys = keys.ToArray();

            jdef.Left = new FdoJoinSource();
            jdef.Left.Class = opts.Left.ClassName;
            jdef.Left.ConnectionString = opts.Left.Connection.ConnectionString;
            jdef.Left.FeatureSchema = opts.Left.SchemaName;
            jdef.Left.Prefix = opts.LeftPrefix;
            jdef.Left.PropertyList = new List<string>(opts.LeftProperties).ToArray();
            jdef.Left.Provider = opts.Left.Connection.Provider;

            jdef.Right = new FdoJoinSource();
            jdef.Right.Class = opts.Right.ClassName;
            jdef.Right.ConnectionString = opts.Right.Connection.ConnectionString;
            jdef.Right.FeatureSchema = opts.Right.SchemaName;
            jdef.Right.Prefix = opts.RightPrefix;
            jdef.Right.PropertyList = new List<string>(opts.RightProperties).ToArray();
            jdef.Right.Provider = opts.Right.Connection.Provider;

            jdef.Target = new FdoJoinTarget();
            jdef.Target.Class = opts.Target.ClassName;
            jdef.Target.ConnectionString = opts.Target.Connection.ConnectionString;
            jdef.Target.FeatureSchema = opts.Target.SchemaName;
            jdef.Target.Provider = opts.Target.Connection.Provider;

            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
                ser.Serialize(writer, jdef);
            }
        }
    }

    /// <summary>
    /// Helper class for Task Definition serialization
    /// </summary>
    public sealed class TaskDefinitionHelper
    {
        /// <summary>
        /// File extension for bulk copy definitions
        /// </summary>
        public const string BULKCOPYDEFINITION = ".BulkCopyDefinition";
        /// <summary>
        /// File extension for join definitions
        /// </summary>
        public const string JOINDEFINITION = ".JoinDefinition";

        /// <summary>
        /// Determines whether [the specified file] is a definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefinitionFile(string file)
        {
            if (!File.Exists(file))
                return false;

            string ext = Path.GetExtension(file).ToLower();

            return (ext == BULKCOPYDEFINITION.ToLower()) || (ext == JOINDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether [the specified file] is a bulk copy definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a bulk copy definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBulkCopy(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == BULKCOPYDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether the specified file is a join definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the specified file is a join definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJoin(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == JOINDEFINITION.ToLower());
        }
    }

    /// <summary>
    /// Standalone task definition loader. Use this loader when using only the Core API. 
    /// Do not use this loader in the context of the FDO Toolbox application.
    /// </summary>
    public class DefinitionLoader : BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <returns></returns>
        protected override FdoConnection CreateConnection(string provider, string connStr)
        {
            return new FdoConnection(provider, connStr);
        }
    }
}
