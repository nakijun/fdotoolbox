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
using System.IO;
using FdoToolbox.Core;

namespace FdoUtil
{
    public class RunTaskCommand : ConsoleCommand
    {
        private string _taskFile;
        private string _logFile;
        private StreamWriter logWriter;

        public RunTaskCommand(string taskFile, string logFile)
        {
            _taskFile = taskFile;
            _logFile = logFile;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            try
            {
                if (!string.IsNullOrEmpty(_logFile))
                    logWriter = new StreamWriter(_logFile);

                ITask task = TaskLoader.LoadTask(_taskFile, true);
                task.OnLogTaskMessage += new TaskProgressMessageEventHandler(task_OnLogTaskMessage);
                task.OnItemProcessed += new TaskPercentageEventHandler(task_OnItemProcessed);
                task.OnTaskMessage += new TaskProgressMessageEventHandler(task_OnTaskMessage);
                task.ValidateTaskParameters();
                task.Execute();
                WriteLine("Task completed.");
                if (logWriter != null)
                    WriteLine("Log written to {0}", _logFile);

                retCode = CommandStatus.E_OK;
            }
            catch (TaskValidationException ex)
            {
                retCode = CommandStatus.E_FAIL_TASK_VALIDATION;
                WriteLine("Task validation failed.\n\n{0}", ex.ToString());
                if (logWriter != null)
                    logWriter.WriteLine("Task validation failed.\n\n{0}", ex.ToString());
            }
            catch (Exception ex)
            {
                retCode = CommandStatus.E_FAIL_BULK_COPY;
                WriteLine("Exception caught during execution.\n\n{0}", ex.ToString());
                if (logWriter != null)
                    logWriter.WriteLine("Exception caught during execution.\n\n{0}", ex.ToString());
            }
            finally
            {
                if (logWriter != null)
                {
                    logWriter.Close();
                    logWriter.Dispose();
                }
            }

            return (int)retCode;
        }

        void task_OnLogTaskMessage(string msg)
        {
            if (logWriter != null)
                logWriter.WriteLine(msg);
        }

        void task_OnTaskMessage(string msg)
        {
            WriteLine(msg);
        }

        void task_OnItemProcessed(int pc)
        {
            WriteLine("{0}% done", pc);
        }
    }
}
