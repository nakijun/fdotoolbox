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
using System.Windows.Forms;

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Manages menu visibility of commands
    /// </summary>
    public class MenuStateMgr : IMenuStateMgr
    {
        internal MenuStateMgr() { _MenuItems = new Dictionary<string, List<ToolStripItem>>(); }

        private Dictionary<string, List<ToolStripItem>> _MenuItems;

        public void EnableCommand(string cmdName, bool enabled)
        {
            if (_MenuItems.ContainsKey(cmdName))
            {
                _MenuItems[cmdName].ForEach(delegate(ToolStripItem item) { item.Enabled = enabled; });
            }
        }

        public void RegisterMenuItem(string cmdName, ToolStripItem menuItem)
        {
            if (!_MenuItems.ContainsKey(cmdName))
                _MenuItems[cmdName] = new List<ToolStripItem>();

            _MenuItems[cmdName].Add(menuItem);
        }
    }
}
