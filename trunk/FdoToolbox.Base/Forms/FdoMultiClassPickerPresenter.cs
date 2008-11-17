using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Forms
{
    public interface IFdoMultiClassPickerView
    {
        FeatureSchemaCollection SchemaList { set; }
        FeatureSchema SelectedSchema { get; }

        ClassCollection ClassList { set; }
        IList<ClassDefinition> SelectedClasses { get; }

        string Title { set; }
        string Message { set; }
    }

    public class FdoMultiClassPickerPresenter
    {
        private readonly IFdoMultiClassPickerView _view;

        public FdoMultiClassPickerPresenter(IFdoMultiClassPickerView view, string title, string message)
        {
            _view = view;
            _view.Title = title;
            _view.Message = message;
        }

        public void Init(FeatureSchemaCollection schemas)
        {
            _view.SchemaList = schemas;
        }

        public void SchemaChanged()
        {
            if (_view.SelectedSchema != null)
            {
                _view.ClassList = _view.SelectedSchema.Classes;
            }
        }
    }
}
