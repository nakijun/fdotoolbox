using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;

namespace FdoToolbox.Core
{
    public sealed class ResourceUtil
    {
        private static ResourceManager _resMan;

        static ResourceUtil()
        {
            _resMan = new ResourceManager("Strings", typeof(ResourceUtil).Assembly);
        }

        public static string GetString(string key)
        {
            return _resMan.GetString(key);
        }

        public static string GetStringFormatted(string key, params object[] args)
        {
            string str = _resMan.GetString(key);
            return string.Format(str, args);
        }
    }
}
