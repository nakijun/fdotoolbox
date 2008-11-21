using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using System.IO;

namespace FdoUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FdoUtilApp app = new FdoUtilApp())
            {
                string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(dir, "FDO");
                //Console.WriteLine("Setting FDO Path: {0}", path);
                FdoAssemblyResolver.InitializeFdo(path);
                app.Run(args);
            }
        }
    }
}
