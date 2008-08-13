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
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Forms;
using System.Threading;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Core.Modules
{
#if DEBUG
    public class TestModule : ModuleBase
    {
        public override string Name
        {
            get { return "test"; }
        }

        public override string Description
        {
            get { return "Test Module"; }
        }

        public override void Initialize()
        {
            
        }

        public override void Cleanup()
        {
            
        }

        [Command("test_runtask", "Test Run task")]
        public void TestRunTask()
        {
            DummyTask task = new DummyTask();
            new TaskProgressDlg(task).Run();
        }
    }

    public class DummyTask : TaskBase
    {
        public override void ValidateTaskParameters()
        {
            
        }

        public override void DoExecute()
        {
            try
            {
                bool timestamp = AppGateway.RunningApplication.Preferences.GetBooleanPref(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE);
                AppGateway.RunningApplication.Preferences.SetBooleanPref(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE, false);
                for (int i = 0; i < 100; i++)
                {
                    if (i % 10 == 0)
                    {
                        SendMessage(i + " items processed");
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(500);
                    }
                }
                AppGateway.RunningApplication.Preferences.SetBooleanPref(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE, timestamp);

                SendMessage("Task completed");
            }
            catch (ThreadAbortException)
            {
                System.Diagnostics.Trace.WriteLine("Cancelled operation");
                Thread.ResetAbort();
            }
        }

        public override TaskType TaskType
        {
            get { return TaskType.Dummy; }
        }

        public override bool IsCountable
        {
            get { return false; }
        }
    }
#endif
}
