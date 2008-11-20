using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Common;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public partial class GeometryTypeCtl : CheckedListBox
    {
        public GeometryTypeCtl()
        {
            InitializeComponent();
            GeometryType[] gtypes = (GeometryType[])Enum.GetValues(typeof(GeometryType));
            LoadGeometryTypes(gtypes);
        }

        public GeometryTypeCtl(FdoConnection conn)
        {
            InitializeComponent();
            GeometryType[] gtypes = (GeometryType[])conn.Capability.GetObjectCapability(CapabilityType.FdoCapabilityType_GeometryTypes);
            LoadGeometryTypes(gtypes);
        }

        private void LoadGeometryTypes(GeometryType[] gtypes)
        {
            foreach (GeometryType gt in gtypes)
            {
                this.Items.Add(gt, false);
            }
        }

        public int GeometryTypes
        {
            get
            {
                GeometryType gtype = GeometryType.GeometryType_None;
                foreach (int idx in this.CheckedIndices)
                {
                    gtype |= (GeometryType)Enum.Parse(typeof(GeometryType), this.Items[idx].ToString());
                }
                return (int)gtype;
            }
            set
            {
                if (value != (int)GeometryType.GeometryType_None)
                {
                    if ((value & (int)GeometryType.GeometryType_CurvePolygon) == (int)GeometryType.GeometryType_CurvePolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_CurvePolygon), true);
                    if ((value & (int)GeometryType.GeometryType_CurveString) == (int)GeometryType.GeometryType_CurveString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_CurveString), true);
                    if ((value & (int)GeometryType.GeometryType_LineString) == (int)GeometryType.GeometryType_LineString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_LineString), true);
                    if ((value & (int)GeometryType.GeometryType_MultiCurvePolygon) == (int)GeometryType.GeometryType_MultiCurvePolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiCurvePolygon), true);
                    if ((value & (int)GeometryType.GeometryType_MultiCurveString) == (int)GeometryType.GeometryType_MultiCurveString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiCurveString), true);
                    //if ((def.GeometryTypes & (int)GeometryType.GeometryType_MultiGeometry) == (int)GeometryType.GeometryType_MultiGeometry)
                    //    this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiGeometry), true);
                    if ((value & (int)GeometryType.GeometryType_MultiLineString) == (int)GeometryType.GeometryType_MultiLineString)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiLineString), true);
                    if ((value & (int)GeometryType.GeometryType_MultiPoint) == (int)GeometryType.GeometryType_MultiPoint)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiPoint), true);
                    if ((value & (int)GeometryType.GeometryType_MultiPolygon) == (int)GeometryType.GeometryType_MultiPolygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_MultiPolygon), true);
                    if ((value & (int)GeometryType.GeometryType_Point) == (int)GeometryType.GeometryType_Point)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_Point), true);
                    if ((value & (int)GeometryType.GeometryType_Polygon) == (int)GeometryType.GeometryType_Polygon)
                        this.SetItemChecked(this.Items.IndexOf(GeometryType.GeometryType_Polygon), true);
                }
            }
        }
    }
}
