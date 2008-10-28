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
    /// <summary>
    /// FDO feature filter interface to join features with a database source. Each feature read
    /// from the input pipe is merged with the defined data source (in the form of a DataTable).
    /// The merged result is written to the output pipe.
    /// </summary>
    public interface IFdoDatabaseJoinFilter : IFdoOperation
    {
        /// <summary>
        /// The data source to merge with
        /// </summary>
        DataTable DataSource { get; set; }

        /// <summary>
        /// Gets or sets the method of joing 
        /// </summary>
        JoinType JoinType { get; set; }

        /// <summary>
        /// Gets or sets the prefix that is applied to the names of all the
        /// database columns before merging
        /// </summary>
        string DataPrefix { get; set; }
    }

    /// <summary>
    /// Defines how to merge features and rows
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// Only matching feature/row pairs are written to the output pipe
        /// </summary>
        Inner,
        /// <summary>
        /// Features and any matching rows are written to the output pipe, non-matching
        /// rows are omitted from the properties written.
        /// </summary>
        LeftOuter,
        /// <summary>
        /// Rows and any matching features are written to the output pipe, non-matching
        /// features are omitted from the properties written.
        /// </summary>
        RightOuter
    }
}
