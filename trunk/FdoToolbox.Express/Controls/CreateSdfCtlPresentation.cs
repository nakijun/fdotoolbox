using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateSdfView
    {
        string SdfFile { get; }
        string FeatureSchemaDefinition { get; }
        bool CreateConnection { get; }
        string ConnectionName { get; set; }
        bool ConnectionEnabled { set; }
    }

    public class CreateSdfPresenter
    {
        private readonly ICreateSdfView _view;
        private FdoConnectionManager _connMgr;

        public CreateSdfPresenter(ICreateSdfView view)
        {
            _view = view;
            _connMgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            CheckConnect();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        public bool CreateSdf()
        {
            if (ExpressUtility.CreateFlatFileDataSource("OSGeo.SDF", _view.SdfFile))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", _view.SdfFile);
                if (FileService.FileExists(_view.FeatureSchemaDefinition))
                {
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_view.FeatureSchemaDefinition);
                    }
                }
                if (_view.CreateConnection)
                {
                    _connMgr.AddConnection(_view.ConnectionName, conn);
                }
                else
                {
                    conn.Dispose();
                }
            }
            return true;
        }

        public void CheckConnect()
        {
            if (!_view.CreateConnection)
                _view.ConnectionName = "";

            _view.ConnectionEnabled = _view.CreateConnection;
        }
    }
}
