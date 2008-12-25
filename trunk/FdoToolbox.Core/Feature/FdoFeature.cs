using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// FDO-friendly <see cref="DataRow"/>
    /// </summary>
    public class FdoFeature : DataRow
    {
        internal FdoFeature(DataRowBuilder rb) : base(rb) { }

        /// <summary>
        /// Returns the item array for this feature. Geometry values are
        /// converted to FGF text form.
        /// </summary>
        /// <returns></returns>
        public object[] GeometriesAsText()
        {
            object[] items = this.ItemArray;
            object[] objs = new object[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                if(items[i] != null && items[i] != DBNull.Value)
                {
                    IGeometry geom = items[i] as IGeometry;
                    if (geom != null)
                    {
                        try
                        {
                            objs[i] = geom.Text;
                        }
                        catch
                        {
                            objs[i] = "INVALID GEOMETRY";
                        }
                    }
                    else
                    {
                        objs[i] = items[i];
                    }
                }
            }
            return objs;
        }
    }
}
