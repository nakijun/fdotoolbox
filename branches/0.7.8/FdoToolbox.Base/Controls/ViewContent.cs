#region LGPL Header
// Copyright (C) 2009, Jackie Ng
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
//
// See license.txt for more/additional licensing information
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A base view class that provides common functionality for all view content
    /// </summary>
    public partial class ViewContent : UserControl, IViewContent
    {
        private IFdoConnectionManager connMgr;

        public ViewContent()
        {
            InitializeComponent();
            connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            if (connMgr != null) //Will be null in design mode. this.DesignMode is lying to me!?
            {
                this.Disposed += new EventHandler(OnDisposed);
                connMgr.BeforeConnectionRemove += new ConnectionBeforeRemoveHandler(OnBeforeConnectionRemove);
                connMgr.ConnectionAdded += new ConnectionEventHandler(OnConnectionAdded);
                connMgr.ConnectionRefreshed += new ConnectionEventHandler(OnConnectionRefreshed);
                connMgr.ConnectionRemoved += new ConnectionEventHandler(OnConnectionRemoved);
                connMgr.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
            }
        }

        protected virtual void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e) { }

        protected virtual void OnConnectionRemoved(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnConnectionRefreshed(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnConnectionAdded(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnBeforeConnectionRemove(object sender, ConnectionBeforeRemoveEventArgs e) { }

        void OnDisposed(object sender, EventArgs e)
        {
            connMgr.BeforeConnectionRemove -= OnBeforeConnectionRemove;
            connMgr.ConnectionAdded -= OnConnectionAdded;
            connMgr.ConnectionRefreshed -= OnConnectionRefreshed;
            connMgr.ConnectionRemoved -= OnConnectionRemoved;
            connMgr.ConnectionRenamed -= OnConnectionRenamed;
        }

        public virtual bool CanClose
        {
            get { return true; }
        }

        public void Close()
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        public event EventHandler ViewContentClosing;

        private string _title;

        public virtual string Title
        {
            get { return _title; }
            set { _title = value; TitleChanged(this, EventArgs.Empty); }
        }

        public event EventHandler TitleChanged = delegate { };

        public Control ContentControl
        {
            get { return this; }
        }

        public void ShowError(Exception ex)
        {
            MessageService.ShowError(ex);
        }

        public void ShowError(string message)
        {
            MessageService.ShowError(message);
        }

        public void ShowMessage(string title, string message)
        {
            if (!string.IsNullOrEmpty(title))
                MessageService.ShowMessage(message, title);
            else
                MessageService.ShowMessage(message);
        }

        public bool Confirm(string title, string message)
        {
            if (!string.IsNullOrEmpty(title))
                return MessageService.AskQuestion(message, title);
            else
                return MessageService.AskQuestion(message);
        }

        public bool ConfirmFormatted(string title, string format, params string[] args)
        {
            if (!string.IsNullOrEmpty(title))
                return MessageService.AskQuestionFormatted(title, format, args);
            else
                return MessageService.AskQuestionFormatted(format, args);
        }
    }
}
