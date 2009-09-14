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
using OSGeo.FDO.Expression;

namespace FdoToolbox.Core.ETL.Operations
{
    public class FdoDataValueConversionOperation : FdoOperationBase
    {
        private List<DataTypeMapping> _mappings;
        private bool _nullOnFailedConversion;
        private bool _truncate;

        public FdoDataValueConversionOperation(List<DataTypeMapping> mapping, bool nullOnFailedConversion, bool truncate)
        {
            _mappings = mapping;
            _nullOnFailedConversion = nullOnFailedConversion;
            _truncate = truncate;
        }

        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                yield return ConvertValues(row);
            }
        }

        private FdoRow ConvertValues(FdoRow row)
        {
            foreach (DataTypeMapping mp in _mappings)
            {
                if (row[mp.SourceProperty] != null)
                {
                    LiteralValue old = ValueConverter.GetConvertedValue(row[mp.SourceProperty]);
                    if (old.LiteralValueType == LiteralValueType.LiteralValueType_Data)
                    {
                        DataValue converted = ValueConverter.ConvertDataValue((DataValue)old, mp.TargetDataType, _nullOnFailedConversion, _truncate);
                        row[mp.SourceProperty] = ValueConverter.GetClrValue(converted);
                    }
                }
            }
            return row;
        }
    }
}
