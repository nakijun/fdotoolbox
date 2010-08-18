#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    public partial class CreateSqlServerCtl : CreateRdbmsCtl
    {
        public CreateSqlServerCtl()
        {
            InitializeComponent();
            this.Title = "Create SQL Server Data Store";
        }

        public override string Provider
        {
            get
            {
                return "OSGeo.SQLServerSpatial";
            }
        }

        public override SpatialContextInfo CreateDefaultSpatialContext()
        {
            var sc = new SpatialContextInfo();
            sc.Name = "Default";
            sc.XYTolerance = sc.ZTolerance = this.Tolerance;
            sc.CoordinateSystem = this.CSName;
            
            sc.IsActive = true;
            sc.ExtentType = this.ExtentType;
            if (sc.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
            {
                string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                double llx = Convert.ToDouble(this.LowerLeftX);
                double lly = Convert.ToDouble(this.LowerLeftY);
                double urx = Convert.ToDouble(this.UpperRightX);
                double ury = Convert.ToDouble(this.UpperRightY);
                sc.ExtentGeometryText = string.Format(wktfmt,
                    llx, lly,
                    urx, lly,
                    urx, ury,
                    llx, ury,
                    llx, lly);
            }
            return sc;
        }
    }
}
