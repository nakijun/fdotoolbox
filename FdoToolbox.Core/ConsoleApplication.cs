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

namespace FdoToolbox.Core
{
    public abstract class ConsoleApplication
    {
        public ConsoleApplication()
        {
            AppConsole.Out = new CmdConsoleOutputStream();
            AppConsole.Err = new CmdConsoleErrorStream();
            AppConsole.DoAlert += delegate(string title, string message)
            {
                AppConsole.WriteLine("{0}: {1}", title, message);
            };
            AppConsole.DoConfirm += delegate(string title, string message)
            {
                AppConsole.WriteLine("{0}: {1} [y/n]", title, message);
                ConsoleKeyInfo input = Console.ReadKey();
                while (input.Key != ConsoleKey.Y && input.Key != ConsoleKey.N)
                {
                    AppConsole.WriteLine("Unknown response. Try again.");
                    AppConsole.WriteLine("{0}: {1} [y/n]", title, message);
                }
                return input.Key == ConsoleKey.Y;
            };
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        public string AppPath { get { return Path.GetDirectoryName(Application.ExecutablePath); } }

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

        public abstract void ParseArguments(string[] args, int minArguments, int maxArguments);

        public abstract void ShowUsage();

        public abstract void Run(string[] args);

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

        /// <summary>
        /// Gets an argument value with the given prefix. Arguments follow the
        /// convention [prefix]:[value]
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="args"></param>
        /// <returns>The argument value if found, otherwise null</returns>
        protected string GetArgument(string prefix, string [] args)
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
    }
}
