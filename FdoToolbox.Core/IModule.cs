using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Extension Module Interface
    /// </summary>
    public interface IModule
    {
        string Name { get; }
        string Description { get; }

        ICollection<string> CommandNames { get; }
        IList<Command> Commands { get; }
        Command GetCommand(string name);

        void Initialize();
        void Cleanup();
    }
}
