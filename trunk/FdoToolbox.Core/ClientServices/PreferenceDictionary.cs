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
using FdoToolbox.Core.Configuration;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ClientServices
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

        double GetDoublePref(string name);
        int GetIntegerPref(string name);
        bool GetBooleanPref(string name);
        string GetStringPref(string name);

        ICollection<string> DoublePreferences { get; }
        ICollection<string> IntegerPreferences { get; }
        ICollection<string> BooleanPreferences { get; }
        ICollection<string> StringPreferences { get; }

        void SetDefaultValue(string name, string value);
        void SetDefaultValue(string name, int value);
        void SetDefaultValue(string name, double value);
        void SetDefaultValue(string name, bool value);

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

        public void SetStringPref(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new PreferenceException("pref value cannot be empty or null: " + name);
            _StringPrefs[name] = value;
        }

        public void SetDoublePref(string name, double value)
        {
            _DoublePrefs[name] = value;
        }

        public void SetIntegerPref(string name, int value)
        {
            _IntegerPrefs[name] = value;
        }

        public void SetBooleanPref(string name, bool value)
        {
            _BooleanPrefs[name] = value;
        }

        public bool GetBooleanPref(string name)
        {
            if (_BooleanPrefs.ContainsKey(name))
                return _BooleanPrefs[name];

            throw new PreferenceException(name);
        }

        public int GetIntegerPref(string name)
        {
            if (_IntegerPrefs.ContainsKey(name))
                return _IntegerPrefs[name];

            throw new PreferenceException(name);
        }

        public double GetDoublePref(string name)
        {
            if (_DoublePrefs.ContainsKey(name))
                return _DoublePrefs[name];

            throw new PreferenceException(name);
        }

        public string GetStringPref(string name)
        {
            if (_StringPrefs.ContainsKey(name))
                return _StringPrefs[name];

            throw new PreferenceException(name);
        }

        public ICollection<string> DoublePreferences
        {
            get
            {
                return _DoublePrefs.Keys;
            }
        }

        public ICollection<string> IntegerPreferences
        {
            get
            {
                return _IntegerPrefs.Keys;
            }
        }

        public ICollection<string> BooleanPreferences
        {
            get
            {
                return _BooleanPrefs.Keys;
            }
        }

        public ICollection<string> StringPreferences
        {
            get
            {
                return _StringPrefs.Keys;
            }
        }

        public void Save()
        {
            Preferences prefs = new Preferences();
            List<BooleanPref> bprefs = new List<BooleanPref>();
            List<StringPref> sprefs = new List<StringPref>();
            List<DoublePref> dprefs = new List<DoublePref>();
            List<IntegerPref> iprefs = new List<IntegerPref>();
            
            foreach (string pref in this.DoublePreferences)
            {
                DoublePref dp = new DoublePref();
                dp.name = pref;
                dp.value = this.GetDoublePref(pref);
                dprefs.Add(dp);
            }
            foreach (string pref in this.IntegerPreferences)
            {
                IntegerPref ip = new IntegerPref();
                ip.name = pref;
                ip.value = this.GetIntegerPref(pref).ToString();
                iprefs.Add(ip);
            }
            foreach (string pref in this.StringPreferences)
            {
                StringPref sp = new StringPref();
                sp.name = pref;
                sp.value = this.GetStringPref(pref); ;
                sprefs.Add(sp);
            }
            foreach (string pref in this.BooleanPreferences)
            {
                BooleanPref bp = new BooleanPref();
                bp.name = pref;
                bp.value = this.GetBooleanPref(pref);
                bprefs.Add(bp);
            }

            prefs.BooleanPrefs = bprefs.ToArray();
            prefs.DoublePrefs = dprefs.ToArray();
            prefs.IntegerPrefs = iprefs.ToArray();
            prefs.StringPrefs = sprefs.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(Preferences));
            using (XmlTextWriter writer = new XmlTextWriter("Preferences.xml", Encoding.UTF8))
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, prefs);
            }
        }

        public void SetDefaultValue(string name, string value)
        {
            if (!_StringPrefs.ContainsKey(name))
                _StringPrefs[name] = value;
        }

        public void SetDefaultValue(string name, int value)
        {
            if (!_IntegerPrefs.ContainsKey(name))
                _IntegerPrefs[name] = value;
        }

        public void SetDefaultValue(string name, double value)
        {
            if (!_DoublePrefs.ContainsKey(name))
                _DoublePrefs[name] = value;
        }

        public void SetDefaultValue(string name, bool value)
        {
            if (!_BooleanPrefs.ContainsKey(name))
                _BooleanPrefs[name] = value;
        }
    }
}
