using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Indicates the property type from a FDO reader
    /// </summary>
    public enum FdoPropertyType
    {
        /// <summary>
        /// Property is a boolean
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// Property is a byte
        /// </summary>
        Byte = 1,
        /// <summary>
        /// Property is a DateTime
        /// </summary>
        DateTime = 2,
        /// <summary>
        /// Property is a decimal
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// Property is a double
        /// </summary>
        Double = 4,
        /// <summary>
        /// Property is a Int16
        /// </summary>
        Int16 = 5,
        /// <summary>
        /// Property is a Int32
        /// </summary>
        Int32 = 6,
        /// <summary>
        /// Property is a Int64
        /// </summary>
        Int64 = 7,
        /// <summary>
        /// Property is a single
        /// </summary>
        Single = 8,
        /// <summary>
        /// Property is a string
        /// </summary>
        String = 9,
        /// <summary>
        /// Property is a BLOB
        /// </summary>
        BLOB = 10,
        /// <summary>
        /// Property is a CLOB
        /// </summary>
        CLOB = 11,
        /// <summary>
        /// Property is a Geometry
        /// </summary>
        Geometry,
        /// <summary>
        /// Property is an object
        /// </summary>
        Object,
        /// <summary>
        /// Property is an association
        /// </summary>
        Association,
        /// <summary>
        /// Property is a raster
        /// </summary>
        Raster
    }
}
