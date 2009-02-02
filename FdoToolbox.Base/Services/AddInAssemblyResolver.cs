#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox-addins, jumpinjackie@gmail.com
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
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace FdoToolbox.Base.Services
{
    /// <summary>
    /// This is a helper class that aids in resolving assemblies that are outside
    /// of the application's directory or the Global Assembly Cache
    /// </summary>
    public class AddInAssemblyResolver
    {
        /// <summary>
        /// Registers the libraries. Use this method if your add-in references additional
        /// libraries that are not part of the FDO Toolbox installation.
        /// 
        /// Failure to do this will result in FileLoadExceptions when attempting to load
        /// a referenced assembly that is not part of the FDO Toolbox installation.
        /// </summary>
        /// <param name="path">The path where the additional assemblies are located</param>
        /// <param name="assemblies">The assemblies to register</param>
        public static void RegisterLibraries(string path, params string[] assemblies)
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                string fdoPath = path;

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
                        strTempAssmbPath = Path.Combine(fdoPath, args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll");
                        break;
                    }

                }
                //Load the assembly from the specified path. 					
                MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

                //Return the loaded assembly.
                return MyAssembly;
            };
        }
    }
}
