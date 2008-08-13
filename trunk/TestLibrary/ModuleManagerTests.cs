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
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Commands;
using System.IO;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Tests
{
    [TestFixture]
    public class ModuleManagerTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(ModuleLoadException))]
        public void TestLoadBadModuleDuplicateCmdName()
        {
            IModuleMgr mgr = new ModuleMgr();
            mgr.LoadModule(new MockModule1());
            mgr.LoadModule(new MockModule2());
        }

        [Test]
        [ExpectedException(typeof(ModuleLoadException))]
        public void TestLoadBadModuleInvalidAssembly()
        {
            IModuleMgr mgr = new ModuleMgr();
            string file = Path.Combine(AppGateway.RunningApplication.AppPath, "Test.xml");
            Console.WriteLine("Loading: {0}", file);
            mgr.LoadExtension(file);
        }
    }

    public class MockModule1 : ModuleBase
    {
        public override string Name
        {
            get { return "mock1"; }
        }

        public override string Description
        {
            get { return "mock module 1"; }
        }

        public override void Initialize()
        {
            
        }

        public override void Cleanup()
        {
            
        }

        [Command("test", "test")]
        public void Test()
        {
        }
    }

    public class MockModule2 : ModuleBase
    {
        public override string Name
        {
            get { return "mock2"; }
        }

        public override string Description
        {
            get { return "mock module 2"; }
        }

        public override void Initialize()
        {

        }

        public override void Cleanup()
        {

        }

        [Command("test", "test")]
        public void Test()
        {
        }
    }
}
