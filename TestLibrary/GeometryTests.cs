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
using NUnit.Framework;
using OSGeo.FDO.Geometry;
using FdoToolbox.Core.Utility;

namespace FdoToolbox.Tests
{
    /*
     This test fixture tests the FGF conversion (to WKB and FGF text) facilities
     */
    [TestFixture]
    [Category("FdoToolboxCore")]
    public class GeometryTests
    {
        const int FDO_DIM_XY = 0;
        const int FDO_DIM_Z = 1;
        const int FDO_DIM_M = 2;

        private FgfGeometryFactory _factory;

        public GeometryTests() { _factory = new FgfGeometryFactory(); }

        [Test]
        public void TestPoint()
        {
            IGeometry geom = _factory.CreatePoint(FDO_DIM_XY, new double[] { 0.1, 0.2 });
            TestConversion(geom);
        }

        [Test]
        public void TestLineString()
        {
            double[] ordsXY = new double[6];
            ordsXY[0] = 0.0; ordsXY[1] = 1.0;
            ordsXY[2] = 2.0; ordsXY[3] = 3.0;
            ordsXY[4] = 4.0; ordsXY[5] = 5.0;

            IGeometry geom = _factory.CreateLineString(FDO_DIM_XY, ordsXY.Length, ordsXY);

            TestConversion(geom);
        }

        [Test]
        public void TestPolygon()
        {
            double [] ordsXYExt = new double[10];
            ordsXYExt[0] = 0.0; ordsXYExt[1] = 0.0;
            ordsXYExt[2] = 5.0; ordsXYExt[3] = 0.0;
            ordsXYExt[4] = 5.0; ordsXYExt[5] = 5.0;
            ordsXYExt[6] = 0.0; ordsXYExt[7] = 5.0;
            ordsXYExt[8] = 0.0; ordsXYExt[9] = 0.0;
 	
            double [] ordsXYInt1 = new double[8];
            ordsXYInt1[0] = 1.0; ordsXYInt1[1] = 1.0;
            ordsXYInt1[2] = 2.0; ordsXYInt1[3] = 1.0;
            ordsXYInt1[4] = 2.0; ordsXYInt1[5] = 2.0;
            ordsXYInt1[6] = 1.0; ordsXYInt1[7] = 1.0;

            double [] ordsXYInt2 = new double[8];
            ordsXYInt2[0] = 3.0; ordsXYInt2[1] = 3.0;
            ordsXYInt2[2] = 4.0; ordsXYInt2[3] = 3.0;
            ordsXYInt2[4] = 4.0; ordsXYInt2[5] = 4.0;
            ordsXYInt2[6] = 3.0; ordsXYInt2[7] = 3.0;

            ILinearRing extRing = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt.Length, ordsXYExt);
            ILinearRing intRing1 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt1.Length, ordsXYInt1);
            ILinearRing intRing2 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt2.Length, ordsXYInt2);

            LinearRingCollection intRings = new LinearRingCollection();
            intRings.Add(intRing1);
            intRings.Add(intRing2);
    
   	        IPolygon polygon = _factory.CreatePolygon(extRing, intRings);

            TestConversion(polygon);
        }

        [Test]
        public void TestMultiPoint()
        {
            double[] ordsXY1 = new double[2];
	        ordsXY1[0] = 1.0; ordsXY1[1] = 2.0;

	        double[] ordsXY2 = new double[2];
	        ordsXY2[0] = 4.0; ordsXY2[1] = 5.0;

	        double[] ordsXY3 = new double[2];
	        ordsXY3[0] = 7.0; ordsXY3[1] = 8.0;

	        IPoint pnt1 = _factory.CreatePoint(FDO_DIM_XY, ordsXY1);
	        IPoint pnt2 = _factory.CreatePoint(FDO_DIM_XY, ordsXY2);
	        IPoint pnt3 = _factory.CreatePoint(FDO_DIM_XY, ordsXY3);

            PointCollection pnts = new PointCollection();
	        pnts.Add(pnt1);
            pnts.Add(pnt2);
	        pnts.Add(pnt3);

	        IMultiPoint multiPnt = _factory.CreateMultiPoint(pnts);

            TestConversion(multiPnt);
        }

        [Test]
        public void TestMultiLineString()
        {
            double[] ordsXYZ1 = new double[6];
	        ordsXYZ1[0] = 0.0; ordsXYZ1[1] = 1.0;
	        ordsXYZ1[2] = 3.0; ordsXYZ1[3] = 4.0;
	        ordsXYZ1[4] = 6.0; ordsXYZ1[5] = 7.0;

	        double[] ordsXYZ2 = new double[6];
	        ordsXYZ2[0] = 9.0; ordsXYZ2[1] = 10.0; 
	        ordsXYZ2[2] = 12.0; ordsXYZ2[3] = 13.0;
	        ordsXYZ2[4] = 15.0; ordsXYZ2[5] = 16.0;

            ILineString lineString1 = _factory.CreateLineString(FDO_DIM_XY, ordsXYZ1.Length, ordsXYZ1);
            ILineString lineString2 = _factory.CreateLineString(FDO_DIM_XY, ordsXYZ2.Length, ordsXYZ2);

            LineStringCollection lineStrings = new LineStringCollection();
	        lineStrings.Add(lineString1);
	        lineStrings.Add(lineString2);

	        IMultiLineString multiLine = _factory.CreateMultiLineString(lineStrings);

            TestConversion(multiLine);
        }

        [Test]
        public void TestMultiPolygon()
        {
            // 1st polygon
	        double[] ordsXYExt1 = new double[10];
	        ordsXYExt1[0] = 0.0; ordsXYExt1[1] = 0.0;
	        ordsXYExt1[2] = 5.0; ordsXYExt1[3] = 0.0;
	        ordsXYExt1[4] = 5.0; ordsXYExt1[5] = 5.0;
	        ordsXYExt1[6] = 0.0; ordsXYExt1[7] = 5.0;
	        ordsXYExt1[8] = 0.0; ordsXYExt1[9] = 0.0;

	        double[] ordsXYInt11 = new double[8];
	        ordsXYInt11[0] = 1.0; ordsXYInt11[1] = 1.0;
	        ordsXYInt11[2] = 2.0; ordsXYInt11[3] = 1.0;
	        ordsXYInt11[4] = 2.0; ordsXYInt11[5] = 2.0;
	        ordsXYInt11[6] = 1.0; ordsXYInt11[7] = 1.0;

	        double[] ordsXYInt12 = new double[8];
	        ordsXYInt12[0] = 3.0; ordsXYInt12[1] = 3.0;
	        ordsXYInt12[2] = 4.0; ordsXYInt12[3] = 3.0;
	        ordsXYInt12[4] = 4.0; ordsXYInt12[5] = 4.0;
	        ordsXYInt12[6] = 3.0; ordsXYInt12[7] = 3.0;

            ILinearRing extRing1 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt1.Length, ordsXYExt1);
            ILinearRing intRing11 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt11.Length, ordsXYInt11);
            ILinearRing intRing12 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt12.Length, ordsXYInt12);

            LinearRingCollection intRings1 = new LinearRingCollection();
	        intRings1.Add(intRing11);
	        intRings1.Add(intRing12);

	        IPolygon polygon1 = _factory.CreatePolygon(extRing1, intRings1);

	        // 2nd polygon
	        double[] ordsXYExt2 = new double[10];
	        ordsXYExt2[0] = 0.0; ordsXYExt2[1] = 0.0;
	        ordsXYExt2[2] = 5.0; ordsXYExt2[3] = 0.0;
	        ordsXYExt2[4] = 5.0; ordsXYExt2[5] = 5.0;
	        ordsXYExt2[6] = 0.0; ordsXYExt2[7] = 5.0;
	        ordsXYExt2[8] = 0.0; ordsXYExt2[9] = 0.0;

	        double[] ordsXYInt21 = new double[8];
	        ordsXYInt21[0] = 1.0; ordsXYInt21[1] = 1.0;
	        ordsXYInt21[2] = 2.0; ordsXYInt21[3] = 1.0;
	        ordsXYInt21[4] = 2.0; ordsXYInt21[5] = 2.0;
	        ordsXYInt21[6] = 1.0; ordsXYInt21[7] = 1.0;

	        double[] ordsXYInt22 = new double[8];
	        ordsXYInt22[0] = 3.0; ordsXYInt22[1] = 3.0;
	        ordsXYInt22[2] = 4.0; ordsXYInt22[3] = 3.0;
	        ordsXYInt22[4] = 4.0; ordsXYInt22[5] = 4.0;
	        ordsXYInt22[6] = 3.0; ordsXYInt22[7] = 3.0;

	        ILinearRing extRing2 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt2.Length, ordsXYExt2);
	        ILinearRing intRing21 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt21.Length, ordsXYInt21);
	        ILinearRing intRing22 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt22.Length, ordsXYInt22);

            LinearRingCollection intRings2 = new LinearRingCollection();
	        intRings2.Add(intRing21);
	        intRings2.Add(intRing22);

	        IPolygon polygon2 = _factory.CreatePolygon(extRing2, intRings2);

            PolygonCollection polygons = new PolygonCollection();
	        polygons.Add(polygon1);
	        polygons.Add(polygon2);

	        IMultiPolygon multiPoly = _factory.CreateMultiPolygon(polygons);

            TestConversion(multiPoly);
        }

        [Test]
        public void TestPointText()
        {
            IGeometry geom = _factory.CreatePoint(FDO_DIM_XY, new double[] { 0.1, 0.2 });
            TestText(geom);
        }

        [Test]
        public void TestLineStringText()
        {
            double[] ordsXY = new double[6];
            ordsXY[0] = 0.0; ordsXY[1] = 1.0;
            ordsXY[2] = 2.0; ordsXY[3] = 3.0;
            ordsXY[4] = 4.0; ordsXY[5] = 5.0;

            IGeometry geom = _factory.CreateLineString(FDO_DIM_XY, ordsXY.Length, ordsXY);

            TestText(geom);
        }

        [Test]
        public void TestPolygonText()
        {
            double[] ordsXYExt = new double[10];
            ordsXYExt[0] = 0.0; ordsXYExt[1] = 0.0;
            ordsXYExt[2] = 5.0; ordsXYExt[3] = 0.0;
            ordsXYExt[4] = 5.0; ordsXYExt[5] = 5.0;
            ordsXYExt[6] = 0.0; ordsXYExt[7] = 5.0;
            ordsXYExt[8] = 0.0; ordsXYExt[9] = 0.0;

            double[] ordsXYInt1 = new double[8];
            ordsXYInt1[0] = 1.0; ordsXYInt1[1] = 1.0;
            ordsXYInt1[2] = 2.0; ordsXYInt1[3] = 1.0;
            ordsXYInt1[4] = 2.0; ordsXYInt1[5] = 2.0;
            ordsXYInt1[6] = 1.0; ordsXYInt1[7] = 1.0;

            double[] ordsXYInt2 = new double[8];
            ordsXYInt2[0] = 3.0; ordsXYInt2[1] = 3.0;
            ordsXYInt2[2] = 4.0; ordsXYInt2[3] = 3.0;
            ordsXYInt2[4] = 4.0; ordsXYInt2[5] = 4.0;
            ordsXYInt2[6] = 3.0; ordsXYInt2[7] = 3.0;

            ILinearRing extRing = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt.Length, ordsXYExt);
            ILinearRing intRing1 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt1.Length, ordsXYInt1);
            ILinearRing intRing2 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt2.Length, ordsXYInt2);

            LinearRingCollection intRings = new LinearRingCollection();
            intRings.Add(intRing1);
            intRings.Add(intRing2);

            IPolygon polygon = _factory.CreatePolygon(extRing, intRings);

            TestText(polygon);
        }

        [Test]
        public void TestMultiPointText()
        {
            double[] ordsXY1 = new double[2];
            ordsXY1[0] = 1.0; ordsXY1[1] = 2.0;

            double[] ordsXY2 = new double[2];
            ordsXY2[0] = 4.0; ordsXY2[1] = 5.0;

            double[] ordsXY3 = new double[2];
            ordsXY3[0] = 7.0; ordsXY3[1] = 8.0;

            IPoint pnt1 = _factory.CreatePoint(FDO_DIM_XY, ordsXY1);
            IPoint pnt2 = _factory.CreatePoint(FDO_DIM_XY, ordsXY2);
            IPoint pnt3 = _factory.CreatePoint(FDO_DIM_XY, ordsXY3);

            PointCollection pnts = new PointCollection();
            pnts.Add(pnt1);
            pnts.Add(pnt2);
            pnts.Add(pnt3);

            IMultiPoint multiPnt = _factory.CreateMultiPoint(pnts);

            TestText(multiPnt);
        }

        [Test]
        public void TestMultiLineStringText()
        {
            double[] ordsXYZ1 = new double[6];
            ordsXYZ1[0] = 0.0; ordsXYZ1[1] = 1.0;
            ordsXYZ1[2] = 3.0; ordsXYZ1[3] = 4.0;
            ordsXYZ1[4] = 6.0; ordsXYZ1[5] = 7.0;

            double[] ordsXYZ2 = new double[6];
            ordsXYZ2[0] = 9.0; ordsXYZ2[1] = 10.0;
            ordsXYZ2[2] = 12.0; ordsXYZ2[3] = 13.0;
            ordsXYZ2[4] = 15.0; ordsXYZ2[5] = 16.0;

            ILineString lineString1 = _factory.CreateLineString(FDO_DIM_XY, ordsXYZ1.Length, ordsXYZ1);
            ILineString lineString2 = _factory.CreateLineString(FDO_DIM_XY, ordsXYZ2.Length, ordsXYZ2);

            LineStringCollection lineStrings = new LineStringCollection();
            lineStrings.Add(lineString1);
            lineStrings.Add(lineString2);

            IMultiLineString multiLine = _factory.CreateMultiLineString(lineStrings);

            TestText(multiLine);
        }

        [Test]
        public void TestMultiPolygonText()
        {
            // 1st polygon
            double[] ordsXYExt1 = new double[10];
            ordsXYExt1[0] = 0.0; ordsXYExt1[1] = 0.0;
            ordsXYExt1[2] = 5.0; ordsXYExt1[3] = 0.0;
            ordsXYExt1[4] = 5.0; ordsXYExt1[5] = 5.0;
            ordsXYExt1[6] = 0.0; ordsXYExt1[7] = 5.0;
            ordsXYExt1[8] = 0.0; ordsXYExt1[9] = 0.0;

            double[] ordsXYInt11 = new double[8];
            ordsXYInt11[0] = 1.0; ordsXYInt11[1] = 1.0;
            ordsXYInt11[2] = 2.0; ordsXYInt11[3] = 1.0;
            ordsXYInt11[4] = 2.0; ordsXYInt11[5] = 2.0;
            ordsXYInt11[6] = 1.0; ordsXYInt11[7] = 1.0;

            double[] ordsXYInt12 = new double[8];
            ordsXYInt12[0] = 3.0; ordsXYInt12[1] = 3.0;
            ordsXYInt12[2] = 4.0; ordsXYInt12[3] = 3.0;
            ordsXYInt12[4] = 4.0; ordsXYInt12[5] = 4.0;
            ordsXYInt12[6] = 3.0; ordsXYInt12[7] = 3.0;

            ILinearRing extRing1 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt1.Length, ordsXYExt1);
            ILinearRing intRing11 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt11.Length, ordsXYInt11);
            ILinearRing intRing12 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt12.Length, ordsXYInt12);

            LinearRingCollection intRings1 = new LinearRingCollection();
            intRings1.Add(intRing11);
            intRings1.Add(intRing12);

            IPolygon polygon1 = _factory.CreatePolygon(extRing1, intRings1);

            // 2nd polygon
            double[] ordsXYExt2 = new double[10];
            ordsXYExt2[0] = 0.0; ordsXYExt2[1] = 0.0;
            ordsXYExt2[2] = 5.0; ordsXYExt2[3] = 0.0;
            ordsXYExt2[4] = 5.0; ordsXYExt2[5] = 5.0;
            ordsXYExt2[6] = 0.0; ordsXYExt2[7] = 5.0;
            ordsXYExt2[8] = 0.0; ordsXYExt2[9] = 0.0;

            double[] ordsXYInt21 = new double[8];
            ordsXYInt21[0] = 1.0; ordsXYInt21[1] = 1.0;
            ordsXYInt21[2] = 2.0; ordsXYInt21[3] = 1.0;
            ordsXYInt21[4] = 2.0; ordsXYInt21[5] = 2.0;
            ordsXYInt21[6] = 1.0; ordsXYInt21[7] = 1.0;

            double[] ordsXYInt22 = new double[8];
            ordsXYInt22[0] = 3.0; ordsXYInt22[1] = 3.0;
            ordsXYInt22[2] = 4.0; ordsXYInt22[3] = 3.0;
            ordsXYInt22[4] = 4.0; ordsXYInt22[5] = 4.0;
            ordsXYInt22[6] = 3.0; ordsXYInt22[7] = 3.0;

            ILinearRing extRing2 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYExt2.Length, ordsXYExt2);
            ILinearRing intRing21 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt21.Length, ordsXYInt21);
            ILinearRing intRing22 = _factory.CreateLinearRing(FDO_DIM_XY, ordsXYInt22.Length, ordsXYInt22);

            LinearRingCollection intRings2 = new LinearRingCollection();
            intRings2.Add(intRing21);
            intRings2.Add(intRing22);

            IPolygon polygon2 = _factory.CreatePolygon(extRing2, intRings2);

            PolygonCollection polygons = new PolygonCollection();
            polygons.Add(polygon1);
            polygons.Add(polygon2);

            IMultiPolygon multiPoly = _factory.CreateMultiPolygon(polygons);

            TestText(multiPoly);
        }

        [Test]
        public void TestMultiCurvePolygonText()
        {
            IMultiCurvePolygon multiCurvePoly = CreateMultiCurvePolygon(3, 100);
            TestText(multiCurvePoly);
        }

        [Test]
        public void TestMultiCurveStringText()
        {
            ICurveString curveString1 = CreateCurveString(100);
            ICurveString curveString2 = CreateCurveString(200);
            ICurveString curveString3 = CreateCurveString(300);

            CurveStringCollection curveStrings = new CurveStringCollection();
            curveStrings.Add(curveString1);
            curveStrings.Add(curveString2);
            curveStrings.Add(curveString3);

            IMultiCurveString multiCurveString = _factory.CreateMultiCurveString(curveStrings);

            TestText(multiCurveString);
        }

        [Test]
        public void TestMultiGeometryText()
        {
            IMultiGeometry multiGeometry = CreateMultiGeometry();
            TestText(multiGeometry);
        }

        private IMultiGeometry CreateMultiGeometry()
        {
            GeometryCollection geometries = new GeometryCollection();

	        IGeometry geometry;

            // CurvePolygon
            geometry = CreateCurvePolygon(0);
	        geometries.Add(geometry);

            // CurveString
            // Not doing CurveString because of some unfixed defect.
            // It may be the same one that sometimes affects MultiPolygon.
            geometry = CreateCurveString(100);
	        geometries.Add(geometry);

            // LineString
	        double[] ordsXY = new double[6];
	        ordsXY[0] = 0.0; ordsXY[1] = 1.0;
	        ordsXY[2] = 2.0; ordsXY[3] = 3.0;
	        ordsXY[4] = 4.0; ordsXY[5] = 5.0;
            geometry = _factory.CreateLineString(FDO_DIM_XY, 6, ordsXY);
	        geometries.Add(geometry);

            // Point
	        double[]	ordsXYZ = new double[3];
	        ordsXYZ[0] = 5.0; ordsXYZ[1] = 3.0; ordsXYZ[2] = 2.0;
            IPoint pt = _factory.CreatePoint(FDO_DIM_XY | FDO_DIM_Z, ordsXYZ);
            
            geometry = pt;
	        geometries.Add(geometry);

            // Polygon
            // Not doing Polygon because of some unfixed defect.
            // It may be the same one that sometimes affects MultiPolygon.
	        double[] ordsXYExt = new double[10];
	        ordsXYExt[0] = 0.0; ordsXYExt[1] = 0.0;
	        ordsXYExt[2] = 5.0; ordsXYExt[3] = 0.0;
	        ordsXYExt[4] = 5.0; ordsXYExt[5] = 5.0;
	        ordsXYExt[6] = 0.0; ordsXYExt[7] = 5.0;
	        ordsXYExt[8] = 0.0; ordsXYExt[9] = 0.0;
	        double[] ordsXYInt1 = new double[8];
	        ordsXYInt1[0] = 1.0; ordsXYInt1[1] = 1.0;
	        ordsXYInt1[2] = 2.0; ordsXYInt1[3] = 1.0;
	        ordsXYInt1[4] = 2.0; ordsXYInt1[5] = 2.0;
	        ordsXYInt1[6] = 1.0; ordsXYInt1[7] = 1.0;
	        double[] ordsXYInt2 = new double[8];
	        ordsXYInt2[0] = 3.0; ordsXYInt2[1] = 3.0;
	        ordsXYInt2[2] = 4.0; ordsXYInt2[3] = 3.0;
	        ordsXYInt2[4] = 4.0; ordsXYInt2[5] = 4.0;
	        ordsXYInt2[6] = 3.0; ordsXYInt2[7] = 3.0;

	        ILinearRing extRing = _factory.CreateLinearRing(FDO_DIM_XY, 10, ordsXYExt);
	        ILinearRing intRing1 = _factory.CreateLinearRing(FDO_DIM_XY, 8, ordsXYInt1);
	        ILinearRing intRing2 = _factory.CreateLinearRing(FDO_DIM_XY, 8, ordsXYInt2);
            LinearRingCollection intRings = new LinearRingCollection();
	        intRings.Add(intRing1);
	        intRings.Add(intRing2);
	        geometry = _factory.CreatePolygon(extRing, intRings);
	        geometries.Add(geometry);

            // Make MultiGeometry from the many geometries collected above.
	        return _factory.CreateMultiGeometry(geometries);
        }

        private IMultiCurvePolygon CreateMultiCurvePolygon(int numCurvePolys, double offset)
        {
            CurvePolygonCollection curvePolys = new CurvePolygonCollection();

	        for (int i=0; i<numCurvePolys; i++)
	        {
		        ICurvePolygon curvePoly = CreateCurvePolygon(i+offset);
		        curvePolys.Add(curvePoly);
	        }

	        return _factory.CreateMultiCurvePolygon(curvePolys);
        }

        private ICurvePolygon CreateCurvePolygon(double offset)
        {
            IRing extRing = CreateRing(offset + 100);

	        int numIntRings = 2;

            RingCollection intRings = new RingCollection();

	        IRing ring1 = CreateRing(offset + 200);
	        IRing ring2 = CreateRing(offset + 300);
	        intRings.Add(ring1);
	        intRings.Add(ring2);

	        return _factory.CreateCurvePolygon(extRing, intRings);
        }

        private IRing CreateRing(double offset)
        {
            // Ring is a closed entity.
	        // Create and return a ring consisting of
	        // one circulararcsegment and one linearstringsegment

	        // arcseg  = (0,0), (0,1), (1,2)
	        // lineseg = (1,2), (0,0)

	        IDirectPosition startPos = _factory.CreatePositionXY(offset+0.0, offset+0.0);
	        IDirectPosition midPos = _factory.CreatePositionXY(offset+0.0, offset+1.0);
	        IDirectPosition endPos = _factory.CreatePositionXY(offset+1.0, offset+2.0);

	        ICircularArcSegment arcSeg = _factory.CreateCircularArcSegment(startPos, midPos, endPos);

            DirectPositionCollection points = new DirectPositionCollection();
	        IDirectPosition fromPt = _factory.CreatePositionXY(offset+1.0, offset+2.0);
	        IDirectPosition toPt = _factory.CreatePositionXY(offset+0.0, offset+0.0);
            points.Add(fromPt);
            points.Add(toPt);

	        ILineStringSegment lineSeg = _factory.CreateLineStringSegment(points);

            CurveSegmentCollection curveSegs = new CurveSegmentCollection();
	        curveSegs.Add(arcSeg);
	        curveSegs.Add(lineSeg);

	        return _factory.CreateRing(curveSegs);
        }

        private ICurveString CreateCurveString(double offset)
        {
            IDirectPosition startPos = _factory.CreatePositionXY(offset + 0.0, offset + 0.0);
            IDirectPosition midPos = _factory.CreatePositionXY(offset + 0.0, offset + 1.0);
            IDirectPosition endPos = _factory.CreatePositionXY(offset + 1.0, offset + 2.0);

	        ICircularArcSegment arcSeg = _factory.CreateCircularArcSegment(startPos, midPos, endPos);

            DirectPositionCollection points = new DirectPositionCollection();
	        IDirectPosition pt1 = _factory.CreatePositionXY(offset+1.0, offset+2.0);
	        IDirectPosition pt2 = _factory.CreatePositionXY(offset+3.0, offset+0.0);
	        IDirectPosition pt3 = _factory.CreatePositionXY(offset+3.0, offset+2.0);
            points.Add(pt1);
            points.Add(pt2);
	        points.Add(pt3);

	        ILineStringSegment lineSeg = _factory.CreateLineStringSegment(points);

            CurveSegmentCollection curveSegs = new CurveSegmentCollection();
            curveSegs.Add(arcSeg);
            curveSegs.Add(lineSeg);

	        return _factory.CreateCurveString(curveSegs);
        }

        private void TestText(IGeometry geom)
        {
            string fgfText = geom.Text;
            byte[] fgf = _factory.GetFgf(geom);

            geom.Dispose();
            
            //Test the method
            string text = FdoGeometryUtil.GetFgfText(fgf);
            Assert.IsFalse(string.IsNullOrEmpty(text));

            Console.WriteLine("FGF Text: " + fgfText);
            Console.WriteLine("Text:     " + text);

            Assert.AreEqual(fgfText, text, "FGF texts did not match");

            try
            {
                //Create a new geometry from our converted text
                using (IGeometry g = _factory.CreateGeometry(text)) { }
            }
            catch (OSGeo.FDO.Common.Exception)
            {
                Assert.Fail("FGF text should be valid");
            }
        }

        private void TestConversion(IGeometry geom)
        {
            //Get the bits returned by FDO
            string fgfText = geom.Text;
            byte[] fgf = _factory.GetFgf(geom);
            byte[] wkb = _factory.GetWkb(geom);

            geom.Dispose();

            //Get our converted wkb
            byte[] convertedWkb = FdoGeometryUtil.Fgf2Wkb(fgf);
            Assert.IsNotNull(convertedWkb);

            //Our converted wkb should be the same as the wkb returned by FDO
            Assert.AreEqual(wkb.Length, convertedWkb.Length);
            Assert.IsTrue(BinaryEquals(wkb, convertedWkb));

            //The text created by our converted wkb should match the text
            //returned by FDO
            geom = _factory.CreateGeometryFromWkb(convertedWkb);
            Assert.AreEqual(fgfText, geom.Text);

            geom.Dispose();
        }

        private bool BinaryEquals(byte[] wkb, byte[] convertedWkb)
        {
            if (wkb.Length != convertedWkb.Length)
                return false;

            for (int i = 0; i < wkb.Length; i++)
            {
                if (wkb[i] != convertedWkb[i])
                    return false;
            }
            return true;
        }
    }
}
