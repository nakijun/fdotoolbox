using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FdoToolbox.Base
{
    public interface ISubView
    {
        /// <summary>
        /// The underlying control
        /// </summary>
        Control ContentControl { get; }
    }
}
