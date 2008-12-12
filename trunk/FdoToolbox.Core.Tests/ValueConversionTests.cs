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
        [Test]
        public void TestBooleanConversion()
        {
            BooleanValue b = new BooleanValue(true);
            DataValue dv = null;

            //To BLOB
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Boolean, true, true);
            Assert.IsNull(dv);

            //To Boolean
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Boolean, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Boolean);
            Assert.AreEqual(dv, b);

            dv = null;
            //To Byte
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Byte, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Byte);
            
            //To CLOB
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_CLOB, true, true);
            Assert.IsNull(dv);

            //To DateTime
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_DateTime, true, true);
            Assert.IsNull(dv);

            //To Decimal
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Decimal, true, true);
            Assert.IsNull(dv);

            //To Double
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Double, true, true);
            Assert.IsNull(dv);

            dv = null;
            //To String
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_String, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_String);

            dv = null;
            //To Int16
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int16, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int16);

            dv = null;
            //To Int32
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int32, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int32);

            dv = null;
            //To Int64
            dv = ValueConverter.ConvertDataValue(b, DataType.DataType_Int64, true, true);
            Assert.IsNotNull(dv);
            Assert.AreEqual(dv.DataType, DataType.DataType_Int64);
        }

        [Test]
        public void TestBLOBConversion()
        {
            
        }

        [Test]
        public void TestByteConversion()
        {
            
        }

        [Test]
        public void TestCLOBConversion()
        {
            
        }

        [Test]
        public void TestDateTimeConversion()
        {
            
        }

        [Test]
        public void TestDecimalConversion()
        {
           
        }

        [Test]
        public void TestDoubleConversion()
        {
            
        }

        [Test]
        public void TestInt16Conversion()
        {
            
        }

        [Test]
        public void TestInt32Conversion()
        {
            
        }

        [Test]
        public void TestInt64Conversion()
        {
            
        }

        [Test]
        public void TestSingleConversion()
        {
            
        }

        [Test]
        public void TestStringConversion()
        {
            
        }
    }
}
