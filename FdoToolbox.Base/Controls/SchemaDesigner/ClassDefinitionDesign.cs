using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public abstract class ClassDefinitionDesign : INotifyPropertyChanged
    {
        private OSGeo.FDO.Schema.ClassDefinition _classDef;

        public ClassDefinitionDesign(OSGeo.FDO.Schema.ClassDefinition cd)
        {
            _classDef = cd;
        }
        
        [Browsable(false)]
        public OSGeo.FDO.Schema.ClassDefinition ClassDefinition
        {
            get { return _classDef; }
        }

        [Browsable(false)]
        public OSGeo.FDO.Schema.ClassDefinition BaseClass
        {
            get { return _classDef.BaseClass; }
            set { _classDef.BaseClass = value; }
        }

        [Description("The type of class")]
        public ClassType ClassType
        {
            get { return _classDef.ClassType; }
        }

        [Description("The identity properties of this class")]
        [Editor(typeof(IdentityPropertyLookupEditor), typeof(UITypeEditor))]
        public DataPropertyDefinitionCollection IdentityProperties 
        {
            get { return _classDef.IdentityProperties; }
            set 
            {
                _classDef.IdentityProperties.Clear();
                foreach (DataPropertyDefinition dp in value)
                {
                    _classDef.IdentityProperties.Add(dp);
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IdentityProperties"));
            }
        }

        [Description("Indicates whether this class is abstract")]
        public bool IsAbstract
        {
            get { return _classDef.IsAbstract; }
            set { _classDef.IsAbstract = value; }
        }

        [Description("Indicates whether this class is computed")]
        public bool IsComputed
        {
            get { return _classDef.IsComputed; }
            set { _classDef.IsComputed = value; }
        }

        [Description("The name of this class")]
        public string Name
        {
            get { return _classDef.Name; }
            set
            {
                _classDef.Name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        [Description("The unique constraints of this class")]
        public UniqueConstraintCollection UniqueConstraints
        {
            get { return _classDef.UniqueConstraints; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
