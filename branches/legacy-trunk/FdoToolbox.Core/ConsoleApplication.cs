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
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.IO;
using FdoToolbox.Core.Commands;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Base Application class for console applications. Provides basic
    /// services such as argument parsing.
    /// 
    /// In order for argument parsing to work, arguments must be defined 
    /// in the form of [-prefix]:[value] pairs.
    /// </summary>
    public abstract class ConsoleApplication : BaseApplication, IDisposable
    {
        protected IConsoleCommand _Command;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ConsoleApplication() : base()
        {
            AppConsole.Out = new CmdConsoleOutputStream();
            AppConsole.Err = new CmdConsoleErrorStream();
            AppConsole.DoAlert += delegate(MessageEventArgs e)
            {
                AppConsole.WriteLine("{0}: {1}", e.Title, e.Message);
            };
            AppConsole.DoConfirm += delegate(MessageEventArgs e)
            {
                AppConsole.WriteLine("{0}: {1} [y/n]", e.Title, e.Message);
                ConsoleKeyInfo input = Console.ReadKey();
                while (input.Key != ConsoleKey.Y && input.Key != ConsoleKey.N)
                {
                    AppConsole.WriteLine("Unknown response. Try again.");
                    AppConsole.WriteLine("{0}: {1} [y/n]", e.Title, e.Message);
                }
                return input.Key == ConsoleKey.Y;
            };
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        //This handler is called only when the common language runtime tries to bind to the assembly and fails.
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string fdoPath = Path.Combine(this.AppPath, "FDO\\");

            //Retrieve the list of referenced assemblies in an array of AssemblyName.
            Assembly MyAssembly, objExecutingAssemblies;
            string strTempAssmbPath = "";

            objExecutingAssemblies = Assembly.GetExecutingAssembly();
            AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

            //Loop through the array of referenced assembly names.
            foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
            {
                //Check for the assembly names that have raised the "AssemblyResolve" event.
                if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
                {
                    //Build the path of the assembly from where it has to be loaded.				
                    strTempAssmbPath = fdoPath + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                    break;
                }

            }
            //Load the assembly from the specified path. 					
            MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

            //Return the loaded assembly.
            return MyAssembly;
        }

        /// <summary>
        /// Throws an ArgumentException if the given parameter value is empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        protected static void ThrowIfEmpty(string value, string parameter)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Missing required parameter: " + parameter);
        }

        /// <summary>
        /// Parse application-specific arguments.
        /// </summary>
        /// <param name="args">The array of commandline arguments</param>
        public abstract void ParseArguments(string[] args);

        /// <summary>
        /// Display usage information for this application
        /// </summary>
        public abstract void ShowUsage();

        /// <summary>
        /// Run the application
        /// </summary>
        /// <param name="args">The array of commandline arguments</param>
        public virtual void Run(string[] args)
        {
            try
            {
                ParseArguments(args);
            }
            catch (ArgumentException ex)
            {
                AppConsole.Err.WriteLine(ex.Message);
                ShowUsage();
                return;
            }

#if DEBUG
            if (_Command != null)
                AppConsole.WriteLine("Silent: {0}\nTest: {1}", _Command.IsSilent, _Command.IsTestOnly);
#endif

            int retCode = (int)CommandStatus.E_OK;
            if (_Command != null)
            {
                try
                {
                    retCode = _Command.Execute();
                }
                catch (Exception ex)
                {
                    AppConsole.WriteException(ex);
                    retCode = (int)CommandStatus.E_FAIL_UNKNOWN;
                }
            }
#if DEBUG
            AppConsole.WriteLine("Status: {0}", retCode);
#endif
            System.Environment.ExitCode = retCode;
        }

        /// <summary>
        /// Verifies the file name exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The file name.</returns>
        protected string CheckFile(string fileName)
        {
            string file = fileName;
            if (!File.Exists(file))
            {
                if (!Path.IsPathRooted(file))
                    file = Path.Combine(this.AppPath, file);

                if (!File.Exists(file))
                    throw new ArgumentException("Unable to find file: " + fileName);
            }
            return file;
        }

        private bool _IsTest;

        /// <summary>
        /// Is this application being run in simulation mode?
        /// </summary>
        public bool IsTestOnly
        {
            get { return _IsTest; }
            set { _IsTest = value; }
        }

        private bool _IsSilent;

        /// <summary>
        /// Is this application running silent? (no console output)
        /// </summary>
        public bool IsSilent
        {
            get { return _IsSilent; }
            set { _IsSilent = value; }
        }
	

        /// <summary>
        /// Gets an argument value with the given prefix. Arguments follow the
        /// convention [prefix]:[value]
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="args"></param>
        /// <returns>The argument value if found, otherwise null</returns>
        protected static string GetArgument(string prefix, string [] args)
        {
            if (args.Length == 0)
                return null;

            foreach (string arg in args)
            {
                if (arg.StartsWith(prefix))
                {
                    string argument = arg.Substring(arg.IndexOf(":")+1);
                    return argument;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a given parameter switch was defined
        /// </summary>
        /// <param name="strSwitch"></param>
        /// <returns></returns>
        protected static bool IsSwitchDefined(string strSwitch, string [] args)
        {
            if (args.Length == 0)
                return false;

            foreach (string arg in args)
            {
                if (arg == strSwitch || arg.StartsWith(strSwitch))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the default FDO path is valid. If not the user will be asked to 
        /// enter a new FDO path. If this is valid, it will be set as the default FDO path
        /// </summary>
        protected override void CheckFdoPath()
        {
            string fdoPath = this.Preferences.GetStringPref(PreferenceNames.PREF_STR_FDO_HOME);
            if (!Directory.Exists(fdoPath))
            {
                while (!Directory.Exists(fdoPath))
                {
                    Console.Write("Please enter the path where the FDO libaries are located (this is a one-off thing):");
                    fdoPath = Console.ReadLine();
                }
                this.Preferences.SetStringPref(PreferenceNames.PREF_STR_FDO_HOME, fdoPath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Preferences.Save();
            }
        }
    }
}