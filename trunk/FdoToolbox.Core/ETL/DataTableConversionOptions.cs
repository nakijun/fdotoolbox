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
using System.Data;

namespace FdoToolbox.Core.ETL
{
    public class DataTableConversionOptions
    {
        private DataTable _Table;

        /// <summary>
        /// The source DataTable
        /// </summary>
        public DataTable Table
        {
            get { return _Table; }
            set { _Table = value; }
        }

        private string _SchemaName;

        /// <summary>
        /// The schema to copy to
        /// </summary>
        public string SchemaName
        {
            get { return _SchemaName; }
            set { _SchemaName = value; }
        }

        private string _ClassName;

        /// <summary>
        /// The name of the feature class to be created and copied to
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        private string _Provider;

        /// <summary>
        /// The FDO provider name. Must be a flat-file provider
        /// </summary>
        public string FdoProvider
        {
            get { return _Provider; }
            set { _Provider = value; }
        }

        private string _File;

        /// <summary>
        /// The file to copy the DataTable to
        /// </summary>
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }

        private bool _UseFdoMetaData;

        /// <summary>
        /// If true, will create the corresponding class definition assuming columns in the
        /// given DataTable are tagged with the proper FDO metadata. Otherwise, the 
        /// properties of the DataTable columns will be used to create the class definition, also
        /// the resulting class will be a non-feature class. If false, the target provider must
        /// support non-feature classes otherwise an exception will be thrown during task validation.
        /// </summary>
        public bool UseFdoMetaData
        {
            get { return _UseFdoMetaData; }
            set { _UseFdoMetaData = value; }
        }

        public DataTableConversionOptions(DataTable table, string provider, string file) 
        {
            this.Table = table;
            this.FdoProvider = provider;
            this.File = file;
            this.UseFdoMetaData = true;
        } 
    }
}
