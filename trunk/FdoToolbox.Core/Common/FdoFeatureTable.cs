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
    public class FdoFeatureTable : FdoDataTable<FeatureClass>
    {
        public FdoFeatureTable(string name, string description) : base(name, description) { }

        public FdoFeatureTable(FeatureClass fc) : base() 
        {
            InitFromClass(fc);
        }

        public override void InitFromClass(FeatureClass classDef)
        {
            base.InitFromClass(classDef);
            this.GeometryColumn = (FdoGeometryColumn)this.Columns[classDef.GeometryProperty.Name];
        }

        private FdoGeometryColumn _GeomColumn;

        public FdoGeometryColumn GeometryColumn
        {
            get { return _GeomColumn; }
            set { _GeomColumn = value; }
        }

        public override FeatureClass GetClassDefinition()
        {
            FeatureClass classDef = base.GetClassDefinition();
            int gidx = classDef.Properties.IndexOf(_GeomColumn.ColumnName);
            classDef.GeometryProperty = (GeometricPropertyDefinition)classDef.Properties[gidx];
            return classDef;
        }

        protected override FeatureClass CreateClassDefinition()
        {
            return new FeatureClass(this.TableName, this.Description);
        }
    }
}
