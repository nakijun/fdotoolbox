using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// FDO input pipeline operation
    /// </summary>
    public class FdoInputOperation : FdoOperationBase
    {
        private FdoConnection _conn;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoInputOperation"/> class.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="query">The query.</param>
        public FdoInputOperation(FdoConnection conn, FeatureQueryOptions query)
        {
            _conn = conn;
            this.Query = query;
        }

        private FeatureQueryOptions _Query;

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public FeatureQueryOptions Query
        {
            get { return _Query; }
            set { _Query = value; }
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                using (FdoFeatureReader reader = service.SelectFeatures(this.Query))
                {
                    while (reader.ReadNext())
                    {
                        yield return CreateRowFromReader(reader);
                    }
                }
            }
        }

        private FdoRow CreateRowFromReader(FdoFeatureReader reader)
        {
            return FdoRow.FromFeatureReader(reader);
        }
    }
}
