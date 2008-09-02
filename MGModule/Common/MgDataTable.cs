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
using System.Data;
using OSGeo.MapGuide.MaestroAPI;

namespace MGModule.Common
{
    public class MgDataTable : DataTable
    {
        public void LoadFromReader(OSGeo.MapGuide.MaestroAPI.FeatureSetReader reader)
        {
            this.Columns.Clear();
            this.PrimaryKey = null;

            foreach (FeatureSetColumn col in reader.Columns)
            {
                this.Columns.Add(col.Name);
            }

            while (reader.Read())
            {
                DataRow row = this.NewRow();

                foreach (FeatureSetColumn col in reader.Columns)
                {
                    if (!reader.Row.IsValueNull(col.Name))
                        row[col.Name] = reader.Row[col.Name];
                    else
                        row[col.Name] = DBNull.Value;
                }

                this.Rows.Add(row);
            }
        }
    }
}
