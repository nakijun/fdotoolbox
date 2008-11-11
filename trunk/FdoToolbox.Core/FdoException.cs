using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FdoToolbox.Core
{
    [Serializable]
    public class FdoException : Exception
    {
        protected FdoException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public FdoException() : base() { }
        public FdoException(string msg) : base(msg) { }
        public FdoException(string msg, Exception inner) : base(msg, inner) { }
        internal FdoException(OSGeo.FDO.Common.Exception ex) : base(ex.Message, ex) { }
    }
}
