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
using OSGeo.FDO.Expression;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Adapter class that converts an FDO IFeatureReader to an enumerable
    /// type
    /// </summary>
    internal class FdoReaderAdapter : IEnumerable<FdoFeature>
    {
        Enumerator enumerator;

        public FdoReaderAdapter(IFeatureReader reader)
        {
            this.enumerator = new Enumerator(reader);
        }

        class Enumerator : IEnumerator<FdoFeature>
        {
            private IFeatureReader _reader;
            private ClassDefinition _classDef;
            FdoFeature current;

            public Enumerator(IFeatureReader reader)
            {
                _reader = reader;
                _classDef = reader.GetClassDefinition();
            }

            public FdoFeature Current
            {
                get { return this.current; }
            }

            public void Dispose()
            {
                _reader.Close();
                _reader.Dispose();
                _classDef.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.current; }
            }

            public bool MoveNext()
            {
                if (_reader.ReadNext())
                {
                    FdoFeature feat = new FdoFeature();
                    foreach (PropertyDefinition pd in _classDef.Properties)
                    {
                        string pName = pd.Name;
                        if (!_reader.IsNull(pName))
                        {
                            switch (pd.PropertyType)
                            {
                                case PropertyType.PropertyType_DataProperty:
                                    {
                                        DataPropertyDefinition dp = pd as DataPropertyDefinition;
                                        switch (dp.DataType)
                                        {
                                            case DataType.DataType_BLOB:
                                                feat[pName] = new BLOBValue(_reader.GetLOB(pName).Data);
                                                break;
                                            case DataType.DataType_Boolean:
                                                feat[pName] = new BooleanValue(_reader.GetBoolean(pName));
                                                break;
                                            case DataType.DataType_Byte:
                                                feat[pName] = new ByteValue(_reader.GetByte(pName));
                                                break;
                                            case DataType.DataType_CLOB:
                                                feat[pName] = new CLOBValue(_reader.GetLOB(pName).Data);
                                                break;
                                            case DataType.DataType_DateTime:
                                                feat[pName] = new DateTimeValue(_reader.GetDateTime(pName));
                                                break;
                                            case DataType.DataType_Decimal:
                                                feat[pName] = new DecimalValue(_reader.GetDouble(pName));
                                                break;
                                            case DataType.DataType_Double:
                                                feat[pName] = new DoubleValue(_reader.GetDouble(pName));
                                                break;
                                            case DataType.DataType_Int16:
                                                feat[pName] = new Int16Value(_reader.GetInt16(pName));
                                                break;
                                            case DataType.DataType_Int32:
                                                feat[pName] = new Int32Value(_reader.GetInt32(pName));
                                                break;
                                            case DataType.DataType_Int64:
                                                feat[pName] = new Int64Value(_reader.GetInt64(pName));
                                                break;
                                            case DataType.DataType_Single:
                                                feat[pName] = new SingleValue(_reader.GetSingle(pName));
                                                break;
                                            case DataType.DataType_String:
                                                feat[pName] = new StringValue(_reader.GetString(pName));
                                                break;
                                        }
                                    }
                                    break;
                                case PropertyType.PropertyType_GeometricProperty:
                                    {
                                        feat[pName] = new GeometryValue(_reader.GetGeometry(pName));
                                    }
                                    break;
                                default:
                                    throw new NotSupportedException("Unsupported property type: " + pd.PropertyType);
                            }
                        }
                    }
                    this.current = feat;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException("Reset not supported");
            }
        }

        public IEnumerator<FdoFeature> GetEnumerator()
        {
            Enumerator e = this.enumerator;
            if (e == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }
            this.enumerator = null;
            return e;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
