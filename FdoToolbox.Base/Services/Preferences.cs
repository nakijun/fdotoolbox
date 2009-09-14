#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using System.IO;

namespace FdoToolbox.Base
{
    /// <summary>
    /// A strongly-typed preferences class
    /// </summary>
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
        public static readonly string PREF_EXCLUDE_PARTIAL_SCHEMA = "ProvidersExcludePartialSchema";
        public static readonly string PREF_DATA_PREVIEW_RANDOM_COLORS = "DataPreviewRandomColors";

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

        /// <summary>
        /// Gets or sets the list of providers where enhanced IDescribeSchema will not be
        /// used.
        /// </summary>
        /// <value>The exclude partial schema providers.</value>
        public static string[] ExcludePartialSchemaProviders
        {
            get 
            { 
                string str = properties.Get<string>(PREF_EXCLUDE_PARTIAL_SCHEMA, "OSGeo.ODBC");
                return str.Split(';');
            }
            set 
            { 
                properties.Set<string>(PREF_EXCLUDE_PARTIAL_SCHEMA, string.Join(";", value)); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use a randomly generated color theme for map previews.
        /// 
        /// If false, map preview will use a monochromatic theme.
        /// </summary>
        public static bool DataPreviewRandomColors
        {
            get
            {
                return properties.Get<bool>(PREF_DATA_PREVIEW_RANDOM_COLORS, true);
            }
            set
            {
                properties.Set<bool>(PREF_DATA_PREVIEW_RANDOM_COLORS, value);
            }
        }
    }
}
