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
using System.Runtime.Serialization;

namespace FdoToolbox.Core
{
    [Serializable]
    public class BulkCopyException : Exception
    {
        protected BulkCopyException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public BulkCopyException() : base() { }
        public BulkCopyException(string msg) : base(msg) { }
        public BulkCopyException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class SpatialJoinException : Exception
    {
        protected SpatialJoinException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public SpatialJoinException() : base() { }
        public SpatialJoinException(string msg) : base(msg) { }
        public SpatialJoinException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class DbToSpatialException : Exception
    {
        protected DbToSpatialException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public DbToSpatialException() : base() { }
        public DbToSpatialException(string msg) : base(msg) { }
        public DbToSpatialException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class PreferenceException : Exception
    {
        protected PreferenceException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public PreferenceException() : base() { }
        public PreferenceException(string msg) : base(msg) { }
        public PreferenceException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class UnsupportedException : Exception
    {
        protected UnsupportedException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public UnsupportedException() : base() { }
        public UnsupportedException(string msg) : base(msg) { }
        public UnsupportedException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class TaskLoaderException : Exception
    {
        protected TaskLoaderException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public TaskLoaderException() : base() { }
        public TaskLoaderException(string msg) : base(msg) { }
        public TaskLoaderException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class TaskValidationException : Exception
    {
        protected TaskValidationException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public TaskValidationException() : base() { }
        public TaskValidationException(string msg) : base(msg) { }
        public TaskValidationException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class FeatureServiceException : Exception
    {
        protected FeatureServiceException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public FeatureServiceException() : base() { }
        public FeatureServiceException(string msg) : base(msg) { }
        public FeatureServiceException(string msg, Exception inner) : base(msg, inner) { }
    }

    [Serializable]
    public class DataTableConversionException : Exception
    {
        protected DataTableConversionException(SerializationInfo serInfo, StreamingContext ctx) : base(serInfo, ctx) { }
        public DataTableConversionException() : base() { }
        public DataTableConversionException(string msg) : base(msg) { }
        public DataTableConversionException(string msg, Exception inner) : base(msg, inner) { }
    }
}