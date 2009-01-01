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

        protected void FirePropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
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
                FirePropertyChanged("Name");
            }
        }

        [Description("The description of the property")]
        public string Description
        {
            get { return _propDef.Description; }
            set 
            { 
                _propDef.Description = value;
                FirePropertyChanged("Description");
            }
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
