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
    public class FdoOutputOperation : FdoOperationBase
    {
        private FdoConnection _conn;
        private FdoFeatureService _service;
        private NameValueCollection _mappings;

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
            if (_mappings == null)
            {
                foreach (FdoRow row in rows)
                {
                    _service.InsertFeature(this.ClassName, row.ToPropertyValueCollection(), false);
                }
            }
            else
            {
                foreach (FdoRow row in rows)
                {
                    //This is basically the inlined version of FdoRow::ToPropertyValueCollection
                    //except that we use the mapped (target) property name instead of the source property
                    //name. Also we exclude any un-mapped properties from the final insert
                    PropertyValueCollection values = new PropertyValueCollection();
                    foreach (string col in row.Columns)
                    {
                        //Is mapped and not null?
                        if (_mappings[col] != null && row[col] != null && row[col] != DBNull.Value)
                        {
                            ValueExpression dv = ValueConverter.GetConvertedValue(row[col]);
                            if (dv != null)
                            {
                                string mappedProperty = _mappings[col];
                                PropertyValue pv = new PropertyValue(mappedProperty, dv);
                                values.Add(pv);
                            }
                        }
                    }

                    //No mappings -> No insert
                    if(values.Count > 0)
                        _service.InsertFeature(this.ClassName, values, false);
                }
            }
            yield break;
        }
    }
}
