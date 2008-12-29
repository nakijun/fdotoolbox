using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Utility class to access string resources
    /// </summary>
    public sealed class ResourceUtil
    {
        private static ResourceManager _resMan;

        static ResourceUtil()
        {
            _resMan = Strings.ResourceManager;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal static string GetString(string key)
        {
            return _resMan.GetString(key);
        }

        /// <summary>
        /// Gets the string formatted.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        internal static string GetStringFormatted(string key, params object[] args)
        {
            string str = _resMan.GetString(key);
            return string.Format(str, args);
        }

        /// <summary>
        /// Gets the resource manager for this string bundle
        /// </summary>
        public static ResourceManager StringResourceManager
        {
            get { return Strings.ResourceManager; }
        }
    }
}
