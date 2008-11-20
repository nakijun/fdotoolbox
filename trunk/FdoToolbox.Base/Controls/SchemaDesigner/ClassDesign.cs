using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class ClassDesign : ClassDefinitionDesign
    {
        private OSGeo.FDO.Schema.Class _cls;

        public ClassDesign(OSGeo.FDO.Schema.Class cls)
            : base(cls)
        {
            _cls = cls;
        }
    }
}
