#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Expression;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Use this class to set filter criteria for selecting features from a FDO datastore
    /// </summary>
    public class FeatureQueryOptions
    {
        private string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        private List<string> _PropertyList;

        public ReadOnlyCollection<string> PropertyList
        {
            get { return _PropertyList.AsReadOnly(); }
        }

        private Dictionary<string, Expression> _ComputedProperties;

        public Dictionary<string, Expression> ComputedProperties
        {
            get { return _ComputedProperties; }
        }

        private string _Filter;

        public string Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }

        public FeatureQueryOptions(string className)
        {
            _ClassName = className;
            _PropertyList = new List<string>();
            _ComputedProperties = new Dictionary<string, Expression>();
            _OrderBy = new List<string>();
        }

        public bool IsFilterSet
        {
            get { return !string.IsNullOrEmpty(_Filter); }
        }

        public void AddComputedProperty(string alias, string expression)
        {
            _ComputedProperties.Add(alias, Expression.Parse(expression));
        }

        public void AddComputedProperty(NameValueCollection computedProperties)
        {
            if (computedProperties != null && computedProperties.Count > 0)
            {
                foreach (string alias in computedProperties.Keys)
                {
                    AddComputedProperty(alias, computedProperties[alias]);
                }
            }
        }

        public void RemoveComputedProperty(string alias)
        {
            if (_ComputedProperties.ContainsKey(alias))
                _ComputedProperties.Remove(alias);
        }

        public void RemoveFeatureProperty(string propertyName)
        {
            _PropertyList.Remove(propertyName);
        }

        public void AddFeatureProperty(string propertyName)
        {
            _PropertyList.Add(propertyName);
        }

        public void AddFeatureProperty(IEnumerable<string> propertyNames)
        {
            _PropertyList.AddRange(propertyNames);
        }

        private List<string> _OrderBy;

        public ReadOnlyCollection<string> OrderBy
        {
            get { return _OrderBy.AsReadOnly(); }
        }

        private OrderingOption _OrderingOption;

        public OrderingOption OrderOption
        {
            get { return _OrderingOption; }
        }

        public void SetOrderingOption(IEnumerable<string> propertyNames, OrderingOption option)
        {
            _OrderBy.Clear();
            _OrderBy.AddRange(propertyNames);
            _OrderingOption = option;
        }
    }

    /// <summary>
    /// Use this class to set the filter criteria used to select groups of features from a FDO datastore or for restricting the values returned to be unique.
    /// </summary>
    public class FeatureAggregateOptions : FeatureQueryOptions
    {
        private bool _Distinct;

        public bool Distinct
        {
            get { return _Distinct; }
            set { _Distinct = value; }
        }

        private string _GroupFilter;

        public string GroupFilter
        {
            get { return _GroupFilter; }
        }

        private List<string> _GroupByProperties;

        public IEnumerable<string> GroupByProperties
        {
            get { return _GroupByProperties; }
        }

        public FeatureAggregateOptions(string className)
            : base(className)
        {
            _GroupByProperties = new List<string>();
        }

        public void SetGroupingFilter(IEnumerable<string> groupByProperties, string groupFilter)
        {
            _GroupFilter = groupFilter;
            _GroupByProperties.Clear();
            _GroupByProperties.AddRange(groupByProperties);
        }
    }
}
