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
    public class FeatureClassDesign : ClassDefinitionDesign
    {
        private OSGeo.FDO.Schema.FeatureClass _featClass;

        public FeatureClassDesign(OSGeo.FDO.Schema.FeatureClass fc, FdoConnection conn)
            : base(fc, conn)
        {
            _featClass = fc;
        }

        [Description("The geometry property for this feature class")]
        [Editor(typeof(GeometryPropertyLookupEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.GeometricPropertyDefinition GeometryProperty
        {
            get { return _featClass.GeometryProperty; }
            set 
            { 
                _featClass.GeometryProperty = value;
                FirePropertyChanged("GeometryProperty");
            }
        }
    }
}
