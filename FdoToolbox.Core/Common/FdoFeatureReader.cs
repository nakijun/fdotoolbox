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
    public class FdoFeatureReader : FdoReader<IFeatureReader>, IFeatureReader
    {
        private ClassDefinition _classDefinition;
        private string[] _names;
        private Type[] _types;
        private Dictionary<string, int> _ordinals;

        internal FdoFeatureReader(IFeatureReader reader) : base(reader)
        {
            _classDefinition = reader.GetClassDefinition();
            _ordinals = new Dictionary<string, int>();
            _types = new Type[_classDefinition.Properties.Count];
            _names = new string[_classDefinition.Properties.Count];
            for (int i = 0; i < _names.Length; i++)
            {
                PropertyDefinition pd = _classDefinition.Properties[i];
                _names[i] = pd.Name;
                _ordinals.Add(_names[i], i);
                
                DataPropertyDefinition dp = pd as DataPropertyDefinition;
                GeometricPropertyDefinition gp = pd as GeometricPropertyDefinition;
                if (dp != null)
                    _types[i] = FdoDataColumn.GetTypeFromDataType(dp.DataType);
                else if (gp != null)
                    _types[i] = typeof(byte[]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _classDefinition.Dispose();
            }
        }

        public override string GetNameAt(int i)
        {
            return _names[i];
        }

        public override int Depth
        {
            get { return _internalReader.GetDepth(); }
        }

        public override System.Data.DataTable GetSchemaTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int FieldCount
        {
            get { return _names.Length; }
        }

        public override Type GetFieldType(int i)
        {
            return _types[i];
        }

        public override int GetOrdinal(string name)
        {
            return _ordinals[name];
        }

        public ClassDefinition GetClassDefinition()
        {
            return _classDefinition;
        }

        public int GetDepth()
        {
            return _internalReader.GetDepth();
        }

        public IFeatureReader GetFeatureObject(string propertyName)
        {
            return _internalReader.GetFeatureObject(propertyName);
        }

        public override string GetName(int i)
        {
            return _names[i];
        }
    }
}
