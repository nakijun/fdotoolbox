using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Commands.SpatialContext;
using System.ComponentModel;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Data Transfer Object for Spatial Contexts
    /// </summary>
    public class SpatialContextInfo
    {
        private string _name;

        [DisplayName("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _IsActive;

        [DisplayName("Active")]
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }

        private double _ZTolerance;

        [DisplayName("Z Tolerance")]
        public double ZTolerance
        {
            get { return _ZTolerance; }
            set { _ZTolerance = value; }
        }

        private double _XYTolerance;

        [DisplayName("X/Y Tolerance")]
        public double XYTolerance
        {
            get { return _XYTolerance; }
            set { _XYTolerance = value; }
        }

        private string _Description;

        [DisplayName("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _CoordinateSystem;

        [DisplayName("Coordinate System")]
        public string CoordinateSystem
        {
            get { return _CoordinateSystem; }
            set { _CoordinateSystem = value; }
        }

        private SpatialContextExtentType _ExtentType;

        [DisplayName("Extent Type")]
        public SpatialContextExtentType ExtentType
        {
            get { return _ExtentType; }
            set { _ExtentType = value; }
        }

        private string _ExtentGeometryText;

        [DisplayName("Extent Geometry")]
        public string ExtentGeometryText
        {
            get { return _ExtentGeometryText; }
            set { _ExtentGeometryText = value; }
        }

        private string _CoordinateSystemWkt;

        [DisplayName("Coordinate System WKT")]
        public string CoordinateSystemWkt
        {
            get { return _CoordinateSystemWkt; }
            set { _CoordinateSystemWkt = value; }
        }
    }
}
