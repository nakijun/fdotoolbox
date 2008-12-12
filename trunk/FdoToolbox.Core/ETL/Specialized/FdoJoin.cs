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

    /// <summary>
    /// A specialized form of <see cref="EtlProcess"/> that merges
    /// two feature classes into one
    /// </summary>
    public class FdoJoin : FdoSpecializedEtlProcess
    {
        private FdoJoinOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FdoJoin(FdoJoinOptions options) { _options = options; }

        protected override void Initialize()
        {
               
        }

        /// <summary>
        /// Inner specialized implementation of join operation
        /// </summary>
        class ConcreteJoin : FdoNestedLoopsJoinOperation
        {
            FdoJoinType joinType;
            NameValueCollection attributePairs;
            SpatialOperations? spatialJoinPredicate;

            public ConcreteJoin(FdoJoinType joinType)
            {
                this.attributePairs = new NameValueCollection();
                this.joinType = joinType;
            }

            public SpatialOperations SpatialPredicate
            {
                set { spatialJoinPredicate = value; }
            }

            public void AddAttributeJoinCondition(string leftProperty, string rightProperty)
            {
                this.attributePairs.Add(leftProperty, rightProperty);
            }

            protected override FdoRow MergeRows(FdoRow leftRow, FdoRow rightRow)
            {
                FdoRow row = new FdoRow();
                switch (joinType)
                {
                    case FdoJoinType.Inner:
                        {
                            throw new NotImplementedException();   
                        }
                        break;
                    case FdoJoinType.Left:
                        {
                            row.Copy(leftRow);
                        }
                        break;
                    case FdoJoinType.Right:
                        {
                            row.Copy(rightRow);
                        }
                        break;
                }
                return row;
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
    }
}
