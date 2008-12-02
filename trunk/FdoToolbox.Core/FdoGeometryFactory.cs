using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Globally accessible singleton geometry factory
    /// </summary>
    public sealed class FdoGeometryFactory : FgfGeometryFactory, IFdoGeometryFactory
    {
        private static FdoGeometryFactory _instance;

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static FdoGeometryFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FdoGeometryFactory();
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// FDO geometry factory interface
    /// </summary>
    public interface IFdoGeometryFactory
    {
        ICircularArcSegment CreateCircularArcSegment(IDirectPosition startPosition, IDirectPosition midPosition, IDirectPosition endPosition);
        ICurvePolygon CreateCurvePolygon(IRing exteriorRing, RingCollection interiorRings);
        ICurveString CreateCurveString(CurveSegmentCollection curveSegments);
        IGeometry CreateGeometry(IEnvelope envelope);
        IGeometry CreateGeometry(IGeometry geometry);
        IGeometry CreateGeometry(string text);
        IGeometry CreateGeometryFromFgf(byte[] bytes);
        IGeometry CreateGeometryFromFgf(byte[] bytes, int count);
        IGeometry CreateGeometryFromWkb(byte[] bytes);
        ILinearRing CreateLinearRing(DirectPositionCollection positions);
        ILinearRing CreateLinearRing(int dimensionality, int ordinateNumber, double[] ordinates);
        ILineString CreateLineString(DirectPositionCollection positions);
        ILineString CreateLineString(int dimensionType, int ordinateNumber, double[] ordinates);
        ILineStringSegment CreateLineStringSegment(DirectPositionCollection positions);
        ILineStringSegment CreateLineStringSegment(int dimType, int ordinateNumber, double[] ordinates);
        IMultiCurvePolygon CreateMultiCurvePolygon(CurvePolygonCollection curvePolygons);
        IMultiCurveString CreateMultiCurveString(CurveStringCollection curveStrings);
        IMultiGeometry CreateMultiGeometry(GeometryCollection geometries);
        IMultiLineString CreateMultiLineString(LineStringCollection lineStrings);
        IMultiPoint CreateMultiPoint(PointCollection points);
        IMultiPoint CreateMultiPoint(int dimensionality, int ordinateNumber, double[] ordinates);
        IMultiPolygon CreateMultiPolygon(PolygonCollection polygons);
        IPoint CreatePoint(IDirectPosition position);
        IPoint CreatePoint(int dimensionality, double[] ordinates);
        IPolygon CreatePolygon(ILinearRing exteriorRing, LinearRingCollection interiorRings);
        IRing CreateRing(CurveSegmentCollection curveSegments);
        byte[] GetFgf(IGeometry geometry);
        byte[] GetWkb(IGeometry geometry);
    }
}
