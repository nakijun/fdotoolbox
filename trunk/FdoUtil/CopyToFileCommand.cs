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
using FdoToolbox.Core.Feature;
using System.IO;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core;
using FdoToolbox.Core.ETL.Specialized;
using OSGeo.FDO.Schema;

namespace FdoUtil
{
    public class CopyToFileCommand : ConsoleCommand
    {
        private string _srcProvider;
        private string _srcConnStr;
        private string _srcSchema;
        private List<string> _srcClasses;
        private string _destPath;
        private string _srcSpatialContext;

        public CopyToFileCommand(string srcProvider, string srcConnStr, string srcSchema, List<string> srcClasses, string destPath, string srcSpatialContext)
        {
            _srcProvider = srcProvider;
            _srcConnStr = srcConnStr;
            _srcSchema = srcSchema;
            _srcClasses = srcClasses;
            _destPath = destPath;
            _srcSpatialContext = srcSpatialContext;
        }

        public override int Execute()
        {
            CommandStatus retCode;

            FdoConnection srcConn = new FdoConnection(_srcProvider, _srcConnStr);
            FdoConnection destConn = null;
            //Directory given, assume SHP
            if (Directory.Exists(_destPath))
            {
                destConn = new FdoConnection("OSGeo.SHP", "DefaultFileLocation=" + _destPath);
            }
            else
            {
                if (ExpressUtility.CreateFlatFileDataSource(_destPath))
                    destConn = ExpressUtility.CreateFlatFileConnection(_destPath);
                else
                    throw new FdoException("Could not create data source: " + _destPath);
            }

            try
            {
                srcConn.Open();
                destConn.Open();

                FdoBulkCopyOptions options = new FdoBulkCopyOptions(srcConn, destConn);
                using (FdoFeatureService srcService = srcConn.CreateFeatureService())
                using (FdoFeatureService destService = destConn.CreateFeatureService())
                {
                    //See if spatial context needs to be copied
                    if (!string.IsNullOrEmpty(_srcSpatialContext))
                    {
                        Console.WriteLine("Copying spatial context");
                        SpatialContextInfo srcCtx = srcService.GetSpatialContext(_srcSpatialContext);
                        if (srcCtx != null)
                        {
                            options.AddSourceSpatialContext(srcCtx);
                        }
                    }
                    FeatureSchema srcSchema = null;
                    //See if partial class list is needed
                    if (_srcClasses.Count > 0)
                    {
                        srcSchema = srcService.PartialDescribeSchema(_srcSchema, _srcClasses);
                    }
                    else //Full copy
                    {
                        srcSchema = srcService.GetSchemaByName(_srcSchema);
                    }

                    IncompatibleSchema incSchema;
                    if (destService.CanApplySchema(srcSchema, out incSchema))
                    {
                        Console.WriteLine("Applying source schema to target");
                        destService.ApplySchema(srcSchema);

                        //Now set class copy options
                        foreach (ClassDefinition cd in srcSchema.Classes)
                        {
                            options.AddClassCopyOption(cd.Name, cd.Name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Applying modified source schema to target");
                        FeatureSchema fixedSchema = destService.AlterSchema(srcSchema, incSchema);
                        destService.ApplySchema(fixedSchema);

                        //Now set class copy options
                        foreach (ClassDefinition cd in fixedSchema.Classes)
                        {
                            options.AddClassCopyOption(cd.Name, cd.Name);
                        }
                    }

                    


                    FdoBulkCopy copy = new FdoBulkCopy(options);
                    copy.ProcessMessage += new MessageEventHandler(OnMessage);
                    copy.ProcessCompleted += new EventHandler(OnCompleted);
                    Console.WriteLine("Executing bulk copy");
                    copy.Execute();
                    retCode = CommandStatus.E_OK;
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                retCode = CommandStatus.E_FAIL_UNKNOWN;
            }
            finally
            {
                srcConn.Dispose();
                destConn.Dispose();
            }
            return (int)retCode;
        }

        void OnCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Copy completed");
        }

        void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
