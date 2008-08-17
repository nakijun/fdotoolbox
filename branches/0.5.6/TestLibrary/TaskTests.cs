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
using NUnit.Framework;
using FdoToolbox.Core;
using FdoToolbox.Core.IO;
using System.IO;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Tests
{
    [TestFixture]
    [Category("FdoToolboxCore")]
    public class TaskTests : BaseTest
    {
        [Test]
        public void TestLoadTask()
        {
            string file = "Target.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                ITask task = TaskLoader.LoadTask("Join.task", true);
                Assert.IsNotNull(task);
                SpatialJoinTask join = (SpatialJoinTask)task;
                join.Options.PrimarySource.Connection.Dispose();
                join.Options.SecondarySource.Connection.Dispose();
                join.Options.Target.Connection.Dispose();

                task = TaskLoader.LoadTask("Copy.task", true);
                SpatialBulkCopyTask bcp = (SpatialBulkCopyTask)task;
                bcp.Options.Source.Connection.Dispose();
                bcp.Options.Target.Connection.Dispose();
                Assert.IsNotNull(task);
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestMalformedTask()
        {
            ITask task = TaskLoader.LoadTask("Test.xml", true);
            Assert.IsNull(task);
        }

        [Test]
        public void TestSaveTask()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadSourceConnection()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadTargetConnection()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadSourceSchema()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadTarget()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailNonExistentSpatialContext()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadSourceClass()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadSourceProperty()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadTargetProperty()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadSourceFilter()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateBcpFailBadGlobalSpatialFilter()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadLeftConnection()
        {
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadRightConnection()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadTargetConnection()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadTargetSchema()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailTargetClassExists()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadLeftSchema()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadLeftClass()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadLeftProperties()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadRightTable()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailBadRightColumns()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailColumnPrefixRequired()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        [ExpectedException(typeof(TaskValidationException))]
        public void TestValidateJoinFailInvalidJoinPair()
        {
            Assert.Fail("Not implemented");
        }
    }
}
