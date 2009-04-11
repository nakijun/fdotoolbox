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
using FdoToolbox.Core.Feature;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class ObjectPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private OSGeo.FDO.Schema.ObjectPropertyDefinition _objDef;
        
        public ObjectPropertyDefinitionDesign(OSGeo.FDO.Schema.ObjectPropertyDefinition op)
            : base(op)
        {
            _objDef = op;
        }

        public ObjectPropertyDefinitionDesign(OSGeo.FDO.Schema.ObjectPropertyDefinition op, FdoConnection conn)
            : base(op, conn)
        {
            _objDef = op;
        }

        [Description("The reference to the class definition that defines the type of this property")]
        public OSGeo.FDO.Schema.ClassDefinition Class
        {
            get { return _objDef.Class; }
            set
            {
                _objDef.Class = value;
                this.FirePropertyChanged("Class");
            }
        }

        [Description("The referecnes to a Data Property Definition to use for uniquely identifying instances of the contained class within a single parent object instance. This value is only used for the ObjectType_Collection and ObjectType_OrderedCollection object property types. The Data Property Definition must belong to the Class Definition that defines the type of this property.")]
        public OSGeo.FDO.Schema.DataPropertyDefinition IdentityProperty
        {
            get { return _objDef.IdentityProperty; }
            set
            {
                _objDef.IdentityProperty = value;
                this.FirePropertyChanged("IdentityProperty");
            }
        }

        [Description("The type of this object property (value, collection or ordered collection)")]
        public OSGeo.FDO.Schema.ObjectType ObjectType
        {
            get { return _objDef.ObjectType; }
            set
            {
                _objDef.ObjectType = value;
                this.FirePropertyChanged("ObjectType");
            }
        }

        [Description("The order type of this object property (ascending or descending). This property is only applicable if the property type is set to ObjectType_OrderedCollection")]
        public OSGeo.FDO.Schema.OrderType OrderType
        {
            get { return _objDef.OrderType; }
            set
            {
                _objDef.OrderType = value;
                this.FirePropertyChanged("OrderType");
            }
        }
    }
}
