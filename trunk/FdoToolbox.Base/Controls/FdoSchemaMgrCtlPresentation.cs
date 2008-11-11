using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoSchemaMgrView
    {
        string Title { set; }
        bool ApplyEnabled { set; }
        bool Standalone { get; set; }

        void AddSchema(FeatureSchema schema);
        void RemoveSchema(FeatureSchema schema);
        FeatureSchemaCollection SchemaList { get; set; }
        object ClassList { set; }

        FeatureSchema SelectedSchema { get; }
        ClassDefinition SelectedClass { get; }

        int SchemaCount { get; }
        int ClassCount { get; }

        bool AddSchemaEnabled { set; }
        bool DeleteSchemaEnabled { set; }

        bool EditClassEnabled { set; }
        bool DeleteClassEnabled { set; }

        bool AddClassEnabled { set; }
        bool AddFeatureClassEnabled { set; }

        void AddClass(ClassDefinition classDef);
    }

    public class FdoSchemaMgrPresenter
    {
        private readonly IFdoSchemaMgrView _view;
        private FdoConnection _conn;
        private FdoFeatureService _service;

        private bool _supportsSchemaModification;
        private bool _supportsMultipleSchemas;
        private bool _canDestroySchema;

        public FdoSchemaMgrPresenter(IFdoSchemaMgrView view, FdoConnection conn) : this(view, conn, conn.CreateFeatureService()) { }

        public FdoSchemaMgrPresenter(IFdoSchemaMgrView view, FdoConnection conn, FdoFeatureService service)
        {
            _view = view;
            _conn = conn;
            _service = service;
            _view.Title = ResourceService.GetString("TITLE_SCHEMA_MANAGEMENT");
            ToggleUI();
        }

        private void ToggleUI()
        {
            _supportsSchemaModification = _conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSchemaModification).Value;
            _supportsMultipleSchemas = _conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSchemas).Value;
            _canDestroySchema = _service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySchema);

            ClassType[] ctypes = (ClassType[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ClassTypes);
            _view.AddClassEnabled = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_Class) >= 0;
            _view.AddFeatureClassEnabled = Array.IndexOf<ClassType>(ctypes, ClassType.ClassType_FeatureClass) >= 0;

            _view.DeleteSchemaEnabled = _canDestroySchema && _supportsSchemaModification;
            _view.DeleteClassEnabled = _supportsSchemaModification;
            _view.EditClassEnabled = _supportsSchemaModification;
        }

        public void GetSchemas()
        {
            _view.SchemaList = _service.DescribeSchema();
        }

        public void GetClasses()
        {
            _view.ClassList = _view.SelectedSchema.Classes;
        }

        public void AddSchema(FeatureSchema schema)
        {
            _view.AddSchema(schema);
            CheckDirty();
        }

        public void EditSchema()
        {

        }

        public void DeleteSchema()
        {
            if (_view.SelectedSchema != null)
            {
                _view.RemoveSchema(_view.SelectedSchema);
                CheckDirty();
            }
        }

        public void AddClass(ClassDefinition classDef)
        {
            _view.AddClass(classDef);
        }

        public void DeleteClass()
        {
            FeatureSchema schema = _view.SelectedSchema;
            ClassDefinition cls = _view.SelectedClass;
            if (schema != null && cls != null)
            {
                schema.Classes.Remove(cls);
            }
        }

        public bool CheckDirty()
        {
            bool dirty = false;
            foreach (FeatureSchema schema in _view.SchemaList)
            {
                if (schema.ElementState != SchemaElementState.SchemaElementState_Unchanged)
                    dirty = true;
            }
            _view.ApplyEnabled = dirty;
            return dirty;
        }

        public bool ApplyChanges()
        {
            _service.ApplySchemas(_view.SchemaList);
            return true;
        }

        public void SchemaSelected()
        {
            FeatureSchema schema = _view.SelectedSchema;
            if (schema != null)
            {
                GetClasses();
                _view.AddSchemaEnabled = _supportsMultipleSchemas;
                _view.DeleteSchemaEnabled = _canDestroySchema && _supportsSchemaModification;
            }
            else
            {
                _view.DeleteSchemaEnabled = false;
            }
        }

        public void ClassSelected()
        {
            ClassDefinition classDef = _view.SelectedClass;
            if (classDef != null)
            {
                _view.EditClassEnabled = classDef.Capabilities.SupportsWrite;
                _view.DeleteClassEnabled = _canDestroySchema && _supportsSchemaModification;
            }
            else
            {
                _view.EditClassEnabled = false;
                _view.DeleteClassEnabled = false;
            }
        }
    }
}
