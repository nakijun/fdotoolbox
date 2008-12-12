using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls
{
    public interface IQuerySubView : ISubView
    {
        void FireMapPreviewStateChanged(bool enabled);
        event MapPreviewStateEventHandler MapPreviewStateChanged;
    }

    public delegate void MapPreviewStateEventHandler(object sender, bool enabled);
}
