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

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private string _LogFile;

        public string LogFile
        {
            get { return _LogFile; }
            set { _LogFile = value; }
        }
	
        public override void Run(string[] args)
        {
            try
            {
                ParseArguments(args, 1, 2);
            }
            catch (ArgumentException ex)
            {
                AppConsole.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.LogFile))
                    logWriter = new StreamWriter(this.LogFile);

                ITask task = TaskLoader.LoadTask(this.FileName, true);
                task.OnLogTaskMessage += new TaskProgressMessageEventHandler(task_OnLogTaskMessage);
                task.OnItemProcessed += new TaskPercentageEventHandler(task_OnItemProcessed);
                task.OnTaskMessage += new TaskProgressMessageEventHandler(task_OnTaskMessage);
                task.ValidateTaskParameters();
                task.Execute();
                AppConsole.WriteLine("Task completed.");
                if (logWriter != null)
                    AppConsole.WriteLine("Log written to {0}", this.LogFile);
            }
            catch (Exception ex)
            {
                AppConsole.WriteLine("Exception caught during execution.\n\n{0}", ex.ToString());
                if(logWriter != null)
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
        }

        void task_OnLogTaskMessage(string msg)
        {
            if(logWriter != null)
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

        public override void ShowUsage()
        {
            AppConsole.WriteLine("Usage: TaskRun.exe -file:<task definition file> [-log:<log file>]");
        }

        public override void ParseArguments(string[] args, int minArguments, int maxArguments)
        {
            if (args.Length < minArguments || args.Length > maxArguments)
            {
                throw new ArgumentException("Insufficent arguments");
            }

            string fileName = GetArgument("-file", args);
            string logFile = GetArgument("-log", args);

            if (!File.Exists(fileName))
            {
                if (!Path.IsPathRooted(fileName))
                    fileName = Path.Combine(this.AppPath, fileName);

                if (!File.Exists(fileName))
                    throw new ArgumentException("Unable to find task definition: " + fileName);
                else
                    this.FileName = fileName;
            }
            else
            {
                this.FileName = fileName;
            }
            this.LogFile = logFile;
        }
    }
}
