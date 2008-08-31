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

namespace FdoToolbox.Core.ETL
{
#if TEST
    public class DataTypeMapping
#else
    internal class DataTypeMapping
#endif
    {
        private string _SourceProperty;

        public string SourceProperty
        {
            get { return _SourceProperty; }
            set { _SourceProperty = value; }
        }

        private string _TargetProperty;

        public string TargetProperty
        {
            get { return _TargetProperty; }
            set { _TargetProperty = value; }
        }

        private DataType _SourceDataType;

        public DataType SourceDataType
        {
            get { return _SourceDataType; }
            set { _SourceDataType = value; }
        }

        private DataType _TargetDataType;

        public DataType TargetDataType
        {
            get { return _TargetDataType; }
            set { _TargetDataType = value; }
        }

#if TEST
        public DataTypeMapping(string srcName, string targetName, DataType srcType, DataType targetType)
#else
        internal DataTypeMapping(string srcName, string targetName, DataType srcType, DataType targetType)
#endif
        {
            this.SourceProperty = srcName;
            this.TargetProperty = targetName;
            this.SourceDataType = srcType;
            this.TargetDataType = targetType;
        }

        internal DataTypeMapping(DataPropertyDefinition srcProp, DataPropertyDefinition targetProp)
            : this(srcProp.Name, targetProp.Name, srcProp.DataType, targetProp.DataType)
        { }
    }
}