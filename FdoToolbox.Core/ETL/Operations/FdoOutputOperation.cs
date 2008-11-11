using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;

namespace FdoToolbox.Core.ETL.Operations
{
    public class FdoOutputOperation : FdoOperationBase
    {
        private FdoConnection _conn;
        private FdoFeatureService _service;

        public FdoOutputOperation(FdoConnection conn, string className)
        {
            _conn = conn;
            _service = conn.CreateFeatureService();
            this.ClassName = className;
        }

        private string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                _service.InsertFeature(this.ClassName, row.ToPropertyValueCollection(), false);
            }
            yield break;
        }
    }
}
