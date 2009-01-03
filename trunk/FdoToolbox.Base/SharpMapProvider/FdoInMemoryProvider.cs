#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
using System.Text;
using SharpMap.Data.Providers;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Geometry;
using SharpMap.Converters.WellKnownBinary;
using FdoToolbox.Core;

namespace FdoToolbox.Base.SharpMapProvider
{
    public class FdoInMemoryProvider : IProvider
    {
        private FdoFeatureTable _data;

        public FdoInMemoryProvider() { }

        public FdoFeatureTable DataSource
        {
            get { return _data; }
            set { _data = value; }
        }

        public List<SharpMap.Geometries.Geometry> GetGeometriesInView(SharpMap.Geometries.BoundingBox bbox)
        {
            FdoGeometryFactory fact = FdoGeometryFactory.Instance;

            if (_data == null || _data.Rows.Count == 0 || string.IsNullOrEmpty(_data.GeometryColumn))
                return new List<SharpMap.Geometries.Geometry>();

            List<SharpMap.Geometries.Geometry> geoms = new List<SharpMap.Geometries.Geometry>();
            foreach (FdoFeature feat in _data.Rows)
            {
                try
                {
                    //Get the WKB form of the geometry
                    OSGeo.FDO.Geometry.IGeometry geom = (OSGeo.FDO.Geometry.IGeometry)feat[_data.GeometryColumn];
                    byte[] wkb = fact.GetWkb(geom);
                    geoms.Add(GeometryFromWKB.Parse(wkb));
                }
                catch { }
            }
            return geoms;
        }

        public List<uint> GetObjectIDsInView(SharpMap.Geometries.BoundingBox bbox)
        {
            return null;
        }

        public SharpMap.Geometries.Geometry GetGeometryByID(uint oid)
        {
            return null;
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.Geometry geom, SharpMap.Data.FeatureDataSet ds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExecuteIntersectionQuery(SharpMap.Geometries.BoundingBox box, SharpMap.Data.FeatureDataSet ds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFeatureCount()
        {
            if (_data != null)
                return _data.Rows.Count;
            else
                return 0;
        }

        public SharpMap.Data.FeatureDataRow GetFeature(uint RowID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public SharpMap.Geometries.BoundingBox GetExtents()
        {
            SharpMap.Geometries.BoundingBox bbox = null;
            if (_data == null || _data.Rows.Count == 0 || string.IsNullOrEmpty(_data.GeometryColumn))
                return new SharpMap.Geometries.BoundingBox(0.0, 0.0, 0.0, 0.0);
            
            foreach (FdoFeature feat in _data.Rows)
            {
                if (feat[_data.GeometryColumn] != null && feat[_data.GeometryColumn] != DBNull.Value)
                {
                    try
                    {
                        OSGeo.FDO.Geometry.IGeometry geom = (OSGeo.FDO.Geometry.IGeometry)feat[_data.GeometryColumn];
                        if (bbox != null)
                        {
                            if (geom.Envelope.MaxX > bbox.Max.X)
                                bbox.Max.X = geom.Envelope.MaxX;
                            if (geom.Envelope.MaxY > bbox.Max.Y)
                                bbox.Max.Y = geom.Envelope.MaxY;
                            if (geom.Envelope.MinX < bbox.Min.X)
                                bbox.Min.X = geom.Envelope.MinX;
                            if (geom.Envelope.MinY < bbox.Min.Y)
                                bbox.Min.Y = geom.Envelope.MinY;
                        }
                        else
                        {
                            bbox = new SharpMap.Geometries.BoundingBox(geom.Envelope.MinX, geom.Envelope.MinY, geom.Envelope.MaxX, geom.Envelope.MaxY);
                        }
                    }
                    catch { }
                }
            }
            if (bbox != null)
                return bbox;
            else
                return new SharpMap.Geometries.BoundingBox(0.0, 0.0, 0.0, 0.0);
        }

        public string ConnectionID
        {
            get { return _data.TableName; }
        }

        public void Open()
        {
            
        }

        public void Close()
        {
            
        }

        public bool IsOpen
        {
            get { return true; }
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
