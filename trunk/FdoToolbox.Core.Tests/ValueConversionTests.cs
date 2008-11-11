using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.Tests
{
    [TestFixture]
    public class ValueConversionTests
    {
        ValueExpression GetValue(DataTypeMapping mapping, object value)
        {
            return null;
            //return SpatialBulkCopyTask.GetConvertedValue(mapping, value);
        }

        [Test]
        public void TestBooleanConversion()
        {
            DataType srcType = DataType.DataType_Boolean;
            bool srcValue = true;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertPassedConversion<BooleanValue>(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertPassedConversion<ByteValue>(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertFailedConversion(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertFailedConversion(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertFailedConversion(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestBLOBConversion()
        {
            DataType srcType = DataType.DataType_BLOB;
            byte[] srcValue = Encoding.UTF8.GetBytes("Foobar");
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertPassedConversion<BLOBValue>(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertFailedConversion(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertFailedConversion(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertFailedConversion(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertFailedConversion(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertFailedConversion(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertFailedConversion(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertFailedConversion(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestByteConversion()
        {
            DataType srcType = DataType.DataType_Byte;
            byte srcValue = (byte)125;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertPassedConversion<ByteValue>(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertFailedConversion(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertFailedConversion(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertFailedConversion(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        private void AssertPassedConversion<T>(DataType targetType, object srcValue, DataTypeMapping mapping) where T : ValueExpression
        {
            try
            {
                ValueExpression expr;
                mapping.TargetDataType = targetType;
                expr = GetValue(mapping, srcValue);
                Assert.IsTrue(expr is T);
                if (expr is BLOBValue)
                    AssertHelper.BinaryEquals((expr as BLOBValue).Data, (byte[])srcValue);
                else if (expr is CLOBValue)
                    AssertHelper.BinaryEquals((expr as CLOBValue).Data, (byte[])srcValue);
                else if (expr is BooleanValue)
                    Assert.AreEqual(Convert.ToBoolean((expr as BooleanValue).Boolean), Convert.ToBoolean(srcValue));
                else if (expr is ByteValue)
                    Assert.AreEqual((expr as ByteValue).Byte, Convert.ToByte(srcValue));
                else if (expr is DateTimeValue)
                    Assert.AreEqual((expr as DateTimeValue).DateTime, Convert.ToDateTime(srcValue));
                else if (expr is DecimalValue)
                    Assert.AreEqual((expr as DecimalValue).Decimal, Convert.ToSingle(srcValue));
                else if (expr is DoubleValue)
                    Assert.AreEqual((expr as DoubleValue).Double, Convert.ToDouble(srcValue));
                else if (expr is Int16Value)
                    Assert.AreEqual((expr as Int16Value).Int16, Convert.ToInt16(srcValue));
                else if (expr is Int32Value)
                    Assert.AreEqual((expr as Int32Value).Int32, Convert.ToInt32(srcValue));
                else if (expr is Int64Value)
                    Assert.AreEqual((expr as Int64Value).Int64, Convert.ToInt64(srcValue));
                else if (expr is SingleValue)
                    Assert.AreEqual((expr as SingleValue).Single, Convert.ToSingle(srcValue));
                else if (expr is StringValue)
                    Assert.AreEqual((expr as StringValue).String, Convert.ToString(srcValue));
            }
            catch (Exception ex)
            {
                Assert.Fail("Conversion should pass:\n" + ex.ToString());
            }
        }

        private void AssertFailedConversion(DataType targetType, object srcValue, DataTypeMapping mapping)
        {
            ValueExpression expr;
            mapping.TargetDataType = targetType;
            try
            {
                expr = GetValue(mapping, srcValue);
                Assert.Fail("Should not convert " + mapping.SourceDataType + " to " + mapping.TargetDataType);
            }
            catch (Exception) { }
        }

        [Test]
        public void TestCLOBConversion()
        {
            DataType srcType = DataType.DataType_CLOB;
            byte[] srcValue = Encoding.UTF8.GetBytes("Foobar");
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertPassedConversion<CLOBValue>(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertFailedConversion(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertFailedConversion(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertFailedConversion(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertFailedConversion(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertFailedConversion(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertFailedConversion(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertFailedConversion(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestDateTimeConversion()
        {
            DataType srcType = DataType.DataType_DateTime;
            DateTime srcValue = new DateTime(2008, 12, 12);
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertPassedConversion<DateTimeValue>(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertFailedConversion(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertFailedConversion(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertFailedConversion(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertFailedConversion(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertFailedConversion(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertFailedConversion(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestDecimalConversion()
        {
            DataType srcType = DataType.DataType_Decimal;
            decimal srcValue = (decimal)1243;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestDoubleConversion()
        {
            DataType srcType = DataType.DataType_Double;
            double srcValue = 1.345945834;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            //AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestInt16Conversion()
        {
            DataType srcType = DataType.DataType_Int16;
            short srcValue = Int16.MaxValue;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestInt32Conversion()
        {
            DataType srcType = DataType.DataType_Int32;
            int srcValue = 42;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestInt64Conversion()
        {
            DataType srcType = DataType.DataType_Int64;
            long srcValue = 42;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestSingleConversion()
        {
            DataType srcType = DataType.DataType_Single;
            float srcValue = 1.4534949f;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);

            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, srcValue, mapping);

            //To Boolean
            AssertFailedConversion(DataType.DataType_Boolean, srcValue, mapping);

            //To Byte
            AssertFailedConversion(DataType.DataType_Byte, srcValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, srcValue, mapping);

            //To DateTime
            AssertFailedConversion(DataType.DataType_DateTime, srcValue, mapping);

            //To Decimal
            //AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, srcValue, mapping);

            //To Double
            //AssertPassedConversion<DoubleValue>(DataType.DataType_Double, srcValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, srcValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, srcValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, srcValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, srcValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, srcValue, mapping);
        }

        [Test]
        public void TestStringConversion()
        {
            DataType srcType = DataType.DataType_String;
            DataTypeMapping mapping = new DataTypeMapping("Foo", "Bar", srcType, DataType.DataType_BLOB);
            string badValue = "Foobar";
            //To BLOB
            AssertFailedConversion(DataType.DataType_BLOB, badValue, mapping);

            //To Boolean
            AssertPassedConversion<BooleanValue>(DataType.DataType_Boolean, "true", mapping);
            AssertFailedConversion(DataType.DataType_Boolean, badValue, mapping);

            //To Byte
            AssertPassedConversion<ByteValue>(DataType.DataType_Byte, "42", mapping);
            AssertFailedConversion(DataType.DataType_Byte, badValue, mapping);

            //To CLOB
            AssertFailedConversion(DataType.DataType_CLOB, badValue, mapping);

            //To DateTime
            AssertPassedConversion<DateTimeValue>(DataType.DataType_DateTime, "12/12/2008", mapping);
            AssertFailedConversion(DataType.DataType_DateTime, badValue, mapping);

            //To Decimal
            AssertPassedConversion<DecimalValue>(DataType.DataType_Decimal, "42.0", mapping);
            AssertFailedConversion(DataType.DataType_Decimal, badValue, mapping);

            //To Double
            AssertPassedConversion<DoubleValue>(DataType.DataType_Double, "3.232453", mapping);
            AssertFailedConversion(DataType.DataType_Double, badValue, mapping);

            //To Int16
            AssertPassedConversion<Int16Value>(DataType.DataType_Int16, "42", mapping);
            AssertFailedConversion(DataType.DataType_Int16, badValue, mapping);

            //To Int32
            AssertPassedConversion<Int32Value>(DataType.DataType_Int32, "32767", mapping);
            AssertFailedConversion(DataType.DataType_Int32, badValue, mapping);

            //To Int64
            AssertPassedConversion<Int64Value>(DataType.DataType_Int64, "3823290849302", mapping);
            AssertFailedConversion(DataType.DataType_Int64, badValue, mapping);

            //To Single
            AssertPassedConversion<SingleValue>(DataType.DataType_Single, "1.349384345", mapping);
            AssertFailedConversion(DataType.DataType_Single, badValue, mapping);

            //To String
            AssertPassedConversion<StringValue>(DataType.DataType_String, "This should be fine", mapping);
        }
    }
}
