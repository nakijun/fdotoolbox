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
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Class options for <see cref="FdoBulkCopy"/>
    /// </summary>
    public class FdoClassCopyOptions
    {
        private FdoConnection _source;

        /// <summary>
        /// Gets the source connection which to copy features from
        /// </summary>
        public FdoConnection SourceConnection
        {
            get { return _source; }
        }

        private FdoConnection _target;
       
        /// <summary>
        /// Gets the target connection which to write features to
        /// </summary>
        public FdoConnection TargetConnection
        {
            get { return _target; }
        }

        private string _sourceClass;

        /// <summary>
        /// Gets the source feature class to copy from
        /// </summary>
        public string SourceClassName
        {
            get { return _sourceClass; }
        }

        private string _targetClass;

        /// <summary>
        /// Gets the target feature class to write to
        /// </summary>
        public string TargetClassName
        {
            get { return _targetClass; }
        }

        private string _SourceFilter;

        /// <summary>
        /// Gets or sets the filter to apply to the source class query
        /// </summary>
        public string SourceFilter
        {
            get { return _SourceFilter; }
            set { _SourceFilter = value; }
        }

        private bool _DeleteTarget;

        /// <summary>
        /// Determines if the data in the target feature class should be 
        /// deleted before commencing copying.
        /// </summary>
        public bool DeleteTarget
        {
            get { return _DeleteTarget; }
            set { _DeleteTarget = value; }
        }
	
        private NameValueCollection _propertyMappings;

        /// <summary>
        /// Gets the property mappings. If this is empty, then all source properties 
        /// will be used as target properties
        /// </summary>
        public NameValueCollection PropertyMappings
        {
            get { return _propertyMappings; }
        }

        private NameValueCollection _sourceExpressions;

        /// <summary>
        /// Gets the source expressions. 
        /// </summary>
        public NameValueCollection SourceExpressions
        {
            get { return _sourceExpressions; }
        }

        /// <summary>
        /// Gets the list of source property names. Use this to get the mapped (target)
        /// property name. If this is empty, then all source properties will be used
        /// as target properties
        /// </summary>
        public string[] SourcePropertyNames
        {
            get { return _propertyMappings.AllKeys; }
        }

        /// <summary>
        /// Adds a source to target property mapping.
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="targetProperty"></param>
        public void AddPropertyMapping(string sourceProperty, string targetProperty)
        {
            _propertyMappings[sourceProperty] = targetProperty;
        }

        private int _BatchSize;

        /// <summary>
        /// Gets or sets the batch size. If greater than zero, a batch insert operation
        /// will be used instead of a regular insert operation (if supported by the
        /// target connection)
        /// </summary>
        public int BatchSize
        {
            get { return _BatchSize; }
            set { _BatchSize = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="srcConn"></param>
        /// <param name="destConn"></param>
        /// <param name="srcClass"></param>
        /// <param name="destClass"></param>
        public FdoClassCopyOptions(FdoConnection srcConn, FdoConnection destConn, string srcClass, string destClass)
        {
            if (srcConn == null)
                throw new ArgumentNullException("srcConn");
            if (destConn == null)
                throw new ArgumentNullException("destConn");
            if (string.IsNullOrEmpty(srcClass))
                throw new ArgumentException("parameter srcClass is null or empty");
            if (string.IsNullOrEmpty(destClass))
                throw new ArgumentException("parameter destClass is null or empty");

            _propertyMappings = new NameValueCollection();
            _sourceExpressions = new NameValueCollection();
            _source = srcConn;
            _target = destConn;
            _sourceClass = srcClass;
            _targetClass = destClass;
        }

        /// <summary>
        /// Adds the source expression.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddSourceExpression(string key, string value)
        {
            _sourceExpressions[key] = value;
        }
    }
}
