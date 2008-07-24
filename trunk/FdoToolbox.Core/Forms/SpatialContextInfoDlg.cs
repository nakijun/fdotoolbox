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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Forms
{
    /// <summary>
    /// A Data-Entry form for new Spatial Contexts
    /// </summary>
    public partial class SpatialContextInfoDlg : Form
    {
        private ConnectionInfo _BoundConnection;

        internal SpatialContextInfoDlg()
        {
            InitializeComponent();
        }

        public SpatialContextInfoDlg(ConnectionInfo conn)
            : this()
        {
            _BoundConnection = conn;
            cmbExtentType.DataSource = _BoundConnection.Connection.ConnectionCapabilities.SpatialContextTypes;
        }

        public SpatialContextInfoDlg(ConnectionInfo conn, SpatialContextInfo ctx)
            : this(conn)
        {
            txtName.Enabled = false;
            txtName.Text = ctx.Name;
            txtCoordSys.Text = ctx.CoordinateSystem;
            txtCoordSysWkt.Text = ctx.CoordinateSystemWkt;
            txtDescription.Text = ctx.Description;
            if (!string.IsNullOrEmpty(ctx.ExtentGeometryText))
            {
                using (FgfGeometryFactory factory = new FgfGeometryFactory())
                {
                    using (IGeometry geom = factory.CreateGeometry(ctx.ExtentGeometryText))
                    {
                        txtLowerLeftX.Text = geom.Envelope.MinX + "";
                        txtUpperRightX.Text = geom.Envelope.MaxX + "";
                        txtLowerLeftY.Text = geom.Envelope.MinY + "";
                        txtUpperRightY.Text = geom.Envelope.MaxY + "";
                    }
                }
            }
            txtXYTolerance.Text = ctx.XYTolerance + "";
            txtZTolerance.Text = ctx.ZTolerance + "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
                this.DialogResult = DialogResult.OK;
        }

        private bool ValidateForm()
        {
            bool valid = true;
            double dVal = default(double);
            errorProvider.Clear();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                errorProvider.SetError(txtName, "Required");
                valid = false;
            }
            if (!Double.TryParse(txtXYTolerance.Text, out dVal))
            {
                errorProvider.SetError(txtXYTolerance, "Not a double");
                valid = false;
            }
            if (!Double.TryParse(txtZTolerance.Text, out dVal))
            {
                errorProvider.SetError(txtZTolerance, "Not a double");
                valid = false;
            }
            //Static Extent requires the extent fields to be filled and valid
            //double numbers
            if ((SpatialContextExtentType)cmbExtentType.SelectedItem == SpatialContextExtentType.SpatialContextExtentType_Static)
            {
                ValidateExtents(ref valid, ref dVal);
            }
            else //Dynamic extents means extents are optional
            {
                bool allEmpty = (string.IsNullOrEmpty(txtLowerLeftX.Text) &&
                    string.IsNullOrEmpty(txtLowerLeftY.Text) &&
                    string.IsNullOrEmpty(txtUpperRightX.Text) &&
                    string.IsNullOrEmpty(txtUpperRightY.Text));
                //If some or all fields are filled, validate them all
                if (!allEmpty)
                {
                    ValidateExtents(ref valid, ref dVal);
                }
            }
            return valid;
        }

        private void ValidateExtents(ref bool valid, ref double dVal)
        {
            if (string.IsNullOrEmpty(txtLowerLeftX.Text))
            {
                errorProvider.SetError(txtLowerLeftX, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(txtLowerLeftY.Text))
            {
                errorProvider.SetError(txtLowerLeftY, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(txtUpperRightX.Text))
            {
                errorProvider.SetError(txtUpperRightX, "Required");
                valid = false;
            }
            else if (string.IsNullOrEmpty(txtUpperRightY.Text))
            {
                errorProvider.SetError(txtUpperRightY, "Required");
                valid = false;
            }
            else if (!Double.TryParse(txtLowerLeftX.Text, out dVal))
            {
                errorProvider.SetError(txtLowerLeftX, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(txtLowerLeftY.Text, out dVal))
            {
                errorProvider.SetError(txtLowerLeftY, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(txtUpperRightX.Text, out dVal))
            {
                errorProvider.SetError(txtUpperRightX, "Not a double");
                valid = false;
            }
            else if (!Double.TryParse(txtUpperRightY.Text, out dVal))
            {
                errorProvider.SetError(txtUpperRightY, "Not a double");
                valid = false;
            }
            else if (Convert.ToDouble(txtLowerLeftX.Text) > Convert.ToDouble(txtUpperRightX.Text))
            {
                errorProvider.SetError(txtLowerLeftX, "Lower Left X is greater than Upper Right X");
                errorProvider.SetError(txtUpperRightX, "Lower Left X is greater than Upper Right X");
                valid = false;
            }
            else if (Convert.ToDouble(txtLowerLeftY.Text) > Convert.ToDouble(txtUpperRightY.Text))
            {
                errorProvider.SetError(txtLowerLeftY, "Lower Left Y is greater than Upper Right Y");
                errorProvider.SetError(txtUpperRightY, "Lower Left Y is greater than Upper Right Y");
                valid = false;
            }
        }

        public static SpatialContextInfo Edit(ConnectionInfo conn, SpatialContextInfo ctx)
        {
            SpatialContextInfoDlg dlg = new SpatialContextInfoDlg(conn, ctx);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SpatialContextInfo info = new SpatialContextInfo();
                info.Name = dlg.txtName.Text;
                info.Description = dlg.txtDescription.Text;
                info.CoordinateSystem = dlg.txtCoordSys.Text;
                info.CoordinateSystemWkt = dlg.txtCoordSysWkt.Text;
                info.XYTolerance = Convert.ToDouble(dlg.txtXYTolerance.Text);
                info.ZTolerance = Convert.ToDouble(dlg.txtZTolerance.Text);
                info.ExtentType = (SpatialContextExtentType)dlg.cmbExtentType.SelectedItem;

                //Only consider extent if all 4 values are defined
                if (!string.IsNullOrEmpty(dlg.txtLowerLeftX.Text) &&
                   !string.IsNullOrEmpty(dlg.txtLowerLeftY.Text) &&
                   !string.IsNullOrEmpty(dlg.txtUpperRightX.Text) &&
                   !string.IsNullOrEmpty(dlg.txtUpperRightY.Text))
                {
                    string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                    double llx = Convert.ToDouble(dlg.txtLowerLeftX.Text);
                    double lly = Convert.ToDouble(dlg.txtLowerLeftY.Text);
                    double urx = Convert.ToDouble(dlg.txtUpperRightX.Text);
                    double ury = Convert.ToDouble(dlg.txtUpperRightY.Text);
                    info.ExtentGeometryText = string.Format(wktfmt,
                        llx, lly,
                        urx, lly,
                        urx, ury,
                        llx, ury,
                        llx, lly);
                }
                return info;
            }
            return null;
        }

        public static SpatialContextInfo CreateNew(ConnectionInfo conn)
        {
            SpatialContextInfoDlg dlg = new SpatialContextInfoDlg(conn);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SpatialContextInfo info = new SpatialContextInfo();
                info.Name = dlg.txtName.Text;
                info.Description = dlg.txtDescription.Text;
                info.CoordinateSystem = dlg.txtCoordSys.Text;
                info.CoordinateSystemWkt = dlg.txtCoordSysWkt.Text;
                info.XYTolerance = Convert.ToDouble(dlg.txtXYTolerance.Text);
                info.ZTolerance = Convert.ToDouble(dlg.txtZTolerance.Text);
                info.ExtentType = (SpatialContextExtentType)dlg.cmbExtentType.SelectedItem;
                //Only consider extent if all 4 values are defined
                if (!string.IsNullOrEmpty(dlg.txtLowerLeftX.Text) &&
                   !string.IsNullOrEmpty(dlg.txtLowerLeftY.Text) &&
                   !string.IsNullOrEmpty(dlg.txtUpperRightX.Text) &&
                   !string.IsNullOrEmpty(dlg.txtUpperRightY.Text))
                {
                    string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                    double llx = Convert.ToDouble(dlg.txtLowerLeftX.Text);
                    double lly = Convert.ToDouble(dlg.txtLowerLeftY.Text);
                    double urx = Convert.ToDouble(dlg.txtUpperRightX.Text);
                    double ury = Convert.ToDouble(dlg.txtUpperRightY.Text);
                    info.ExtentGeometryText = string.Format(wktfmt,
                        llx, lly,
                        urx, lly,
                        urx, ury,
                        llx, ury,
                        llx, lly);
                }
                return info;
            }
            return null;
        }
    }
}