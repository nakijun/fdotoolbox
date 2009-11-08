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
    internal interface IFdoStandardQueryView : IQuerySubView
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

    internal class FdoStandardQueryPresenter
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
