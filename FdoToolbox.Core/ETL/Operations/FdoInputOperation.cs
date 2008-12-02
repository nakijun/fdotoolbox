using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    public class FdoInputOperation : FdoOperationBase
    {
        private FdoConnection _conn;

        public FdoInputOperation(FdoConnection conn, FeatureQueryOptions query)
        {
            _conn = conn;
            this.Query = query;
        }

        private FeatureQueryOptions _Query;

        public FeatureQueryOptions Query
        {
            get { return _Query; }
            set { _Query = value; }
        }

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
