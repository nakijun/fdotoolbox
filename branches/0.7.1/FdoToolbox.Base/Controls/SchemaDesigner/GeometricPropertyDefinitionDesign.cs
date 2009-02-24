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
using System.Drawing.Design;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class GeometricPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private OSGeo.FDO.Schema.GeometricPropertyDefinition _geomDef;

        public GeometricPropertyDefinitionDesign(OSGeo.FDO.Schema.GeometricPropertyDefinition gd)
            : base(gd)
        {
            _geomDef = gd;
        }

        public GeometricPropertyDefinitionDesign(OSGeo.FDO.Schema.GeometricPropertyDefinition gd, FdoConnection conn)
            : base(gd, conn)
        {
            _geomDef = gd;
        }

        [Description("The geometric types that can be stored in this geometric property")]
        [Editor(typeof(GeometryTypeEditor), typeof(UITypeEditor))]
        public int GeometryTypes
        {
            get { return _geomDef.GeometryTypes; }
            set 
            { 
                _geomDef.GeometryTypes = value;
                FirePropertyChanged("GeometryTypes");
            }
        }

        [Description("Indicates if the geometry of this property includes elevation values")]
        public bool HasElevation
        {
            get { return _geomDef.HasElevation; }
            set 
            { 
                _geomDef.HasElevation = value;
                FirePropertyChanged("HasElevation");
            }
        }

        [Description("Indicates if the geometry of this property includes measurement values")]
        public bool HasMeasure
        {
            get { return _geomDef.HasMeasure; }
            set 
            { 
                _geomDef.HasMeasure = value;
                FirePropertyChanged("HasMeasure");
            }
        }

        [Description("Indicates if this property is read-only")]
        public bool ReadOnly
        {
            get { return _geomDef.ReadOnly; }
            set 
            { 
                _geomDef.ReadOnly = value;
                FirePropertyChanged("ReadOnly");
            }
        }

        [Description("The spatial context association to this property")]
        [Editor(typeof(SpatialContextAssociationEditor), typeof(UITypeEditor))]
        public string SpatialContextAssociation
        {
            get { return _geomDef.SpatialContextAssociation; }
            set 
            { 
                _geomDef.SpatialContextAssociation = value;
                FirePropertyChanged("SpatialContextAssociation");
            }
        }
    }
}
