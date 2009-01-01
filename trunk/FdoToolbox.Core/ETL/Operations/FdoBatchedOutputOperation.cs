#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Output pipeline operation with support for batch insertion
    /// </summary>
    public class FdoBatchedOutputOperation : FdoOutputOperation
    {
        private int _BatchSize;

        /// <summary>
        /// The batch size 
        /// </summary>
        public int BatchSize
        {
            get { return _BatchSize; }
            set { _BatchSize = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        /// <param name="batchSize"></param>
        public FdoBatchedOutputOperation(FdoConnection conn, string className, int batchSize)
            : base(conn, className)
        {
            _BatchSize = batchSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        /// <param name="propertyMappings"></param>
        /// <param name="batchSize"></param>
        public FdoBatchedOutputOperation(FdoConnection conn, string className, NameValueCollection propertyMappings, int batchSize)
            : base(conn, className, propertyMappings)
        {
            _BatchSize = batchSize;
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            int count = 0;
            string prefix = "param";
            using (IInsert insertCmd = _service.CreateCommand<IInsert>(CommandType.CommandType_Insert))
            {
                //Prepare command for batch insert
                insertCmd.SetFeatureClassName(this.ClassName);
                foreach (FdoRow row in rows)
                {
                    //Prepare the parameter placeholders
                    if (insertCmd.PropertyValues.Count == 0)
                    {
                        foreach (string col in row.Columns)
                        {
                            string pName = col;
                            string paramName = prefix + pName;
                            insertCmd.PropertyValues.Add(new PropertyValue(pName, new Parameter(paramName)));
                        }
                    }

                    ParameterValueCollection pVals = row.ToParameterValueCollection(prefix, _mappings);
                    insertCmd.BatchParameterValues.Add(pVals);
                    count++;

                    //Insert the batch
                    if (count == this.BatchSize)
                    {
                        using (IFeatureReader reader = insertCmd.Execute())
                        {
                            reader.Close();
                        }
                        count = 0;
                    }
                }

                //Insert the remaining batch
                if (count > 0)
                {
                    using (IFeatureReader reader = insertCmd.Execute())
                    {
                        reader.Close();
                    }
                    count = 0;
                }
            }
            yield break;
        }
    }
}
