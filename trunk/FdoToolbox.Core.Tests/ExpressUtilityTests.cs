using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Utility;
using NUnit.Framework;
using System.Collections.Specialized;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Connections;
using System.IO;

namespace FdoToolbox.Core.Tests
{
    [TestFixture]
    public class ExpressUtilityTests
    {
        [Test]
        public void TestCreateFlatFile()
        {
        }

        [Test]
        public void TestStringToNvc()
        {
            string str = "Username=Foo;Password=Bar;Service=localhost";
            NameValueCollection nvc = ExpressUtility.ConvertFromString(str);

            Assert.AreEqual(nvc["Username"], "Foo");
            Assert.AreEqual(nvc["Password"], "Bar");
            Assert.AreEqual(nvc["Service"], "localhost");
        }

        [Test]
        public void TestNvcToString()
        { 
            
        }

        [Test]
        public void TestClrTypeToDataType()
        {

        }

        [Test]
        public void TestDataTypeToClrType()
        {

        }
    }
}
