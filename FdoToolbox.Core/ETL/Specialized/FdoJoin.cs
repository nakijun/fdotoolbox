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

namespace FdoToolbox.Core.ETL.Specialized
{
    using Operations;
    using System.Collections.Specialized;
    using OSGeo.FDO.Geometry;
    using OSGeo.FDO.Filter;
    using OSGeo.FDO.Spatial;
    using FdoToolbox.Core.ETL.Pipelines;
    using FdoToolbox.Core.Feature;
    using OSGeo.FDO.Schema;
    using System.Collections.ObjectModel;
    using FdoToolbox.Core.Configuration;
    using System.Xml.Serialization;
    using System.IO;
    using FdoToolbox.Core.Utility;
    using System.Diagnostics;

    /// <summary>
    /// A specialized form of <see cref="EtlProcess"/> that merges
    /// two feature classes into one. The merged class is created
    /// before commencing the join
    /// </summary>
    public class FdoJoin : FdoSpecializedEtlProcess
    {
        private int _ReportFrequency = 50;

        /// <summary>
        /// Gets or sets the frequency at which progress feedback is made
        /// </summary>
        /// <value>The report frequency.</value>
        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }

        private FdoJoinOptions _options;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public FdoJoinOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoJoin"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public FdoJoin(FdoJoinOptions options) { _options = options; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoJoin"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="reportFrequency">The report frequency.</param>
        public FdoJoin(FdoJoinOptions options, int reportFrequency) : this(options) { _ReportFrequency = reportFrequency; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            var sw = new Stopwatch();
            sw.Start();
            _options.Validate();

            SendMessage("Setting up left and right sides of the join");

            // Abstract:
            //
            // The current built-in join infrastructure is very naive as it uses nested
            // loops. Being a O(m*n) operation, the slowdown becomes readily apparent
            // as the size of the data you're working with increases. As such, the
            // current infrastructure is woefully inadequate for large datasets.
            //
            // How can we fix this problem? We could try to implement various join
            // algorithms for different scenarios, which would be a laborious exercise
            // in itself.
            //
            // Or, we can just delegate this problem to the universal swiss army-knife of
            // databases, SQLite.
            //
            // SQLite has many things going for it, including:
            //  - Support for common types of joins (important!)
            //  - LIGHTING FAST insert performance. The current FdoInputOperation is already optimised for SQLite
            //  - LIGHTING FAST read performance 
            //  - Ability to use SQL to modify the database internals, such as creating indexes (FDO provider supports SQL commands)
            //
            // As such, SQLite is the perfect candidate for a temp data store to merge two
            // disparate data sources. The time spent setting up this temp SQLite database (ie. Copying "left" and
            // "right" side data into it) is negligible in the grand scheme of things.
            //
            // Process Overview:
            //
            // 1. Create temp SQLite database
            // 2. Pump left and right sides into this database
            // 3. Create indexes on join columns of both tables (IMPORTANT)
            // 4. Create a view encapsulating our join
            // 5. Copy this view out to our target
            //
            // Additional Notes:
            //
            // We will have to change our supported join types to line up with what SQLite supports, which
            // are:
            //  - INNER JOIN
            //  - LEFT OUTER JOIN
            // 
            // SQLite does not support RIGHT OUTER JOINs but these could be emulated by inverting the
            // "left" and "right" tables for the LEFT OUTER JOIN. FULL OUTER JOIN is not supporte by
            // SQLite so this will be removed from our API.
            //
            // Since this SQLite database is temporary, we don't bother with putting
            // the right spatial context in there. Spatial contexts do not (should not) affect
            // the underlying coordinates of any geometries moving to and from the data store.
            //
            // SQLite views by default are represented as non-Feature classes. Geometry properties
            // default to BLOB data types. To "fix" this we need to add a new entry to the geometry_columns
            // metadata table. This may produce an incorrect feature class (ie. Has 1-n geometry properties
            // but no designated one), this is okay as we only care that the properties are there and the 
            // temp-target property mappings check out.
            //
            // Although the implementation will change, the requirements remain the same, which are:
            //
            // 1. The target class must not already exist (as it will be created)
            // 2. If no designated geometry is specified, then the class definition will be FdoClass and not FdoFeatureClass

            ClassDefinition leftCls = null;
            ClassDefinition rightCls = null;
            ClassDefinition mergedCls = null;

            using (var leftSvc = _options.Left.Connection.CreateFeatureService())
            using (var rightSvc = _options.Right.Connection.CreateFeatureService())
            {
                leftCls = leftSvc.GetClassByName(_options.Left.SchemaName, _options.Left.ClassName);
                rightCls = rightSvc.GetClassByName(_options.Right.SchemaName, _options.Right.ClassName);

                if (leftCls == null)
                    throw new FdoETLException("Left class not found " + _options.Left.SchemaName + ":" + _options.Left.ClassName);

                if (rightCls == null)
                    throw new FdoETLException("Right class not found " + _options.Right.SchemaName + ":" + _options.Right.ClassName);

                var leftJoinProps = new List<string>(_options.JoinPairs.AllKeys);
                var rightJoinProps = new List<string>();
                foreach (var p in leftJoinProps)
                {
                    rightJoinProps.Add(_options.JoinPairs[p]);
                }

                var leftGeom = (!string.IsNullOrEmpty(_options.GeometryProperty) && _options.Side == JoinSide.Left) ? _options.GeometryProperty : null;
                var rightGeom = (!string.IsNullOrEmpty(_options.GeometryProperty) && _options.Side == JoinSide.Right) ? _options.GeometryProperty : null;

                PrepareClass(leftCls, _options.LeftProperties, leftJoinProps, _options.LeftPrefix, leftGeom);
                PrepareClass(rightCls, _options.RightProperties, rightJoinProps, _options.RightPrefix, rightGeom);

                mergedCls = CreateMergedClass(leftCls, rightCls);
            }

            var dprops = new NameValueCollection();
            dprops["File"] = Path.GetTempFileName();

            var tempSchema = new FeatureSchema("Default", "");
            var leftCopy = FdoFeatureService.CloneClass(leftCls);
            var rightCopy = FdoFeatureService.CloneClass(rightCls);

            string leftClassName = "LEFT_SIDE";
            string rightClassName = "RIGHT_SIDE";

            leftCopy.Name = leftClassName;
            rightCopy.Name = rightClassName;

            tempSchema.Classes.Add(leftCopy);
            tempSchema.Classes.Add(rightCopy);
            
            //Create SQLite database
            Register(new FdoCreateDataStoreOperation("OSGeo.SQLite", dprops, null));

            //Apply temp schema
            var tempConn = new FdoConnection("OSGeo.SQLite", "File=" + dprops["File"]);
            Register(new FdoApplySchemaOperation(tempConn, tempSchema));

#if DEBUG
            Register(new FdoSingleActionOperation(() => { SendMessage("Temp db created in: " + dprops["File"]); }));
#endif

            //Prep property mappings for bulk copy
            var leftMaps = new NameValueCollection();
            var rightMaps = new NameValueCollection();

            var leftQuery = new FeatureQueryOptions(leftCls.Name);
            var rightQuery = new FeatureQueryOptions(rightCls.Name);

            foreach (var leftp in _options.LeftProperties)
            {
                leftMaps.Add(leftp, leftp);
                leftQuery.AddFeatureProperty(leftp);
            }
            foreach (var rightp in _options.RightProperties)
            {
                rightMaps.Add(rightp, rightp);
                rightQuery.AddFeatureProperty(rightp);
            }
            
            //don't forget join keys
            foreach (string l in _options.JoinPairs.Keys)
            {
                string r = _options.JoinPairs[l];

                if (!_options.LeftProperties.Contains(l))
                {
                    leftQuery.AddFeatureProperty(l);
                    leftMaps.Add(l, l);
                }

                if (!_options.RightProperties.Contains(r))
                {
                    rightQuery.AddFeatureProperty(r);
                    rightMaps.Add(r, r);
                }

            }

            //don't forget geometry!
            if (!string.IsNullOrEmpty(_options.GeometryProperty))
            {
                if (_options.Side == JoinSide.Left)
                {
                    leftQuery.AddFeatureProperty(_options.GeometryProperty);
                    leftMaps.Add(_options.GeometryProperty, _options.GeometryProperty);
                }
                else
                {
                    rightQuery.AddFeatureProperty(_options.GeometryProperty);
                    rightMaps.Add(_options.GeometryProperty, _options.GeometryProperty);
                }
            }

            var copyLeftErrors = new List<Exception>();
            var copyRightErrors = new List<Exception>();
            var copyTargetErrors = new List<Exception>();

            //Copy left source
            ParameterlessAction copyLeft = () =>
            {
                var copy = ExpressUtility.CreateBulkCopy(
                    _options.Left.Connection,
                    tempConn,
                    _options.Left.SchemaName,
                    leftQuery,
                    tempSchema.Name,    //temp sqlite schema name
                    leftClassName,      //sqlite "left" class name
                    leftMaps);

                copy.ProcessMessage += delegate(object sender, MessageEventArgs e)
                {
                    SendMessage(e.Message);
                };
                copy.Execute();
                copyLeftErrors.AddRange(copy.GetAllErrors());
            };
            Register(new FdoSingleActionOperation(copyLeft));

            //Register(new FdoInputOperation(_options.Left.Connection, leftQuery));
            //Register(new FdoOutputOperation(tempConn, leftClassName, leftMaps));
            
            //Copy right source
            ParameterlessAction copyRight = () =>
            {
                var copy = ExpressUtility.CreateBulkCopy(
                    _options.Right.Connection,
                    tempConn,
                    _options.Right.SchemaName,
                    rightQuery,
                    tempSchema.Name,    //temp sqlite schema name
                    rightClassName,      //sqlite "right" class name
                    rightMaps);

                copy.ProcessMessage += delegate(object sender, MessageEventArgs e)
                {
                    SendMessage(e.Message);
                };
                copy.Execute();
                copyRightErrors.AddRange(copy.GetAllErrors());
            };
            Register(new FdoSingleActionOperation(copyRight));

            //Register(new FdoInputOperation(_options.Right.Connection, rightQuery));
            //Register(new FdoOutputOperation(tempConn, rightClassName, rightMaps));

            string srcClass = "VIEW_INPUT";

            //Create indexes on left and right sides to optimize read performance
            ParameterlessAction indexLeft = () =>
            {
                using (var svc = tempConn.CreateFeatureService())
                {
                    SendMessage("Creating left side index in temp db");
                    string sql = "CREATE INDEX IDX_LEFT_ID ON " + leftClassName + "(";
                    var tokens = new List<string>();
                    foreach (string p in _options.JoinPairs.Keys)
                    {
                        tokens.Add(p);
                    }
                    sql = sql + string.Join(", ", tokens.ToArray()) + ")";
                    svc.ExecuteSQLNonQuery(sql);
                }
            };
            ParameterlessAction indexRight = () =>
            {
                using (var svc = tempConn.CreateFeatureService())
                {
                    SendMessage("Creating right side index in temp db");
                    string sql = "CREATE INDEX IDX_RIGHT_ID ON " + rightClassName + "(";
                    var tokens = new List<string>();
                    foreach (string p in _options.JoinPairs.Keys)
                    {
                        string prop = _options.JoinPairs[p];
                        tokens.Add(prop);
                    }
                    sql = sql + string.Join(", ", tokens.ToArray()) + ")";
                    svc.ExecuteSQLNonQuery(sql);
                }
            };
            Register(new FdoSingleActionOperation(indexLeft));
            Register(new FdoSingleActionOperation(indexRight));

            //Create view
            ParameterlessAction createView = () =>
            {
                using (var svc = tempConn.CreateFeatureService())
                {
                    SendMessage("Creating view in temp db");
                    StringBuilder sql = new StringBuilder("CREATE VIEW ");
                    sql.Append(srcClass + " AS SELECT ");
                    foreach (var p in _options.LeftProperties)
                    {
                        sql.Append("l." + p + ", ");
                    }

                    if (!string.IsNullOrEmpty(_options.GeometryProperty))
                    {
                        if (_options.Side == JoinSide.Left)
                            sql.Append("l." + _options.GeometryProperty + ", ");
                        else
                            sql.Append("r." + _options.GeometryProperty + ", ");
                    }

                    int rc = _options.RightProperties.Count;
                    int i = 0;
                    foreach (var p in _options.RightProperties)
                    {
                        if (i == rc - 1)
                            sql.Append("r." + p + " FROM ");
                        else
                            sql.Append("r." + p + ", ");
                        i++;
                    }
                    sql.Append(leftClassName + " l ");

                    switch (_options.JoinType)
                    {
                        case FdoJoinType.Inner:
                            sql.Append("INNER JOIN " + rightClassName + " r ON ");
                            break;
                        case FdoJoinType.Left:
                            sql.Append("LEFT OUTER JOIN " + rightClassName + " r ON ");
                            break;
                        default:
                            throw new FdoETLException("Unsupported join type: " + _options.JoinType);
                    }

                    rc = _options.JoinPairs.Count;
                    i = 0;
                    foreach (string left in _options.JoinPairs.Keys)
                    {
                        string right = _options.JoinPairs[left];

                        if (i == rc - 1)
                            sql.Append("l." + left + " = r." + right);
                        else
                            sql.Append("l." + left + " = r." + right + " AND ");

                        i++;
                    }

                    svc.ExecuteSQLNonQuery(sql.ToString());
                }
            };
            Register(new FdoSingleActionOperation(createView));

            //Hack FDO metadata to make this a feature class
            if (!string.IsNullOrEmpty(_options.GeometryProperty))
            {
                ParameterlessAction reg = () =>
                {
                    using (var svc = tempConn.CreateFeatureService())
                    {
                        SendMessage("Exposing view as a FDO feature class");
                        string sql = "INSERT INTO geometry_columns(f_table_name, f_geometry_column, geometry_type, geometry_dettype, coord_dimension, srid, geometry_format) VALUES('" + srcClass + "','" + _options.GeometryProperty + "',15,7743,0,0,'FGF')";
                        svc.ExecuteSQLNonQuery(sql);
                    }
                };
                Register(new FdoSingleActionOperation(reg));
            }

            //Copy view to target
            ParameterlessAction applyTarget = () =>
            {
                using (var svc = _options.Target.Connection.CreateFeatureService())
                {
                    SendMessage("Fetching target schema");
                    var schema = svc.GetSchemaByName(_options.Target.SchemaName);

                    IncompatibleClass cls;
                    if (!svc.CanApplyClass(mergedCls, out cls))
                    {
                        SendMessage("Fixing incompatibilities in merged class");
                        mergedCls = svc.AlterClassDefinition(mergedCls, cls);
                    }

                    SendMessage("Adding merged class to target schema");
                    schema.Classes.Add(mergedCls);
                    SendMessage("Applying modified target schema");
                    svc.ApplySchema(schema);
                }
            };
            Register(new FdoSingleActionOperation(applyTarget));

            var tempQuery = new FeatureQueryOptions("VIEW_INPUT");
            var targetMapping = new NameValueCollection();
            foreach(PropertyDefinition p in mergedCls.Properties)
            {
                tempQuery.AddFeatureProperty(p.Name);
                //Target class is a replica of the temp one, so all properties
                //have the same name in both source and target
                targetMapping[p.Name] = p.Name;
            }
            
            ParameterlessAction copyToTarget = () =>
            {
                var copy = ExpressUtility.CreateBulkCopy(
                    tempConn,
                    _options.Target.Connection,
                    tempSchema.Name,
                    tempQuery,
                    _options.Target.SchemaName,
                    _options.Target.ClassName,
                    targetMapping);

                copy.ProcessMessage += delegate(object sender, MessageEventArgs e)
                {
                    SendMessage(e.Message);
                };
                copy.Execute();
                copyTargetErrors.AddRange(copy.GetAllErrors());
                sw.Stop();
            };
            Register(new FdoSingleActionOperation(copyToTarget));
            
            //Log all errors
            ParameterlessAction logErrors = () =>
            {
                SendMessage(copyLeftErrors.Count + " errors encountered copying left source to temp db");
                _allErrors.AddRange(copyLeftErrors);
                SendMessage(copyRightErrors.Count + " errors encountered copying right source to temp db");
                _allErrors.AddRange(copyRightErrors);
                SendMessage(copyTargetErrors.Count + " errors encountered copying left source to target");
                _allErrors.AddRange(copyTargetErrors);
                SendMessage("Join Operation completed in " + sw.Elapsed.ToString());
            };
            Register(new FdoSingleActionOperation(logErrors));
        }

        private List<Exception> _allErrors = new List<Exception>();

        public override IEnumerable<Exception> GetAllErrors()
        {
            return _allErrors;
        }

        private static void PrepareClass(ClassDefinition cls, ICollection<string> propNames, ICollection<string> joinProps, string prefix, string geomProp)
        {
            var props = cls.Properties;
            var remove = new List<PropertyDefinition>();
            foreach (PropertyDefinition p in props)
            {
                if (!propNames.Contains(p.Name) && !joinProps.Contains(p.Name))
                {
                    if (!string.IsNullOrEmpty(geomProp) && p.Name == geomProp)
                        continue;

                    remove.Add(p);
                }
            }
            foreach (PropertyDefinition p in remove)
            {
                props.Remove(p);
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                foreach (PropertyDefinition p in props)
                {
                    p.Name = prefix + p.Name;
                }
            }
        }

        private ClassDefinition CreateMergedClass(ClassDefinition leftCls, ClassDefinition rightCls)
        {
            ClassDefinition cls = null;

            if (!string.IsNullOrEmpty(_options.GeometryProperty))
                cls = new FeatureClass(_options.Target.ClassName, "");
            else
                cls = new Class(_options.Target.ClassName, "");

            var props = cls.Properties;
            foreach (PropertyDefinition p in leftCls.Properties)
            {
                int idx = props.IndexOf(p.Name);
                if (idx < 0)
                    props.Add(FdoFeatureService.CloneProperty(p));
            }
            foreach (PropertyDefinition p in rightCls.Properties)
            {
                int idx = props.IndexOf(p.Name);
                if (idx < 0)
                    props.Add(FdoFeatureService.CloneProperty(p));
            }

            DataPropertyDefinition fid = new DataPropertyDefinition("FID", "Autogenerated ID");
            fid.DataType = DataType.DataType_Int32;
            fid.IsAutoGenerated = true;
            fid.Nullable = false;

            props.Add(fid);
            cls.IdentityProperties.Add(fid);

            if (!string.IsNullOrEmpty(_options.GeometryProperty))
            {
                int idx = props.IndexOf(_options.GeometryProperty);
                if (idx < 0)
                {
                    throw new FdoETLException("Property not found in merged class: " + _options.GeometryProperty);
                }
                else
                {
                    var p = props[idx];
                    if (p.PropertyType != PropertyType.PropertyType_GeometricProperty)
                        throw new FdoETLException("Designated property is not a geometry property: " + _options.GeometryProperty);

                    ((FeatureClass)cls).GeometryProperty = (GeometricPropertyDefinition)p;
                }
            }

            return cls;
        }

        /// <summary>
        /// Called when [feature processed].
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % this.ReportFrequency == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[Join => {0}]: {1} features processed", className, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when [finished processing].
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoBatchedOutputOperation)
            {
                FdoBatchedOutputOperation bop = op as FdoBatchedOutputOperation;
                string className = bop.ClassName;
                SendMessageFormatted("[Join => {0}]: {1} features processed in {2}", className, bop.BatchInsertTotal, op.Statistics.Duration.ToString());
            }
            else if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[Join => {0}]: {1} features processed in {2}", className, op.Statistics.OutputtedRows, op.Statistics.Duration.ToString());
            }
        }

        /// <summary>
        /// Saves this process to a file
        /// </summary>
        /// <param name="file">The file to save this process to</param>
        /// <param name="name">The name of the process</param>
        public override void Save(string file, string name)
        {
            FdoJoinTaskDefinition join = new FdoJoinTaskDefinition();
            join.name = name;
            join.Left = new FdoJoinSource();
            join.Right = new FdoJoinSource();
            join.Target = new FdoJoinTarget();
            join.JoinSettings = new FdoJoinSettings();

            join.Left.Class = _options.Left.ClassName;
            join.Left.ConnectionString = _options.Left.Connection.ConnectionString;
            join.Left.FeatureSchema = _options.Left.SchemaName;
            join.Left.Prefix = _options.LeftPrefix;
            join.Left.PropertyList = new List<string>(_options.LeftProperties).ToArray();
            join.Left.Provider = _options.Left.Connection.Provider;

            join.Right.Class = _options.Right.ClassName;
            join.Right.ConnectionString = _options.Right.Connection.ConnectionString;
            join.Right.FeatureSchema = _options.Right.SchemaName;
            join.Right.Prefix = _options.RightPrefix;
            join.Right.PropertyList = new List<string>(_options.RightProperties).ToArray();
            join.Right.Provider = _options.Right.Connection.Provider;

            join.Target.Class = _options.Target.ClassName;
            join.Target.ConnectionString = _options.Target.Connection.ConnectionString;
            join.Target.FeatureSchema = _options.Target.SchemaName;
            join.Target.Provider = _options.Target.Connection.Provider;

            join.JoinSettings.DesignatedGeometry = new FdoDesignatedGeometry();
            if (!string.IsNullOrEmpty(_options.GeometryProperty))
            {
                join.JoinSettings.DesignatedGeometry.Property = _options.GeometryProperty;
                join.JoinSettings.DesignatedGeometry.Side = _options.Side;
            }
            if (_options.SpatialJoinPredicate.HasValue)
            {
                join.JoinSettings.SpatialPredicate = (SpatialPredicate)Enum.Parse(typeof(SpatialPredicate), _options.SpatialJoinPredicate.Value.ToString());
            }
            join.JoinSettings.ForceOneToOne = _options.ForceOneToOne;
            join.JoinSettings.JoinType = (JoinType)Enum.Parse(typeof(JoinType), _options.JoinType.ToString());
            List<JoinKey> keys = new List<JoinKey>();
            foreach (string key in _options.JoinPairs.Keys)
            {
                JoinKey k = new JoinKey();
                k.left = key;
                k.right = _options.JoinPairs[key];
                keys.Add(k);
            }
            join.JoinSettings.JoinKeys = keys.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(FdoJoinTaskDefinition));
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                serializer.Serialize(writer, join);
            }
        }

        /// <summary>
        /// Determines if this process is capable of persistence
        /// </summary>
        /// <value></value>
        public override bool CanSave
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the file extension associated with this process. For tasks where <see cref="CanSave"/> is
        /// false, an empty string is returned
        /// </summary>
        /// <returns></returns>
        public override string GetFileExtension()
        {
            return TaskDefinitionHelper.JOINDEFINITION;
        }

        /// <summary>
        /// Gets a description of this process
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return ResourceUtil.GetString("DESC_JOIN_DEFINITION");
        }
    }
}
