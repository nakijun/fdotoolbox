using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Output pipeline operation
    /// </summary>
    public class FdoOutputOperation : FdoOperationBase
    {
        /// <summary>
        /// The output connection
        /// </summary>
        protected FdoConnection _conn;
        /// <summary>
        /// The service bound to the output connection
        /// </summary>
        protected FdoFeatureService _service;
        /// <summary>
        /// The property value mappings
        /// </summary>
        protected NameValueCollection _mappings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        public FdoOutputOperation(FdoConnection conn, string className)
        {
            _conn = conn;
            _service = conn.CreateFeatureService();
            this.ClassName = className;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        /// <param name="propertyMappings"></param>
        public FdoOutputOperation(FdoConnection conn, string className, NameValueCollection propertyMappings)
            : this(conn, className)
        {
            _mappings = propertyMappings;
        }

        private string _ClassName;

        /// <summary>
        /// The name of the feature class to write features to
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
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
                using (PropertyValueCollection propVals = row.ToPropertyValueCollection(_mappings))
                {
                    _service.InsertFeature(this.ClassName, propVals, false);
                    RaiseFeatureProcessed(row);
                }
            }
            yield break;
        }
    }
}
