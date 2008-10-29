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
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Defines the options for converting a database table to a FDO point
    /// feature class.
    /// </summary>
    public class DbToPointCopyOptions
    {
        private DatabaseConnection _Source;
        
        /// <summary>
        /// The source database connection
        /// </summary>
        public DatabaseConnection Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        private FdoConnection _Target;

        /// <summary>
        /// The target FDO connection
        /// </summary>
        public FdoConnection Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
	
        public DbToPointCopyOptions(DatabaseConnection source, FdoConnection target)
        {
            this.Source = source;
            this.Target = target;
            _ColumnList = new List<string>();
        }

        private List<string> _ColumnList;

        /// <summary>
        /// A list of columns to copy from the database table
        /// </summary>
        public ReadOnlyCollection<string> ColumnList
        {
            get { return _ColumnList.AsReadOnly(); }
        }

        /// <summary>
        /// Adds a column to be copied
        /// </summary>
        /// <param name="column"></param>
        public void AddColumn(string column)
        {
            _ColumnList.Add(column);
        }

        /// <summary>
        /// Adds a list of columns to be copied
        /// </summary>
        /// <param name="columns"></param>
        public void AddColumns(IEnumerable<string> columns)
        {
            _ColumnList.AddRange(columns);
        }

        private string _XColumn;

        /// <summary>
        /// The "X" point coordinate column of the database table
        /// </summary>
        public string XColumn
        {
            get { return _XColumn; }
            set { _XColumn = value; }
        }

        private string _YColumn;

        /// <summary>
        /// The "Y" point coordinate column of the database table
        /// </summary>
        public string YColumn
        {
            get { return _YColumn; }
            set { _YColumn = value; }
        }

        private string _ZColumn;

        /// <summary>
        /// The "Z" point coordinate column of the database table
        /// </summary>
        public string ZColumn
        {
            get { return _ZColumn; }
            set { _ZColumn = value; }
        }

        private string _Database;

        /// <summary>
        /// The database to copy from
        /// </summary>
        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        private string _Table;

        /// <summary>
        /// The table to copy from
        /// </summary>
        public string Table
        {
            get { return _Table; }
            set { _Table = value; }
        }

        private string _SchemaName;

        /// <summary>
        /// The FDO feature schema to copy to
        /// </summary>
        public string SchemaName
        {
            get { return _SchemaName; }
            set { _SchemaName = value; }
        }

        private string _ClassName;

        /// <summary>
        /// The FDO feature class to copy to
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }
	
    }
}
