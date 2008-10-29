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
using SharpMap.Data.Providers;
using OSGeo.FDO.Commands.Feature;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.Utility;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.Converters.WellKnownText;

namespace FdoToolbox.Lib.SharpMapProvider
{
    /// <summary>
    /// Sharp Map Data Provider for in-memory FDO feature data
    /// </summary>
    public class FdoInMemoryProvider : IProvider
    {
        private FdoDataTable _data;

        public FdoInMemoryProvider() { }

        public FdoDataTable DataSource
        {
            get { return _data; }
            set { _data = value; }
        }

        public void Close()
        {
            
        }

        public string ConnectionID
        {
            get { return _data.TableName; }
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.BoundingBox box, SharpMap.Data.FeatureDataSet ds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.Geometry geom, SharpMap.Data.FeatureDataSet ds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public SharpMap.Geometries.BoundingBox GetExtents()
        {
            SharpMap.Geometries.BoundingBox bbox = null;
            if (_data.TableType == OSGeo.FDO.Schema.ClassType.ClassType_FeatureClass)
            {
                FdoGeometryColumn geomCol = (_data as FdoFeatureTable).GeometryColumn;
                foreach (System.Data.DataRow row in _data.Rows)
                {
                    if (row[geomCol] != null && row[geomCol] != DBNull.Value)
                    {
                        if (geomCol.DataType == typeof(byte[]))
                        {
                            byte[] fgf = (byte[])row[geomCol];
                            byte[] wkb = FdoGeometryUtil.Fgf2Wkb(fgf);
                            SharpMap.Geometries.Geometry g = GeometryFromWKB.Parse(wkb);
                            SharpMap.Geometries.BoundingBox b = g.GetBoundingBox();
                            if (bbox != null)
                            {
                                if (b.Max.X > bbox.Max.X)
                                    bbox.Max.X = b.Max.X;
                                if (b.Max.Y > bbox.Max.Y)
                                    bbox.Max.Y = b.Max.Y;
                                if (b.Min.X < bbox.Min.X)
                                    bbox.Min.X = b.Min.X;
                                if (b.Min.Y < bbox.Min.Y)
                                    bbox.Min.Y = b.Min.Y;
                            }
                            else
                            {
                                bbox = b;
                            }
                        }
                        else
                        {
                            string text = row[geomCol].ToString();
                            SharpMap.Geometries.Geometry g = GeometryFromWKT.Parse(text);
                            SharpMap.Geometries.BoundingBox b = g.GetBoundingBox();
                            if (bbox != null)
                            {
                                if (b.Max.X > bbox.Max.X)
                                    bbox.Max.X = b.Max.X;
                                if (b.Max.Y > bbox.Max.Y)
                                    bbox.Max.Y = b.Max.Y;
                                if (b.Min.X < bbox.Min.X)
                                    bbox.Min.X = b.Min.X;
                                if (b.Min.Y < bbox.Min.Y)
                                    bbox.Min.Y = b.Min.Y;
                            }
                            else
                            {
                                bbox = b;
                            }
                        }
                    }
                }
            }
            if (bbox == null)
                return new SharpMap.Geometries.BoundingBox(0.0, 0.0, 0.0, 0.0);
            return bbox;
        }

        public SharpMap.Data.FeatureDataRow GetFeature(uint RowID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFeatureCount()
        {
            return _data.Rows.Count;
        }

        public List<SharpMap.Geometries.Geometry> GetGeometriesInView(SharpMap.Geometries.BoundingBox bbox)
        {
            List<SharpMap.Geometries.Geometry> geoms = new List<SharpMap.Geometries.Geometry>();
            if (_data.TableType == OSGeo.FDO.Schema.ClassType.ClassType_FeatureClass)
            {
                FdoGeometryColumn geomCol = (_data as FdoFeatureTable).GeometryColumn;
                foreach (System.Data.DataRow row in _data.Rows)
                {
                    if (row[geomCol] != null && row[geomCol] != DBNull.Value)
                    {
                        if (geomCol.DataType == typeof(byte[]))
                        {
                            byte[] fgf = (byte[])row[geomCol];
                            byte[] wkb = FdoGeometryUtil.Fgf2Wkb(fgf);
                            SharpMap.Geometries.Geometry g = GeometryFromWKB.Parse(wkb);
                            geoms.Add(g);
                        }
                        else
                        {
                            string text = row[geomCol].ToString();
                            SharpMap.Geometries.Geometry g = GeometryFromWKT.Parse(text);
                            geoms.Add(g);
                        }
                    }
                }
            }
            return geoms;
        }

        public SharpMap.Geometries.Geometry GetGeometryByID(uint oid)
        {
            return null;
        }

        public List<uint> GetObjectIDsInView(SharpMap.Geometries.BoundingBox bbox)
        {
            return null;
        }

        public bool IsOpen
        {
            get { return true; }
        }

        public void Open()
        {
            
        }

        public int SRID
        {
            get
            {
                return -1;
            }
            set
            {
                
            }
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
