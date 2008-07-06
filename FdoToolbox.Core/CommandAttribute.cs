using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Defines a Command that will invoke the attributed
    /// method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _DisplayName;

        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
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

        private CommandInvocationType _InvcationType;

        public CommandInvocationType InvocationType
        {
            get { return _InvcationType; }
            set { _InvcationType = value; }
        }
	

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the command</param>
        public CommandAttribute(string name, string displayName)
        {
            this.InvocationType = CommandInvocationType.All;
            this.Name = name;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="displayName">The display name of the command (Shown in menus)</param>
        public CommandAttribute(string name, string displayName, string description)
        {
            this.InvocationType = CommandInvocationType.All;
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
        }
    }
}
