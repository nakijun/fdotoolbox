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
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Schema;
using System.Data;

namespace FdoToolbox.Core.Common
{
    /**
     * This should really derive from FdoReader<T>, but since ISQLDataReader does
     * not derive from IReader, that is not possible. Until that is fixed in FDO itself
     * we have no choice but to duplicate the FdoReader implementation, ugh!
     * 
     * http://trac.osgeo.org/fdo/ticket/359
     */

    public class FdoSqlReader : IDataReader, ISQLDataReader
    {
        private Dictionary<string, int> _ordinals;
        private string[] _names;
        private Type[] _types;
        private ISQLDataReader _internalReader;

        internal FdoSqlReader(ISQLDataReader reader)
        {
            _internalReader = reader;
            _ordinals = new Dictionary<string, int>();
            int count = reader.GetColumnCount();
            _types = new Type[count];
            _names = new string[count];
            for (int i = 0; i < count; i++)
            {
                string name = _internalReader.GetColumnName(i);
                _names[i] = name;
                _ordinals.Add(name, i);

                PropertyType ptype = _internalReader.GetPropertyType(name);
                if (ptype == PropertyType.PropertyType_DataProperty)
                    _types[i] = FdoDataColumn.GetTypeFromDataType(_internalReader.GetColumnType(name));
                else if (ptype == PropertyType.PropertyType_GeometricProperty)
                    _types[i] = typeof(byte[]);
            }
        }

        public string GetNameAt(int i)
        {
            return _internalReader.GetColumnName(i);
        }

        public int Depth
        {
            get { return -1; }
        }

        public System.Data.DataTable GetSchemaTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FieldCount
        {
            get { return _internalReader.GetColumnCount(); }
        }

        public Type GetFieldType(int i)
        {
            DataType dt = _internalReader.GetColumnType(_names[i]);
            return FdoDataColumn.GetTypeFromDataType(dt);
        }

        public int GetOrdinal(string name)
        {
            return _ordinals[name];   
        }

        public int GetColumnCount()
        {
            return _internalReader.GetColumnCount();
        }

        public string GetColumnName(int index)
        {
            return _internalReader.GetColumnName(index);
        }

        public OSGeo.FDO.Schema.DataType GetColumnType(string name)
        {
            return _internalReader.GetColumnType(name);
        }

        public OSGeo.FDO.Schema.PropertyType GetPropertyType(string name)
        {
            return _internalReader.GetPropertyType(name);
        }

        public void Close()
        {
            _internalReader.Close();
        }

        public bool GetBoolean(string name)
        {
            return _internalReader.GetBoolean(name);
        }

        public byte GetByte(string name)
        {
            return _internalReader.GetByte(name);
        }

        public DateTime GetDateTime(string name)
        {
            return _internalReader.GetDateTime(name);
        }

        public double GetDouble(string name)
        {
            return _internalReader.GetDouble(name);
        }

        public byte[] GetGeometry(string name)
        {
            return _internalReader.GetGeometry(name);
        }

        public short GetInt16(string name)
        {
            return _internalReader.GetInt16(name);
        }

        public int GetInt32(string name)
        {
            return _internalReader.GetInt32(name);
        }

        public long GetInt64(string name)
        {
            return _internalReader.GetInt64(name);
        }

        public OSGeo.FDO.Expression.LOBValue GetLOB(string name)
        {
            return _internalReader.GetLOB(name);
        }

        public OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(string name)
        {
            return _internalReader.GetLOBStreamReader(name);
        }
        
        public float GetSingle(string name)
        {
            return _internalReader.GetSingle(name);
        }

        public string GetString(string name)
        {
            return _internalReader.GetString(name);
        }

        public bool IsNull(string name)
        {
            return _internalReader.IsNull(name);
        }

        public bool ReadNext()
        {
            return _internalReader.ReadNext();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _internalReader.Dispose();
            }
        }

        public bool IsClosed
        {
            get { return false; }
        }

        public bool NextResult()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Read()
        {
            return _internalReader.ReadNext();
        }

        public int RecordsAffected
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool GetBoolean(int i)
        {
            return _internalReader.GetBoolean(GetNameAt(i));
        }

        public byte GetByte(int i)
        {
            return _internalReader.GetByte(GetNameAt(i));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public char GetChar(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.IDataReader GetData(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetDataTypeName(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime GetDateTime(int i)
        {
            return _internalReader.GetDateTime(GetNameAt(i));
        }

        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(_internalReader.GetDouble(GetNameAt(i)));
        }

        public double GetDouble(int i)
        {
            return _internalReader.GetDouble(GetNameAt(i));
        }

        public float GetFloat(int i)
        {
            return _internalReader.GetSingle(GetNameAt(i));
        }

        public Guid GetGuid(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public short GetInt16(int i)
        {
            return _internalReader.GetInt16(GetNameAt(i));
        }

        public int GetInt32(int i)
        {
            return _internalReader.GetInt32(GetNameAt(i));
        }

        public long GetInt64(int i)
        {
            return _internalReader.GetInt64(GetNameAt(i));
        }

        public string GetName(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetString(int i)
        {
            return _internalReader.GetString(GetNameAt(i));
        }

        public object GetValue(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetValues(object[] values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsDBNull(int i)
        {
            return _internalReader.IsNull(GetNameAt(i));
        }

        public object this[string name]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object this[int i]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
