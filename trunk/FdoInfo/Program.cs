using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using System.IO;

namespace FdoInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FdoInfoApp app = new FdoInfoApp())
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
