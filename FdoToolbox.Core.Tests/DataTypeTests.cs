using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.Tests
{
    [TestFixture]
    public class DataTypeTests
    {
        [Test]
        public void TestConvertability()
        {
            //BLOB
            Assert.IsTrue(IsConvertable(DataType.DataType_BLOB, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Boolean));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_DateTime));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Decimal));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Double));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Int16));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Int32));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Int64));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_Single));
            Assert.IsFalse(IsConvertable(DataType.DataType_BLOB, DataType.DataType_String));

            //Boolean
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_String));
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Byte));
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Int64));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_DateTime));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Decimal));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Double));
            Assert.IsFalse(IsConvertable(DataType.DataType_Boolean, DataType.DataType_Single));

            //Byte
            Assert.IsFalse(IsConvertable(DataType.DataType_Byte, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Byte, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Byte, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Byte, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Byte, DataType.DataType_String));

            //CLOB
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Boolean));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Byte));
            Assert.IsTrue(IsConvertable(DataType.DataType_CLOB, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_DateTime));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Decimal));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Double));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Int16));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Int32));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Int64));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_Single));
            Assert.IsFalse(IsConvertable(DataType.DataType_CLOB, DataType.DataType_String));

            //DateTime
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Boolean));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_CLOB));
            Assert.IsTrue(IsConvertable(DataType.DataType_DateTime, DataType.DataType_DateTime));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Decimal));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Double));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Int16));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Int32));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Int64));
            Assert.IsFalse(IsConvertable(DataType.DataType_DateTime, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_DateTime, DataType.DataType_String));

            //Decimal
            Assert.IsFalse(IsConvertable(DataType.DataType_Decimal, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Decimal, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Decimal, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Decimal, DataType.DataType_String));

            //Double
            Assert.IsFalse(IsConvertable(DataType.DataType_Double, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Double, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Double, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Double, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Double, DataType.DataType_String));

            //Int16
            Assert.IsFalse(IsConvertable(DataType.DataType_Int16, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int16, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int16, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int16, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int16, DataType.DataType_String));

            //Int32
            Assert.IsFalse(IsConvertable(DataType.DataType_Int32, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int32, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int32, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int32, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int32, DataType.DataType_String));

            //Int64
            Assert.IsFalse(IsConvertable(DataType.DataType_Int64, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int64, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int64, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Int64, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Int64, DataType.DataType_String));

            //Single
            Assert.IsFalse(IsConvertable(DataType.DataType_Single, DataType.DataType_BLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Single, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_Single, DataType.DataType_CLOB));
            Assert.IsFalse(IsConvertable(DataType.DataType_Single, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_Single, DataType.DataType_String));

            //String
            Assert.IsFalse(IsConvertable(DataType.DataType_String, DataType.DataType_BLOB));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Boolean));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Byte));
            Assert.IsFalse(IsConvertable(DataType.DataType_String, DataType.DataType_CLOB));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_DateTime));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Decimal));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Double));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Int16));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Int32));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Int64));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_Single));
            Assert.IsTrue(IsConvertable(DataType.DataType_String, DataType.DataType_String));
        }

        private bool IsConvertable(DataType d1, DataType d2)
        {
            return ValueConverter.IsConvertible(d1, d2);
        }
    }
}
