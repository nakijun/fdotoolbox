using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Thrown when any error is thrown by the FDO API
    /// </summary>
    [Serializable]
    public class FdoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="serInfo">The serialization info.</param>
        /// <param name="ctx">The streaming context.</param>
        protected FdoException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        public FdoException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public FdoException(string msg) : base(msg) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FdoException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public FdoException(string msg, Exception inner) : base(msg, inner) { }
        internal FdoException(OSGeo.FDO.Common.Exception ex) : base(ex.Message, ex) { }
    }
}
