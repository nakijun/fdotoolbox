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
            set 
            { 
                _classDef.BaseClass = value;
                FirePropertyChanged("BaseClass");
            }
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
                FirePropertyChanged("IdentityProperties");
            }
        }

        [Description("Indicates whether this class is abstract")]
        public bool IsAbstract
        {
            get { return _classDef.IsAbstract; }
            set 
            { 
                _classDef.IsAbstract = value;
                FirePropertyChanged("IsAbstract");
            }
        }

        [Description("Indicates whether this class is computed")]
        public bool IsComputed
        {
            get { return _classDef.IsComputed; }
            set 
            { 
                _classDef.IsComputed = value;
                FirePropertyChanged("IsComputed");
            }
        }

        [Description("The name of this class")]
        public string Name
        {
            get { return _classDef.Name; }
            set
            {
                _classDef.Name = value;
                FirePropertyChanged("Name");
            }
        }

        [Description("The unique constraints of this class")]
        public UniqueConstraintCollection UniqueConstraints
        {
            get { return _classDef.UniqueConstraints; }
        }

        protected void FirePropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
