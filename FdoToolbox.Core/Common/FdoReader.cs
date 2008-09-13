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

        public virtual bool ReadNext()
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

        public virtual System.Data.DataTable GetSchemaTable()
        {
            System.Data.DataTable schemaTable = new System.Data.DataTable();

            schemaTable.Columns.Add("ColumnName", typeof(String));
            schemaTable.Columns.Add("ColumnOrdinal", typeof(Int32));
            schemaTable.Columns.Add("ColumnSize", typeof(Int32));
            schemaTable.Columns.Add("NumericPrecision", typeof(Int32));
            schemaTable.Columns.Add("NumericScale", typeof(Int32));
            schemaTable.Columns.Add("IsUnique", typeof(Boolean));
            schemaTable.Columns.Add("IsKey", typeof(Boolean));
            schemaTable.Columns.Add("BaseCatalogName", typeof(String));
            schemaTable.Columns.Add("BaseColumnName", typeof(String));
            schemaTable.Columns.Add("BaseSchemaName", typeof(String));
            schemaTable.Columns.Add("BaseTableName", typeof(String));
            schemaTable.Columns.Add("DataType", typeof(Type));
            schemaTable.Columns.Add("AllowDBNull", typeof(Boolean));
            schemaTable.Columns.Add("ProviderType", typeof(Int32));
            schemaTable.Columns.Add("IsAliased", typeof(Boolean));
            schemaTable.Columns.Add("IsExpression", typeof(Boolean));
            schemaTable.Columns.Add("IsIdentity", typeof(Boolean));
            schemaTable.Columns.Add("IsAutoIncrement", typeof(Boolean));
            schemaTable.Columns.Add("IsRowVersion", typeof(Boolean));
            schemaTable.Columns.Add("IsHidden", typeof(Boolean));
            schemaTable.Columns.Add("IsLong", typeof(Boolean));
            schemaTable.Columns.Add("IsReadOnly", typeof(Boolean));

            schemaTable.BeginLoadData();
            for (int i = 0; i < this.FieldCount; i++)
            {
                System.Data.DataRow schemaRow = schemaTable.NewRow();

                schemaRow["ColumnName"] = GetName(i);
                schemaRow["ColumnOrdinal"] = i;
                schemaRow["ColumnSize"] = -1;
                schemaRow["NumericPrecision"] = 0;
                schemaRow["NumericScale"] = 0;
                schemaRow["IsUnique"] = false;
                schemaRow["IsKey"] = false;
                schemaRow["BaseCatalogName"] = "";
                schemaRow["BaseColumnName"] = GetName(i);
                schemaRow["BaseSchemaName"] = "";
                schemaRow["BaseTableName"] = "";
                schemaRow["DataType"] = GetFieldType(i);
                schemaRow["AllowDBNull"] = true;
                schemaRow["ProviderType"] = 0;
                schemaRow["IsAliased"] = false;
                schemaRow["IsExpression"] = false;
                schemaRow["IsIdentity"] = false;
                schemaRow["IsAutoIncrement"] = false;
                schemaRow["IsRowVersion"] = false;
                schemaRow["IsHidden"] = false;
                schemaRow["IsLong"] = false;
                schemaRow["IsReadOnly"] = false;

                schemaTable.Rows.Add(schemaRow);
                schemaRow.AcceptChanges();

            }
            schemaTable.EndLoadData();

            return schemaTable;
        }

        public bool IsClosed
        {
            get { return false; }
        }

        public virtual bool NextResult()
        {
            return _internalReader.ReadNext();
        }

        public virtual bool Read()
        {
            return _internalReader.ReadNext();
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public abstract int FieldCount { get; }

        public bool GetBoolean(int i)
        {
            return _internalReader.GetBoolean(GetName(i));
        }

        public byte GetByte(int i)
        {
            return _internalReader.GetByte(GetName(i));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return _internalReader.GetDateTime(GetName(i));
        }

        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(_internalReader.GetDouble(GetName(i)));
        }

        public double GetDouble(int i)
        {
            return _internalReader.GetDouble(GetName(i));
        }

        public abstract Type GetFieldType(int i);

        public float GetFloat(int i)
        {
            return _internalReader.GetSingle(GetName(i));
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            return _internalReader.GetInt16(GetName(i));
        }

        public int GetInt32(int i)
        {
            return _internalReader.GetInt32(GetName(i));
        }

        public long GetInt64(int i)
        {
            return _internalReader.GetInt64(GetName(i));
        }

        public abstract string GetName(int i);

        public abstract int GetOrdinal(string name);

        public string GetString(int i)
        {
            return _internalReader.GetString(GetName(i));
        }

        public object GetValue(int i)
        {
            Type t = GetFieldType(i);
            string name = GetName(i);
            if (t == typeof(bool))
                return GetBoolean(name);
            else if (t == typeof(byte))
                return GetByte(name);
            else if (t == typeof(DateTime))
                return GetDateTime(name);
            else if (t == typeof(decimal))
                return GetDecimal(i);
            else if (t == typeof(double))
                return GetDouble(name);
            else if (t == typeof(short))
                return GetInt16(name);
            else if (t == typeof(int))
                return GetInt32(name);
            else if (t == typeof(long))
                return GetInt64(name);
            else if (t == typeof(float))
                return GetSingle(name);
            else if (t == typeof(string))
                return GetString(name);
            return DBNull.Value;
        }

        public int GetValues(object[] values)
        {
            int numToFill = System.Math.Min(values.Length, this.FieldCount);
            for (int i = 0; i < numToFill; i++)
            {
                if (this[i] != null)
                    values[i] = this[i];
                else
                    values[i] = DBNull.Value;
            }
            return numToFill;
        }

        public bool IsDBNull(int i)
        {
            return _internalReader.IsNull(GetName(i));
        }

        public object this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }
    }
}
