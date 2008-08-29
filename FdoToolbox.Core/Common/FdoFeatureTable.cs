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
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Common
{
    public class FdoFeatureTable : FdoDataTable
    {
        public FdoFeatureTable(string name, string description) : base(name, description) { }

        public FdoFeatureTable(FeatureClass fc) : base() 
        {
            InitFromClass(fc);
        }

        public override void InitFromClass(ClassDefinition classDef)
        {
            if (classDef.ClassType != ClassType.ClassType_FeatureClass)
                throw new ArgumentException("The class is not a feature class");

            base.InitFromClass(classDef);
            FeatureClass fc = classDef as FeatureClass;
            if(fc.GeometryProperty != null)
                this.GeometryColumn = (FdoGeometryColumn)this.Columns[fc.GeometryProperty.Name];
        }

        private FdoGeometryColumn _GeomColumn;

        public FdoGeometryColumn GeometryColumn
        {
            get { return _GeomColumn; }
            set { _GeomColumn = value; }
        }

        public override ClassDefinition GetClassDefinition()
        {
            FeatureClass classDef = (FeatureClass)base.GetClassDefinition();
            int gidx = classDef.Properties.IndexOf(_GeomColumn.ColumnName);
            classDef.GeometryProperty = (GeometricPropertyDefinition)classDef.Properties[gidx];
            return classDef;
        }

        protected override ClassDefinition CreateClassDefinition()
        {
            return new FeatureClass(this.TableName, this.Description);
        }
    }
}
