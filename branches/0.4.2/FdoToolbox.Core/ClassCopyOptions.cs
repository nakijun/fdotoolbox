#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Source Class Copy options
    /// </summary>
    public class ClassCopyOptions
    {
        private NameValueCollection _PropertyMappings;
        private Dictionary<string, PropertyDefinition> _PropertyDefinitions;

        private ClassDefinition _ClassDef;

        /// <summary>
        /// The source class name
        /// </summary>
        public string ClassName
        {
            get { return _ClassDef.Name; }
        }

        private string _AttributeFilter;

        /// <summary>
        /// The attribute filter to apply when reading from the
        /// source class
        /// </summary>
        public string AttributeFilter
        {
            get { return _AttributeFilter; }
            set { _AttributeFilter = value; }
        }
	

        private string _TargetClassName;

        /// <summary>
        /// The target class name
        /// </summary>
        public string TargetClassName
        {
            get { return _TargetClassName; }
            set { _TargetClassName = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">The name of the source class</param>
        public ClassCopyOptions(ClassDefinition classDef)
        {
            _ClassDef = classDef;
            _PropertyMappings = new NameValueCollection();
            _PropertyDefinitions = new Dictionary<string, PropertyDefinition>();
        }

        /// <summary>
        /// Alternative constructor. Used for express BCP tasks
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="includeAllProperties"></param>
        public ClassCopyOptions(ClassDefinition classDef, bool includeAllProperties)
            : this(classDef)
        {
            if (includeAllProperties)
            {
                foreach (PropertyDefinition def in classDef.Properties)
                {
                    //Omit any non-writable properties
                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null && dataDef.ReadOnly)
                        continue;
                    if (geomDef != null && geomDef.ReadOnly)
                        continue;
                    this.AddProperty(def, def.Name);
                }
                this.TargetClassName = _ClassDef.Name;
            }
        }

        /// <summary>
        /// The source class definition
        /// </summary>
        public ClassDefinition SourceClassDefinition
        {
            get { return _ClassDef; }
        }

        private bool _DeleteClassData;

        /// <summary>
        /// If true, delete all data on the target before copying
        /// </summary>
        public bool DeleteClassData
        {
            get { return _DeleteClassData; }
            set { _DeleteClassData = value; }
        }


        /// <summary>
        /// The properties of the source class to copy
        /// </summary>
        /// <returns></returns>
        public string[] PropertyNames
        {
            get
            {
                return _PropertyMappings.AllKeys;
            }
        }

        /// <summary>
        /// Returns the mapped property name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetTargetPropertyName(string name)
        {
            return _PropertyMappings[name];
        }

        /// <summary>
        /// Returns the associated property definition
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PropertyDefinition GetPropertyDefinition(string name)
        {
            return _PropertyDefinitions[name];
        }

        /// <summary>
        /// Adds a property to be copied from the source class
        /// </summary>
        /// <param name="name">The name of the property</param>
        public void AddProperty(PropertyDefinition srcProp, string targetProp)
        {
            _PropertyMappings.Add(srcProp.Name, targetProp);
            _PropertyDefinitions.Add(srcProp.Name, srcProp);
        }
    }
}
