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
using FdoToolbox.Base.Controls.SchemaDesigner;
using ICSharpCode.Core;

using Comp = System.ComponentModel;
using System.Drawing;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoSchemaDesignerView : IViewContent
    {
        void AddImage(string name, Bitmap bmp);
        void SetSchemaNode(string name);
        void AddClassNode(string name, string imageKey);
        void AddPropertyNode(string className, string propName, string imageKey);

        void SetPropertyImage(string className, string propName, string imageKey);

        void RemoveClassNode(string className);
        void RemovePropertyNode(string className, string propName);

        string SelectedSchema { get; }
        string SelectedClass { get; }
        string SelectedProperty { get; }

        void LoadSchema(string file);
        void RefreshTree();
        string SelectedName { set; }
        object SelectedObject { set; }

        ClassType[] SupportedClassTypes { set; }
        PropertyType[] SupportedPropertyTypes { set; }

        bool ApplyEnabled { set; }
        bool FixEnabled { set; }

        event EventHandler SchemaApplied;

        bool LoadEnabled { set; }
    }

    public class FdoSchemaDesignerPresenter
    {
        private readonly IFdoSchemaDesignerView _view;
        private FdoConnection _conn;

        private FeatureSchema _schema;

        public const string RES_SCHEMA = "chart_organisation";
        public const string RES_FEATURE_CLASS = "feature_class";
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
            _view.LoadEnabled = true;
        }

        public FdoSchemaDesignerPresenter(IFdoSchemaDesignerView view, FdoConnection conn, string schemaName)
        {
            _view = view;
            ImageInit();
            _conn = conn;
            using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(_conn))
            {
                _schema = service.GetSchemaByName(schemaName);
                if (_schema == null)
                    throw new InvalidOperationException("Could not find schema named " + schemaName);
                SetSchemaNode();
                SetCapabilities();
                FillTree();
            }
            _view.LoadEnabled = false;
        }

        private void FillTree()
        {
            foreach (ClassDefinition cls in _schema.Classes)
            {
                if (cls.ClassType == ClassType.ClassType_FeatureClass)
                    _view.AddClassNode(cls.Name, RES_FEATURE_CLASS);
                else
                    _view.AddClassNode(cls.Name, RES_CLASS);
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    switch (pd.PropertyType)
                    {
                        case PropertyType.PropertyType_DataProperty:
                            DataPropertyDefinition dp = pd as DataPropertyDefinition;
                            if (cls.IdentityProperties.Contains(dp))
                                _view.AddPropertyNode(cls.Name, pd.Name, RES_ID);
                            else
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
            //Standalone
            if (_conn == null)
            {
                _view.FixEnabled = false;
                _view.SupportedClassTypes = (ClassType[])Enum.GetValues(typeof(ClassType));
                _view.SupportedPropertyTypes = (PropertyType[])Enum.GetValues(typeof(PropertyType));
            }
            else //Contextual
            {
                _view.FixEnabled = true;
                _view.SupportedClassTypes = (ClassType[])_conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_ClassTypes);

                List<PropertyType> ptypes = new List<PropertyType>();
                ptypes.Add(PropertyType.PropertyType_DataProperty);
                ptypes.Add(PropertyType.PropertyType_GeometricProperty);
                if (_conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsAssociationProperties))
                    ptypes.Add(PropertyType.PropertyType_AssociationProperty);
                if (_conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsObjectProperties))
                    ptypes.Add(PropertyType.PropertyType_ObjectProperty);

                _view.SupportedPropertyTypes = ptypes.ToArray();
            }
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
            _view.AddImage(RES_FEATURE_CLASS, ResourceService.GetBitmap(RES_FEATURE_CLASS));
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
                    if (e.PropertyName == "Name")
                        _view.SelectedName = c.Name;
                    else if (e.PropertyName == "IdentityProperties")
                        UpdatePropertyIcons(_view.SelectedClass);
                    CheckDirtyState();
                };
                _view.SelectedObject = c;
            }
        }

        private void UpdatePropertyIcons(string clsName)
        {
            ClassDefinition cls = GetClass(_view.SelectedClass);
            if (cls != null)
            {
                foreach (PropertyDefinition pd in cls.Properties)
                {
                    DataPropertyDefinition dp = pd as DataPropertyDefinition;
                    if (dp != null)
                    {
                        if (cls.IdentityProperties.Contains(dp))
                            _view.SetPropertyImage(clsName, pd.Name, RES_ID);
                        else
                            _view.SetPropertyImage(clsName, pd.Name, RES_DATA_PROPERTY);
                    }
                }
            }
        }

        private ClassDefinitionDesign GetClassDesigner(ClassDefinition cls)
        {
            switch (cls.ClassType)
            {
                case ClassType.ClassType_Class:
                    return new ClassDesign((Class)cls, _conn);
                case ClassType.ClassType_FeatureClass:
                    return new FeatureClassDesign((FeatureClass)cls, _conn);
                default:
                    return null;
            }
        }

        public bool CanAddAssociationProperty(ref string reason)
        {
            //The only requirement is that another class definition exists in the schema
            foreach (ClassDefinition cd in _schema.Classes)
            {
                if (cd.Name != _view.SelectedClass)
                {
                    return true;
                }
            }
            reason = "No other class definitions exist in the current schema";
            return false;
        }

        public bool CanAddObjectProperty(ref string reason)
        {
            //The only requirement is that another class definition exists in the schema
            foreach (ClassDefinition cd in _schema.Classes)
            {
                if (cd.Name != _view.SelectedClass)
                {
                    return true;
                }
            }
            reason = "No other class definitions exist in the current schema";
            return false;
        }

        const int DEFAULT_PROPERTY_LENGTH = 255;

        public void PropertySelected()
        {
            PropertyDefinition prop = GetProperty(_view.SelectedClass, _view.SelectedProperty);
            if (prop != null)
            {
                PropertyDefinitionDesign p = GetPropertyDesigner(prop);
                p.PropertyChanged += delegate(object sender, Comp.PropertyChangedEventArgs e)
                {
                    if(e.PropertyName == "Name")
                        _view.SelectedName = p.Name;
                    else if (e.PropertyName == "DataType")
                    {
                        DataPropertyDefinitionDesign dd = (DataPropertyDefinitionDesign)p;
                        DataType dt = dd.DataType;
                        if ((dt == DataType.DataType_String || dt == DataType.DataType_BLOB || dt == DataType.DataType_CLOB) && dd.Length == 0)
                            dd.Length = DEFAULT_PROPERTY_LENGTH;
                        else
                            dd.Length = 0;
                    }
                    CheckDirtyState();
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
                case PropertyType.PropertyType_AssociationProperty:
                    return new AssociationPropertyDefinitionDesign((AssociationPropertyDefinition)prop, _conn);
                case PropertyType.PropertyType_ObjectProperty:
                    return new ObjectPropertyDefinitionDesign((ObjectPropertyDefinition)prop, _conn);
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
            while (_schema.Classes.IndexOf(name) >= 0)
            {
                name = "Class" + counter++;
            }
            Class cls = new Class(name, "");
            _schema.Classes.Add(cls);
            _view.AddClassNode(cls.Name, RES_CLASS);
            CheckDirtyState();
        }

        public void AddFeatureClass()
        {
            string name = "Class" + counter++;
            while (_schema.Classes.IndexOf(name) >= 0)
            {
                name = "Class" + counter++;
            }
            FeatureClass cls = new FeatureClass(name, "");
            _schema.Classes.Add(cls);
            _view.AddClassNode(cls.Name, RES_FEATURE_CLASS);
            CheckDirtyState();
        }

        public void AddDataProperty()
        {
            ClassDefinition cls = GetClass(_view.SelectedClass);
            if (cls != null)
            {
                string name = "DataProperty" + counter++;
                while (cls.Properties.IndexOf(name) >= 0)
                {
                    name = "DataProperty" + counter++;
                }
                DataPropertyDefinition dp = new DataPropertyDefinition(name, "");
                //Make some sensible string default length
                if (dp.DataType == DataType.DataType_String && dp.Length == 0)
                    dp.Length = DEFAULT_PROPERTY_LENGTH;
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
                while (cls.Properties.IndexOf(name) >= 0)
                {
                    name = "GeomProperty" + counter++;
                }
                GeometricPropertyDefinition dp = new GeometricPropertyDefinition(name, "");
                cls.Properties.Add(dp);
                _view.AddPropertyNode(cls.Name, dp.Name, RES_GEOM);
                CheckDirtyState();
            }
        }

        public void AddAssocationProperty()
        {
            //Only add association property if there is at least one other class definition in the current schema
            //that satisifies the requirements of this association property
            string reason = null;
            if (!this.CanAddAssociationProperty(ref reason))
            {
                _view.ShowError(reason);
            }
            else
            {
                ClassDefinition cls = GetClass(_view.SelectedClass);
                if (cls != null)
                {
                    string name = "AssocProperty" + counter++;
                    while (cls.Properties.IndexOf(name) >= 0)
                    {
                        name = "AssocProperty" + counter++;
                    }
                    AssociationPropertyDefinition ap = new AssociationPropertyDefinition(name, "");
                    ap.ReverseMultiplicity = "0";
                    cls.Properties.Add(ap);
                    _view.AddPropertyNode(cls.Name, ap.Name, RES_ASSOC);
                    CheckDirtyState();
                }
            }
        }

        public void AddObjectProperty()
        {
            //Only add object property if there is at least one other class definition in the current schema
            //that satisfies the requirements of this object property
            string reason = null;
            if (!this.CanAddObjectProperty(ref reason))
            {
                _view.ShowError(reason);
            }
            else
            {
                ClassDefinition cls = GetClass(_view.SelectedClass);
                if (cls != null)
                {
                    string name = "ObjectProperty" + counter++;
                    while (cls.Properties.IndexOf(name) >= 0)
                    {
                        name = "ObjectProperty" + counter++;
                    }
                    ObjectPropertyDefinition op = new ObjectPropertyDefinition(name, "");
                    cls.Properties.Add(op);
                    _view.AddPropertyNode(cls.Name, op.Name, RES_OBJECT);
                    CheckDirtyState();
                }
            }
        }

        private void CheckDirtyState()
        {
            _view.ApplyEnabled = ((_schema.ElementState == SchemaElementState.SchemaElementState_Modified || _schema.ElementState == SchemaElementState.SchemaElementState_Added) && _conn != null);
        }

        private void ValidateSchema()
        {
            if (_conn != null)
            {
                using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(_conn))
                {
                    IncompatibleSchema incSchema;
                    if (!service.CanApplySchema(_schema, out incSchema))
                        throw new NotSupportedException(ResourceService.GetStringFormatted("ERR_SCHEMA_CANNOT_BE_APPLIED", incSchema.ToString()));
                }
            }

            //Check zero-length strings/blobs/clobs
            foreach (ClassDefinition cd in _schema.Classes)
            {
                List<string> zeroLengthProps = new List<string>();
                foreach (PropertyDefinition pd in cd.Properties)
                {
                    //Ignore deleted elements
                    if (pd.ElementState == SchemaElementState.SchemaElementState_Deleted)
                        continue;

                    if (pd.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        DataPropertyDefinition dp = (DataPropertyDefinition)pd;
                        if ((dp.DataType == DataType.DataType_BLOB || dp.DataType == DataType.DataType_CLOB || dp.DataType == DataType.DataType_String) && dp.Length <= 0)
                        {
                            zeroLengthProps.Add(dp.Name);
                        }
                    }
                }
                if (zeroLengthProps.Count > 0)
                {
                    string msg = cd.Name + "\n\n" + string.Join("\n", zeroLengthProps.ToArray());
                    throw new InvalidOperationException(ResourceService.GetStringFormatted("ERR_SCHEMA_ZERO_LENGTH_PROPERTIES", msg));
                }
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
                using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(conn))
                {
                    service.ApplySchema(_schema);
                }
            }
        }

        public void SaveSchemaToXml(string xmlFile)
        {
            ValidateSchema();
            if (_conn != null) 
            {
                //We're contextual, so we need to force a model sync (ie. Apply Schema) first
                //to purge this schema of deleted elements, otherwise they will be written to
                //the xml file
                if (_schema.ElementState != SchemaElementState.SchemaElementState_Unchanged)
                {
                    //Can't continue if user doesn't apply schema first
                    if (!_view.Confirm("Apply Schema", "You have unsaved changes. To save this schema, you must first apply it to the current connetion. Continue?"))
                        return;

                    try
                    {
                        ApplySchema();
                        _schema.WriteXml(xmlFile);
                    }
                    catch (Exception ex)
                    {
                        _view.ShowError(ex);
                    }
                }
                else //No changes, just write the thing
                {
                    _schema.WriteXml(xmlFile);
                }
            }
            else //standalone
            {
                //If modified, force a model update first before writing
                if(_schema.ElementState != SchemaElementState.SchemaElementState_Unchanged)
                    _schema.AcceptChanges();

                _schema.WriteXml(xmlFile);
            }
        }

        public void FixSchema()
        {
            if (_conn != null)
            {
                using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(_conn))
                {
                    IncompatibleSchema incSchema;
                    if (!service.CanApplySchema(_schema, out incSchema))
                    {
                        _schema = service.AlterSchema(_schema, incSchema);
                        SetSchemaNode();
                        FillTree();
                        _view.ShowMessage("Fix Incompatibilities", "Schema Incompatibilities fixed. Please review this schema again before applying.");
                    }
                    else
                    {
                        _view.ShowMessage("Fix Incompatibilities", "No incompatibilities found");
                    }
                }
            }
        }

        public void ApplySchema()
        {
            if (_conn != null)
            {
                ValidateSchema();
                using (FdoFeatureService service = FdoConnectionUtil.CreateFeatureService(_conn))
                {
                    service.ApplySchema(_schema);
                }
                CheckDirtyState();
            }
        }

        public void RemoveProperty()
        {
            string className = _view.SelectedClass;
            string propName = _view.SelectedProperty;
            ClassDefinition cls = GetClass(className);
            if (cls != null)
            {
                PropertyDefinition pd = GetProperty(className, propName);
                if (pd != null)
                {
                    pd.Delete();
                    _view.RemovePropertyNode(className, propName);
                    CheckDirtyState();
                }
            }
        }

        public void RemoveClass()
        {
            string className = _view.SelectedClass;
            ClassDefinition cls = GetClass(className);
            if (cls != null)
            {
                cls.Delete();
                _view.RemoveClassNode(className);
                CheckDirtyState();
            }
        }

        public void ValidateProperty(object obj, string name, object value)
        {
            
        }

        public void Load(string file)
        {
            FeatureSchemaCollection fsc = new FeatureSchemaCollection(null);
            fsc.ReadXml(file);
            _schema = fsc[0];
            SetSchemaNode();
            FillTree();
        }

        internal bool MatchesConnection(FdoConnection conn)
        {
            return _conn == conn;
        }
    }
}
