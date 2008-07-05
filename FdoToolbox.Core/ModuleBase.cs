using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Base Module class. A module is a collection of commands.
    /// Each command is a discrete unit of functionality
    /// 
    /// Commands are associated to module methods by applying
    /// the [Command] attribute to the desired methods.
    /// 
    /// Care must be taken with command names. A ModuleLoadException
    /// will be thrown when attempting to load a module with a command
    /// whose name already exists in the global namespace.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Initialize();
        public abstract void Cleanup();
        protected Dictionary<string, Command> _CommandDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModuleBase() 
        { 
            _CommandDictionary = new Dictionary<string, Command>(); 
            
            //Find all methods with [Command] attribute applied
            Type t = this.GetType();
            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo method in methods)
            {
                CommandAttribute [] cmdAttrs = (CommandAttribute[])method.GetCustomAttributes(typeof(CommandAttribute), true);
                if (cmdAttrs != null && cmdAttrs.Length == 1)
                {
                    CommandAttribute attr = cmdAttrs[0];
                    Command cmd = new Command(
                        attr.Name,
                        attr.DisplayName,
                        attr.Description,
                        (CommandExecuteHandler)Delegate.CreateDelegate(typeof(CommandExecuteHandler), this, method)
                    );
                    cmd.ShortcutKeys = attr.ShortcutKeys;
                    AddCommand(cmd);
                }
            }
        }

        public ICollection<string> CommandNames
        {
            get { return _CommandDictionary.Keys; }
        }

        public Command GetCommand(string name)
        {
            if (_CommandDictionary.ContainsKey(name))
                return _CommandDictionary[name];
            return null;
        }

        /// <summary>
        /// Register a module command
        /// </summary>
        /// <param name="cmd">The command object to register</param>
        /// <exception cref="ModuleLoadException">
        /// thrown if this command name already exists
        /// </exception>
        public void AddCommand(Command cmd)
        {
            if (_CommandDictionary.ContainsKey(cmd.Name))
                throw new ModuleLoadException("A command named " + cmd.Name + " already exists in this module");
            _CommandDictionary.Add(cmd.Name, cmd);
        }

        public IList<Command> Commands
        {
            get { return new List<Command>(_CommandDictionary.Values); }
        }
    }
}
