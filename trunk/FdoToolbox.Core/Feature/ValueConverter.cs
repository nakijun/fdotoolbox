using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Feature
{
    public sealed class ValueConverter
    {
        private static FgfGeometryFactory _Factory;

        static ValueConverter() { _Factory = new FgfGeometryFactory(); }

        public static FdoPropertyType FromDataType(DataType dt)
        {
            //return (FdoPropertyType)Enum.Parse(typeof(FdoPropertyType), dt.ToString());
            switch (dt)
            {
                case DataType.DataType_BLOB:
                    return FdoPropertyType.BLOB;
                case DataType.DataType_Boolean:
                    return FdoPropertyType.Boolean;
                case DataType.DataType_Byte:
                    return FdoPropertyType.Byte;
                case DataType.DataType_CLOB:
                    return FdoPropertyType.CLOB;
                case DataType.DataType_DateTime:
                    return FdoPropertyType.DateTime;
                case DataType.DataType_Decimal:
                    return FdoPropertyType.Decimal;
                case DataType.DataType_Double:
                    return FdoPropertyType.Double;
                case DataType.DataType_Int16:
                    return FdoPropertyType.Int16;
                case DataType.DataType_Int32:
                    return FdoPropertyType.Int32;
                case DataType.DataType_Int64:
                    return FdoPropertyType.Int64;
                case DataType.DataType_Single:
                    return FdoPropertyType.Single;
                case DataType.DataType_String:
                    return FdoPropertyType.String;
                default:
                    throw new ArgumentException("dt");
            }
        }

        public static ValueExpression GetConvertedValue(object value)
        {
            Type t = value.GetType();
            if (t == typeof(byte[]))
            {
                //BLOB
                //CLOB
                return new BLOBValue((byte[])value);
            }
            if (t == typeof(bool))
            {
                return new BooleanValue((bool)value);
            }
            if (t == typeof(byte))
            {
                return new ByteValue((byte)value);
            }
            if (t == typeof(DateTime))
            { 
                return new DateTimeValue((DateTime)value);
            }
            if (t == typeof(Decimal))
            {
                return new DoubleValue(Convert.ToDouble(value));
            }
            if (t == typeof(Double))
            {
                return new DoubleValue((double)value);
            }
            if (t == typeof(Int16))
            {
                return new Int16Value((Int16)value);
            }
            if (t == typeof(Int32))
            {
                return new Int32Value((Int32)value);
            }
            if (t == typeof(Int64))
            {
                return new Int64Value((Int64)value);
            }
            if (t == typeof(Single))
            {
                return new SingleValue((Single)value);
            }
            if (t == typeof(String))
            {
                return new StringValue((String)value);
            }

            IGeometry geom = value as IGeometry;
            if (geom != null)
            {
                return new GeometryValue(_Factory.GetFgf(geom));
            }

            return null;
        }
    }
}
