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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core.Commands
{
    public class Command
    {
        private string _Name;

        //[Description("The name of the command. Must be unique in the global namespace"]
        [DisplayName("Command Name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _DisplayName;

        [DisplayName("Display Name")]
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
	

        private string _Description;

        [DisplayName("Description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private Image _CmdImage;

        [DisplayName("Icon")]
        public Image CommandImage
        {
            get { return _CmdImage; }
            set { _CmdImage = value; }
        }

        private Keys _ShortcutKeys;

        [DisplayName("Shortcut Keys")]
        public Keys ShortcutKeys
        {
            get { return _ShortcutKeys; }
            set { _ShortcutKeys = value; }
        }

        private CommandInvocationType _InvocationType;

        [DisplayName("Invocation Mode")]
        public CommandInvocationType InvocationType
        {
            get { return _InvocationType; }
            set { _InvocationType = value; }
        }

        private string _ModuleName;

        [DisplayName("Parent Module")]
        public string ModuleName
        {
            get { return _ModuleName; }
        }
	
        public Command(string name, string display, string description, CommandExecuteHandler execMethod, string parentModuleName)
        {
            this.InvocationType = CommandInvocationType.All;
            this.Name = name;
            this.DisplayName = display;
            this.Description = description;
            this.OnExecute = execMethod;
            _ModuleName = parentModuleName;
        }

        public Command(string name, string display, string description, Image img, CommandExecuteHandler execMethod, string parentModuleName)
            : this(name, display, description, execMethod, parentModuleName)
        {
            this.CommandImage = img;
        }

        private CommandExecuteHandler OnExecute;

        public void Execute()
        {
            if (this.OnExecute != null)
            {
                AppConsole.WriteLine(">>> {0}", this.Name);
                this.OnExecute();
            }
        }
    }

    public enum CommandInvocationType : int
    {
        Console = 1,
        UI = 2,
        All = 3
    }
}
