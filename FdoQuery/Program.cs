using System;
using System.Collections.Generic;
using System.Text;

namespace FdoQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FdoQueryApp app = new FdoQueryApp())
            {
                app.Run(args);
            }
        }
    }
}
