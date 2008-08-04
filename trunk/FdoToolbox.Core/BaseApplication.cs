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
using System.IO;
using System.Reflection;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Base application class
    /// </summary>
    public abstract class BaseApplication
    {
        public BaseApplication()
        {
            InitializePrefs();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            CheckFdoPath();
        }

        //This handler is called only when the common language runtime tries to bind to the assembly and fails.
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string fdoPath = this.Preferences.GetStringPref(PreferenceNames.PREF_STR_FDO_HOME);

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
        }

        private void InitializePrefs()
        {
            PreferenceDictionary dict = new PreferenceDictionary();
            string file = "Preferences.xml";
            if (File.Exists(file))
            {
                dict.LoadPreferences(file);
            }
            else
            {
                InitializeDefaultPrefs(dict);
            }
            _PrefDict = dict;
        }

        private void InitializeDefaultPrefs(PreferenceDictionary dict)
        {
            dict.SetStringPref(PreferenceNames.PREF_STR_WORKING_DIRECTORY, this.AppPath);
            dict.SetStringPref(PreferenceNames.PREF_STR_FDO_HOME, Path.Combine(this.AppPath, "FDO\\"));
            dict.SetBooleanPref(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE, true);
        }

        protected IPreferenceDictionary _PrefDict;

        public IPreferenceDictionary Preferences
        {
            get { return _PrefDict; }
        }

        /// <summary>
        /// Current working directory path of the application
        /// </summary>
        public string AppPath 
        { 
            get 
            { 
                return System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location); 
            } 
        }

        protected abstract void CheckFdoPath();

        protected virtual void Cleanup()
        {
            this.Preferences.Save();
        }
    }
}
