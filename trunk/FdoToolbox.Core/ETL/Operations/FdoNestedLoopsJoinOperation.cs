namespace FdoToolbox.Core.ETL.Operations
{
    using System;
    using System.Collections.Generic;
    using Enumerables;
    using FdoToolbox.Core.ETL.Specialized;

    /// <summary>
    /// Perform a join between two sources
    /// </summary>
    public abstract class FdoNestedLoopsJoinOperation : FdoOperationBase
    {
        private readonly PartialProcessOperation left = new PartialProcessOperation();
        private readonly PartialProcessOperation right = new PartialProcessOperation();
        private static readonly string IsEmptyRowMarker = Guid.NewGuid().ToString();

        /// <summary>
        /// The type of join
        /// </summary>
        protected FdoJoinType joinType = FdoJoinType.Inner;
        /// <summary>
        /// If true will only merge the first matching result. Otherwise all matching results
        /// are merged.
        /// </summary>
        protected bool forceOneToOne = true;
        private FdoRow currentRightRow, currentLeftRow;
        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public FdoNestedLoopsJoinOperation Right(IFdoOperation value)
        {
            right.Register(value);
            return this;
        }

        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public FdoNestedLoopsJoinOperation Left(IFdoOperation value)
        {
            left.Register(value);
            return this;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="ignored">Ignored rows</param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> ignored)
        {
            Initialize();

            if (left == null) throw new InvalidOperationException("Left branch of a join cannot be null");
            if (right == null) throw new InvalidOperationException("Right branch of a join cannot be null");

            Dictionary<FdoRow, object> matchedRightRows = new Dictionary<FdoRow, object>();
            CachingEnumerable<FdoRow> rightEnumerable = new CachingEnumerable<FdoRow>(
                new EventRaisingEnumerator(right, right.Execute(null))
                );
            IEnumerable<FdoRow> execute = left.Execute(null);
            foreach (FdoRow leftRow in new EventRaisingEnumerator(left, execute))
            {
                bool leftNeedOuterJoin = true;
                currentLeftRow = leftRow;
                foreach (FdoRow rightRow in rightEnumerable)
                {
                    currentRightRow = rightRow;
                    if (MatchJoinCondition(leftRow, rightRow))
                    {
                        leftNeedOuterJoin = false;
                        matchedRightRows[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                        if (this.forceOneToOne)
                            break;
                    }
                }
                if (leftNeedOuterJoin)
                {
                    FdoRow emptyRow = new FdoRow();
                    emptyRow[IsEmptyRowMarker] = IsEmptyRowMarker;
                    currentRightRow = emptyRow;
                    if (MatchJoinCondition(leftRow, emptyRow))
                        yield return MergeRows(leftRow, emptyRow);
                    else
                        LeftOrphanRow(leftRow);
                }
            }
            foreach (FdoRow rightRow in rightEnumerable)
            {
                if (matchedRightRows.ContainsKey(rightRow))
                    continue;
                currentRightRow = rightRow;
                FdoRow emptyRow = new FdoRow();
                emptyRow[IsEmptyRowMarker] = IsEmptyRowMarker;
                currentLeftRow = emptyRow;
                if (MatchJoinCondition(emptyRow, rightRow))
                    yield return MergeRows(emptyRow, rightRow);
                else
                    RightOrphanRow(rightRow);
            }
        }

        /// <summary>
        /// Called when a row on the right side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        protected virtual void RightOrphanRow(FdoRow row)
        {

        }

        /// <summary>
        /// Called when a row on the left side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void LeftOrphanRow(FdoRow row)
        {

        }

        /// <summary>
        /// Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract FdoRow MergeRows(FdoRow leftRow, FdoRow rightRow);

        /// <summary>
        /// Check if the two rows match to the join condition.
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract bool MatchJoinCondition(FdoRow leftRow, FdoRow rightRow);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                left.Dispose();
                right.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            left.PrepareForExecution(pipelineExecuter);
            right.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (Exception error in left.GetAllErrors())
            {
                yield return error;
            }
            foreach (Exception error in right.GetAllErrors())
            {
                yield return error;
            }
        }

        /// <summary>
        /// Perform an inner join equality on the two objects.
        /// Null values are not considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool InnerJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
                return false;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        private static bool IsEmptyRow(FdoRow row)
        {
            return row.Contains(IsEmptyRowMarker);
        }

        /// <summary>
        /// Perform an left join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on the right side
        /// with a value on the left is considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool LeftJoin(object left, object right)
        {
            if (IsEmptyRow(currentRightRow))
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Perform an right join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on the left side
        /// with a value on the right is considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool RightJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow))
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Perform an full join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on either side will satisfy this join
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool FullJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
                return true;
            if (left == null || right == null)
                return false;
            return Equals(left, right);
        }
    }
}