using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FdoToolbox.Base
{
    [Serializable]
    public class FdoConnectionException : Exception
    {
        protected FdoConnectionException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public FdoConnectionException() : base() { }
        public FdoConnectionException(string msg) : base(msg) { }
        public FdoConnectionException(string msg, Exception inner) : base(msg, inner) { }
    }
}
