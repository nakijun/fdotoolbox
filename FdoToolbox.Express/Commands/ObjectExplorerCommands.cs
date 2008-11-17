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
}
