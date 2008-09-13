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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Common
{
    public class FdoDataReader : FdoReader<IDataReader>, IDataReader
    {
        private string[] _names;
        private Dictionary<string, int> _ordinals;
        private Type[] _types;

        internal FdoDataReader(IDataReader reader)
            : base(reader)
        {
            _ordinals = new Dictionary<string, int>();
            _names = new string[reader.GetPropertyCount()];
            _types = new Type[reader.GetPropertyCount()];
            for (int i = 0; i < _names.Length; i++)
            {
                string name = reader.GetPropertyName(i);
                _names[i] = name;
                _ordinals.Add(name, i);

                PropertyType ptype = reader.GetPropertyType(name);
                if (ptype == PropertyType.PropertyType_DataProperty)
                    _types[i] = FdoDataColumn.GetTypeFromDataType(reader.GetDataType(name));
                else if (ptype == PropertyType.PropertyType_GeometricProperty)
                    _types[i] = typeof(byte[]);
            }
        }

        public override int Depth
        {
            get { return -1; }
        }

        public override int FieldCount
        {
            get { return _internalReader.GetPropertyCount(); }
        }

        public override Type GetFieldType(int i)
        {
            return _types[i];
        }

        public override int GetOrdinal(string name)
        {
            return _ordinals[name];
        }

        public OSGeo.FDO.Schema.DataType GetDataType(string name)
        {
            return _internalReader.GetDataType(name);
        }

        public int GetPropertyCount()
        {
            return _internalReader.GetPropertyCount();
        }

        public string GetPropertyName(int index)
        {
            return _internalReader.GetPropertyName(index);
        }

        public OSGeo.FDO.Schema.PropertyType GetPropertyType(string name)
        {
            return _internalReader.GetPropertyType(name);
        }

        public override string GetName(int i)
        {
            return _names[i];
        }
    }
}
