using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class FdoBranchingOperation : FdoOperationBase
    {
        private readonly List<IFdoOperation> operations = new List<IFdoOperation>();

        /// <summary>
        /// Adds the specified operation to this branching operation
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public FdoBranchingOperation Add(IFdoOperation op)
        {
            operations.Add(op);
            return this;
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter"></param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            base.PrepareForExecution(pipelineExecuter);
            foreach (IFdoOperation op in operations)
            {
                op.PrepareForExecution(pipelineExecuter);
            }
        }

        /// <summary>
        /// Executes this operation, sending the input of this operation
        /// to all its child operations
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            List<FdoRow> copiedRows = new List<FdoRow>(rows);
            foreach (IFdoOperation operation in operations)
            {
                List<FdoRow> cloned = copiedRows.ConvertAll<FdoRow>(delegate(FdoRow row)
                {
                    return row.Clone();
                });
                IEnumerable<FdoRow> enumerable = operation.Execute(cloned);
                if (enumerable == null)
                    continue;
                IEnumerator<FdoRow> enumerator = enumerable.GetEnumerator();
#pragma warning disable 642
                while (enumerator.MoveNext()) ;
#pragma warning restore 642
            }
            yield break;
        }
    }
}
