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
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace FdoToolbox.Core.Configuration
{
    /// <summary>
    /// Defines a sequence of FdoUtil.exe calls
    /// </summary>
    [Serializable]
    public class SequentialProcessDefinition
    {
        [XmlIgnore]
        private static XmlSerializer _ser;

        [XmlIgnore]
        public static XmlSerializer Serializer
        {
            get
            {
                if (_ser == null)
                    _ser = new XmlSerializer(typeof(SequentialProcessDefinition));

                return _ser;
            }
        }

        /// <summary>
        /// Gets the sequence of FdoUtil.exe calls
        /// </summary>
        [XmlElement(ElementName = "SequentialOperation")]
        public SequentialOperation[] Operations { get; set; }
    }

    /// <summary>
    /// Defines a blocking call to FdoUtil.exe
    /// </summary>
    [Serializable]
    public class SequentialOperation
    {
        /// <summary>
        /// Gets or set the FdoUtil.exe command
        /// </summary>
        [XmlAttribute(AttributeName = "command")]
        [ReadOnly(true)]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets additional arguments for this operation
        /// </summary>
        [XmlElement(ElementName = "OperationArgument")]
        public SequentialOperationArgument[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets whether the sequential process is aborted if this operation
        /// returns a failure result
        /// </summary>
        [XmlAttribute]
        public bool AbortProcessOnFailure { get; set; }

        public override string ToString()
        {
            return string.Format("Command: {0}, AbortOnFailure: {1}", this.Command, this.AbortProcessOnFailure);
        }
    }

    /// <summary>
    /// Defines an argument of FdoUtil.exe
    /// </summary>
    [Serializable]
    public class SequentialOperationArgument
    {
        /// <summary>
        /// Gets or sets the argument name
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the argument value
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}
