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
using NUnit.Framework;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core;

namespace FdoToolbox.Tests
{
    [TestFixture]
    public class PreferenceTest : BaseTest
    {
        const string PREF_BOOL = "pref_bool";
        const string PREF_DOUBLE = "pref_double";
        const string PREF_INT = "pref_int";
        const string PREF_STR = "pref_str";

        const bool BOOL_VALUE = true;
        const double DOUBLE_VALUE = 2.3458935943;
        const int INT_VALUE = 42;
        const string STR_VALUE = "foobar";

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidBoolean()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            InitializeDefault(dict);

            bool val = dict.GetBooleanPref("foobar");
        }

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidDouble()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            InitializeDefault(dict);

            double val = dict.GetDoublePref("foobar");
        }

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidInteger()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            InitializeDefault(dict);

            int val = dict.GetIntegerPref("foobar");
        }

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidString()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            InitializeDefault(dict);

            string val = dict.GetStringPref("foobar");
        }

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidNullStringValue()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            dict.SetStringPref("foo", null);
        }

        [Test]
        [ExpectedException(typeof(PreferenceException))]
        public void TestInvalidEmptyStringValue()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            dict.SetStringPref("foo", string.Empty);
        }

        [Test]
        public void TestInvalidNullPreference()
        {
            IPreferenceDictionary dict = new PreferenceDictionary();
            try
            {
                dict.SetBooleanPref(null, BOOL_VALUE);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.SetDoublePref(null, DOUBLE_VALUE);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.SetIntegerPref(null, INT_VALUE);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.SetStringPref(null, STR_VALUE);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.GetBooleanPref(null);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.GetDoublePref(null);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.GetIntegerPref(null);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }

            try
            {
                dict.GetStringPref(null);
                Assert.Fail("ArgumentNullException not thrown");
            }
            catch (ArgumentNullException) { }
        }

        private void InitializeDefault(IPreferenceDictionary dict)
        {
            dict.SetStringPref(PREF_STR, STR_VALUE);
            dict.SetBooleanPref(PREF_BOOL, BOOL_VALUE);
            dict.SetDoublePref(PREF_DOUBLE, DOUBLE_VALUE);
            dict.SetIntegerPref(PREF_INT, INT_VALUE);
        }
    }
}
