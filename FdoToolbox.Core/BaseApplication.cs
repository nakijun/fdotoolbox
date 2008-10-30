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
        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseApplication()
        {
#if DEBUG || TEST
            //HACK: It's not picking up the DBMS paths from the PATH environment
            //variable when running in debug mode, so set the paths explicitly here
            string oracle_bin = "C:\\oraclexe\\app\\oracle\\product\\10.2.0\\server\\bin";
            string mysql_bin = "C:\\Program Files\\MySQL\\MySQL Server 5.0\\bin";
            string path = Environment.GetEnvironmentVariable("PATH");
            path = string.Format(
                "{0};{1};{2}",
                path,
                oracle_bin,
                mysql_bin);
            Environment.SetEnvironmentVariable("PATH", path);
#endif
            InitializePrefs();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            CheckFdoPath();
            string logpath = this.Preferences.GetStringPref(PreferenceNames.PREF_STR_LOG_PATH);
            if (!Directory.Exists(logpath))
                Directory.CreateDirectory(logpath);
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
            InitializeDefaultPrefs(dict);
            _PrefDict = dict;
        }

        private void InitializeDefaultPrefs(PreferenceDictionary dict)
        {
            dict.SetDefaultValue(PreferenceNames.PREF_STR_WORKING_DIRECTORY, this.AppPath);
            dict.SetDefaultValue(PreferenceNames.PREF_STR_FDO_HOME, Path.Combine(this.AppPath, "FDO\\"));
            dict.SetDefaultValue(PreferenceNames.PREF_BOOL_TIMESTAMP_CONSOLE, true);
            dict.SetDefaultValue(PreferenceNames.PREF_STR_SESSION_DIRECTORY, Path.Combine(this.AppPath, "Session"));
            dict.SetDefaultValue(PreferenceNames.PREF_INT_WARN_DATASET, 1500);
            dict.SetDefaultValue(PreferenceNames.PREF_STR_LOG_PATH, Path.Combine(this.AppPath, "Logs\\"));
        }

        /// <summary>
        /// The preference dictionary
        /// </summary>
        protected IPreferenceDictionary _PrefDict;

        /// <summary>
        /// The preference dictionary
        /// </summary>
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
                Uri uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                return System.IO.Path.GetDirectoryName(
                    uri.LocalPath);
            } 
        }

        /// <summary>
        /// Performs a check to ensure the FDO libraries can be loaded
        /// </summary>
        protected abstract void CheckFdoPath();

        /// <summary>
        /// Perform pre-shutdown cleanup
        /// </summary>
        protected virtual void Cleanup()
        {
            this.Preferences.Save();
        }
    }
}
