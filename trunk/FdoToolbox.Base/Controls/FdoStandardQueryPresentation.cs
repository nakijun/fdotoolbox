using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoStandardQueryView : IQuerySubView
    {
        FeatureSchemaCollection SchemaList { set; }
        ClassCollection ClassList { set; }

        FeatureSchema SelectedSchema { get; }
        ClassDefinition SelectedClass { get; }

        IList<string> PropertyList { set; }
        
        IList<ComputedProperty> ComputedProperties { get; set; }

        string Filter { get; set; }
        int Limit { get; }

        FeatureQueryOptions QueryObject { get; }
        IList<string> SelectPropertyList { get; }
        IList<string> OrderByList { get; }

        string SelectedOrderByProperty { get; }
        string SelectedOrderByPropertyToAdd { get; }

        void AddOrderBy(string prop);
        void RemoveOrderBy(string prop);

        bool OrderingEnabled { get; set; }
    }

    public class FdoStandardQueryPresenter
    {
        private IFdoStandardQueryView _view;
        private FdoConnection _conn;
        private FdoFeatureService _service;

        public FdoStandardQueryPresenter(IFdoStandardQueryView view, FdoConnection conn)
        {
            _view = view;
            _conn = conn;
            _service = _conn.CreateFeatureService();
            _view.OrderingEnabled = false;
        }

        public void GetSchemas()
        {
            _view.SchemaList = _service.DescribeSchema();
        }

        public void SchemaChanged()
        {
            if (_view.SelectedSchema != null)
            {
                _view.ClassList = _view.SelectedSchema.Classes;
            }
        }

        public void ClassChanged()
        {
            if (_view.SelectedClass != null)
            {
                List<string> p = new List<string>();
                foreach (PropertyDefinition pd in _view.SelectedClass.Properties)
                {
                    p.Add(pd.Name);
                }
                _view.PropertyList = p;
                _view.FireMapPreviewStateChanged(_view.SelectedClass.ClassType == ClassType.ClassType_FeatureClass);
            }
        }

        public void RemoveOrderByProperty()
        {
            string prop = _view.SelectedOrderByProperty;
            if (prop != null)
                _view.RemoveOrderBy(prop);
        }

        public void AddOrderByProperty()
        {
            string prop = _view.SelectedOrderByPropertyToAdd;
            if (prop != null && !_view.OrderByList.Contains(prop))
                _view.AddOrderBy(prop);
        }
    }
}
