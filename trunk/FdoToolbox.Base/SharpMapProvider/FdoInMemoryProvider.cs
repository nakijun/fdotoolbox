using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Data.Providers;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Geometry;
using SharpMap.Converters.WellKnownBinary;

namespace FdoToolbox.Base.SharpMapProvider
{
    public class FdoInMemoryProvider : IProvider
    {
        private FdoFeatureTable _data;

        private FgfGeometryFactory _factory = new FgfGeometryFactory();

        public FdoInMemoryProvider() { }

        public FdoFeatureTable DataSource
        {
            get { return _data; }
            set { _data = value; }
        }

        public List<SharpMap.Geometries.Geometry> GetGeometriesInView(SharpMap.Geometries.BoundingBox bbox)
        {
            if (_data == null || _data.Rows.Count == 0 || string.IsNullOrEmpty(_data.GeometryColumn))
                return new List<SharpMap.Geometries.Geometry>();

            List<SharpMap.Geometries.Geometry> geoms = new List<SharpMap.Geometries.Geometry>();
            foreach (FdoFeature feat in _data.Rows)
            {
                try
                {
                    //Get the WKB form of the geometry
                    OSGeo.FDO.Geometry.IGeometry geom = (OSGeo.FDO.Geometry.IGeometry)feat[_data.GeometryColumn];
                    byte[] wkb = _factory.GetWkb(geom);
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
            _factory.Dispose();
        }
    }
}
