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

    /// <summary>
    /// A specialized form of <see cref="EtlProcess"/> that merges
    /// two feature classes into one. The merged class is created
    /// before commencing the join
    /// </summary>
    public class FdoJoin : FdoSpecializedEtlProcess
    {
        private FdoJoinOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FdoJoin(FdoJoinOptions options) { _options = options; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            _options.Validate();

            ConcreteJoin join = new ConcreteJoin(_options.JoinType);

            SendMessage("Setting up left and right sides of the join");

            //Set up left and right sides of the join
            FdoInputOperation left = new FdoInputOperation(_options.Left.Connection, _options.CreateLeftQuery());
            FdoInputOperation right = new FdoInputOperation(_options.Right.Connection, _options.CreateRightQuery());
            join.Left(left);
            join.Right(right);
            join.LeftProperties = _options.LeftProperties;
            join.RightProperties = _options.RightProperties;
            join.LeftPrefix = _options.LeftPrefix;
            join.RightPrefix = _options.RightPrefix;
            join.GeometryProperty = _options.GeometryProperty;
            join.ForceOneToOne = _options.ForceOneToOne;
            foreach (string leftProp in _options.JoinPairs.Keys)
            {
                join.AddAttributeJoinCondition(leftProp, _options.JoinPairs[leftProp]);
            }
            if (_options.SpatialJoinPredicate.HasValue)
                join.SpatialPredicate = _options.SpatialJoinPredicate.Value;
            join.PrepareForExecution(new SingleThreadedPipelineExecuter());

            IFdoOperation output = null;

            //Create target class. The schema must already exist, but the class must *not* already exist.
            using (FdoFeatureService service = _options.Target.Connection.CreateFeatureService())
            {
                //Get target schema
                FeatureSchema schema = service.GetSchemaByName(_options.Target.SchemaName);

                SendMessageFormatted("Creating joined class: {0}", _options.Target.ClassName);
                //Create target class
                ClassDefinition cd = CreateJoinedFeatureClass();
                schema.Classes.Add(cd);

                SendMessageFormatted("Applying altered schema: {0}", _options.Target.SchemaName);

                //Apply altered schema
                IncompatibleSchema incSchema;
                if (!service.CanApplySchema(schema, out incSchema))
                {
                    schema = service.AlterSchema(schema, incSchema);
                    service.ApplySchema(schema);
                }
                else
                {
                    service.ApplySchema(schema);
                }

                //Check batch support
                if (_options.BatchSize > 0 && !service.SupportsBatchInsertion())
                {
                    SendMessage("Batch insert not supported. Using regular inserts");
                    _options.BatchSize = 0;
                }
            }

            if (_options.BatchSize > 0)
                output = new FdoBatchedOutputOperation(_options.Target.Connection, _options.Target.ClassName, _options.BatchSize);
            else
                output = new FdoOutputOperation(_options.Target.Connection, _options.Target.ClassName);

            //Register operations
            Register(join);
            Register(output);
        }

        private ClassDefinition CreateJoinedFeatureClass()
        {
            FeatureClass fc = new FeatureClass(_options.Target.ClassName, string.Empty);

            //Create identity property
            DataPropertyDefinition id = new DataPropertyDefinition("Autogenerated_ID", "Automatically generated ID");
            id.DataType = DataType.DataType_Int32;
            id.IsAutoGenerated = true;
            id.Nullable = false;

            fc.Properties.Add(id);
            fc.IdentityProperties.Add(id);

            using (FdoFeatureService leftService = _options.Left.Connection.CreateFeatureService())
            using (FdoFeatureService rightService = _options.Right.Connection.CreateFeatureService())
            {
                ClassDefinition leftClass = leftService.GetClassByName(_options.Left.SchemaName, _options.Left.ClassName);
                ClassDefinition rightClass = rightService.GetClassByName(_options.Right.SchemaName, _options.Right.ClassName);

                if (leftClass == null)
                    throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_LEFT_CLASS_NOT_FOUND", _options.Left.ClassName));
                if (rightClass == null)
                    throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_RIGHT_CLASS_NOT_FOUND", _options.Right.ClassName));

                foreach (string leftProp in _options.LeftProperties)
                {
                    int pidx = leftClass.Properties.IndexOf(leftProp);
                    if (pidx >= 0)
                    {
                        PropertyDefinition pd = FdoFeatureService.CloneProperty(leftClass.Properties[pidx]);
                        if (!string.IsNullOrEmpty(_options.LeftPrefix))
                            pd.Name = _options.LeftPrefix + pd.Name;

                        fc.Properties.Add(pd);
                    }
                    else
                        throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_LEFT_PROPERTY_NOT_FOUND", leftProp));
                }

                foreach (string rightProp in _options.RightProperties)
                {
                    int pidx = rightClass.Properties.IndexOf(rightProp);
                    if (pidx >= 0)
                    {
                        PropertyDefinition pd = FdoFeatureService.CloneProperty(rightClass.Properties[pidx]);
                        if (!string.IsNullOrEmpty(_options.RightPrefix))
                            pd.Name = _options.RightPrefix + pd.Name;

                        fc.Properties.Add(pd);
                    }
                    else
                        throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_RIGHT_PROPERTY_NOT_FOUND", rightProp));
                }

                //Set designated geometry if specified
                if (!string.IsNullOrEmpty(_options.GeometryProperty))
                {
                    int pidx = fc.Properties.IndexOf(_options.GeometryProperty);
                    if (pidx < 0)
                        throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_TARGET_GEOMETRY_PROPERTY_NOT_FOUND"));

                    PropertyDefinition pd = fc.Properties[pidx];
                    if (pd.PropertyType != PropertyType.PropertyType_GeometricProperty)
                        throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_JOIN_NOT_TARGET_GEOMETRY_PROPERTY", _options.GeometryProperty));

                    //Set geometry property
                    fc.GeometryProperty = (GeometricPropertyDefinition)pd;
                }
            }

            return fc;
        }

        /// <summary>
        /// Called when [feature processed].
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % 50 == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[Join => {0}]: {1} features written", className, op.Statistics.OutputtedRows);
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
                SendMessageFormatted("[Join => {0}]: {1} features written in {2}", className, bop.BatchInsertTotal, op.Statistics.Duration.ToString());
            }
            else if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[Join => {0}]: {1} features written in {2}", className, op.Statistics.OutputtedRows, op.Statistics.Duration.ToString());
            }
        }

        /// <summary>
        /// Inner specialized implementation of join operation
        /// </summary>
        class ConcreteJoin : FdoNestedLoopsJoinOperation
        {
            NameValueCollection attributePairs;
            SpatialOperations? spatialJoinPredicate;
            ICollection<string> leftProperties;
            ICollection<string> rightProperties;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConcreteJoin"/> class.
            /// </summary>
            /// <param name="joinType">Type of the join.</param>
            public ConcreteJoin(FdoJoinType joinType)
            {
                this.attributePairs = new NameValueCollection();
                base.joinType = joinType;
            }

            /// <summary>
            /// If true, will only merge the left side with the first matching result
            /// on the right side. Otherwise, all matching results on the right side
            /// are merged.
            /// </summary>
            public bool ForceOneToOne
            {
                set { base.forceOneToOne = value; }
            }

            private string _LeftPrefix;

            /// <summary>
            /// The left side property qualifier
            /// </summary>
            public string LeftPrefix
            {
                get { return _LeftPrefix; }
                set { _LeftPrefix = value; }
            }

            private string _RightPrefix;

            /// <summary>
            /// The right side property qualifier
            /// </summary>
            public string RightPrefix
            {
                get { return _RightPrefix; }
                set { _RightPrefix = value; }
            }

            private string _GeometryProperty;

            /// <summary>
            /// The designated geometry property. If a prefix is specified, then this
            /// property must also be prefixed
            /// </summary>
            public string GeometryProperty
            {
                get { return _GeometryProperty; }
                set { _GeometryProperty = value; }
            }


            /// <summary>
            /// The left side properties
            /// </summary>
            public ICollection<string> LeftProperties
            {
                get { return leftProperties; }
                set { leftProperties = value; }
            }

            /// <summary>
            /// The right side properties
            /// </summary>
            public ICollection<string> RightProperties
            {
                get { return rightProperties; }
                set { rightProperties = value; }
            }

            /// <summary>
            /// Sets the spatial predicate.
            /// </summary>
            /// <value>The spatial predicate.</value>
            public SpatialOperations SpatialPredicate
            {
                set { spatialJoinPredicate = value; }
            }

            /// <summary>
            /// Adds the attribute join condition.
            /// </summary>
            /// <param name="leftProperty">The left property.</param>
            /// <param name="rightProperty">The right property.</param>
            public void AddAttributeJoinCondition(string leftProperty, string rightProperty)
            {
                this.attributePairs.Add(leftProperty, rightProperty);
            }

            protected override FdoRow MergeRows(FdoRow leftRow, FdoRow rightRow)
            {
                FdoRow row = new FdoRow();

                if (base.joinType == FdoJoinType.Inner)
                {
                    MergeLeft(leftRow, row);
                    MergeRight(rightRow, row);
                }
                else //left, right or full (both)
                {
                    //LEFT JOIN defined -or- left row is not empty
                    if (((base.joinType & FdoJoinType.Left) != 0) || leftRow.Count > 0)
                    {
                        MergeLeft(leftRow, row);
                    }
                    //RIGHT JOIN defined -or- right row is not empty
                    if (((base.joinType & FdoJoinType.Right) != 0) || rightRow.Count > 0)
                    {
                        MergeRight(rightRow, row);
                    }
                }
                if (!string.IsNullOrEmpty(_GeometryProperty))
                    row.DefaultGeometryProperty = _GeometryProperty;

                //switch (joinType)
                //{
                //    case FdoJoinType.Inner:
                //        {
                //            if (leftProperties.Count > 0)
                //            {
                //                if (string.IsNullOrEmpty(_LeftPrefix))
                //                {
                //                    foreach (string prop in leftProperties)
                //                    {
                //                        row[prop] = leftRow[prop];
                //                    }
                //                }
                //                else
                //                {
                //                    foreach (string prop in leftProperties)
                //                    {
                //                        row[_LeftPrefix + prop] = leftRow[prop];
                //                    }
                //                }
                //            }
                //            if (rightProperties.Count > 0)
                //            {
                //                if (string.IsNullOrEmpty(_RightPrefix))
                //                {
                //                    foreach (string prop in rightProperties)
                //                    {
                //                        row[prop] = rightRow[prop];
                //                    }
                //                }
                //                else
                //                {
                //                    foreach (string prop in rightProperties)
                //                    {
                //                        row[_RightPrefix + prop] = rightRow[prop];
                //                    }
                //                }
                //            }
                //            if (!string.IsNullOrEmpty(_GeometryProperty))
                //                row.DefaultGeometryProperty = _GeometryProperty;
                //        }
                //        break;
                //    case FdoJoinType.Left:
                //        {
                //            row.Copy(leftRow);
                //        }
                //        break;
                //    case FdoJoinType.Right:
                //        {
                //            row.Copy(rightRow);
                //        }
                //        break;
                //}
                return row;
            }

            private void MergeRight(FdoRow rightRow, FdoRow currentRow)
            {
                if (rightProperties.Count > 0)
                {
                    if (string.IsNullOrEmpty(_RightPrefix))
                    {
                        foreach (string prop in rightProperties)
                        {
                            currentRow[prop] = rightRow[prop];
                        }
                    }
                    else
                    {
                        foreach (string prop in rightProperties)
                        {
                            currentRow[_RightPrefix + prop] = rightRow[prop];
                        }
                    }
                }
            }

            private void MergeLeft(FdoRow leftRow, FdoRow currentRow)
            {
                if (leftProperties.Count > 0)
                {
                    if (string.IsNullOrEmpty(_LeftPrefix))
                    {
                        foreach (string prop in leftProperties)
                        {
                            currentRow[prop] = leftRow[prop];
                        }
                    }
                    else
                    {
                        foreach (string prop in leftProperties)
                        {
                            currentRow[_LeftPrefix + prop] = leftRow[prop];
                        }
                    }
                }
            }

            protected override bool MatchJoinCondition(FdoRow leftRow, FdoRow rightRow)
            {
                bool equals = false;
                foreach (string leftKey in this.attributePairs)
                {
                    string rightKey = this.attributePairs[leftKey];
                    switch (joinType)
                    {
                        case FdoJoinType.Inner:
                            equals = Equals(leftRow[leftKey], rightRow[rightKey]);
                            break;
                        case FdoJoinType.Left:
                            equals = Equals(leftRow[leftKey], rightRow[rightKey]) || rightRow[rightKey] == null;
                            break;
                        case FdoJoinType.Right:
                            equals = Equals(leftRow[leftKey], rightRow[rightKey]) || leftRow[leftKey] == null;
                            break;
                        case FdoJoinType.Full:
                            equals = Equals(leftRow[leftKey], rightRow[rightKey]) || leftRow[leftKey] == null || rightRow[rightKey] == null;
                            break;
                    }
                }
                //Only test spatial predicate if both rows have defined geometry fields
                if (this.spatialJoinPredicate.HasValue && leftRow.DefaultGeometryProperty != null && rightRow.DefaultGeometryProperty != null)
                {
                    equals = SpatialUtility.Evaluate(leftRow.Geometry, this.spatialJoinPredicate.Value, rightRow.Geometry);
                }
                return equals;
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
