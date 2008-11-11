using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using System.IO;

namespace FdoToolbox.Base
{
    public static class Preferences
    {
        /// <summary>
        /// Group Name
        /// </summary>
        public static readonly string GroupName = "Base.AddIn.Options";

        

        public static readonly string PREF_FDO_PATH = "FdoPath";
        public static readonly string PREF_WORKING_DIR = "WorkingDirectory";
        public static readonly string PREF_WARN_DATASET = "DataPreviewWarnLimit";
        public static readonly string PREF_SESSION_DIR = "SessionDirectory";
        public static readonly string PREF_LOG_PATH = "LogPath";

        static Properties properties;

        static Preferences()
        {
            properties = PropertyService.Get(GroupName, new Properties());
        }

        static Properties Properties
        {
            get { return properties; }
        }

        public static event PropertyChangedEventHandler PropertyChanged
        {
            add { properties.PropertyChanged += value; }
            remove { properties.PropertyChanged -= value; }
        }

        /// <summary>
        /// The path to the FDO assemblies
        /// </summary>
        public static string FdoPath
        {
            get { return properties.Get<string>(PREF_FDO_PATH, Path.Combine(FileUtility.ApplicationRootPath, "FDO")); }
            set { properties.Set(PREF_FDO_PATH, value); }
        }

        /// <summary>
        /// The working directory
        /// </summary>
        public static string WorkingDirectory
        {
            get { return properties.Get<string>(PREF_WORKING_DIR, FileUtility.ApplicationRootPath); }
            set { properties.Set(PREF_WORKING_DIR, value); }
        }

        /// <summary>
        /// The path where logs will be written
        /// </summary>
        public static string LogPath
        {
            get { return properties.Get<string>(PREF_LOG_PATH, Path.Combine(FileUtility.ApplicationRootPath, "Logs")); }
            set { properties.Set(PREF_LOG_PATH, value); }
        }

        /// <summary>
        /// The path where the session data will be persisted to and loaded from
        /// </summary>
        public static string SessionDirectory
        {
            get { return properties.Get<string>(PREF_SESSION_DIR, Path.Combine(FileUtility.ApplicationRootPath, "Session")); }
            set { properties.Set(PREF_SESSION_DIR, value); }
        }

        /// <summary>
        /// Determines the limit at which the Data Preview will warn you about large result sets. 
        /// </summary>
        public static int DataPreviewWarningLimit
        {
            get { return properties.Get<int>(PREF_WARN_DATASET, 1500); }
            set { properties.Set<int>(PREF_WARN_DATASET, value); }
        }
    }
}
