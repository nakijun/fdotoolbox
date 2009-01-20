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
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.sdf"));
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.sqlite"));
            Assert.IsTrue(ExpressUtility.CreateFlatFileDataSource("Test.db"));

            File.Delete("Test.sdf");
            File.Delete("Test.sqlite");
            File.Delete("Test.db");
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
            NameValueCollection nvc = new NameValueCollection();
            nvc["Username"] = "Foo";
            nvc["Password"] = "Bar";
            nvc["Service"] = "localhost";
            string str = ExpressUtility.ConvertFromNameValueCollection(nvc);
            Assert.AreEqual(str, "Username=Foo;Password=Bar;Service=localhost");
        }
    }
}
