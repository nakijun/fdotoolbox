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
using OSGeo.FDO.Connections;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.Commands;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.Utility;

namespace FdoUtil
{
    public class ExpressBcpCommand : ConsoleCommand
    {
        private string _SrcProvider;
        private string _SrcConnStr;
        private string _SrcSchema;
        private string _DestProvider;
        private string _DestFile;
        private List<string> _SrcClasses;
        private string _SrcSpatialContext;

        public ExpressBcpCommand(
            string srcProvider,
            string srcConnStr,
            string srcSchema,
            string destProvider,
            string destFile,
            List<string> classes,
            string srcSpatialContext
            )
        {
            _SrcProvider = srcProvider;
            _SrcConnStr = srcConnStr;
            _SrcSchema = srcSchema;
            _DestProvider = destProvider;
            _DestFile = destFile;
            _SrcClasses = classes;
            _SrcSpatialContext = srcSpatialContext;
        }

        public override int Execute()
        {
            CommandStatus retCode;

            IConnection srcConn = null;
            IConnection destConn = null;
            try
            {
                srcConn = FeatureAccessManager.GetConnectionManager().CreateConnection(_SrcProvider);
                destConn = FeatureAccessManager.GetConnectionManager().CreateConnection(_DestProvider);

                srcConn.ConnectionString = _SrcConnStr;
                if (_DestProvider.StartsWith("OSGeo.SDF"))
                {
                    if (ExpressUtility.CreateSDF(_DestFile))
                        destConn.ConnectionString = string.Format("File={0}", _DestFile);
                }

                srcConn.Open();
                destConn.Open();

                SpatialConnectionInfo srcConnInfo = new SpatialConnectionInfo("SOURCE", srcConn);
                SpatialConnectionInfo destConnInfo = new SpatialConnectionInfo("TARGET", destConn);

                SpatialBulkCopyOptions options = new SpatialBulkCopyOptions(srcConnInfo, destConnInfo);
                options.CopySpatialContexts = !string.IsNullOrEmpty(_SrcSpatialContext);

                if (options.CopySpatialContexts)
                    options.SourceSpatialContexts.Add(_SrcSpatialContext);

                options.SourceSchemaName = _SrcSchema;
                if (_SrcClasses.Count > 0)
                {
                    options.ClearClassCopyOptions();
                    ClassCollection srcClasses = SpatialBulkCopyTask.GetSourceClasses(options);
                    foreach (ClassDefinition classDef in srcClasses)
                    {
                        if (_SrcClasses.Contains(classDef.Name))
                        {
                            WriteLine("Adding class to copy: {0}", classDef.Name);
                            options.AddClassCopyOption(new ClassCopyOptions(classDef));
                        }
                    }
                }
                else
                {
                    ClassCollection srcClasses = SpatialBulkCopyTask.GetSourceClasses(options);
                    foreach (ClassDefinition classDef in srcClasses)
                    {
                        WriteLine("Adding class to copy: {0}", classDef.Name);
                        options.AddClassCopyOption(new ClassCopyOptions(classDef));
                    }
                }

                SpatialBulkCopyTask task = new SpatialBulkCopyTask("BCP", options);
                task.OnItemProcessed += new TaskPercentageEventHandler(task_OnItemProcessed);
                task.OnTaskMessage += new TaskProgressMessageEventHandler(task_OnTaskMessage);
                task.ValidateTaskParameters();
                task.Execute();

                retCode = CommandStatus.E_OK;
            }
            catch (Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_BULK_COPY;
            }
            finally
            {
                if (srcConn != null)
                {
                    if (srcConn.ConnectionState != ConnectionState.ConnectionState_Closed)
                        srcConn.Close();

                    srcConn.Dispose();
                }

                if (destConn != null)
                {
                    if (destConn.ConnectionState != ConnectionState.ConnectionState_Closed)
                        destConn.Close();

                    destConn.Dispose();
                }
            }

            return (int)retCode;
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
