#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using OSGeo.FDO.Common;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Schema;
using System.Diagnostics;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    public abstract class SchemaElementDecorator<T> : INotifyPropertyChanged where T : SchemaElement
    {
        private T _el;

        public SchemaElementDecorator(T el)
        {
            _el = el;
        }

        public T DecoratedObject
        {
            get { return _el; }
        }

        public string Name
        {
            get { return _el.Name; }
            set 
            {
                if (_el.Name != value)
                {
                    _el.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get { return _el.Description; }
            set 
            {
                if (_el.Description != value)
                {
                    _el.Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
#if DEBUG
            Debug.WriteLine(string.Format("PropertyChanged: {0}.{1}", this.DecoratedObject.GetType().Name, propertyName));
#endif

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FeatureSchemaDecorator : SchemaElementDecorator<FeatureSchema>
    {
        public FeatureSchemaDecorator(FeatureSchema schema)
            : base(schema)
        {
            
        }

        public void AddClass(ClassDefinition cls)
        {
            var schema = this.DecoratedObject;
            var classes = schema.Classes;
            classes.Add(cls);
            OnPropertyChanged("Classes");
        }

        public ClassCollection Classes
        {
            get { return this.DecoratedObject.Classes; }
        }

        public void AcceptChanges()
        {
            this.DecoratedObject.AcceptChanges();
        }

        public void RejectChanges()
        {
            this.DecoratedObject.RejectChanges();
        }
    }

    public abstract class ClassDefinitionDecorator : SchemaElementDecorator<ClassDefinition>
    {
        public ClassDefinitionDecorator(ClassDefinition cls)
            : base(cls)
        {

        }

        public string ParentName
        {
            get
            {
                if (this.DecoratedObject.Parent != null)
                {
                    return this.DecoratedObject.Parent.Name;
                }
                return string.Empty;
            }
        }

        public ClassDefinition BaseClass
        {
            get { return this.DecoratedObject.BaseClass; }
            set
            {
                var cls = this.DecoratedObject;
                if (cls.BaseClass != value)
                {
                    cls.BaseClass = value;
                    OnPropertyChanged("BaseClass");
                }
            }
        }

        public ReadOnlyDataPropertyDefinitionCollection BaseIdentityProperties
        {
            get { return this.DecoratedObject.BaseIdentityProperties; }
        }

        public ClassType ClassType
        {
            get { return this.DecoratedObject.ClassType; }
        }

        public DataPropertyDefinitionCollection IdentityProperties
        {
            get { return this.DecoratedObject.IdentityProperties; }
        }

        public bool IsAbstract
        {
            get { return this.DecoratedObject.IsAbstract; }
            set
            {
                if (this.DecoratedObject.IsAbstract != value)
                {
                    this.DecoratedObject.IsAbstract = value;
                    OnPropertyChanged("IsAbstract");
                }
            }
        }

        public bool IsComputed
        {
            get { return this.DecoratedObject.IsComputed; }
            set
            {
                if (this.DecoratedObject.IsComputed != value)
                {
                    this.DecoratedObject.IsComputed = value;
                    OnPropertyChanged("IsComputed");
                }
            }
        }

        public PropertyDefinitionCollection Properties
        {
            get { return this.DecoratedObject.Properties; }
        }

        public string QualifiedName
        {
            get { return this.DecoratedObject.QualifiedName; }
        }

        public UniqueConstraintCollection UniqueConstraints
        {
            get { return this.DecoratedObject.UniqueConstraints; }
        }

        public ReadOnlyPropertyDefinitionCollection GetBaseProperties()
        {
            return this.DecoratedObject.GetBaseProperties();
        }

        public void SetBaseProperties(PropertyDefinitionCollection value)
        {
            this.DecoratedObject.SetBaseProperties(value);
        }

        internal void RemoveIdentityProperty(string name)
        {
            var cls = this.DecoratedObject;
            if (cls.IdentityProperties.Contains(name))
            {
                var dp = cls.IdentityProperties[name];
                cls.IdentityProperties.Remove(dp);
                OnPropertyChanged("IdentityProperties");
            }
        }

        internal void MarkAsIdentity(string name)
        {
            var cls = this.DecoratedObject;
            if (cls.IdentityProperties.Contains(name))
                return;

            if (cls.Properties.Contains(name))
            {
                var p = cls.Properties[name];
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    var dp = (DataPropertyDefinition)p;
                    dp.Nullable = false;
                    cls.IdentityProperties.Add(dp);
                    OnPropertyChanged("IdentityProperties");
                }
            }
        }
    }

    public class FeatureClassDecorator : ClassDefinitionDecorator
    {
        private FeatureClass _fc;

        private BindingList<string> _availGeomProperties;

        public BindingList<string> AvailableGeometricProperties
        {
            get { return _availGeomProperties; }
        }

        public FeatureClassDecorator(FeatureClass cls) : base(cls) 
        {
            _fc = cls;
            _availGeomProperties = new BindingList<string>();
            foreach (PropertyDefinition p in cls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    _availGeomProperties.Add(p.Name);
                }
            }
        }

        public GeometricPropertyDefinition GeometryProperty
        {
            get { return _fc.GeometryProperty; }
            set 
            {
                if (_fc.GeometryProperty != value)
                {
                    _fc.GeometryProperty = value;
                    OnPropertyChanged("GeometryProperty");
                }
            }
        }

        internal void AssignGeometricProperty(string name)
        {
            var cls = _fc;
            if (cls.Properties.Contains(name))
            {
                var p = cls.Properties[name];
                if (p.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    var g = (GeometricPropertyDefinition)p;
                    cls.GeometryProperty = g;
                }
            }
        }
    }

    public class ClassDecorator : ClassDefinitionDecorator
    {
        public ClassDecorator(Class cls) : base(cls) { }
    }

    public abstract class PropertyDefinitionDecorator : SchemaElementDecorator<PropertyDefinition>
    {
        public PropertyDefinitionDecorator(PropertyDefinition prop) : base(prop) { }

        public PropertyType PropertyType
        {
            get { return this.DecoratedObject.PropertyType; }
        }

        public bool IsSystem
        {
            get { return this.DecoratedObject.IsSystem; }
            set
            {
                if (this.DecoratedObject.IsSystem != value)
                {
                    this.DecoratedObject.IsSystem = value;
                    OnPropertyChanged("IsSystem");
                }
            }
        }

        public string QualifiedName
        {
            get { return this.DecoratedObject.QualifiedName; }
        }
    }

    public class GeometricPropertyDefinitionDecorator : PropertyDefinitionDecorator
    {
        private GeometricPropertyDefinition _g;

        public GeometricPropertyDefinitionDecorator(GeometricPropertyDefinition p) : base(p) { _g = p; }

        public int GeometryTypes
        {
            get { return _g.GeometryTypes; }
            set
            {
                if (_g.GeometryTypes != value)
                {
                    _g.GeometryTypes = value;
                    OnPropertyChanged("GeometryTypes");
                }
            }
        }

        public bool HasElevation
        {
            get { return _g.HasElevation; }
            set
            {
                if (_g.HasElevation != value)
                {
                    _g.HasElevation = value;
                    OnPropertyChanged("HasElevation");
                }
            }
        }

        public bool HasMeasure
        {
            get { return _g.HasMeasure; }
            set
            {
                if (_g.HasMeasure != value)
                {
                    _g.HasMeasure = value;
                    OnPropertyChanged("HasMeasure");
                }
            }
        }

        public bool ReadOnly
        {
            get { return _g.ReadOnly; }
            set
            {
                if (_g.ReadOnly != value)
                {
                    _g.ReadOnly = value;
                    OnPropertyChanged("ReadOnly");
                }
            }
        }

        public string SpatialContextAssociation
        {
            get { return _g.SpatialContextAssociation; }
            set
            {
                if (_g.SpatialContextAssociation != value)
                {
                    _g.SpatialContextAssociation = value;
                    OnPropertyChanged("SpatialContextAssociation");
                }
            }
        }

        public GeometryType[] SpecificGeometryTypes
        {
            get { return _g.SpecificGeometryTypes; }
            set
            {
                if (_g.SpecificGeometryTypes != value)
                {
                    _g.SpecificGeometryTypes = value;
                    OnPropertyChanged("SpecificGeometryTypes");
                }
            }
        }
    }

    public class DataPropertyDefinitionDecorator : PropertyDefinitionDecorator
    {
        private DataPropertyDefinition _d;

        public DataPropertyDefinitionDecorator(DataPropertyDefinition p) : base(p) { _d = p; }

        public DataType DataType
        {
            get { return _d.DataType; }
            set
            {
                if (_d.DataType != value)
                {
                    _d.DataType = value;
                    OnPropertyChanged("DataType");
                }
            }
        }

        public string DefaultValue
        {
            get { return _d.DefaultValue; }
            set
            {
                if (_d.DefaultValue != value)
                {
                    _d.DefaultValue = value;
                    OnPropertyChanged("DefaultValue");
                }
            }
        }

        public bool IsAutoGenerated
        {
            get { return _d.IsAutoGenerated; }
            set
            {
                if (_d.IsAutoGenerated != value)
                {
                    _d.IsAutoGenerated = value;
                    OnPropertyChanged("IsAutoGenerated");
                }
            }
        }

        public int Length
        {
            get { return _d.Length; }
            set
            {
                if (_d.Length != value)
                {
                    _d.Length = value;
                    OnPropertyChanged("Length");
                }
            }
        }

        public bool Nullable
        {
            get { return _d.Nullable; }
            set
            {
                if (_d.Nullable != value)
                {
                    _d.Nullable = value;
                    OnPropertyChanged("Nullable");
                }
            }
        }

        public int Precision
        {
            get { return _d.Precision; }
            set
            {
                if (_d.Precision != value)
                {
                    _d.Precision = value;
                    OnPropertyChanged("Precision");
                }
            }
        }

        public bool ReadOnly
        {
            get { return _d.ReadOnly; }
            set
            {
                if (_d.ReadOnly != value)
                {
                    _d.ReadOnly = value;
                    OnPropertyChanged("ReadOnly");
                }
            }
        }

        public int Scale
        {
            get { return _d.Scale; }
            set
            {
                if (_d.Scale != value)
                {
                    _d.Scale = value;
                    OnPropertyChanged("Scale");
                }
            }   
        }

        public PropertyValueConstraint ValueConstraint
        {
            get { return _d.ValueConstraint; }
            set
            {
                if (_d.ValueConstraint != value)
                {
                    _d.ValueConstraint = value;
                    OnPropertyChanged("ValueConstraint");
                }
            }   
        }

        internal bool IsIdentity()
        {
            var cls = (ClassDefinition)this.DecoratedObject.Parent;
            return cls.IdentityProperties.Contains(this.Name);
        }
    }

    public class AssociationPropertyDefinitionDecorator : PropertyDefinitionDecorator
    {
        private AssociationPropertyDefinition _a;

        public AssociationPropertyDefinitionDecorator(AssociationPropertyDefinition p) : base(p) { _a = p; }

        public ClassDefinition AssociatedClass
        {
            get { return _a.AssociatedClass; }
            set
            {
                _a.AssociatedClass = value;
                OnPropertyChanged("AssociatedClass");
            }
        }

        public DeleteRule DeleteRule
        {
            get { return _a.DeleteRule; }
            set
            {
                _a.DeleteRule = value;
                OnPropertyChanged("DeleteRule");
            }
        }

        public DataPropertyDefinitionCollection IdentityProperties
        {
            get { return _a.IdentityProperties; }
        }

        public bool IsReadOnly
        {
            get { return _a.IsReadOnly; }
            set
            {
                _a.IsReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }

        public bool LockCascade
        {
            get { return _a.LockCascade; }
            set
            {
                _a.LockCascade = value;
                OnPropertyChanged("LockCascade");
            }
        }

        public string Multiplicity
        {
            get { return _a.Multiplicity; }
            set
            {
                _a.Multiplicity = value;
                OnPropertyChanged("Multiplicity");
            }
        }

        public DataPropertyDefinitionCollection ReverseIdentityProperties
        {
            get { return _a.ReverseIdentityProperties; }
        }

        public string ReverseMultiplicity
        {
            get { return _a.ReverseMultiplicity; }
            set
            {
                _a.ReverseMultiplicity = value;
                OnPropertyChanged("ReverseMultiplicity");
            }
        }

        public string ReverseName
        {
            get { return _a.ReverseName; }
            set
            {
                _a.ReverseName = value;
                OnPropertyChanged("ReverseName");
            }
        }

        internal KeyMapping[] GetMappings()
        {
            var aids = _a.IdentityProperties;
            var rids = _a.ReverseIdentityProperties;

            Debug.Assert(aids.Count == rids.Count);

            List<KeyMapping> mappings = new List<KeyMapping>();

            for (int i = 0; i < aids.Count; i++)
            {
                var ap = aids[i];
                var rp = rids[i];

                mappings.Add(new KeyMapping(ap.Name, rp.Name));
            }

            return mappings.ToArray();
        }

        /// <summary>
        /// Helper method to set the identity and reverse identity properties
        /// </summary>
        /// <param name="map"></param>
        internal void AddKeyMapping(KeyMapping map)
        {
            var aids = _a.IdentityProperties;
            var rids = _a.ReverseIdentityProperties;

            Debug.Assert(aids.Count == rids.Count);

            var acls = (ClassDefinition)_a.Parent;
            var rcls = _a.AssociatedClass;

            var ap = acls.Properties[map.Primary];
            var rp = rcls.Properties[map.Foreign];

            Debug.Assert(ap.PropertyType == rp.PropertyType);
            Debug.Assert(ap.PropertyType == PropertyType.PropertyType_DataProperty);

            aids.Add((DataPropertyDefinition)ap);
            rids.Add((DataPropertyDefinition)rp);
        }

        internal void RemoveKeyMapping(int index)
        {
            var aids = _a.IdentityProperties;
            var rids = _a.ReverseIdentityProperties;

            Debug.Assert(aids.Count == rids.Count);

            aids.RemoveAt(index);
            rids.RemoveAt(index);

            OnPropertyChanged("IdentityProperties");
            OnPropertyChanged("ReverseIdentityProperties");
        }

        internal void ResetMappings()
        {
            _a.IdentityProperties.Clear();
            _a.ReverseIdentityProperties.Clear();

            OnPropertyChanged("IdentityProperties");
            OnPropertyChanged("ReverseIdentityProperties");
        }
    }

    public class ObjectPropertyDefinitionDecorator : PropertyDefinitionDecorator
    {
        private ObjectPropertyDefinition _o;

        public ObjectPropertyDefinitionDecorator(ObjectPropertyDefinition p) : base(p) { _o = p; }

        public ClassDefinition Class
        {
            get { return _o.Class; }
            set
            {
                _o.Class = value;
                OnPropertyChanged("Class");
            }
        }

        public DataPropertyDefinition IdentityProperty
        {
            get { return _o.IdentityProperty; }
            set
            {
                _o.IdentityProperty = value;
                OnPropertyChanged("IdentityProperty");
            }
        }

        public ObjectType ObjectType
        {
            get { return _o.ObjectType; }
            set
            {
                _o.ObjectType = value;
                OnPropertyChanged("ObjectType");
            }
        }

        public OrderType OrderType
        {
            get { return _o.OrderType; }
            set
            {
                _o.OrderType = value;
                OnPropertyChanged("OrderType");
            }
        }
    }
}
