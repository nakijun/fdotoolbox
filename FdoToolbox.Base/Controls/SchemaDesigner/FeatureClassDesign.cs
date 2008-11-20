using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class FeatureClassDesign : ClassDefinitionDesign
    {
        private OSGeo.FDO.Schema.FeatureClass _featClass;

        public FeatureClassDesign(OSGeo.FDO.Schema.FeatureClass fc)
            : base(fc)
        {
            _featClass = fc;
        }

        [Description("The geometry property for this feature class")]
        [Editor(typeof(GeometryPropertyLookupEditor), typeof(UITypeEditor))]
        public OSGeo.FDO.Schema.GeometricPropertyDefinition GeometryProperty
        {
            get { return _featClass.GeometryProperty; }
            set { _featClass.GeometryProperty = value; }
        }
    }
}
