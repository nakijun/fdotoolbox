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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Lib.ClientServices;
using FdoToolbox.Core.ClientServices;

namespace FdoToolbox.Lib.Controls
{
    public partial class PreferencesCtl : BaseDocumentCtl
    {
        public PreferencesCtl()
        {
            InitializeComponent();
            this.Title = "Preferences";
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadPreferences();
            base.OnLoad(e);
        }

        private void LoadPreferences()
        {
            IPreferenceDictionary dict = AppGateway.RunningApplication.Preferences;
            
            foreach (string pref in dict.StringPreferences)
            {
                grdPreferences.Rows.Add(pref, dict.GetStringPref(pref), typeof(string));
            }

            foreach (string pref in dict.BooleanPreferences)
            {
                grdPreferences.Rows.Add(pref, dict.GetBooleanPref(pref), typeof(bool));
            }

            foreach (string pref in dict.DoublePreferences)
            {
                grdPreferences.Rows.Add(pref, dict.GetDoublePref(pref), typeof(double));
            }

            foreach (string pref in dict.IntegerPreferences)
            {
                grdPreferences.Rows.Add(pref, dict.GetIntegerPref(pref), typeof(int));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            IPreferenceDictionary dict = AppGateway.RunningApplication.Preferences;
            foreach (DataGridViewRow row in grdPreferences.Rows)
            {
                string name = row.Cells[0].Value.ToString();
                string value = row.Cells[1].Value.ToString();
                Type type = row.Cells[2].Value as Type;
                if (type == typeof(string))
                    dict.SetStringPref(name, value);
                else if (type == typeof(int))
                    dict.SetIntegerPref(name, Convert.ToInt32(value));
                else if (type == typeof(double))
                    dict.SetDoublePref(name, Convert.ToDouble(value));
                else if (type == typeof(bool))
                    dict.SetBooleanPref(name, Convert.ToBoolean(value));
            }
            AppConsole.Alert("Preferences", "Preferences Saved");
            this.Close();
        }
    }
}
