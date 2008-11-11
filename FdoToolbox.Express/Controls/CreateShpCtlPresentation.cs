using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;
using System.IO;
using ICSharpCode.Core;

namespace FdoToolbox.Express.Controls
{
    public interface ICreateShpView
    {
        string ShpFile { get; }
        string FeatureSchemaDefinition { get; }
        bool CreateConnection { get; }
        string ConnectionName { get; set; }
        bool ConnectionEnabled { set; }
    }

    public class CreateShpPresenter
    {
        private readonly ICreateShpView _view;
        private FdoConnectionManager _connMgr;

        public CreateShpPresenter(ICreateShpView view)
        {
            _view = view;
            _connMgr = ServiceManager.Services.GetService<FdoConnectionManager>();
            CheckConnect();
        }

        public bool CheckConnectionName()
        {
            return !_connMgr.NameExists(_view.ConnectionName);
        }

        public bool CreateShp()
        {
            //Creating SHP files is as follows
            //
            // 1. Connect to the *parent* directory of the shape file we want to create
            // 2. Apply the schema to this connection

            if (FileService.FileExists(_view.FeatureSchemaDefinition))
            {
                try
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", Path.GetDirectoryName(_view.ShpFile));
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_view.FeatureSchemaDefinition);
                    }
                    conn.Dispose();
                    if (_view.CreateConnection)
                    {
                        conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", _view.ShpFile);
                        conn.Open();
                        _connMgr.AddConnection(_view.ConnectionName, conn);
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    LoggingService.Error("Failed to create SHP", ex);
                    return false;
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
