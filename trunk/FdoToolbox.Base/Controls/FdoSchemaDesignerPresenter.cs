using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;
using FdoToolbox.Base.Controls.SchemaDesigner;
using ICSharpCode.Core;

using Comp = System.ComponentModel;
using System.Drawing;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoSchemaDesignerView
    {
        void AddImage(string name, Bitmap bmp);
        void SetSchemaNode(string name);
        void AddClassNode(string name, string imageKey);
        void AddPropertyNode(string className, string propName, string imageKey);
        string SelectedSchema { get; }
        string SelectedClass { get; }
        string SelectedProperty { get; }

        void RefreshTree();
        string SelectedName { set; }
        object SelectedObject { set; }

        ClassType[] SupportedClassTypes { set; }
        PropertyType[] SupportedPropertyTypes { set; }

        string Title { set; }
        bool ApplyEnabled { set; }
    }

    public class FdoSchemaDesignerPresenter
    {
        private readonly IFdoSchemaDesignerView _view;
        private FdoConnection _conn;

        private FeatureSchema _schema;

        public const string RES_SCHEMA = "chart_organisation";
        public const string RES_CLASS = "database_table";
        public const string RES_GEOM = "shape_handles";
        public const string RES_ID = "key";
        public const string RES_DATA_PROPERTY = "table";
        public const string RES_RASTER = "image";
        public const string RES_OBJECT = "package";
        public const string RES_ASSOC = "table_relationship";

        private int counter = 0;

        public FdoSchemaDesignerPresenter(IFdoSchemaDesignerView view, FdoConnection conn)
        {
            _view = view;
            ImageInit();
            _conn = conn;
            _schema = new FeatureSchema("Schema1", "");
            SetSchemaNode();
            SetCapabilities();
        }

        public FdoSchemaDesignerPresenter(IFdoSchemaDesignerView view, FdoConnection conn, string schemaName)
        {
            _view = view;
            ImageInit();
            _conn = conn;
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                _schema = service.GetSchemaByName(schemaName);
                if (_schema == null)
                    throw new InvalidOperationException("Could not find schema named " + schemaName);
                SetSchemaNode();
                SetCapabilities();
                foreach (ClassDefinition cls in _schema.Classes)
                {
                    _view.AddClassNode(cls.Name, RES_CLASS);
                    foreach (PropertyDefinition pd in cls.Properties)
                    {
                        switch (pd.PropertyType)
                        {
                            case PropertyType.PropertyType_DataProperty:
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_DATA_PROPERTY);
                                break;
                            case PropertyType.PropertyType_AssociationProperty:
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_ASSOC);
                                break;
                            case PropertyType.PropertyType_GeometricProperty:
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_GEOM);
                                break;
                            case PropertyType.PropertyType_ObjectProperty:
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_OBJECT);
                                break;
                            case PropertyType.PropertyType_RasterProperty:
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_RASTER);
                                break;
                        }
                    }
                }
            }
        }

        public FdoSchemaDesignerPresenter(IFdoSchemaDesignerView view)
        {
            _view = view;
            ImageInit();
            _schema = new FeatureSchema("Schema1", "");
            SetSchemaNode();
            SetCapabilities();
        }

        private void SetCapabilities()
        {
            CheckDirtyState();
            _view.SupportedClassTypes = (ClassType[])Enum.GetValues(typeof(ClassType));
            _view.SupportedPropertyTypes = (PropertyType[])Enum.GetValues(typeof(PropertyType));
        }

        private void SetSchemaNode()
        {
            _view.SetSchemaNode(_schema.Name);
            _view.Title = ResourceService.GetString("TITLE_SCHEMA_DESIGNER") + " - " + _schema.Name;
            FeatureSchemaDesign f = new FeatureSchemaDesign(_schema);
            f.PropertyChanged += delegate(object sender, Comp.PropertyChangedEventArgs e)
            {
                _view.SelectedName = f.Name;
                _view.Title = ResourceService.GetString("TITLE_SCHEMA_DESIGNER") + " - " + f.Name;
            };
            _view.SelectedObject = f;
        }

        private void ImageInit()
        {
            _view.AddImage(RES_SCHEMA, ResourceService.GetBitmap(RES_SCHEMA));
            _view.AddImage(RES_CLASS, ResourceService.GetBitmap(RES_CLASS));
            _view.AddImage(RES_GEOM, ResourceService.GetBitmap(RES_GEOM));
            _view.AddImage(RES_ID, ResourceService.GetBitmap(RES_ID));
            _view.AddImage(RES_DATA_PROPERTY, ResourceService.GetBitmap(RES_DATA_PROPERTY));
            _view.AddImage(RES_RASTER, ResourceService.GetBitmap(RES_RASTER));
            _view.AddImage(RES_OBJECT, ResourceService.GetBitmap(RES_OBJECT));
            _view.AddImage(RES_ASSOC, ResourceService.GetBitmap(RES_ASSOC));
        }

        public void SchemaSelected()
        {
            if (_schema != null)
            {
                FeatureSchemaDesign f = new FeatureSchemaDesign(_schema);
                f.PropertyChanged += delegate(object sender, Comp.PropertyChangedEventArgs e)
                {
                    _view.SelectedName = f.Name;
                    _view.Title = ResourceService.GetString("TITLE_SCHEMA_DESIGNER") + " - " + f.Name;
                };
                _view.SelectedObject = f;
            }
        }

        public void ClassSelected()
        {
            ClassDefinition cls = GetClass(_view.SelectedClass);
            if (cls != null)
            {
                ClassDefinitionDesign c = GetClassDesigner(cls);
                c.PropertyChanged += delegate(object sender, Comp.PropertyChangedEventArgs e)
                {
                    _view.SelectedName = c.Name;
                };
                _view.SelectedObject = c;
            }
        }

        private ClassDefinitionDesign GetClassDesigner(ClassDefinition cls)
        {
            switch (cls.ClassType)
            {
                case ClassType.ClassType_Class:
                    return new ClassDesign((Class)cls);
                case ClassType.ClassType_FeatureClass:
                    return new FeatureClassDesign((FeatureClass)cls);
                default:
                    return null;
            }
        }

        public void PropertySelected()
        {
            PropertyDefinition prop = GetProperty(_view.SelectedClass, _view.SelectedProperty);
            if (prop != null)
            {
                PropertyDefinitionDesign p = GetPropertyDesigner(prop);
                p.PropertyChanged += delegate(object sender, Comp.PropertyChangedEventArgs e)
                {
                    _view.SelectedName = p.Name;
                };
                _view.SelectedObject = p;
            }
        }

        private PropertyDefinitionDesign GetPropertyDesigner(PropertyDefinition prop)
        {
            switch (prop.PropertyType)
            {
                case PropertyType.PropertyType_DataProperty:
                    return new DataPropertyDefinitionDesign((DataPropertyDefinition)prop, _conn);
                case PropertyType.PropertyType_GeometricProperty:
                    return new GeometricPropertyDefinitionDesign((GeometricPropertyDefinition)prop, _conn);
                default:
                    return null;
            }
        }

        private ClassDefinition GetClass(string className)
        {
            if (_schema != null && !string.IsNullOrEmpty(className))
            {
                int cidx = _schema.Classes.IndexOf(className);
                if (cidx >= 0)
                {
                    return _schema.Classes[cidx];
                }
            }
            return null;
        }

        private PropertyDefinition GetProperty(string className, string propertyName)
        {
            if (_schema != null && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(propertyName))
            {
                int cidx = _schema.Classes.IndexOf(className);
                if (cidx >= 0)
                {
                    int pidx = _schema.Classes[cidx].Properties.IndexOf(propertyName);
                    if (pidx >= 0)
                    {
                        return _schema.Classes[cidx].Properties[pidx];
                    }
                }
            }
            return null;
        }

        public void AddClass()
        {
            string name = "Class" + counter++;
            Class cls = new Class(name, "");
            _schema.Classes.Add(cls);
            _view.AddClassNode(cls.Name, RES_CLASS);
            CheckDirtyState();
        }

        public void AddFeatureClass()
        {
            string name = "Class" + counter++;
            FeatureClass cls = new FeatureClass(name, "");
            _schema.Classes.Add(cls);
            _view.AddClassNode(cls.Name, RES_CLASS);
            CheckDirtyState();
        }

        public void AddDataProperty()
        {
            ClassDefinition cls = GetClass(_view.SelectedClass);
            if (cls != null)
            {
                string name = "DataProperty" + counter++;
                DataPropertyDefinition dp = new DataPropertyDefinition(name, "");
                cls.Properties.Add(dp);
                _view.AddPropertyNode(cls.Name, dp.Name, RES_DATA_PROPERTY);
                CheckDirtyState();
            }
        }

        public void AddGeometricProperty()
        {
            ClassDefinition cls = GetClass(_view.SelectedClass);
            if (cls != null)
            {
                string name = "GeomProperty" + counter++;
                GeometricPropertyDefinition dp = new GeometricPropertyDefinition(name, "");
                cls.Properties.Add(dp);
                _view.AddPropertyNode(cls.Name, dp.Name, RES_GEOM);
                CheckDirtyState();
            }
        }

        private void CheckDirtyState()
        {
            _view.ApplyEnabled = (_schema.ElementState == SchemaElementState.SchemaElementState_Modified && _conn != null);
        }

        private void ValidateSchema()
        {
            if (_conn != null)
            {
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    IncompatibleSchema incSchema;
                    if (!service.CanApplySchema(_schema, out incSchema))
                        throw new NotSupportedException("Schema cannot be applied: " + incSchema.ToString());
                }
            }
            else 
            { 
                //
            }
        }

        public void SaveSchemaToSdf(string sdfFile)
        {
            ValidateSchema();
            bool result = ExpressUtility.CreateFlatFileDataSource(sdfFile);
            if (result)
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection(sdfFile);
                conn.Open();
                using (FdoFeatureService service = conn.CreateFeatureService())
                {
                    service.ApplySchema(_schema);
                }
            }
        }

        public void SaveSchemaToXml(string xmlFile)
        {
            ValidateSchema();
            _schema.WriteXml(xmlFile);
        }

        public void ApplySchema()
        {
            if (_conn != null)
            {
                ValidateSchema();
                using (FdoFeatureService service = _conn.CreateFeatureService())
                {
                    service.ApplySchema(_schema);
                }
            }
        }
    }
}
