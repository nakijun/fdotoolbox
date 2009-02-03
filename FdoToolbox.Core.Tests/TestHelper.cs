using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace FdoToolbox.Core.Tests
{
    public sealed class TestHelper
    {
        private static string _asmPath;

        public static string CurrentPath
        {
            get
            {
                if (_asmPath == null)
                {
                    UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
                    _asmPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
                }
                return _asmPath;
            }
        }
    }
}
