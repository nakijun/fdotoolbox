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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.Commands
{
    /// <summary>
    /// Defines a Command that will invoke the attributed
    /// method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandAttribute : Attribute
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
        }

        private string _DisplayName;

        public string DisplayName
        {
            get { return _DisplayName; }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
       
        private Keys _ShortcutKeys;

        public Keys ShortcutKeys
        {
            get { return _ShortcutKeys; }
            set { _ShortcutKeys = value; }
        }

        private string _ImageResourceName;

        /// <summary>
        /// The name of the image resource to associate with this command
        /// </summary>
        public string ImageResourceName
        {
            get { return _ImageResourceName; }
            set { _ImageResourceName = value; }
        }

        private CommandInvocationType _InvocationType;

        public CommandInvocationType InvocationType
        {
            get { return _InvocationType; }
            set { _InvocationType = value; }
        }
	

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the command</param>
        public CommandAttribute(string name, string displayName)
        {
            _InvocationType = CommandInvocationType.All;
            _Name = name;
            _DisplayName = displayName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="displayName">The display name of the command (Shown in menus)</param>
        public CommandAttribute(string name, string displayName, string description)
        {
            _InvocationType = CommandInvocationType.All;
            _Name = name;
            _DisplayName = displayName;
            _Description = description;
        }
    }
}
