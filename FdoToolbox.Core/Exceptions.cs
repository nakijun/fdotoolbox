using System;

namespace FdoToolbox.Core
{
    public class ModuleLoadException : Exception
    {
        public ModuleLoadException() : base() { }
        public ModuleLoadException(string msg) : base(msg) { }
        public ModuleLoadException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class FdoConnectionException : Exception
    {
        public FdoConnectionException() : base() { }
        public FdoConnectionException(string msg) : base(msg) { }
        public FdoConnectionException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class BulkCopyException : Exception
    {
        public BulkCopyException() : base() { }
        public BulkCopyException(string msg) : base(msg) { }
        public BulkCopyException(string msg, Exception inner) : base(msg, inner) { }
    }
}