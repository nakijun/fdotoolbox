using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.CoordinateSystems
{
    /// <summary>
    /// Data transfer object for Coordinate Systems
    /// </summary>
    public class CoordinateSystemDefinition
    {
        private string _Name;

        /// <summary>
        /// The user-defined name of the coordinate system
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;

        /// <summary>
        /// The user-defined description of the coordinate system
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Wkt;

        /// <summary>
        /// The Well Known Text representation of the coordinate system
        /// </summary>
        public string Wkt
        {
            get { return _Wkt; }
            set { _Wkt = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="wkt"></param>
        public CoordinateSystemDefinition(string name, string description, string wkt)
        {
            this.Name = name;
            this.Description = description;
            this.Wkt = wkt;
        }
    }
}
