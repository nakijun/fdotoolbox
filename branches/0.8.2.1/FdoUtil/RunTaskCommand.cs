#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core;
using System.IO;

namespace FdoUtil
{
    public class RunTaskCommand : ConsoleCommand
    {
        private string _file;

        public RunTaskCommand(string file)
        {
            _file = file;
        }

        public override int Execute()
        {
            CommandStatus retCode;
            DefinitionLoader loader = new DefinitionLoader();
            string name = null;
            if (TaskDefinitionHelper.IsBulkCopy(_file))
            {
                using (FdoBulkCopyOptions opts = loader.BulkCopyFromXml(_file, ref name, true))
                {
                    opts.SourceConnection.Open();
                    opts.TargetConnection.Open();
                    FdoBulkCopy copy = new FdoBulkCopy(opts);
                    copy.ProcessMessage += delegate(object sender, MessageEventArgs e)
                    {
                        Console.WriteLine(e.Message);
                    };
                    copy.ProcessAborted += delegate(object sender, EventArgs e)
                    {
                        Console.WriteLine("Bulk Copy Aborted");
                    };
                    copy.ProcessCompleted += delegate(object sender, EventArgs e)
                    {
                        Console.WriteLine("Bulk Copy Completed");
                    };
                    copy.Execute();
                    List<Exception> errors = new List<Exception>(copy.GetAllErrors());
                    if (errors.Count > 0)
                    {
                        string file = "bcp-error-" + DateTime.Now.ToShortTimeString() + ".log";
                        LogErrors(errors, file);
                        Console.WriteLine("Errors were encountered during bulk copy.");
                        retCode = CommandStatus.E_FAIL_BULK_COPY_WITH_ERRORS;
                    }
                    else { retCode = CommandStatus.E_OK; }
                }
            }
            else if (TaskDefinitionHelper.IsJoin(_file))
            {
                using (FdoJoinOptions opts = loader.JoinFromXml(_file, ref name, true))
                {
                    opts.Left.Connection.Open();
                    opts.Right.Connection.Open();
                    opts.Target.Connection.Open();
                    FdoJoin join = new FdoJoin(opts);
                    join.ProcessMessage += delegate(object sender, MessageEventArgs e)
                    {
                        Console.WriteLine(e.Message);
                    };
                    join.Execute();
                    List<Exception> errors = new List<Exception>(join.GetAllErrors());
                    if (errors.Count > 0)
                    {
                        string file = "join-error-" + DateTime.Now.ToShortTimeString() + ".log";
                        LogErrors(errors, file);
                        Console.WriteLine("Errors were encountered during join operation");
                        retCode = CommandStatus.E_FAIL_JOIN_WITH_ERRORS;
                    }
                    else { retCode = CommandStatus.E_OK; }
                }
            }
            else
            {
                retCode = CommandStatus.E_FAIL_UNRECOGNISED_TASK_FORMAT;
            }

            return (int)retCode;
        }

        private void LogErrors(List<Exception> errors, string file)
        {
            string dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (StreamWriter writer = new StreamWriter(file, false))
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    writer.WriteLine("------- EXCEPTION #" + (i + 1) + " -------");
                    writer.WriteLine(errors[i].ToString());
                    writer.WriteLine("------- EXCEPTION END -------");
                }
            }

            Console.WriteLine("Errors have been logged to {0}", file);
        }
    }
}
