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

    public class EventArgs<T> : EventArgs
    {
        private readonly T _Data;

        public T Data
        {
            get { return _Data; }
        }

        public EventArgs(T data) { _Data = data; }
    }

    public class MessageEventArgs : EventArgs
    {
        private readonly string _Title;

        public string Title
        {
            get { return _Title; }
        }

        private readonly string _Message;

        public string Message
        {
            get { return _Message; }
        }

        public MessageEventArgs(string title, string message)
        {
            _Title = title;
            _Message = message;
        }
    }

    
}
