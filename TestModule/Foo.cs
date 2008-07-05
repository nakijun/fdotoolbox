using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;

namespace TestModule
{
    public class Foo : ModuleBase 
    {
        public override string Name
        {
            get { return "foo"; }
        }

        public override string Description
        {
            get { return "Foo Module"; }
        }

        public override void Initialize()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Cleanup()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        [Command("foo", "foo")]
        public void TestFoo()
        {
            AppConsole.Alert("Foo", "Foo");
        }
    }
}
