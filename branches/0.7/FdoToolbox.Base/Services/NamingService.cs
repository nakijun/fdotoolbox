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

namespace FdoToolbox.Base.Services
{
    public class NamingService : IService
    {
        private bool _init = false;

        private Dictionary<string, string> _namePrefixes;
        private Dictionary<string, int> _counter;

        public bool IsInitialized
        {
            get { return _init; }
        }

        public void InitializeService()
        {
            _namePrefixes = new Dictionary<string, string>();
            _counter = new Dictionary<string, int>();
            SetPreferredNamePrefix("OSGeo.SDF", "SDFConnection");
            SetPreferredNamePrefix("OSGeo.SHP", "SHPConnection");
            
            _init = true;
            Initialize(this, EventArgs.Empty);
        }

        public void UnloadService()
        {
            Unload(this, EventArgs.Empty);
        }

        public void SetPreferredNamePrefix(string provider, string prefix)
        {
            _namePrefixes[provider] = prefix;
        }

        public void ResetCounter()
        {
            List<string> keys = new List<string>(_counter.Keys);
            foreach (string k in keys)
            {
                _counter[k] = 0;
            }
        }

        private FdoConnectionManager _manager;

        public string GetDefaultConnectionName(string provider)
        {
            if (!_namePrefixes.ContainsKey(provider))
                _namePrefixes[provider] = "Connection";

            if (!_counter.ContainsKey(provider))
                _counter[provider] = 0;

            if (_manager == null)
                _manager = ServiceManager.Instance.GetService<FdoConnectionManager>();

            string name = _namePrefixes[provider] + _counter[provider];
            while (_manager.NameExists(name))
            {
                _counter[provider]++;
                name = _namePrefixes[provider] + _counter[provider];
            }
            return name;
        }

        public event EventHandler Initialize = delegate { };

        public event EventHandler Unload = delegate { };


        public void Load()
        {
            
        }

        public void Save()
        {
            
        }
    }
}
