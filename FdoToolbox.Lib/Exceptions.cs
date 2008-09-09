using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FdoToolbox.Lib
{
    [Serializable]
    public class ModuleLoadException : Exception
    {
        protected ModuleLoadException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public ModuleLoadException() : base() { }
        public ModuleLoadException(string msg) : base(msg) { }
        public ModuleLoadException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class FdoConnectionException : Exception
    {
        protected FdoConnectionException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public FdoConnectionException() : base() { }
        public FdoConnectionException(string msg) : base(msg) { }
        public FdoConnectionException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class DbConnectionException : Exception
    {
        protected DbConnectionException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public DbConnectionException() : base() { }
        public DbConnectionException(string msg) : base(msg) { }
        public DbConnectionException(string msg, Exception inner) : base(msg, inner) { }
    }
}
