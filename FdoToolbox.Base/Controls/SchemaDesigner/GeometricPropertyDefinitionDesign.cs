using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class GeometricPropertyDefinitionDesign : PropertyDefinitionDesign
    {
        private OSGeo.FDO.Schema.GeometricPropertyDefinition _geomDef;

        public GeometricPropertyDefinitionDesign(OSGeo.FDO.Schema.GeometricPropertyDefinition gd)
            : base(gd)
        {
            _geomDef = gd;
        }

        public GeometricPropertyDefinitionDesign(OSGeo.FDO.Schema.GeometricPropertyDefinition gd, FdoConnection conn)
            : base(gd, conn)
        {
            _geomDef = gd;
        }

        [Description("The geometric types that can be stored in this geometric property")]
        [Editor(typeof(GeometryTypeEditor), typeof(UITypeEditor))]
        public int GeometryTypes
        {
            get { return _geomDef.GeometryTypes; }
            set { _geomDef.GeometryTypes = value; }
        }

        [Description("Indicates if the geometry of this property includes elevation values")]
        public bool HasElevation
        {
            get { return _geomDef.HasElevation; }
            set { _geomDef.HasElevation = value; }
        }

        [Description("Indicates if the geometry of this property includes measurement values")]
        public bool HasMeasure
        {
            get { return _geomDef.HasMeasure; }
            set { _geomDef.HasMeasure = value; }
        }

        [Description("Indicates if this property is read-only")]
        public bool ReadOnly
        {
            get { return _geomDef.ReadOnly; }
            set { _geomDef.ReadOnly = value; }
        }

        [Description("The spatial context association to this property")]
        [Editor(typeof(SpatialContextAssociationEditor), typeof(UITypeEditor))]
        public string SpatialContextAssociation
        {
            get { return _geomDef.SpatialContextAssociation; }
            set { _geomDef.SpatialContextAssociation = value; }
        }
    }
}
