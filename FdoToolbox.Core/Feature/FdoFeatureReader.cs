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
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO reader wrapper class. Wraps the FDO IFeatureReader interface and
    /// the ADO.net IDataReader interface
    /// </summary>
    public class FdoFeatureReader : FdoReader<IFeatureReader>, IFeatureReader
    {
        private ClassDefinition _classDefinition;
        private string[] _names;
        private Type[] _types;
        private Dictionary<string, int> _ordinals;
        private int _limit = -1;
        private int _read = 0;
        private string[] _geometryNames;
        private string _defaultGeometryName;
        private Dictionary<string, FdoPropertyType> _ptypes;

        internal FdoFeatureReader(IFeatureReader reader, int limit)
            : this(reader)
        {
            _limit = limit;
        }

        internal FdoFeatureReader(IFeatureReader reader) : base(reader)
        {
            _classDefinition = reader.GetClassDefinition();
            _ptypes = new Dictionary<string, FdoPropertyType>();
            _ordinals = new Dictionary<string, int>();
            _types = new Type[_classDefinition.Properties.Count];
            _names = new string[_classDefinition.Properties.Count];
            List<string> geoms = new List<string>();
            for (int i = 0; i < _names.Length; i++)
            {
                PropertyDefinition pd = _classDefinition.Properties[i];
                _names[i] = pd.Name;
                string name = _names[i];
                _ordinals.Add(_names[i], i);
                
                DataPropertyDefinition dp = pd as DataPropertyDefinition;
                GeometricPropertyDefinition gp = pd as GeometricPropertyDefinition;
                if (dp != null)
                {
                    _types[i] = ExpressUtility.GetClrTypeFromFdoDataType(dp.DataType);
                    _ptypes[name] = ValueConverter.FromDataType(dp.DataType);
                }
                else if (gp != null)
                {
                    _types[i] = typeof(byte[]);
                    geoms.Add(gp.Name);
                    _ptypes[name] = FdoPropertyType.Geometry;
                }
                else if (pd.PropertyType == PropertyType.PropertyType_ObjectProperty)
                {
                    _ptypes[name] = FdoPropertyType.Object;
                }
                else if (pd.PropertyType == PropertyType.PropertyType_RasterProperty)
                {
                    _ptypes[name] = FdoPropertyType.Raster;
                }
                else if (pd.PropertyType == PropertyType.PropertyType_AssociationProperty)
                {
                    _ptypes[name] = FdoPropertyType.Association;
                }
            }
            _geometryNames = geoms.ToArray();
            if (_classDefinition is FeatureClass)
                _defaultGeometryName = (_classDefinition as FeatureClass).GeometryProperty.Name;
            else if(geoms.Count > 0)
                _defaultGeometryName = geoms[0];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _classDefinition.Dispose();
            }
        }

        public override int Depth
        {
            get { return _internalReader.GetDepth(); }
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

        public override bool ReadNext()
        {
            if (_limit > 0)
            {
                bool read = false;
                if (_limit > _read)
                {
                    read = base.ReadNext();
                    if (read)
                        _read++;
                }
                return read;
            }
            else 
            {
                return base.ReadNext();
            }
        }

        public override bool Read()
        {
            return this.ReadNext();
        }

        public override bool NextResult()
        {
            return this.ReadNext();
        }

        public override string[] GeometryProperties
        {
            get { return _geometryNames; }
        }

        public override string DefaultGeometryProperty
        {
            get { return _defaultGeometryName; }
        }

        public override FdoPropertyType GetFdoPropertyType(string name)
        {
            return _ptypes[name];
        }
    }
}
