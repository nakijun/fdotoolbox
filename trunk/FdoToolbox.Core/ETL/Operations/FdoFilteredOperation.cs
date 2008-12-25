using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A pipeline operation that conditionally discards features sent to it
    /// </summary>
    public class FdoFilteredOperation : FdoOperationBase
    {
        private Predicate<FdoRow> _condition;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="condition">The predicate that determines which rows will be passed on</param>
        public FdoFilteredOperation(Predicate<FdoRow> condition)
        {
            _condition = condition;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                if (_condition(row))
                    yield return row;
            }
        }
    }
}
