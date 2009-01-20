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
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoAggregateQueryView : IQuerySubView
    {
        FeatureSchemaCollection SchemaList { set; }
        ClassCollection ClassList { set; }

        FeatureSchema SelectedSchema { get; }
        ClassDefinition SelectedClass { get; }

        IList<string> PropertyList { set; }

        IList<ComputedProperty> ComputedProperties { get; set; }

        string Filter { get; set; }
        int Limit { get; }

        FeatureAggregateOptions QueryObject { get; }
        IList<string> SelectPropertyList { get; }
        IList<string> OrderByList { get; }

        string SelectedOrderByProperty { get; }
        string SelectedOrderByPropertyToAdd { get; }

        string SelectedGroupByProperty { get; }
        string SelectedGroupByPropertyToAdd { get; }

        void AddOrderBy(string prop);
        void RemoveOrderBy(string prop);

        void AddGroupBy(string prop);
        void RemoveGroupBy(string prop);
        IList<string> GroupByList { get; }
        IList<string> GroupableProperties { set; }

        bool OrderingEnabled { get; set; }
    }

    public class FdoAggregateQueryPresenter
    {
        private readonly IFdoAggregateQueryView _view;
        private FdoConnection _conn;
        private FdoFeatureService _service;

        public FdoAggregateQueryPresenter(IFdoAggregateQueryView view, FdoConnection conn)
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
                List<string> pg = new List<string>();
                foreach (PropertyDefinition pd in _view.SelectedClass.Properties)
                {
                    p.Add(pd.Name);
                    bool groupable = pd.PropertyType == PropertyType.PropertyType_DataProperty && ((pd as DataPropertyDefinition).DataType != DataType.DataType_CLOB && (pd as DataPropertyDefinition).DataType != DataType.DataType_BLOB);
                    if (groupable)
                        pg.Add(pd.Name);
                }
                _view.PropertyList = p;
                _view.GroupableProperties = pg;
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

        public void AddGroupByProperty()
        {
            string prop = _view.SelectedGroupByPropertyToAdd;
            if (prop != null && !_view.GroupByList.Contains(prop))
                _view.AddGroupBy(prop);
        }

        public void RemoveGroupByProperty()
        {
            string prop = _view.SelectedGroupByProperty;
            if (prop != null)
                _view.RemoveGroupBy(prop);
        }
    }
}
