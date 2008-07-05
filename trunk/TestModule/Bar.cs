using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;

namespace TestModule
{
    public class Bar : ModuleBase
    {
        public override string Name
        {
            get { return "bar"; }
        }

        public override string Description
        {
            get { return "Bar Module"; }
        }

        public override void Initialize()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Cleanup()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        [Command("bar", "bar")]
        public void TestBar()
        {
            AppConsole.Alert("Bar", "Bar");
        }
    }
}
