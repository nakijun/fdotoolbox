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
using FdoToolbox.Base;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using System.Collections.ObjectModel;
using FdoToolbox.Core.ETL.Specialized;
using System.Collections.Specialized;
using FdoToolbox.Core;
using FdoToolbox.Tasks.Services;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Connections.Capabilities;

namespace FdoToolbox.Tasks.Controls
{
    public interface IFdoBulkCopyView : IViewContent
    {
        string TaskName { get; set; }

        List<string> SourceConnections { set; }
        List<string> TargetConnections { set; }

        List<string> SourceSchemas { set; }
        List<string> TargetSchemas { set; }

        string SelectedSourceConnection { get; set; }
        string SelectedTargetConnection { get; set; }

        string SelectedSourceSchema { get; set; }
        string SelectedTargetSchema { get; set; }

        List<string> SourceSpatialContexts { get; set; }
        string SpatialFilter { get; }
        int BatchInsertSize { get; set; }

        bool BatchEnabled { set; }
        bool CopySpatialContexts { get; set; }
        bool CanDefineMappings { set; }

        void ClearMappings();
        void AddClass(string className);
        void AddClassSourceFilterOption(string className);
        void AddClassDeleteOption(string className);
        void AddClassProperty(string className, string propName, string imgKey);
        void MapClassProperty(string className, string propName, string targetProp);
        void MapClass(string className, string targetClass);

        bool GetClassDeleteOption(string className);
        string GetClassFilterOption(string className);

        List<string> MappableClasses { set; }

        void SetMappableProperties(string className, List<string> properties);

        void RemoveAllMappings();

        string GetSourceExpression(string className, string alias);

        void AddExpression(string className, string alias, string expr);
        void EditExpression(string className, string alias, string expr);
        void RemoveExpression(string className, string alias);
        void MapExpression(string className, string alias, string targetProp);
        NameValueCollection GetExpressions(string className);

        void CheckSpatialContext(string context, bool state);

        void SetSourceFilter(string className, string value);
        void SetClassDelete(string className, bool value);
    }

    public class FdoBulkCopyPresenter
    {
        private readonly IFdoBulkCopyView _view;
        private readonly IFdoConnectionManager _connMgr;
        private readonly TaskManager _taskMgr;

        private Dictionary<string, string> _classMappings;
        private Dictionary<string, NameValueCollection> _propertyMappings;
        private Dictionary<string, NameValueCollection> _expressionMappings;

        public FdoBulkCopyPresenter(IFdoBulkCopyView view, IFdoConnectionManager connMgr, TaskManager taskMgr)
        {
            _view = view;
            _connMgr = connMgr;
            _classMappings = new Dictionary<string, string>();
            _propertyMappings = new Dictionary<string, NameValueCollection>();
            _expressionMappings = new Dictionary<string, NameValueCollection>();
            _taskMgr = taskMgr;
        }

        public void Init()
        {
            ICollection<string> connNames = _connMgr.GetConnectionNames();
            _view.SourceConnections = new List<string>(connNames);
            _view.TargetConnections = new List<string>(connNames);
            this.SourceConnectionChanged();
            this.TargetConnectionChanged();
        }

        public void Init(FdoBulkCopy copy)
        {
            this.Init();
            _bcp = copy;
            FdoBulkCopyOptions options = _bcp.Options;

            _view.SelectedSourceConnection = _connMgr.GetName(options.SourceConnection);
            _view.SelectedSourceSchema = options.SourceSchema;
            
            _view.SelectedTargetConnection = _connMgr.GetName(options.TargetConnection);
            _view.SelectedTargetSchema = options.TargetSchema;

            foreach (FdoClassCopyOptions copt in options.ClassCopyOptions)
            {
                MapClass(copt.SourceClassName, copt.TargetClassName);

                foreach (string srcProp in copt.SourcePropertyNames)
                {
                    MapProperty(copt.SourceClassName, srcProp, copt.TargetClassName, copt.GetTargetProperty(srcProp));
                }
                foreach (string srcAlias in copt.SourceAliases)
                {
                    _view.AddExpression(copt.SourceClassName, srcAlias, copt.GetExpression(srcAlias));
                    MapExpression(copt.SourceClassName, srcAlias, copt.TargetClassName, copt.GetTargetPropertyForAlias(srcAlias));
                }

                _view.SetClassDelete(copt.SourceClassName, copt.DeleteTarget);
                _view.SetSourceFilter(copt.SourceClassName, copt.SourceFilter);
            }

            if (options.SourceSpatialContexts.Count > 0)
            {
                _view.CopySpatialContexts = true;
                foreach (SpatialContextInfo context in options.SourceSpatialContexts)
                {
                    _view.CheckSpatialContext(context.Name, true);
                }
            }
            else
            {
                _view.CopySpatialContexts = false;
            }
        }

        public void SourceConnectionChanged()
        {
            string connName = _view.SelectedSourceConnection;
            if (connName != null)
            {
                using (FdoFeatureService service = _connMgr.GetConnection(connName).CreateFeatureService())
                {
                    ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                    List<string> ctxNames = new List<string>();
                    foreach (SpatialContextInfo c in contexts)
                    {
                        ctxNames.Add(c.Name);
                    }
                    _view.CopySpatialContexts = (ctxNames.Count > 0);
                    _view.SourceSpatialContexts = ctxNames;
                    _view.SourceSchemas = service.GetSchemaNames();
                    this.SourceSchemaChanged();
                }
            }
        }

        public void TargetConnectionChanged()
        {
            string connName = _view.SelectedTargetConnection;
            if (connName != null)
            {
                using (FdoFeatureService service = _connMgr.GetConnection(connName).CreateFeatureService())
                {
                    _view.BatchEnabled = service.SupportsBatchInsertion();
                    List<string> schemaNames = service.GetSchemaNames();
                    _view.TargetSchemas = schemaNames;
                    _view.CanDefineMappings = (schemaNames.Count > 0);
                    if (schemaNames.Count == 0)
                    {
                        _view.ShowMessage("Warning", "There are no schemas in the target connection. If you save this task, all source classes will be copied");
                        _view.RemoveAllMappings();
                    }
                    this.TargetSchemaChanged();
                }
            }
        }

        public void SourceSchemaChanged()
        {
            string connName = _view.SelectedSourceConnection;
            string schemaName = _view.SelectedSourceSchema;
            if (schemaName != null && connName != null)
            {
                _view.ClearMappings();
                _classMappings.Clear();
                _propertyMappings.Clear();
                _expressionMappings.Clear();
                using (FdoFeatureService service = _connMgr.GetConnection(connName).CreateFeatureService())
                {
                    FeatureSchema schema = service.GetSchemaByName(schemaName);
                    if (schema != null)
                    {
                        foreach (ClassDefinition cd in schema.Classes)
                        {
                            _view.AddClass(cd.Name);
                            _view.AddClassSourceFilterOption(cd.Name);
                            _view.AddClassDeleteOption(cd.Name);
                            SortedDictionary<string, PropertyDefinition> orderedProps = new SortedDictionary<string, PropertyDefinition>();
                            foreach (PropertyDefinition pd in cd.Properties)
                            {
                                orderedProps.Add(pd.Name, pd);
                            }
                            foreach (string propName in orderedProps.Keys)
                            {
                                _view.AddClassProperty(cd.Name, propName, GetImageKey(orderedProps[propName].PropertyType));
                            }
                        }
                    }
                }
            }
        }

        private string GetImageKey(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.PropertyType_AssociationProperty:
                    return "table_relationship";
                case PropertyType.PropertyType_DataProperty:
                    return "table";
                case PropertyType.PropertyType_GeometricProperty:
                    return "shape_handles";
                case PropertyType.PropertyType_ObjectProperty:
                    return "package";
                case PropertyType.PropertyType_RasterProperty:
                    return "image";
            }
            return null;
        }

        public void TargetSchemaChanged()
        {
            _view.RemoveAllMappings();
            FdoConnection targetConn = GetTargetConnection();
            using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(targetConn))
            {
                List<string> classes = service.GetClassNames(_view.SelectedTargetSchema);
                _view.MappableClasses = classes;
            }
        }

        private DataType? GetPropertyDataType(FdoConnection conn, string className, string propertyName)
        {
            using (FdoFeatureService svc = FdoConnectionUtil.CreateFeatureService(conn))
            {
                ClassDefinition cd = svc.GetClassByName(className);
                if (cd != null)
                {
                    PropertyDefinition pd = cd.Properties[propertyName];
                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        return (pd as DataPropertyDefinition).DataType;
                    }
                }
            }
            return null;
        }

        public void MapProperty(string srcClassName, string srcProperty, string destClassName, string destProperty)
        {
            //Throw if not mappable
            DataType? srcDt = GetPropertyDataType(GetSourceConnection(), destClassName, destProperty);
            DataType? dstDt = GetPropertyDataType(GetTargetConnection(), destClassName, destProperty);

            if (srcDt.HasValue && dstDt.HasValue)
            {
                if (!ValueConverter.IsConvertible(srcDt.Value, dstDt.Value))
                    throw new MappingException("Unable to map {0} to {1}. {2} is not convertible to {3}");

                _view.MapClassProperty(srcClassName, srcProperty, destProperty);
                _propertyMappings[srcClassName][srcProperty] = destProperty;

                if (srcDt.Value != dstDt.Value)
                {
                    //Add data type conversion rule
                }
            }
        }

        public void MapExpression(string srcClassName, string srcAlias, string destClassName, string destProperty)
        {
            //Throw if not mappable
            DataType? srcDt = ParseSourceExpression(srcClassName, srcAlias);
            DataType? dstDt = GetPropertyDataType(GetTargetConnection(), destClassName, destProperty);
            if (srcDt.HasValue && dstDt.HasValue)
            {
                if (!ValueConverter.IsConvertible(srcDt.Value, dstDt.Value))
                    throw new MappingException("Unable to map source expression to {0}. Source expression evaluates to {1}, which is not convertible to {2}");

                _view.MapExpression(srcClassName, srcAlias, destProperty);
                _expressionMappings[srcClassName][srcAlias] = destProperty;

                if (srcDt.Value != dstDt.Value)
                {
                    //Add data type conversion rule
                }
            }
        }

        private DataType? ParseSourceExpression(string srcClassName, string srcAlias)
        {
            string exprStr = _view.GetSourceExpression(srcClassName, srcAlias);
            if (!string.IsNullOrEmpty(exprStr))
            {
                Expression expr = Expression.Parse(exprStr);
                if (expr.GetType() == typeof(Function))
                {
                    Function func = expr as Function;
                    //Look-up its name in the capabilities
                    return GetFunctionReturnType(func.Name);
                }
                else if (expr.GetType() == typeof(BinaryExpression))
                {
                    return DataType.DataType_Boolean;
                }
                else if (expr.GetType() == typeof(UnaryExpression))
                {
                    return null;
                }
                else if (expr.GetType() == typeof(Identifier))
                {
                    return null; //TODO: use the data type of the referenced property
                }
                else if (expr.GetType() == typeof(DataValue))
                {
                    DataValue dv = (DataValue)expr;
                    return dv.DataType;
                }
            }
            return null;
        }

        private DataType? GetFunctionReturnType(string name)
        {
            FdoConnection conn = GetSourceConnection();
            FunctionDefinitionCollection funcs = (FunctionDefinitionCollection)conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ExpressionFunctions);
            foreach(FunctionDefinition fd in funcs)
            {
                if (fd.Name == name && fd.ReturnPropertyType == PropertyType.PropertyType_DataProperty)
                {
                    return fd.ReturnType;
                }
            }
            return null;
        }

        public void UnmapClass(string srcClass)
        {
            _classMappings.Remove(srcClass);
            _propertyMappings.Remove(srcClass);
            _view.MapClass(srcClass, null);
        }

        public void UnmapExpression(string className, string expressionAlias)
        {
            if (_expressionMappings.ContainsKey(className))
            {
                _expressionMappings[className].Remove(expressionAlias);
            }
            _view.MapExpression(className, expressionAlias, null);
        }

        public void UnmapProperty(string srcClass, string srcProperty)
        {
            if (_propertyMappings.ContainsKey(srcClass))
            {
                _propertyMappings[srcClass].Remove(srcProperty);
            }
            _view.MapClassProperty(srcClass, srcProperty, null);
        }

        public void MapClass(string srcClass, string targetClass)
        {
            //TODO: Throw if not mappable
            _view.MapClass(srcClass, targetClass);
            _classMappings[srcClass] = targetClass;
            FdoConnection conn = GetTargetConnection();
            using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(conn))
            {
                ClassDefinition cd = service.GetClassByName(_view.SelectedTargetSchema, targetClass);
                if (cd != null)
                {
                    List<string> propNames = new List<string>();
                    _propertyMappings[srcClass] = new NameValueCollection();
                    _expressionMappings[srcClass] = new NameValueCollection();
                    foreach (PropertyDefinition pd in cd.Properties)
                    {
                        //Ignore the following properties
                        //
                        // - Association
                        // - Raster
                        // - Object
                        // - Auto-Generated/ReadOnly Data Properties
                        // - ReadOnly Geometric Properties
                        switch (pd.PropertyType)
                        { 
                            case PropertyType.PropertyType_DataProperty:
                                DataPropertyDefinition dp = pd as DataPropertyDefinition;
                                if (!dp.IsAutoGenerated && !dp.ReadOnly)
                                    propNames.Add(pd.Name);
                                break;
                            case PropertyType.PropertyType_GeometricProperty:
                                GeometricPropertyDefinition gp = pd as GeometricPropertyDefinition;
                                if(!gp.ReadOnly)
                                    propNames.Add(pd.Name);
                                break;
                        }
                    }
                    _view.SetMappableProperties(srcClass, propNames);
                }
            }
        }

        public FdoConnection GetSourceConnection()
        {
            return _connMgr.GetConnection(_view.SelectedSourceConnection);
        }

        public FdoConnection GetTargetConnection()
        {
            return _connMgr.GetConnection(_view.SelectedTargetConnection);
        }

        public void CopySpatialContextChanged()
        {
            _view.CopySpatialContexts = _view.CopySpatialContexts;
        }

        public void SaveTask()
        {
            if (_bcp == null)
            {
                this.ApplySettings();
                _taskMgr.AddTask(_view.TaskName, _bcp);
            }
            else
            {
                this.ApplySettings();
            }
        }

        private NameValueCollection GetSourceExpressions(string className)
        {
            return _view.GetExpressions(className);
        }

        private FdoBulkCopy _bcp;

        internal void ApplySettings()
        {
            //Validate
            List<string> errors = new List<string>();
            if (string.IsNullOrEmpty(_view.TaskName))
                errors.Add(ResourceService.GetString("ERR_TASK_NAME_REQUIRED"));

            FdoConnection source = GetSourceConnection();
            FdoConnection target = GetTargetConnection();

            FdoFeatureService srcService = source.CreateFeatureService();
            FdoFeatureService destService = target.CreateFeatureService();

            if (!destService.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert))
                errors.Add(ResourceService.GetStringFormatted("ERR_UNSUPPORTED_CMD", OSGeo.FDO.Commands.CommandType.CommandType_Insert));

            FdoBulkCopyOptions options = null;
            if (_bcp == null)
            {
                options = new FdoBulkCopyOptions(source, target);
            }
            else
            {
                options = _bcp.Options;
                options.Reset(source, target);
            }
            options.SourceSchema = _view.SelectedSourceSchema;
            options.TargetSchema = _view.SelectedTargetSchema;

            using (srcService)
            using (destService)
            {
                if (_view.CopySpatialContexts)
                {
                    List<string> ctxNames = _view.SourceSpatialContexts;
                    if (ctxNames.Count > 1)
                    {
                        if (!target.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts))
                        {
                            errors.Add(ResourceService.GetString("ERR_UNSUPPORTED_COPY_MULTIPLE_SPATIAL_CONTEXTS"));
                        }
                    }
                }

                if (_view.BatchInsertSize > 0)
                    options.BatchSize = _view.BatchInsertSize;

                foreach (string srcClass in _classMappings.Keys)
                {
                    if (_propertyMappings.ContainsKey(srcClass))
                    {
                        ClassDefinition cd = destService.GetClassByName(_view.SelectedTargetSchema, _classMappings[srcClass]);
                        if (cd != null)
                        {
                            bool delete = _view.GetClassDeleteOption(srcClass);
                            string filter = _view.GetClassFilterOption(srcClass);
                            NameValueCollection sourceExpr = GetSourceExpressions(srcClass);
                            NameValueCollection propMaps = _propertyMappings[srcClass];
                            string targetClass = _classMappings[srcClass];

                            FdoClassCopyOptions copt = new FdoClassCopyOptions(srcClass, targetClass);
                            copt.DeleteTarget = delete;
                            copt.SourceFilter = filter;
                            foreach (string srcProp in propMaps.Keys)
                            {
                                copt.AddPropertyMapping(srcProp, propMaps[srcProp]);
                            }
                            foreach (string alias in sourceExpr.Keys)
                            {
                                string expr = sourceExpr[alias];
                                string targetProp = _expressionMappings[srcClass][alias];
                                copt.AddSourceExpression(alias, expr, targetProp);
                            }
                            options.AddClassCopyOption(copt);
                            //options.AddClassCopyOption(srcClass, _classMappings[srcClass], _propertyMappings[srcClass], sourceExpr, delete, filter);
                        }
                        else
                        {
                            errors.Add(ResourceService.GetStringFormatted("ERR_CLASS_NOT_FOUND", _classMappings[srcClass], _view.SelectedTargetSchema));
                        }
                    }
                }

                foreach (string context in _view.SourceSpatialContexts)
                {
                    options.AddSourceSpatialContext(srcService.GetSpatialContext(context));
                }
            }

            if (errors.Count > 0)
                throw new TaskValidationException(errors);

            if (_bcp == null) //New bulk copy
                _bcp = new FdoBulkCopy(options);
            else //Bulk copy was loaded. Update its options
                _bcp.Options = options;
        }

        internal void LoadFrom(FdoBulkCopy copy)
        {
            
        }
    }

    internal class ExpressionMappingInfo
    {
        public string Expression;
        public string TargetProperty;
    }

    public class MappingException : Exception
    {
        public MappingException() : base() { }
        public MappingException(string msg) : base(msg) { }
        public MappingException(string msg, Exception inner) : base(msg, inner) { } 
    }
}
