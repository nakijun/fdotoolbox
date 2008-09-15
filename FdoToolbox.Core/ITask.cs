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
using System.Threading;

namespace FdoToolbox.Core
{
    public interface ITask
    {
        /// <summary>
        /// The name of the task
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Validate the parameters for this task. Task execution
        /// only proceeds when the parameters are valid
        /// </summary>
        void ValidateTaskParameters();
        /// <summary>
        /// Execute the task
        /// </summary>
        void Execute();
        /// <summary>
        /// The type of task
        /// </summary>
        TaskType TaskType { get; }
        /// <summary>
        /// Determines if this task is countable (for measuring progress)
        /// </summary>
        bool IsCountable { get; }
        /// <summary>
        /// The thread that this task is executing on. Aborting it will cause
        /// a ThreadAbortException to be thrown on the thread executing the task.
        /// </summary>
        Thread ExecutingThread { get; }

        event TaskPercentageEventHandler OnItemProcessed;
        event TaskProgressMessageEventHandler OnTaskMessage;
        event TaskProgressMessageEventHandler OnLogTaskMessage;
    }

    public enum TaskType
    {
        /// <summary>
        /// A dummy task
        /// </summary>
        Dummy,
        /// <summary>
        /// A Spatial bulk copy task
        /// </summary>
        SpatialBulkCopy,
        /// <summary>
        /// A database join task
        /// </summary>
        DbJoin,
        /// <summary>
        /// A non-spatial -> spatial bulk copy task
        /// </summary>
        NonSpatialToSpatialBulkCopy,
        /// <summary>
        /// A spatial -> non-spatial bulk copy task
        /// </summary>
        SpatialToNonSpatialBulkCopy,
        /// <summary>
        /// An ado.net datatable -> feature class copy task
        /// </summary>
        DataTableToFeatureClass
    }
}
