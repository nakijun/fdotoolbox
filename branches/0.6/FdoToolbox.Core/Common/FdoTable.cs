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
using System.Data;

namespace FdoToolbox.Core.Common
{
    /// <summary>
    /// The DataTable equivalent of a FDO non-feature class.
    /// </summary>
    public class FdoTable : FdoDataTable
    {
        public FdoTable(string name, string description) : base(name, description) { }

        public FdoTable(Class cls)
        {
            InitFromClass(cls);
        }

        public FdoTable(DataTable table)
        {
            InitFromTable(table);
        }

        protected override ClassDefinition CreateClassDefinition()
        {
            return new Class(this.TableName, this.Description);
        }

        protected override ClassType GetClassType()
        {
            return ClassType.ClassType_Class;
        }
    }
}
