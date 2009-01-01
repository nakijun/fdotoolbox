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
    public abstract class BaseDefinitionLoader
    {
        protected abstract FdoConnection CreateConnection(string provider, string connStr);

        protected BaseDefinitionLoader() { }

        public FdoBulkCopyOptions BulkCopyFromXml(string file, ref string name, bool owner)
        {
            FdoBulkCopyTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            def = (FdoBulkCopyTaskDefinition)ser.Deserialize(new StreamReader(file));

            return BulkCopyFromXml(def, ref name, owner);
        }

        public FdoBulkCopyOptions BulkCopyFromXml(FdoBulkCopyTaskDefinition def, ref string name, bool owner)
        {
            FdoBulkCopyOptions opt = new FdoBulkCopyOptions(
                CreateConnection(def.Source.Provider, def.Source.ConnectionString),
                CreateConnection(def.Target.Provider, def.Target.ConnectionString), owner);

            name = def.name;

            foreach (FdoClassMapping mapping in def.ClassMappings)
            {
                NameValueCollection exprs = new NameValueCollection();
                NameValueCollection maps = new NameValueCollection();
                foreach (FdoPropertyMapping pmap in mapping.Properties)
                {
                    maps.Add(pmap.SourceProperty, pmap.TargetProperty);
                }
                foreach (FdoExpressionMapping emap in mapping.Expressions)
                {
                    exprs.Add(emap.SourceExpression, emap.TargetProperty);
                }
                opt.AddClassCopyOption(mapping.SourceClass, mapping.TargetClass, maps, exprs, mapping.DeleteTarget, mapping.Filter);
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

        public FdoJoinOptions JoinFromXml(string file, ref string name, bool owner)
        {
            FdoJoinTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
            def = (FdoJoinTaskDefinition)ser.Deserialize(new StreamReader(file));

            return JoinFromXml(def, ref name, owner);
        }

        private FdoJoinOptions JoinFromXml(FdoJoinTaskDefinition def, ref string name, bool owner)
        {
            FdoJoinOptions opts = new FdoJoinOptions(owner);
            name = def.name;
            opts.GeometryProperty = def.JoinSettings.DesignatedGeometry;
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
                def.Right.FeatureSchema,
                def.Right.Class);

            opts.LeftPrefix = def.Left.Prefix;
            opts.RightPrefix = def.Right.Prefix;
            if (def.JoinSettings.SpatialPredicateSpecified)
                opts.SpatialJoinPredicate = (SpatialOperations)Enum.Parse(typeof(SpatialOperations), def.JoinSettings.SpatialPredicate.ToString());

            return opts;
        }

        public void ToXml(FdoJoinOptions opts, string name, string file)
        {
            FdoJoinTaskDefinition jdef = new FdoJoinTaskDefinition();
            jdef.name = name;
            jdef.JoinSettings = new FdoJoinSettings();
            jdef.JoinSettings.DesignatedGeometry = opts.GeometryProperty;
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

    public sealed class TaskDefinitionHelper
    {
        public const string BULKCOPYDEFINITION = ".BulkCopyDefinition";
        public const string JOINDEFINITION = ".JoinDefinition";

        public static bool IsDefinitionFile(string file)
        {
            if (!File.Exists(file))
                return false;

            string ext = Path.GetExtension(file).ToLower();

            return (ext == BULKCOPYDEFINITION.ToLower()) || (ext == JOINDEFINITION.ToLower());
        }

        public static bool IsBulkCopy(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == BULKCOPYDEFINITION.ToLower());
        }

        public static bool IsJoin(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == JOINDEFINITION.ToLower());
        }
    }

    public class DefinitionLoader : BaseDefinitionLoader
    {
        protected override FdoConnection CreateConnection(string provider, string connStr)
        {
            return new FdoConnection(provider, connStr);
        }
    }
}
