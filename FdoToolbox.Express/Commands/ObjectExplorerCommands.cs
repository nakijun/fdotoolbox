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
using FdoToolbox.Core.Feature;
using System.Windows.Forms;
using FdoToolbox.Base;
using FdoToolbox.Core.Utility;

using Msg = ICSharpCode.Core.MessageService;
using Res = ICSharpCode.Core.ResourceService;
using FdoToolbox.Express.Controls;

namespace FdoToolbox.Express.Commands
{
    public class SaveSchemaAsSdfCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string path = FileService.SaveFile(Res.GetString("TITLE_SAVE_SCHEMA_AS_SDF"), Res.GetString("FILTER_SDF"));
            if (!string.IsNullOrEmpty(path))
            {
                TreeNode schemaNode = Workbench.Instance.ObjectExplorer.GetSelectedNode();
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                FdoConnection conn = mgr.GetConnection(schemaNode.Parent.Name);
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    if (ExpressUtility.CreateFlatFileDataSource("OSGeo.SDF", path))
                    {
                        FdoConnection conn2 = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", path);
                        conn2.Open();
                        using (FdoFeatureService service2 = conn2.CreateFeatureService())
                        {
                            OSGeo.FDO.Schema.FeatureSchema schema = service.GetSchemaByName(schemaNode.Name);
                            service2.ApplySchema(FdoFeatureService.CloneSchema(schema));
                        }
                        bool connect = Msg.AskQuestion("SDF file created. Connect to it?", "Connect?");
                        if (connect)
                        {
                            string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), "");
                            if (name == null)
                                return;

                            while (name == string.Empty || mgr.NameExists(name))
                            {
                                Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                                name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                                if (name == null)
                                    return;
                            }
                            mgr.AddConnection(name, conn2);
                        }
                        else
                        {
                            conn2.Dispose();
                        }
                    }
                }
            }
        }
    }

    public class CopySpatialContextsCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            Workbench wb = Workbench.Instance;
            TreeNode connNode = wb.ObjectExplorer.GetSelectedNode();
            string srcConnName = connNode.Name;
            CopySpatialContextsCtl ctl = new CopySpatialContextsCtl(srcConnName);
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}
