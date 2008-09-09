using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FdoToolbox.Lib.Modules;
using FdoToolbox.Core;

namespace FdoToolbox.Lib
{
    public delegate void ConsoleInputHandler(object sender, EventArgs<string> e);
    public delegate void CommandExecuteHandler();
    public delegate void ConnectionEventHandler(object sender, EventArgs<string> e);
    public delegate void ConnectionRenamedEventHandler(object sender, ConnectionRenameEventArgs e);
    public delegate void ConnectionBeforeRemoveHandler(object sender, ConnectionBeforeRenameEventArgs e);
    public delegate void TabTitleEventHandler(object sender, EventArgs<string> e);
    public delegate void ModuleEventHandler(object sender, EventArgs<IModule> module);

    public class ConnectionRenameEventArgs : EventArgs
    {
        private readonly string _OldName;

        public string OldName
        {
            get { return _OldName; }
        }

        private readonly string _NewName;

        public string NewName
        {
            get { return _NewName; }
        }

        public ConnectionRenameEventArgs(string oldName, string newName)
        {
            _OldName = oldName;
            _NewName = NewName;
        }
    }

    public class ConnectionBeforeRenameEventArgs : CancelEventArgs
    {
        private readonly string _ConnectionName;

        public string ConnectionName
        {
            get { return _ConnectionName; }
        }

        public ConnectionBeforeRenameEventArgs(string name)
        {
            _ConnectionName = name;
            this.Cancel = false;
        }
    }
}
