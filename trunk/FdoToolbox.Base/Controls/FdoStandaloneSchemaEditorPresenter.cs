using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls
{
    public class FdoStandaloneSchemaEditorPresenter
    {
        private readonly IFdoSchemaMgrView _view;

        public FdoStandaloneSchemaEditorPresenter(IFdoSchemaMgrView view)
        {
            _view = view;
        }

        public void Init()
        {
        }

        internal void DeleteClass()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void DeleteSchema()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void AddSchema(OSGeo.FDO.Schema.FeatureSchema schema)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
