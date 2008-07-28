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
using System.Xml;
using System.IO;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Preference dictionary interface
    /// </summary>
    public interface IPreferenceDictionary
    {
        void SetDoublePref(string name, double value);
        void SetIntegerPref(string name, int value);
        void SetBooleanPref(string name, bool value);
        void SetStringPref(string name, string value);

        double? GetDoublePref(string name);
        int? GetIntegerPref(string name);
        bool? GetBooleanPref(string name);
        string GetStringPref(string name);

        List<string> GetDoublePrefNames();
        List<string> GetIntegerPrefNames();
        List<string> GetBooleanPrefNames();
        List<string> GetStringPrefNames();

        void Save();
    }

    public class PreferenceDictionary : IPreferenceDictionary
    {
        private Dictionary<string, bool> _BooleanPrefs;
        private Dictionary<string, string> _StringPrefs;
        private Dictionary<string, int> _IntegerPrefs;
        private Dictionary<string, double> _DoublePrefs;

        public PreferenceDictionary()
        {
            _BooleanPrefs = new Dictionary<string, bool>();
            _IntegerPrefs = new Dictionary<string, int>();
            _StringPrefs = new Dictionary<string, string>();
            _DoublePrefs = new Dictionary<string, double>();
        }

        public void LoadPreferences(string prefsFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(prefsFile);

            XmlNode stringPrefsNode = doc.SelectSingleNode("//Preferences/StringPrefs");
            XmlNode doublePrefsNode = doc.SelectSingleNode("//Preferences/DoublePrefs");
            XmlNode integerPrefsNode = doc.SelectSingleNode("//Preferences/IntegerPrefs");
            XmlNode booleanPrefsNode = doc.SelectSingleNode("//Preferences/BooleanPrefs");

            if (stringPrefsNode != null)
            {
                foreach (XmlNode prefNode in stringPrefsNode.ChildNodes)
                {
                    try
                    {
                        string name = prefNode.Attributes["name"].Value;
                        string value = prefNode.Attributes["value"].Value;
                        SetStringPref(name, value);
                    }
                    catch (Exception ex)
                    {
                        AppConsole.WriteLine("Configuration error: {0}. Skipping preference", ex.Message);
                    }
                }
            }

            if (doublePrefsNode != null)
            {
                foreach (XmlNode prefNode in doublePrefsNode.ChildNodes)
                {
                    try
                    {
                        string name = prefNode.Attributes["name"].Value;
                        double value = Convert.ToDouble(prefNode.Attributes["value"].Value);
                        SetDoublePref(name, value);
                    }
                    catch (Exception ex)
                    {
                        AppConsole.WriteLine("Configuration error: {0}. Skipping preference", ex.Message);
                    }
                }
            }

            if (integerPrefsNode != null)
            {
                foreach (XmlNode prefNode in integerPrefsNode.ChildNodes)
                {
                    try 
                    {
                        string name = prefNode.Attributes["name"].Value;
                        int value = Convert.ToInt32(prefNode.Attributes["value"].Value);
                        SetIntegerPref(name, value);
                    }
                    catch (Exception ex)
                    {
                        AppConsole.WriteLine("Configuration error: {0}. Skipping preference", ex.Message);
                    }
                }
            }

            if (booleanPrefsNode != null)
            {
                foreach (XmlNode prefNode in booleanPrefsNode.ChildNodes)
                {
                    try
                    {
                        string name = prefNode.Attributes["name"].Value;
                        bool value = Convert.ToBoolean(prefNode.Attributes["value"].Value);
                        SetBooleanPref(name, value);
                    }
                    catch (Exception ex)
                    {
                        AppConsole.WriteLine("Configuration error: {0}. Skipping preference", ex.Message);
                    }
                }
            }
        }

        public void SetStringPref(string name, string pref)
        {
            _StringPrefs[name] = pref;
        }

        public void SetDoublePref(string name, double pref)
        {
            _DoublePrefs[name] = pref;
        }

        public void SetIntegerPref(string name, int pref)
        {
            _IntegerPrefs[name] = pref;
        }

        public void SetBooleanPref(string name, bool pref)
        {
            _BooleanPrefs[name] = pref;
        }

        public bool? GetBooleanPref(string name)
        {
            if (_BooleanPrefs.ContainsKey(name))
                return _BooleanPrefs[name];
            return null;
        }

        public int? GetIntegerPref(string name)
        {
            if (_IntegerPrefs.ContainsKey(name))
                return _IntegerPrefs[name];
            return null;
        }

        public double? GetDoublePref(string name)
        {
            if (_DoublePrefs.ContainsKey(name))
                return _DoublePrefs[name];
            return null;
        }

        public string GetStringPref(string name)
        {
            if (_StringPrefs.ContainsKey(name))
                return _StringPrefs[name];
            return null;
        }

        public List<string> GetDoublePrefNames()
        {
            return new List<string>(_DoublePrefs.Keys);
        }

        public List<string> GetIntegerPrefNames()
        {
            return new List<string>(_IntegerPrefs.Keys);
        }

        public List<string> GetBooleanPrefNames()
        {
            return new List<string>(_BooleanPrefs.Keys);
        }

        public List<string> GetStringPrefNames()
        {
            return new List<string>(_StringPrefs.Keys);
        }

        public void Save()
        {
            AppConsole.WriteLine("Saving Preferences");
            string prefXml = Properties.Resources.Preferences;
            StringBuilder doublePref = new StringBuilder();
            StringBuilder integerPref = new StringBuilder();
            StringBuilder stringPref = new StringBuilder();
            StringBuilder booleanPref = new StringBuilder();
            List<string> doubleNames = this.GetDoublePrefNames();
            foreach (string pref in doubleNames)
            {
                double? value = this.GetDoublePref(pref);
                if(value.HasValue)
                    doublePref.Append(string.Format("<Pref name=\"{0}\" value=\"{1}\" />\n", pref, value.Value));
            }
            List<string> integerNames = this.GetIntegerPrefNames();
            foreach (string pref in integerNames)
            {
                int? value = this.GetIntegerPref(pref);
                if (value.HasValue)
                    integerPref.Append(string.Format("<Pref name=\"{0}\" value=\"{1}\" />\n", pref, value.Value));
            }
            List<string> stringNames = this.GetStringPrefNames();
            foreach (string pref in stringNames)
            {
                string value = this.GetStringPref(pref);
                if (value != null)
                    stringPref.Append(string.Format("<Pref name=\"{0}\" value=\"{1}\" />\n", pref, value));
            }
            List<string> booleanNames = this.GetBooleanPrefNames();
            foreach (string pref in booleanNames)
            {
                bool? value = this.GetBooleanPref(pref);
                if (value.HasValue)
                    booleanPref.Append(string.Format("<Pref name=\"{0}\" value=\"{1}\" />\n", pref, value.Value));
            }
            prefXml = string.Format(prefXml,
                stringPref.ToString(),
                doublePref.ToString(),
                integerPref.ToString(),
                booleanPref.ToString());
            File.WriteAllText("Preferences.xml", prefXml);
            AppConsole.WriteLine("Preferences Saved");
        }
    }
}
