using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Feature
{
    public enum FdoPropertyType
    {
        Boolean = 0,
        Byte = 1,
        DateTime = 2,
        Decimal = 3,
        Double = 4,
        Int16 = 5,
        Int32 = 6,
        Int64 = 7,
        Single = 8,
        String = 9,
        BLOB = 10,
        CLOB = 11,
        Geometry,
        Object,
        Association,
        Raster
    }
}
