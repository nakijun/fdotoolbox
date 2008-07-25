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
using FdoToolbox.Core;
using System.IO;

namespace TaskRun
{
    public class TaskRunApp : ConsoleApplication
    {
        private StreamWriter logWriter;

        public override void Run(string[] args)
        {
            if (args.Length != 1)
            {
                ShowUsage();
                return;
            }

            string fileName = args[0];
            if (!File.Exists(fileName))
            {
                AppConsole.WriteLine("Unable to find task definition: {0}", fileName);
                return;
            }

            string logFile = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToFileTimeUtc() + ".log";
            logWriter = new StreamWriter(logFile);

            ITask task = TaskLoader.LoadTask(fileName, true);
            task.OnLogTaskMessage += new TaskProgressMessageEventHandler(task_OnLogTaskMessage);
            task.OnItemProcessed += new TaskPercentageEventHandler(task_OnItemProcessed);
            task.OnTaskMessage += new TaskProgressMessageEventHandler(task_OnTaskMessage);
            try
            {
                task.ValidateTaskParameters();
                task.Execute();
                AppConsole.WriteLine("Task completed. Log written to {0}", logFile);
            }
            catch (Exception ex)
            {
                AppConsole.WriteLine("Exception caught during execution.\n\n{0}", ex.ToString());
                logWriter.WriteLine("Exception caught during execution.\n\n{0}", ex.ToString());
            }
            finally
            {
                logWriter.Close();
                logWriter.Dispose();
            }
        }

        void task_OnLogTaskMessage(string msg)
        {
            logWriter.WriteLine(msg);
        }

        void task_OnTaskMessage(string msg)
        {
            AppConsole.WriteLine(msg);
        }

        void task_OnItemProcessed(int pc)
        {
            AppConsole.WriteLine("{0}% done", pc);
        }

        void ShowUsage()
        {
            AppConsole.WriteLine("Usage: TaskRun.exe [task definition file]");
        }
    }
}
