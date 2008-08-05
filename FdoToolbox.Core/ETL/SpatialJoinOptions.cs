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
    public class SpatialJoinOptions
    {
        private SpatialConnectionInfo _Target;

        public SpatialConnectionInfo Target
        {
            get { return _Target; }
        }

        private SpatialConnectionInfo _PrimarySource;

        public SpatialConnectionInfo PrimarySource
        {
            get { return _PrimarySource; }
        }

        private DbConnectionInfo _SecondarySource;

        public DbConnectionInfo SecondarySource
        {
            get { return _SecondarySource; }
        }

        private SpatialJoinType _JoinType;

        public SpatialJoinType JoinType
        {
            get { return _JoinType; }
            set { _JoinType = value; }
        }

        private string _TargetSchema;

        public string TargetSchema
        {
            get { return _TargetSchema; }
        }

        private string _TargetClassName;

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

        public string PrimaryPrefix
        {
            get { return _PrimaryPrefix; }
            set { _PrimaryPrefix = value; }
        }

        private string _SecondaryPrefix;

        public string SecondaryPrefix
        {
            get { return _SecondaryPrefix; }
            set { _SecondaryPrefix = value; }
        }

        public void SetPrimary(SpatialConnectionInfo connInfo, string schemaName, string className)
        {
            _PrimarySource = connInfo;
            _SchemaName = schemaName;
            _ClassName = className;
        }

        public void SetSecondary(DbConnectionInfo connInfo, string tableName)
        {
            _SecondarySource = connInfo;
            _TableName = tableName;
        }

        public void SetTarget(SpatialConnectionInfo connInfo, string schemaName, string className)
        {
            _Target = connInfo;
            _TargetSchema = schemaName;
            _TargetClassName = className;
        }

        private List<string> _PropertyList;

        private List<string> _ColumnList;

        private Dictionary<string, string> _JoinedPairs;

        private string _SchemaName;

        public string SchemaName
        {
            get { return _SchemaName; }
        }

        private string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
        }

        private string _TableName;

        public string TableName
        {
            get { return _TableName; }
        }

        public string GetMatchingColumn(string propertyName)
        {
            return _JoinedPairs[propertyName];
        }

        public void AddJoinPair(string propertyName, string column)
        {
            _JoinedPairs[propertyName] = column;
        }

        public void AddColumn(string columnName)
        {
            _ColumnList.Add(columnName);
        }

        public void AddProperty(string propertyName)
        {
            _PropertyList.Add(propertyName);
        }

        public string[] GetPropertyNames()
        {
            return _PropertyList.ToArray();
        }

        public string[] GetColumnNames()
        {
            return _ColumnList.ToArray();
        }

        public string[] GetJoinedColumns()
        {
            List<string> cols = new List<string>(_JoinedPairs.Values);
            return cols.ToArray();
        }

        public string[] GetJoinedProperties()
        {
            List<string> properties = new List<string>(_JoinedPairs.Keys);
            return properties.ToArray();
        }

        private SpatialJoinCardinality _Cardinaltiy;

        public SpatialJoinCardinality Cardinality
        {
            get { return _Cardinaltiy; }
            set { _Cardinaltiy = value; }
        }
    }

    public enum SpatialJoinCardinality
    {
        OneToOne,
        OneToMany
    }

    public enum SpatialJoinType
    {
        Inner,
        LeftOuter
    }
}
