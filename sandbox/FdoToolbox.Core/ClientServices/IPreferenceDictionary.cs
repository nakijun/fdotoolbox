#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ClientServices
{
    /// <summary>
    /// Preference dictionary interface
    /// </summary>
    public interface IPreferenceDictionary
    {
        /// <summary>
        /// Sets a double preference value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetDoublePref(string name, double value);

        /// <summary>
        /// Sets an integer preference value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetIntegerPref(string name, int value);

        /// <summary>
        /// Sets a boolean preference value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetBooleanPref(string name, bool value);

        /// <summary>
        /// Sets a string preference value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetStringPref(string name, string value);

        /// <summary>
        /// Gets a double preference value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetDoublePref(string name);

        /// <summary>
        /// Gets an integer preference value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetIntegerPref(string name);

        /// <summary>
        /// Gets a boolean preference value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetBooleanPref(string name);

        /// <summary>
        /// Gets a string preference value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetStringPref(string name);

        /// <summary>
        /// The names of all double preference values
        /// </summary>
        ICollection<string> DoublePreferences { get; }

        /// <summary>
        /// The names of all integer preference values
        /// </summary>
        ICollection<string> IntegerPreferences { get; }

        /// <summary>
        /// The names of all boolean preference values
        /// </summary>
        ICollection<string> BooleanPreferences { get; }

        /// <summary>
        /// The names of all string preference values
        /// </summary>
        ICollection<string> StringPreferences { get; }

        /// <summary>
        /// Sets the default value of a string preference
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetDefaultValue(string name, string value);
        
        /// <summary>
        /// Sets the default value of an integer preference
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetDefaultValue(string name, int value);

        /// <summary>
        /// Sets the default value of a double preference
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetDefaultValue(string name, double value);

        /// <summary>
        /// Sets the default value of a boolean preference
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetDefaultValue(string name, bool value);

        /// <summary>
        /// Persists the preferences
        /// </summary>
        void Save();
    }

}
