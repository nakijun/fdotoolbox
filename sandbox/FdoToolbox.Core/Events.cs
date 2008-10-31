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
using System.ComponentModel;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Core
{
    public delegate void FdoConnectionLoadedEventHandler(EventArgs<FdoConnection> e);
    public delegate void DbConnectionLoadedEventHandler(EventArgs<DatabaseConnection> e);
    public delegate void RemoveFdoConnectionEventHandler(EventArgs<string> e);
    public delegate void RemoveDbConnectionEventHandler(EventArgs<string> e);
    public delegate void AlertHandler(MessageEventArgs e);
    public delegate bool ConfirmHandler(MessageEventArgs e);
    public delegate void TaskPercentageEventHandler(object sender, EventArgs<int> e);
    public delegate void TaskProgressMessageEventHandler(object sender, EventArgs<string> e);
    public delegate void TaskEventHandler(object sender, EventArgs<string> e);
    public delegate string UniqueNameDelegate();
    public delegate void FeatureErrorEventHandler(object sender, FeatureErrorEventArgs e);

    /// <summary>
    /// Feature error event object
    /// </summary>
    public class FeatureErrorEventArgs : EventArgs
    {
        private readonly FdoFeature _feat;
        private readonly Exception _ex;

        /// <summary>
        /// The feature that caused the error
        /// </summary>
        public FdoFeature Feature
        {
            get { return _feat; }
        }

        /// <summary>
        /// The error object
        /// </summary>
        public Exception Exception
        {
            get { return _ex; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="feat"></param>
        /// <param name="ex"></param>
        public FeatureErrorEventArgs(FdoFeature feat, Exception ex)
        {
            _feat = feat;
            _ex = ex;
        }
    }

    /// <summary>
    /// Generic event object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        private readonly T _Data;

        /// <summary>
        /// The event data
        /// </summary>
        public T Data
        {
            get { return _Data; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public EventArgs(T data) { _Data = data; }
    }

    /// <summary>
    /// Message event object
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        private readonly string _Title;

        /// <summary>
        /// The title of the message
        /// </summary>
        public string Title
        {
            get { return _Title; }
        }

        private readonly string _Message;

        /// <summary>
        /// The message content
        /// </summary>
        public string Message
        {
            get { return _Message; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public MessageEventArgs(string title, string message)
        {
            _Title = title;
            _Message = message;
        }
    }

    
}
