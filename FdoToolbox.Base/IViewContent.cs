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
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Base
{
    /// <summary>
    /// Abstract view interface. Can only be closed externally.
    /// </summary>
    public interface IViewContent : ISubView
    {
        /// <summary>
        /// The title of the view
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Fires when the title has been changed
        /// </summary>
        event EventHandler TitleChanged;
        /// <summary>
        /// Detrmines if this view can be closed
        /// </summary>
        bool CanClose { get; }
        /// <summary>
        /// Closes the view. This is meant to be invoked externally.
        /// </summary>
        /// <returns></returns>
        bool Close();
        /// <summary>
        /// Saves the view's content
        /// </summary>
        /// <returns></returns>
        bool Save();
        /// <summary>
        /// Saves the view's content
        /// </summary>
        /// <returns></returns>
        bool SaveAs();
        /// <summary>
        /// Fired when the view has been closed internally
        /// </summary>
        event EventHandler ViewContentClosing;
    }

    public enum ViewRegion
    {
        Left,
        Right,
        Bottom,
        Top,
        Document,
        Floating,
        Dialog
    }
}
