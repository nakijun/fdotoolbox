using System;
using System.Collections.Generic;
using System.Text;

namespace TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            NUnit.ConsoleRunner.Runner.Main(new string[] { "TestLibrary.dll" });
        }
    }
}
