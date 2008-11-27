using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Base;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;
using System.Collections.ObjectModel;
using FdoToolbox.Core.ETL.Specialized;
using System.Collections.Specialized;

namespace FdoToolbox.Tasks.Controls
{
    public interface IFdoBulkCopyView
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

        List<string> SourceSpatialContexts { set; }
        string SpatialFilter { get; }
        int BatchInsertSize { get; set; }

        bool BatchEnabled { set; }
        bool CopySpatialContexts { get; set; }

        void ClearMappings();
        void AddClass(string className);
        void AddClassSourceFilterOption(string className);
        void AddClassDeleteOption(string className);
        void AddClassProperty(string className, string propName, string imgKey);
        void MapClassProperty(string className, string propName, string targetProp);
        void MapClass(string className, string targetClass);

        List<string> MappableClasses { set; }

        void SetMappableProperties(string className, List<string> properties);

        void RemoveAllMappings();

        void AddExpression(string className, string alias, string expr);
        void EditExpression(string className, string alias, string expr);
        void RemoveExpression(string className, string alias);
        void MapExpression(string className, string alias, string targetProp);
    }

    public class FdoBulkCopyPresenter
    {
        private readonly IFdoBulkCopyView _view;
        private IFdoConnectionManager _connMgr;

        private Dictionary<string, string> _classMappings;
        private Dictionary<string, NameValueCollection> _propertyMappings;

        public FdoBulkCopyPresenter(IFdoBulkCopyView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _connMgr = connMgr;
            _classMappings = new Dictionary<string, string>();
            _propertyMappings = new Dictionary<string, NameValueCollection>();
        }

        public void Init()
        {
            ICollection<string> connNames = _connMgr.GetConnectionNames();
            _view.SourceConnections = new List<string>(connNames);
            _view.TargetConnections = new List<string>(connNames);
            this.SourceConnectionChanged();
            this.TargetConnectionChanged();
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
                    _view.TargetSchemas = service.GetSchemaNames();
                    this.TargetSchemaChanged();
                }
            }
        }

        public void SourceSchemaChanged()
        {
            string connName = _view.SelectedTargetConnection;
            string schemaName = _view.SelectedSourceSchema;
            if (schemaName != null && connName != null)
            {
                _view.ClearMappings();
                _classMappings.Clear();
                _propertyMappings.Clear();
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
                            foreach (PropertyDefinition pd in cd.Properties)
                            {
                                _view.AddClassProperty(cd.Name, pd.Name, GetImageKey(pd.PropertyType));
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
            using (FdoFeatureService service = targetConn.CreateFeatureService())
            {
                List<string> classes = service.GetClassNames(_view.SelectedTargetSchema);
                _view.MappableClasses = classes;
            }
        }

        public void MapProperty(string srcClassName, string srcProperty, string destClassName, string destProperty)
        {
            //TODO: Throw if not mappable
            _view.MapClassProperty(srcClassName, srcProperty, destProperty);
            _propertyMappings[srcClassName][srcProperty] = destProperty;
        }

        public void MapExpression(string srcClassName, string srcAlias, string destClassName, string destProperty)
        {
            //TODO: Throw if not mappable
            _view.MapExpression(srcClassName, srcAlias, destProperty);
            _propertyMappings[srcClassName][srcAlias] = destProperty;
        }

        public void UnmapClass(string srcClass)
        {
            _classMappings.Remove(srcClass);
            _propertyMappings.Remove(srcClass);
            _view.MapClass(srcClass, null);
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
            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                ClassDefinition cd = service.GetClassByName(_view.SelectedTargetSchema, targetClass);
                if (cd != null)
                {
                    List<string> propNames = new List<string>();
                    _propertyMappings[srcClass] = new NameValueCollection();
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
            FdoBulkCopyOptions options = new FdoBulkCopyOptions(GetSourceConnection(), GetTargetConnection());
            if ( _view.BatchInsertSize > 0)
                options.BatchSize = _view.BatchInsertSize;

            foreach (string srcClass in _classMappings.Keys)
            {
                if (_propertyMappings.ContainsKey(srcClass))
                {
                    options.AddClassCopyOption(srcClass, _classMappings[srcClass], _propertyMappings[srcClass]);
                }
            }
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
