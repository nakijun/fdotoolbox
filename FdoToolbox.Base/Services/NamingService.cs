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
    }
}
