using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// A <see cref="FdoFeatureTable"/> input source
    /// </summary>
    public class FdoFeatureTableInputOperation : FdoOperationBase
    {
        private FdoFeatureTable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoFeatureTableInputOperation"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public FdoFeatureTableInputOperation(FdoFeatureTable table)
        {
            _table = table;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoFeature feat in _table.Rows)
            {
                yield return FdoRow.FromFeatureRow(feat);
            }
        }
    }
}
