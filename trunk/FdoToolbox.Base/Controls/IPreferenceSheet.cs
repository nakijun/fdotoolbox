using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Base.Controls
{
    public interface IPreferenceSheet
    {
        string Title { get; }
        Control ContentControl { get; }
        void ApplyChanges();
    }
}
