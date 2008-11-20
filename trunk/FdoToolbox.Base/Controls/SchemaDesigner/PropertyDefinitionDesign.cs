using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public abstract class PropertyDefinitionDesign : INotifyPropertyChanged
    {
        private OSGeo.FDO.Schema.PropertyDefinition _propDef;
        private FdoConnection _conn;

        public PropertyDefinitionDesign(OSGeo.FDO.Schema.PropertyDefinition pd)
        {
            _propDef = pd;
        }

        public PropertyDefinitionDesign(OSGeo.FDO.Schema.PropertyDefinition pd, FdoConnection conn)
            : this(pd)
        {
            _conn = conn;
        }

        [Browsable(false)]
        public FdoConnection Connection
        {
            get { return _conn; }
        }

        [Description("The name of the property")]
        public string Name
        {
            get { return _propDef.Name; }
            set 
            { 
                _propDef.Name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        [Description("The description of the property")]
        public string Description
        {
            get { return _propDef.Description; }
            set { _propDef.Description = value; }
        }

        [Description("Gets whether this is a system property")]
        [Browsable(false)]
        public bool IsSystem
        {
            get { return _propDef.IsSystem; }
        }

        [Description("The fully qualified name of this property")]
        [Browsable(false)]
        public string QualifiedName
        {
            get { return _propDef.QualifiedName; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
