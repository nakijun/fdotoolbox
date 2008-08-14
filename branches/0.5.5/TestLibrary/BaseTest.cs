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
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.IO;
using FdoToolbox.Core;

namespace FdoToolbox.Tests
{
    public abstract class BaseTest
    {
        [TestFixtureSetUp]
        public void TestSetup()
        {
            AppGateway.RunningApplication = new MockApplication();

            AppConsole.Out = new CmdConsoleOutputStream();
            AppConsole.Err = new CmdConsoleErrorStream();
            AppConsole.DoAlert += new AlertHandler(AppConsole_DoAlert);
            AppConsole.DoConfirm += new ConfirmHandler(AppConsole_DoConfirm);
        }

        bool AppConsole_DoConfirm(string title, string message)
        {
            return true;
        }

        void AppConsole_DoAlert(string title, string message)
        {
            Console.WriteLine("[{0}] {1}", title, message);
        }
    }
}
