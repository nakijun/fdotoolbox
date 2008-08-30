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

        /// <summary>
        /// Converts a raw DataTable into a FdoDataTable. Please note that if the raw
        /// DataTable has DataColumns whose MaxLength is undefined, they will be set to
        /// int.MaxValue and may cause failure when the converted ClassDefinition is used
        /// in an IApplySchema call. If this is the case, run this ClassDefinition through
        /// the FixDataProperties() method of FeatureService
        /// </summary>
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
                return new FdoTable(table);
            }
        }
    }
}
