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
using OSGeo.FDO.Commands.SQL;

namespace FdoToolbox.Core.Common
{
    public abstract class FdoReader<T> : System.Data.IDataReader, IReader where T : IReader
    {
        protected T _internalReader;

        protected FdoReader(T reader)
        {
            _internalReader = reader;
        }

        public abstract string GetNameAt(int i);

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

        public OSGeo.FDO.Raster.IRaster GetRaster(string name)
        {
            return _internalReader.GetRaster(name);
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

        public abstract int Depth { get; }

        public abstract System.Data.DataTable GetSchemaTable();

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

        public abstract int FieldCount { get; }

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

        public abstract Type GetFieldType(int i);

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

        public abstract int GetOrdinal(string name);

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
