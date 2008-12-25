using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.AppFramework
{
    /// <summary>
    /// Status codes that can be returned by any console application
    /// </summary>
    public enum CommandStatus : int
    {
        /// <summary>
        /// Operation OK, no errors encounters
        /// </summary>
        E_OK = 0,
        /// <summary>
        /// Failed to create SDF
        /// </summary>
        E_FAIL_SDF_CREATE = 1,
        /// <summary>
        /// Failed to apply schema
        /// </summary>
        E_FAIL_APPLY_SCHEMA = 2,
        /// <summary>
        /// Failed to destory datastore
        /// </summary>
        E_FAIL_DESTROY_DATASTORE = 3,
        /// <summary>
        /// Failed to connect
        /// </summary>
        E_FAIL_CONNECT = 4,
        /// <summary>
        /// Failed to serialize a feature schema
        /// </summary>
        E_FAIL_SERIALIZE_SCHEMA_XML = 5,
        /// <summary>
        /// Failed to create datastore
        /// </summary>
        E_FAIL_CREATE_DATASTORE = 6,
        /// <summary>
        /// Failed to bulk copy
        /// </summary>
        E_FAIL_BULK_COPY = 7,
        /// <summary>
        /// Task validation failed
        /// </summary>
        E_FAIL_TASK_VALIDATION = 8,
        /// <summary>
        /// Failed to create connection
        /// </summary>
        E_FAIL_CREATE_CONNECTION = 9,
        /// <summary>
        /// Failed to find intended schema
        /// </summary>
        E_FAIL_SCHEMA_NOT_FOUND = 10,
        /// <summary>
        /// Failed to find intended class
        /// </summary>
        E_FAIL_CLASS_NOT_FOUND = 11,
        /// <summary>
        /// The given capability is not supported
        /// </summary>
        E_FAIL_UNSUPPORTED_CAPABILITY = 12,
        /// <summary>
        /// The query results failed to load
        /// </summary>
        E_FAIL_LOAD_QUERY_RESULTS = 13,
        /// <summary>
        /// Unknown failure
        /// </summary>
        E_FAIL_UNKNOWN = 14
    }
}
