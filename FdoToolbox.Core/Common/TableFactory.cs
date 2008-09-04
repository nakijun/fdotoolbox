#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using System.Data;
using FdoToolbox.Core.ETL;
using OSGeo.FDO.Commands.Feature;

namespace FdoToolbox.Core.Common
{
    public sealed class TableFactory
    {
        public static FdoDataTable CreateTable(ClassDefinition classDef)
        {
            switch (classDef.ClassType)
            {
                case ClassType.ClassType_Class:
                    return new FdoTable((Class)classDef);
                case ClassType.ClassType_FeatureClass:
                    return new FdoFeatureTable((FeatureClass)classDef);
                default:
                    throw new NotSupportedException();
            }
        }

        public static FdoDataTable CreateTable(IFeatureReader reader)
        {
            FdoDataTable table = CreateTable(reader.GetClassDefinition());
            table.LoadFromFeatureReader(reader);
            return table;
        }

        /// <summary>
        /// Converts a raw DataTable into a FdoDataTable. Please note that if the raw
        /// DataTable has DataColumns whose MaxLength is undefined and whose Data Type
        /// is string or byte[], conversion will fail. 
        /// <param name="table"></param>
        /// <returns></returns>
        public static FdoDataTable CreateTable(DataTable table)
        {
            object value = FdoMetaData.GetMetaData(table, FdoMetaDataNames.FDO_CLASS_TYPE);
            if (value != null)
            {
                ClassType ctype = (ClassType)value;
                switch (ctype)
                {
                    case ClassType.ClassType_Class:
                        return new FdoTable(table);
                    case ClassType.ClassType_FeatureClass:
                        return new FdoFeatureTable(table);
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType == typeof(string) ||
                        col.DataType == typeof(byte[]))
                    {
                        if (col.MaxLength <= 0)
                            throw new DataTableConversionException("The given DataTable has zero or negative length string/BLOB columns");
                    }
                }

                return new FdoTable(table);
            }
        }
    }
}
