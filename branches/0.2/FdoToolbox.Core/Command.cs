using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace FdoToolbox.Core
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

        [DisplayName("Image")]
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
	

        public Command(string name, string display, string description, CommandExecuteHandler execMethod)
        {
            this.Name = name;
            this.DisplayName = display;
            this.Description = description;
            this.OnExecute = execMethod;
        }

        public Command(string name, string display, string description, Image img, CommandExecuteHandler execMethod)
            : this(name, display, description, execMethod)
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

    public delegate void CommandExecuteHandler();
}
