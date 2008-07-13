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

    public class TaskLoaderException : Exception
    {
        public TaskLoaderException() : base() { }
        public TaskLoaderException(string msg) : base(msg) { }
        public TaskLoaderException(string msg, Exception inner) : base(msg, inner) { }
    }
}