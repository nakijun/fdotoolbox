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
using System.Collections.Specialized;
using FdoToolbox.Core.Configuration;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Core.ETL.Specialized
{
    public class FdoClassCopyOptions
    {
        private string _SourceConnectionName;
        private string _TargetConnectionName;

        /// <summary>
        /// Gets the name of the source connection.
        /// </summary>
        /// <value>The name of the source connection.</value>
        public string SourceConnectionName
        {
            get { return _SourceConnectionName; }
        }

        /// <summary>
        /// Gets the name of the target connection.
        /// </summary>
        /// <value>The name of the target connection.</value>
        public string TargetConnectionName
        {
            get { return _TargetConnectionName; }
        }

        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
	

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoClassCopyOptions"/> class.
        /// </summary>
        /// <param name="sourceConnectionName">Name of the source connection.</param>
        /// <param name="targetConnectionName">Name of the target connection.</param>
        /// <param name="sourceSchema">The source schema.</param>
        /// <param name="sourceClass">The source class.</param>
        /// <param name="targetSchema">The target schema.</param>
        /// <param name="targetClass">The target class.</param>
        public FdoClassCopyOptions(string sourceConnectionName, string targetConnectionName, string sourceSchema, string sourceClass, string targetSchema, string targetClass)
        {
            _SourceConnectionName = sourceConnectionName;
            _TargetConnectionName = targetConnectionName;
            _sourceClass = sourceClass;
            _sourceSchema = sourceSchema;
            _targetClass = targetClass;
            _targetSchema = targetSchema;

            _propertyMappings = new NameValueCollection();
            _expressionAliasMap = new NameValueCollection();
            _expressionMappings = new NameValueCollection();

            _rules = new Dictionary<string,FdoDataPropertyConversionRule>();
        }

        private string _sourceSchema;

        /// <summary>
        /// Gets the source schema.
        /// </summary>
        /// <value>The source schema.</value>
        public string SourceSchema
        {
            get { return _sourceSchema; }
        }

        private string _targetSchema;

        /// <summary>
        /// Gets the target schema.
        /// </summary>
        /// <value>The target schema.</value>
        public string TargetSchema
        {
            get { return _targetSchema; }
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

        private FdoBulkCopyOptions _Parent;

        /// <summary>
        /// Gets the bulk copy options
        /// </summary>
        public FdoBulkCopyOptions Parent
        {
            get { return _Parent; }
            internal set { _Parent = value; }
        }

        private NameValueCollection _propertyMappings;
        private NameValueCollection _expressionMappings;
        private NameValueCollection _expressionAliasMap;

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
        /// Gets the list of source expression aliases.
        /// </summary>
        /// <value>The source aliases.</value>
        public string[] SourceAliases
        {
            get { return _expressionAliasMap.AllKeys; }
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

        private bool _FlattenGeometries;

        /// <summary>
        /// Gets or sets a value indicating whether to strip the Z and M components of all
        /// geometries
        /// </summary>
        /// <value><c>true</c> if [flatten geometries]; otherwise, <c>false</c>.</value>
        public bool FlattenGeometries
        {
            get { return _FlattenGeometries; }
            set { _FlattenGeometries = value; }
        }

        /// <summary>
        /// Adds the source expression.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="targetProp">The target property.</param>
        public void AddSourceExpression(string alias, string expression, string targetProp)
        {
            _expressionAliasMap[alias] = expression;
            _expressionMappings[alias] = targetProp;
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <param name="srcProp">The source prop.</param>
        /// <returns></returns>
        public string GetTargetProperty(string srcProp)
        {
            return _propertyMappings[srcProp];
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public string GetExpression(string alias)
        {
            return _expressionAliasMap[alias];
        }

        /// <summary>
        /// Gets the target property for alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public string GetTargetPropertyForAlias(string alias)
        {
            if (_expressionAliasMap[alias] != null)
                return _expressionMappings[alias];

            return null;
        }

        private Dictionary<string, FdoDataPropertyConversionRule> _rules;

        /// <summary>
        /// Gets the conversion rules.
        /// </summary>
        /// <value>The conversion rules.</value>
        public ICollection<FdoDataPropertyConversionRule> ConversionRules
        {
            get { return _rules.Values; }
        }

        /// <summary>
        /// Adds a data conversion rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public void AddDataConversionRule(string name, FdoDataPropertyConversionRule rule)
        {
            _rules.Add(name, rule);
        }

        /// <summary>
        /// Gets the data conversion rule.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FdoDataPropertyConversionRule GetDataConversionRule(string name)
        {
            if (_rules.ContainsKey(name))
                return _rules[name];
            return null;
        }

        internal static FdoClassCopyOptions FromElement(FdoCopyTaskElement el, FdoConnectionEntryElement[] connectionEntries)
        {
            FdoConnection sourceConn = null;
            FdoConnection targetConn = null;

            foreach (FdoConnectionEntryElement connEnt in connectionEntries)
            {
                if (connEnt.name == el.Source.connection)
                    sourceConn = new FdoConnection(connEnt.provider, connEnt.ConnectionString);

                if (connEnt.name == el.Target.connection)
                    targetConn = new FdoConnection(connEnt.provider, connEnt.ConnectionString);

                if (sourceConn != null && targetConn != null)
                    break;
            }

            if (sourceConn == null)
                throw new TaskLoaderException("The referenced source connection is not defined");

            if (targetConn == null)
                throw new TaskLoaderException("The referenced target connection is not defined");

            FdoClassCopyOptions opts = new FdoClassCopyOptions(el.Source.connection, el.Target.connection, el.Source.schema, el.Source.@class, el.Target.schema, el.Target.@class);
            opts.DeleteTarget = el.Options.DeleteTarget;
            opts.SourceFilter = el.Options.Filter;
            if (!el.Options.FlattenGeometriesSpecified)
                opts.FlattenGeometries = false;
            else
                opts.FlattenGeometries = el.Options.FlattenGeometries;
            if (!string.IsNullOrEmpty(el.Options.BatchSize))
                opts.BatchSize = Convert.ToInt32(el.Options.BatchSize);
            opts.Name = el.name;

            sourceConn.Open();
            targetConn.Open();

            using (FdoFeatureService srcSvc = sourceConn.CreateFeatureService(true))
            using (FdoFeatureService dstSvc = targetConn.CreateFeatureService(true))
            {
                ClassDefinition srcClass = srcSvc.GetClassByName(el.Source.@class);
                ClassDefinition dstClass = dstSvc.GetClassByName(el.Target.@class);

                // 
                foreach (FdoPropertyMappingElement propMap in el.PropertyMappings)
                {
                    if (srcClass.Properties.IndexOf(propMap.source) < 0)
                        throw new TaskLoaderException("The property mapping (" + propMap.source + " -> " + propMap.target +") contains a source property not found in the source class definition");

                    if (dstClass.Properties.IndexOf(propMap.target) < 0)
                        throw new TaskLoaderException("The property mapping (" + propMap.source + " -> " + propMap.target + ") contains a target property not found in the target class definition");

                    PropertyDefinition sp = srcClass.Properties[propMap.source];
                    PropertyDefinition tp = dstClass.Properties[propMap.target];

                    if (sp.PropertyType != tp.PropertyType)
                        throw new TaskLoaderException("The properties in the mapping (" + propMap.source + " -> " + propMap.target + ") are of different types");

                    //if (sp.PropertyType != PropertyType.PropertyType_DataProperty)
                    //    throw new TaskLoaderException("One or more properties in the mapping (" + propMap.source + " -> " + propMap.target + ") is not a data property");

                    DataPropertyDefinition sdp = sp as DataPropertyDefinition;
                    DataPropertyDefinition tdp = tp as DataPropertyDefinition;

                    opts.AddPropertyMapping(propMap.source, propMap.target);

                    //Property mapping is between two data properties
                    if (sdp != null && tdp != null)
                    {
                        //Types not equal, so add a conversion rule
                        if (sdp.DataType != tdp.DataType)
                        {
                            FdoDataPropertyConversionRule rule = new FdoDataPropertyConversionRule(
                                propMap.source,
                                propMap.target,
                                sdp.DataType,
                                tdp.DataType,
                                propMap.nullOnFailedConversion,
                                propMap.truncate);
                            opts.AddDataConversionRule(propMap.source, rule);
                        }
                    }
                }

                //
                foreach (FdoExpressionMappingElement exprMap in el.ExpressionMappings)
                {
                    if (string.IsNullOrEmpty(exprMap.target))
                        continue;

                    opts.AddSourceExpression(exprMap.alias, exprMap.Expression, exprMap.target);

                    FdoPropertyType? pt = ExpressionUtility.ParseExpressionType(exprMap.Expression, sourceConn);
                    if (pt.HasValue)
                    {
                        DataType? srcDt = ValueConverter.GetDataType(pt.Value);
                        if (srcDt.HasValue)
                        {
                            PropertyDefinition tp = dstClass.Properties[exprMap.target];
                            DataPropertyDefinition tdp = tp as DataPropertyDefinition;
                            if (tdp != null)
                            {
                                if (srcDt.Value != tdp.DataType)
                                {
                                    FdoDataPropertyConversionRule rule = new FdoDataPropertyConversionRule(
                                        exprMap.alias,
                                        exprMap.target,
                                        srcDt.Value,
                                        tdp.DataType,
                                        exprMap.nullOnFailedConversion,
                                        exprMap.truncate);
                                    opts.AddDataConversionRule(exprMap.alias, rule);
                                }
                            }
                        }
                    }
                }

                sourceConn.Close();
                targetConn.Close();
            }

            return opts;
        }

        internal FdoCopyTaskElement ToElement()
        {
            FdoCopyTaskElement el = new FdoCopyTaskElement();
            el.name = this.Name;
            el.Options = new FdoCopyOptionsElement();
            el.Options.DeleteTarget = this.DeleteTarget;
            el.Options.Filter = this.SourceFilter;
            if (this.BatchSize > 0)
                el.Options.BatchSize = this.BatchSize.ToString();

            el.Source = new FdoCopySourceElement();
            
            el.Source.connection = this.SourceConnectionName;
            el.Source.schema = this.SourceSchema;
            el.Source.@class = this.SourceClassName;

            el.Target = new FdoCopyTargetElement();

            el.Target.connection = this.TargetConnectionName;
            el.Target.schema = this.TargetSchema;
            el.Target.@class = this.TargetClassName;

            List<FdoPropertyMappingElement> propMappings = new List<FdoPropertyMappingElement>();
            List<FdoExpressionMappingElement> exprMappings = new List<FdoExpressionMappingElement>();

            Dictionary<string, FdoPropertyMappingElement> convRules = new Dictionary<string, FdoPropertyMappingElement>();
            foreach (FdoDataPropertyConversionRule rule in this.ConversionRules)
            {
                FdoPropertyMappingElement map = new FdoPropertyMappingElement();
                //map.sourceDataType = rule.SourceDataType.ToString();
                //map.targetDataType = rule.TargetDataType.ToString();
                map.nullOnFailedConversion = rule.NullOnFailure;
                map.truncate = rule.Truncate;
                map.source = rule.SourceProperty;
                map.target = rule.TargetProperty;
                convRules.Add(map.source, map);
            }

            foreach (string prop in this.SourcePropertyNames)
            {
                if (convRules.ContainsKey(prop))
                {
                    propMappings.Add(convRules[prop]);
                }
                else 
                {
                    FdoPropertyMappingElement map = new FdoPropertyMappingElement();
                    map.source = prop;
                    map.target = this.GetTargetProperty(prop);
                    propMappings.Add(map);
                }
            }

            foreach (string alias in this.SourceAliases)
            {
                FdoExpressionMappingElement map = new FdoExpressionMappingElement();
                map.alias = alias;
                map.Expression = this.GetExpression(alias);
                map.target = this.GetTargetPropertyForAlias(alias);
                exprMappings.Add(map);
            }

            el.PropertyMappings = propMappings.ToArray();
            el.ExpressionMappings = exprMappings.ToArray();

            return el;
        }
    }
}
