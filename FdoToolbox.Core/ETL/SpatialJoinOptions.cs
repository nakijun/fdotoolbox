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
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Defines options for merging a Database table with a FDO feature class
    /// </summary>
    public class SpatialJoinOptions
    {
        private FdoConnection _Target;

        /// <summary>
        /// The target connection (where the join results will be written to)
        /// </summary>
        public FdoConnection Target
        {
            get { return _Target; }
        }

        private FdoConnection _PrimarySource;

        /// <summary>
        /// The primary FDO connection
        /// </summary>
        public FdoConnection PrimarySource
        {
            get { return _PrimarySource; }
        }

        private DatabaseConnection _SecondarySource;

        /// <summary>
        /// The secondary database connection
        /// </summary>
        public DatabaseConnection SecondarySource
        {
            get { return _SecondarySource; }
        }

        private SpatialJoinType _JoinType;

        /// <summary>
        /// The type of join to perform
        /// </summary>
        public SpatialJoinType JoinType
        {
            get { return _JoinType; }
            set { _JoinType = value; }
        }

        private string _TargetSchema;

        /// <summary>
        /// The feature schema to write the join results to
        /// </summary>
        public string TargetSchema
        {
            get { return _TargetSchema; }
        }

        private string _TargetClassName;

        /// <summary>
        /// The feature class to write the join results to. This class must not already
        /// exist, as it will be created during the join process.
        /// </summary>
        public string TargetClassName
        {
            get { return _TargetClassName; }
        }
	
        public SpatialJoinOptions()
        {
            _JoinedPairs = new Dictionary<string, string>();
            _PropertyList = new List<string>();
            _ColumnList = new List<string>();
            this.Cardinality = SpatialJoinCardinality.OneToOne;
            this.JoinType = SpatialJoinType.LeftOuter;
            _PrimaryPrefix = "";
            _SecondaryPrefix = "";
        }

        private string _PrimaryPrefix;

        /// <summary>
        /// The prefix that will be prepended to each primary property (to avoid 
        /// name collisions with secondary columns)
        /// </summary>
        public string PrimaryPrefix
        {
            get { return _PrimaryPrefix; }
            set { _PrimaryPrefix = value; }
        }

        private string _SecondaryPrefix;

        /// <summary>
        /// The prefix that will be prepended to each secondary property (to avoid
        /// name collisions with primary properties)
        /// </summary>
        public string SecondaryPrefix
        {
            get { return _SecondaryPrefix; }
            set { _SecondaryPrefix = value; }
        }

        /// <summary>
        /// Sets the primary source
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetPrimary(FdoConnection connInfo, string schemaName, string className)
        {
            _PrimarySource = connInfo;
            _SchemaName = schemaName;
            _ClassName = className;
        }

        /// <summary>
        /// Sets the secondary source
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="tableName"></param>
        public void SetSecondary(DatabaseConnection connInfo, string tableName)
        {
            _SecondarySource = connInfo;
            _TableName = tableName;
        }

        /// <summary>
        /// Sets the target where join results will be written to
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        public void SetTarget(FdoConnection connInfo, string schemaName, string className)
        {
            _Target = connInfo;
            _TargetSchema = schemaName;
            _TargetClassName = className;
        }

        private List<string> _PropertyList;

        private List<string> _ColumnList;

        private Dictionary<string, string> _JoinedPairs;

        private string _SchemaName;

        /// <summary>
        /// The primary feature schema
        /// </summary>
        public string SchemaName
        {
            get { return _SchemaName; }
        }

        private string _ClassName;

        /// <summary>
        /// The primary feature class
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
        }

        private string _TableName;

        /// <summary>
        /// The secondary database table
        /// </summary>
        public string TableName
        {
            get { return _TableName; }
        }

        /// <summary>
        /// Gets the column to join on
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetMatchingColumn(string propertyName)
        {
            return _JoinedPairs[propertyName];
        }

        /// <summary>
        /// Adds a primary/secondary name pair that defines what columns
        /// and properties will be joined.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="column"></param>
        public void AddJoinPair(string propertyName, string column)
        {
            _JoinedPairs[propertyName] = column;
        }

        /// <summary>
        /// Adds a secondary column to the join output
        /// </summary>
        /// <param name="columnName"></param>
        public void AddColumn(string columnName)
        {
            _ColumnList.Add(columnName);
        }

        /// <summary>
        /// Adds a primary property to the join output
        /// </summary>
        /// <param name="propertyName"></param>
        public void AddProperty(string propertyName)
        {
            _PropertyList.Add(propertyName);
        }

        /// <summary>
        /// Gets the primary properties that are to be part of the 
        /// join output
        /// </summary>
        /// <returns></returns>
        public string[] GetPropertyNames()
        {
            return _PropertyList.ToArray();
        }

        /// <summary>
        /// Gets the secondary columns that are to be part of the 
        /// join output
        /// </summary>
        /// <returns></returns>
        public string[] GetColumnNames()
        {
            return _ColumnList.ToArray();
        }

        /// <summary>
        /// Gets the secondary columns that are to be joined on
        /// </summary>
        /// <returns></returns>
        public string[] GetJoinedColumns()
        {
            List<string> cols = new List<string>(_JoinedPairs.Values);
            return cols.ToArray();
        }

        /// <summary>
        /// Gets the primary properties that are to be joined on
        /// </summary>
        /// <returns></returns>
        public string[] GetJoinedProperties()
        {
            List<string> properties = new List<string>(_JoinedPairs.Keys);
            return properties.ToArray();
        }

        private SpatialJoinCardinality _Cardinaltiy;

        /// <summary>
        /// The cardinality of the join results
        /// </summary>
        public SpatialJoinCardinality Cardinality
        {
            get { return _Cardinaltiy; }
            set { _Cardinaltiy = value; }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public enum SpatialJoinCardinality
    {
        OneToOne,
        OneToMany
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public enum SpatialJoinType
    {
        Inner,
        LeftOuter
    }
}
