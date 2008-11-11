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
using FdoToolbox.Lib;
using FdoToolbox.Lib.Modules;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Lib.Commands;
using FdoToolbox.Core.ClientServices;

namespace TestModule
{
    public class Foo : ModuleBase 
    {
        public override string Name
        {
            get { return "foo"; }
        }

        public override string Description
        {
            get { return "Foo Module"; }
        }

        public override void Initialize()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Cleanup()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        [Command("foo", "foo")]
        public void TestFoo()
        {
            AppConsole.Alert("Foo", "Foo");
        }
    }
}
