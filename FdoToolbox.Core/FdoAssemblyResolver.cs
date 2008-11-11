using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Helper class to locate FDO assemblies
    /// </summary>
    public sealed class FdoAssemblyResolver
    {
        static string[] assemblies = { "OSGeo.FDO.dll", "OSGeo.FDO.Common.dll", "OSGeo.FDO.Geometry.dll" };

        /// <summary>
        /// Checks if the given path is a valid path containing FDO assemblies
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidFdoPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            foreach (string asm in assemblies)
            {
                string asmPath = Path.Combine(path, asm);
                if (!File.Exists(asmPath))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Pre-loads the FDO libraries from a given path. Call this method only if your FDO libraries are not in
        /// the same path as the assembly containing the consumer of this method.
        /// </summary>
        /// <param name="path"></param>
        //public static void InitializeFdo(string path)
        //{
        //    foreach (string asm in assemblies)
        //    {
        //        Assembly.LoadFrom(Path.Combine(path, asm));
        //    }
        //}

        /// <summary>
        /// Sets the path where FDO assemblies will be loaded.
        /// </summary>
        /// <param name="path"></param>
        public static void InitializeFdo(string path)
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
