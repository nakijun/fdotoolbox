using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Filter;
using System.Collections.Specialized;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Tasks.Controls
{
    public interface IFdoJoinView
    {
        string Name { get; set; }

        List<string> LeftConnections { set; }
        List<string> RightConnections { set; }
        List<string> TargetConnections { set; }

        List<string> LeftSchemas { set; }
        List<string> RightSchemas { set; }
        List<string> TargetSchemas { set; }

        List<string> LeftClasses { set; }
        List<string> RightClasses { set; }

        Array JoinTypes { set; }
        Array SpatialPredicates { set; }

        string SelectedLeftConnection { get; set; }
        string SelectedRightConnection { get; set; }
        string SelectedTargetConnection { get; set; }

        string SelectedLeftSchema { get; set; }
        string SelectedRightSchema { get; set; }
        string SelectedTargetSchema { get; set; }

        string SelectedLeftClass { get; set; }
        string SelectedRightClass { get; set; }

        NameValueCollection TargetGeometryProperties { set; }
        string SelectedTargetGeometryProperty { get; }

        /// <summary>
        /// Sets the properties or gets the checked properties
        /// </summary>
        List<string> LeftProperties { get; set; }

        /// <summary>
        /// Sets the properties or gets the checked properties
        /// </summary>
        List<string> RightProperties { get; set; }

        int BatchSize { get; set; }
        bool BatchEnabled { get; set; }

        bool SpatialPredicateEnabled { get; set; }
        SpatialOperations SelectedSpatialPredicate { get; set; }

        void ClearJoins();
        void AddPropertyJoin(string left, string right);
        void RemoveJoin(string left);

        NameValueCollection GetJoinedProperties();
    }

    internal enum JoinSourceType
    {
        Left,
        Right,
        Target
    }

    public class FdoJoinPresenter
    {
        private readonly IFdoJoinView _view;
        private readonly IFdoConnectionManager _connMgr;

        public FdoJoinPresenter(IFdoJoinView view, IFdoConnectionManager connMgr)
        {
            _view = view;
            _view.JoinTypes = Enum.GetValues(typeof(FdoJoinType));
            _view.SpatialPredicates = Enum.GetValues(typeof(SpatialOperations));
            _connMgr = connMgr;
        }

        public void Init()
        {
            _view.LeftConnections = new List<string>(_connMgr.GetConnectionNames());
            _view.RightConnections = new List<string>(_connMgr.GetConnectionNames());
            _view.TargetConnections = new List<string>(_connMgr.GetConnectionNames());

            ConnectionChanged(JoinSourceType.Left);
            ConnectionChanged(JoinSourceType.Right);
            ConnectionChanged(JoinSourceType.Target);
        }

        internal FdoConnection GetConnection(JoinSourceType type)
        {
            switch (type)
            {
                case JoinSourceType.Left:
                    return _connMgr.GetConnection(_view.SelectedLeftConnection);
                case JoinSourceType.Right:
                    return _connMgr.GetConnection(_view.SelectedRightConnection);
                case JoinSourceType.Target:
                    return _connMgr.GetConnection(_view.SelectedTargetConnection);
                default:
                    return null;
            }
        }

        internal void ConnectionChanged(JoinSourceType type)
        {
            if(type != JoinSourceType.Target)
                _view.ClearJoins();

            FdoConnection conn = GetConnection(type);
            if (conn != null)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            _view.LeftSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                        case JoinSourceType.Right:
                            _view.RightSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                        case JoinSourceType.Target:
                            _view.TargetSchemas = service.GetSchemaNames();
                            SchemaChanged(type);
                            break;
                    }
                }
            }
        }

        internal void SchemaChanged(JoinSourceType type)
        {
            if (type == JoinSourceType.Target)
                return;

            _view.ClearJoins();
            FdoConnection conn = GetConnection(type);
            if (conn != null)
            {
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            _view.LeftClasses = service.GetClassNames(_view.SelectedLeftSchema);
                            ClassChanged(type);
                            break;
                        case JoinSourceType.Right:
                            _view.RightClasses = service.GetClassNames(_view.SelectedRightSchema);
                            ClassChanged(type);
                            break;
                    }
                }
            }
        }

        internal void ClassChanged(JoinSourceType type)
        {
            if(type == JoinSourceType.Target)
                return;
            
            _view.ClearJoins();

            FdoConnection conn = GetConnection(type);
            if(conn != null)
            {
                using(FdoFeatureService service = conn.CreateFeatureService())
                {
                    switch (type)
                    {
                        case JoinSourceType.Left:
                            {
                                string schema = _view.SelectedLeftSchema;
                                string className = _view.SelectedLeftClass;

                                ClassDefinition cd = service.GetClassByName(schema, className);
                                if (cd != null)
                                {
                                    List<string> properties = new List<string>();
                                    foreach(PropertyDefinition pd in cd.Properties)
                                    {
                                        properties.Add(pd.Name);
                                    }
                                    _view.LeftProperties = properties;
                                }
                            }
                            break;
                        case JoinSourceType.Right:
                            {
                                string schema = _view.SelectedRightSchema;
                                string className = _view.SelectedRightClass;

                                ClassDefinition cd = service.GetClassByName(schema, className);
                                if (cd != null)
                                {
                                    List<string> properties = new List<string>();
                                    foreach (PropertyDefinition pd in cd.Properties)
                                    {
                                        properties.Add(pd.Name);
                                    }
                                    _view.RightProperties = properties;
                                }
                            }
                            break;
                    }
                }
            }
        }

        internal void SpatialPredicateStateChanged()
        {
            _view.SpatialPredicateEnabled = _view.SpatialPredicateEnabled;
        }
    }
}
