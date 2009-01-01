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
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using FdoToolbox.Express.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Commands
{
    public class CreateSdfCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            CreateSdfCtl ctl = new CreateSdfCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);

            //string sdfFile = FileService.SaveFile(ResourceService.GetString("TITLE_CREATE_SDF"), ResourceService.GetString("FILTER_SDF"));
            //if (sdfFile != null)
            //{
            //    if (System.IO.File.Exists(sdfFile))
            //        System.IO.File.Delete(sdfFile);

            //    if (ExpressUtility.CreateFlatFileDataSource("OSGeo.SDF", sdfFile))
            //    {
            //        if (MessageService.AskQuestion(ResourceService.GetString("MSG_CONNECT_SDF"), ResourceService.GetString("TITLE_CREATE_SDF")))
            //        {
            //            string name = string.Empty;
            //            name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_NEW_CONNECTION"), ResourceService.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
            //            if (name == null)
            //                return;

            //            while (name == string.Empty)
            //            {
            //                name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_NEW_CONNECTION"), ResourceService.GetString("PROMPT_ENTER_NEW_CONNECTION_NAME"), name);
            //                if (name == null)
            //                    return;
            //            }

            //            FdoConnectionManager mgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            //            FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", sdfFile);
            //            mgr.AddConnection(name, conn);
            //        }
            //    }
            //}
        }
    }

    public class CreateShpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            CreateShpCtl ctl = new CreateShpCtl();
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}
