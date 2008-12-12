using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Controls the <see cref="FdoJoin"/> operation
    /// </summary>
    public class FdoJoinOptions
    {
        private FdoSource _Left;

        /// <summary>
        /// Gets the left join source
        /// </summary>
        public FdoSource Left { get { return _Left; } }

        private FdoSource _Right;

        /// <summary>
        /// Gets the right join source
        /// </summary>
        public FdoSource Right { get { return _Right; } }

        private FdoSource _Target;

        /// <summary>
        /// Gets the join target
        /// </summary>
        public FdoSource Target { get { return _Target; } }

        private string _LeftPrefix;

        /// <summary>
        /// Gets or sets the left column prefix that is applied in the event of a name collision in the merged feature
        /// </summary>
        public string LeftPrefix
        {
            get { return _LeftPrefix; }
            set { _LeftPrefix = value; }
        }

        private string _RightPrefix;

        /// <summary>
        /// Gets or sets the right column prefix that is applied in the event of a name collision in the merged feature
        /// </summary>
        public string RightPrefix
        {
            get { return _RightPrefix; }
            set { _RightPrefix = value; }
        }

        /// <summary>
        /// Sets the left source of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetLeft(FdoConnection conn, string schemaName, string className)
        {
            _Left = new FdoSource(conn, schemaName, className);
        }

        /// <summary>
        /// Sets the right source of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetRight(FdoConnection conn, string schemaName, string className)
        {
            _Right = new FdoSource(conn, schemaName, className);
        }

        /// <summary>
        /// Sets the target of the join
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetTarget(FdoConnection conn, string schemaName, string className)
        {
            _Target = new FdoSource(conn, schemaName, className);
        }
    }

    /// <summary>
    /// Defines a FDO source
    /// </summary>
    public class FdoSource
    {
        private FdoConnection _Connection;

        /// <summary>
        /// The connection for this source
        /// </summary>
        public FdoConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        private string _SchemaName;

        /// <summary>
        /// The schema name
        /// </summary>
        public string SchemaName            
        {
            get { return _SchemaName; }
            set { _SchemaName = value; }
        }

        private string _ClassName;

        /// <summary>
        /// The class name
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="schema"></param>
        /// <param name="className"></param>
        public FdoSource(FdoConnection conn, string schema, string className)
        {
            _Connection = conn;
            _SchemaName = schema;
            _ClassName = className;
        }
    }

    /// <summary>
    /// Defines the possible join types
    /// </summary>
    public enum FdoJoinType
    {
        /// <summary>
        /// Inner join, only matching objects from both sides are merged
        /// </summary>
        Inner,
        /// <summary>
        /// Left join, left side objects are merged with right side objects regardless of whether the right side object exists or not
        /// </summary>
        Left,
        /// <summary>
        /// Right join, right side objects are merged with left side objects regardless of whether the left side object exists or not
        /// </summary>
        Right,
        /// <summary>
        /// Full join, both sides of the join are merged regardless of whether either side exists or not
        /// </summary>
        Full,
    }
}
