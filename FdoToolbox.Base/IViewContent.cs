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
